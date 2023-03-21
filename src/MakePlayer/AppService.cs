﻿using MakePlayer.Cliens;
using MakePlayer.Option;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net;
using SystemModule;

namespace MakePlayer
{
    public class AppService : BackgroundService
    {
        private readonly MakePlayOptions options;
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private int _loginIndex;
        private int _loginTimeTick;

        public AppService(IOptions<MakePlayOptions> options)
        {
            this.options = options.Value;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            options.ChrCount = HUtil32._MIN(options.ChrCount, options.TotalChrCount);
            _loginTimeTick = HUtil32.GetTickCount() - 1000 * options.ChrCount;
            ClientManager.Start();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            ClientManager.Stop();
            _cancellation.Cancel();
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!_cancellation.IsCancellationRequested)
            {
                if (options.TotalChrCount > 0)
                {
                    if ((HUtil32.GetTickCount() - _loginTimeTick) > 1000 * options.ChrCount)
                    {
                        _loginTimeTick = HUtil32.GetTickCount();
                        if (options.TotalChrCount >= options.ChrCount)
                        {
                            options.TotalChrCount -= options.ChrCount;
                        }
                        else
                        {
                            options.TotalChrCount = 0;
                        }
                        for (var i = 0; i < options.ChrCount; i++)
                        {
                            var playClient = new PlayClient();
                            playClient.SessionId = Guid.NewGuid().ToString("N");
                            playClient.CreateAccount = options.NewAccount;
                            playClient.LoginAccount = string.Concat(options.LoginAccount, _loginIndex);
                            if (playClient.LoginAccount.Length > 10)
                            {
                                playClient.LoginAccount = playClient.LoginAccount.Substring(1, 8);
                            }
                            playClient.LoginPasswd = playClient.LoginAccount;
                            playClient.ChrName = playClient.LoginAccount;
                            playClient.ServerName = options.ServerName;
                            //playClient.ClientSocket.Close();
                            playClient.ClientSocket.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(options.Address), options.Port);
                            playClient.ConnectTick = HUtil32.GetTickCount() + (i + 1) * 3000;
                            ClientManager.AddClient(playClient.SessionId, playClient);
                            _loginIndex++;
                        }
                    }
                }
                ClientManager.Run();
                Thread.Sleep(TimeSpan.FromMilliseconds(50));
            }
            return Task.CompletedTask;
        }
    }
}
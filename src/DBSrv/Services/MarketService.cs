﻿using System;
using System.Linq;
using DBSrv.Storage;
using NLog;
using SystemModule.Sockets;
using SystemModule.Sockets.AsyncSocketServer;

namespace DBSrv.Services
{
    /// <summary>
    /// 拍卖行数据存储服务
    /// GameSrv-> DBSrv
    /// </summary>
    public class MarketService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ICacheStorage _cacheStorage;
        private readonly IMarketStorage _marketStorage;
        private readonly SocketServer _socketServer;

        public MarketService(ICacheStorage cacheStorage, IMarketStorage marketStorage)
        {
            _cacheStorage = cacheStorage;
            _marketStorage = marketStorage;
            _socketServer = new SocketServer(byte.MaxValue, 1024);
            _socketServer.OnClientConnect += ServerSocketClientConnect;
            _socketServer.OnClientDisconnect += ServerSocketClientDisconnect;
            _socketServer.OnClientRead += ServerSocketClientRead;
            _socketServer.OnClientError += ServerSocketClientError;
        }

        public void Start()
        {
            _socketServer.Init();
            _socketServer.Start("127.0.0.1", 5700);
        }

        public void Stop()
        {
            _socketServer.Shutdown();
        }

        private void ServerSocketClientConnect(object sender, AsyncUserToken e)
        {
            _logger.Info("新的客户端连接 " + e.RemoteIPaddr);
        }

        private void ServerSocketClientRead(object sender, AsyncUserToken e)
        {
            ProcessMarketPacket();
        }

        private void ServerSocketClientDisconnect(object sender, AsyncUserToken e)
        {
            
        }
        
        private void ServerSocketClientError(object sender, AsyncSocketErrorEventArgs e)
        {
            
        }

        private void ProcessMarketPacket()
        {
            //todo 根据封包保存或拉取拍卖行数据
            // 0:GameSrv链接成功后第一次主动拉取拍卖行数据
            // 1:GameSrv保存拍卖行数据
        }
        
        private void PushMarketData()
        {
            //todo 根据服务器分组推送到各个GameSrv或者推送到所有GameSrv
            var marketItems = _marketStorage.QueryMarketItems(0);
            if (!marketItems.Any())
            {
                _logger.Info("当前服务器分组拍卖行数据为空,推送拍卖行数据失败.");
                return;
            }
            var socketList = _socketServer.GetSockets();
            foreach (var client in socketList)
            {
                if (_socketServer.IsOnline(client.ConnectionId))
                {
                    _socketServer.Send(client.ConnectionId, Array.Empty<byte>());//推送拍卖行数据
                }
            }
        }
    }
}
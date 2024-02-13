﻿using DBSrv.Conf;
using DBSrv.Services.Impl;
using DBSrv.Storage;
using DBSrv.Storage.Impl;
using DBSrv.Storage.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenMir2;
using OpenMir2.Common;
using Serilog;
using Spectre.Console;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace DBSrv
{
    public class AppServer
    {
        private static PeriodicTimer _timer;
        private static SettingConf _setting;
        private readonly IHost _host;

        public AppServer()
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            if (!Enum.TryParse<StoragePolicy>(_setting.StoreageType, true, out StoragePolicy storagePolicy))
            {
                throw new Exception("数据存储配置文件错误或者不支持该存储类型");
            }
            switch (storagePolicy)
            {
                case StoragePolicy.MySQL:
                    LoadAssembly(builder.Services, "MySQL");
                    break;
                case StoragePolicy.MongoDB:
                    LoadAssembly(builder.Services, "MongoDB");
                    break;
                case StoragePolicy.Sqlite:
                    LoadAssembly(builder.Services, "Sqlite");
                    break;
                case StoragePolicy.Local:
                    LoadAssembly(builder.Services, "Local");
                    break;
            }
            builder.Services.AddSingleton(_setting);
            builder.Services.AddSingleton<ClientSession>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<DataService>();
            builder.Services.AddSingleton<MarketService>();
            builder.Services.AddSingleton<ICacheStorage, CacheStorageService>();
            builder.Services.AddHostedService<TimedService>();
            builder.Services.AddHostedService<AppService>();

            builder.Logging.ClearProviders();
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
            builder.Logging.AddSerilog();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            LogService.Logger = Log.Logger;

            _host = builder.Build();
        }

        public void Initialize()
        {
            LogService.Info("初始化配置文件...");
            ConfigManager configManager = new ConfigManager();
            configManager.LoadConfig();
            _setting = configManager.GetConfig;
            LogService.Info("配置文件读取完成...");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            DBShare.Initialization();
            LogService.Info("正在读取基础配置信息...");
            DBShare.LoadConfig();
            LogService.Info($"加载IP授权文件列表成功...[{DBShare.ServerIpCount}]");
            LogService.Info("读取基础配置信息完成...");
            LoadServerInfo();
            LoadChrNameList("DenyChrName.txt");
            LoadClearMakeIndexList("ClearMakeIndex.txt");
            await _host.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _host.StopAsync(cancellationToken);
        }

        private void LoadAssembly(IServiceCollection services, string storageName)
        {
            string storageFileName = $"DBSrv.Storage.{storageName}.dll";
            string storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, storageFileName);
            if (!File.Exists(storagePath))
            {
                throw new Exception($"{storageFileName} 存储策略文件不存在,服务启动失败.");
            }
            AssemblyLoadContext context = new AssemblyLoadContext(storagePath);
            context.Resolving += ContextResolving;
            Assembly assembly = context.LoadFromAssemblyPath(storagePath);
            if (assembly == null)
            {
                throw new Exception($"获取{storageName}数据存储实例失败，请确认文件是否正确.");
            }
            Type playDataStorageType = assembly.GetType($"DBSrv.Storage.{storageName}.PlayDataStorage", true);
            Type playRecordStorageType = assembly.GetType($"DBSrv.Storage.{storageName}.PlayRecordStorage", true);
            Type marketStorageType = assembly.GetType($"DBSrv.Storage.{storageName}.MarketStoageService", true);
            if (playDataStorageType == null)
            {
                throw new ArgumentNullException(nameof(storageName), "获取数据存储实例失败，请确认文件是否正确或程序版本是否正确.");
            }
            if (playRecordStorageType == null)
            {
                throw new ArgumentNullException(nameof(storageName), "获取数据索引存储实例失败，请确认文件是否正确或程序版本是否正确.");
            }
            if (marketStorageType == null)
            {
                throw new ArgumentNullException(nameof(storageName), "获取拍卖行存储实例失败，请确认文件是否正确或程序版本是否正确.");
            }
            StorageOption storageOption = new StorageOption(_setting.ConnctionString);
            IPlayDataStorage playDataStorage = (IPlayDataStorage)Activator.CreateInstance(playDataStorageType, storageOption);
            IPlayRecordStorage playRecordStorage = (IPlayRecordStorage)Activator.CreateInstance(playRecordStorageType, storageOption);
            IMarketStorage marketStorage = (IMarketStorage)Activator.CreateInstance(marketStorageType, storageOption);
            if (playDataStorage == null)
            {
                throw new ArgumentNullException(nameof(storageName), "创建数据存储实例失败，请确认文件是否正确或程序版本是否正确.");
            }
            if (playRecordStorage == null)
            {
                throw new ArgumentNullException(nameof(storageName), "创建数据索引存储实例失败，请确认文件是否正确或程序版本是否正确.");
            }
            if (marketStorage == null)
            {
                throw new ArgumentNullException(nameof(storageName), "创建拍卖行数据存储实力失败，请确认文件是否正确或程序版本是否正确.");
            }
            services.AddSingleton(playDataStorage);
            services.AddSingleton(playRecordStorage);
            services.AddSingleton(marketStorage);
            LogService.Info($"[{storageName}]数据存储引擎初始化成功.");
        }

        /// <summary>
        /// 加载依赖项
        /// </summary>
        /// <returns></returns>
        private Assembly ContextResolving(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            string expectedPath = Path.Combine(AppContext.BaseDirectory, assemblyName.Name + ".dll");
            if (File.Exists(expectedPath))
            {
                try
                {
                    using FileStream stream = File.OpenRead(expectedPath);
                    return context.LoadFromStream(stream);
                }
                catch (Exception ex)
                {
                    LogService.Error($"加载依赖项{expectedPath}发生异常：{ex.Message},{ex.StackTrace}");
                }
            }
            else
            {
                LogService.Error($"依赖项不存在：{expectedPath}");
            }
            return null;
        }

        private static void Stop()
        {
            AnsiConsole.Status().Start("Disconnecting...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
            });
        }

        private async Task ProcessLoopAsync()
        {
            string input = null;
            do
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                if (input.StartsWith("/exit") && AnsiConsole.Confirm("Do you really want to exit?"))
                {
                    return;
                }

                string firstTwoCharacters = input[..2];

                if (firstTwoCharacters switch
                {
                    "/s" => ShowServerStatus(),
                    "/c" => ClearConsole(),
                    "/q" => Exit(),
                    _ => null
                } is Task task)
                {
                    await task;
                    continue;
                }

            } while (input is not "/exit");
        }

        private static Task Exit()
        {
            Environment.Exit(Environment.ExitCode);
            return Task.CompletedTask;
        }

        private static Task ClearConsole()
        {
            Console.Clear();
            AnsiConsole.Clear();
            return Task.CompletedTask;
        }

        private async Task ShowServerStatus()
        {
            DBShare.ShowLog = false;
            UserService userService = _host.Services.GetService<UserService>();
            if (userService == null)
            {
                return;
            }
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            SelGateInfo[] serverList = userService.GetGates.ToArray();
            Table table = new Table().Expand().BorderColor(Color.Grey);
            table.AddColumn("[yellow]ServerName[/]");
            table.AddColumn("[yellow]EndPoint[/]");
            table.AddColumn("[yellow]Status[/]");
            table.AddColumn("[yellow]Sessions[/]");
            table.AddColumn("[yellow]Send[/]");
            table.AddColumn("[yellow]Revice[/]");
            table.AddColumn("[yellow]Queue[/]");

            await AnsiConsole.Live(table)
                 .AutoClear(true)
                 .Overflow(VerticalOverflow.Crop)
                 .Cropping(VerticalOverflowCropping.Bottom)
                 .StartAsync(async ctx =>
                 {
                     foreach (int _ in Enumerable.Range(0, serverList.Length))
                     {
                         table.AddRow(new[] { new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-"), new Markup("-") });
                     }

                     while (await _timer.WaitForNextTickAsync())
                     {
                         for (int i = 0; i < serverList.Length; i++)
                         {
                             (string serverIp, string status, string sessionCount, string reviceTotal, string sendTotal, string queueCount) = serverList[i].GetStatus();

                             table.UpdateCell(i, 0, "[bold][blue]SelGate[/][/]");
                             table.UpdateCell(i, 1, ($"[bold]{serverIp}[/]"));
                             table.UpdateCell(i, 2, ($"[bold]{status}[/]"));
                             table.UpdateCell(i, 3, ($"[bold]{sessionCount}[/]"));
                             table.UpdateCell(i, 4, ($"[bold]{sendTotal}[/]"));
                             table.UpdateCell(i, 5, ($"[bold]{reviceTotal}[/]"));
                             table.UpdateCell(i, 6, ($"[bold]{queueCount}[/]"));
                         }
                         ctx.Refresh();
                     }
                 });
        }

        private void LoadServerInfo()
        {
            string sSelGateIPaddr = string.Empty;
            string sGameGateIPaddr = string.Empty;
            string sGameGate = string.Empty;
            string sGameGatePort = string.Empty;
            string sMapName = string.Empty;
            string sMapInfo = string.Empty;
            string sServerIndex = string.Empty;
            StringList loadList = new StringList();
            if (!File.Exists(DBShare.GateConfFileName))
            {
                return;
            }
            loadList.LoadFromFile(DBShare.GateConfFileName);
            if (loadList.Count <= 0)
            {
                LogService.Error("加载游戏服务配置文件ServerInfo.txt失败.");
                return;
            }
            int nRouteIdx = 0;
            int nGateIdx = 0;
            for (int i = 0; i < loadList.Count; i++)
            {
                string sLineText = loadList[i].Trim();
                if (!string.IsNullOrEmpty(sLineText) && !sLineText.StartsWith(";"))
                {
                    sGameGate = HUtil32.GetValidStr3(sLineText, ref sSelGateIPaddr, new[] { " ", "\09" });
                    if ((string.IsNullOrEmpty(sGameGate)) || (string.IsNullOrEmpty(sSelGateIPaddr)))
                    {
                        continue;
                    }
                    DBShare.RouteInfo[nRouteIdx] = new GateRouteInfo();
                    DBShare.RouteInfo[nRouteIdx].SelGateIP = sSelGateIPaddr.Trim();
                    DBShare.RouteInfo[nRouteIdx].GateCount = 0;
                    nGateIdx = 0;
                    while (!string.IsNullOrEmpty(sGameGate))
                    {
                        sGameGate = HUtil32.GetValidStr3(sGameGate, ref sGameGateIPaddr, new[] { " ", "\09" });
                        string[] gamrGates = sGameGate.Split(",");
                        if (gamrGates.Length == 0)
                        {
                            sGameGate = HUtil32.GetValidStr3(sGameGate, ref sGameGatePort, new[] { " ", "\09" });
                            DBShare.RouteInfo[nRouteIdx].GameGateIP[nGateIdx] = sGameGateIPaddr.Trim();
                            DBShare.RouteInfo[nRouteIdx].GameGatePort[nGateIdx] = HUtil32.StrToInt(sGameGatePort, 0);
                            nGateIdx++;
                        }
                        else
                        {
                            for (int j = 0; j < gamrGates.Length; j++)
                            {
                                DBShare.RouteInfo[nRouteIdx].GameGateIP[nGateIdx] = sGameGateIPaddr.Trim();
                                DBShare.RouteInfo[nRouteIdx].GameGatePort[nGateIdx] = HUtil32.StrToInt(gamrGates[j], 0);
                                nGateIdx++;
                            }
                            sGameGate = string.Empty;
                        }
                    }
                    DBShare.RouteInfo[nRouteIdx].GateCount = nGateIdx;
                    nRouteIdx++;
                }
            }
            LogService.Info($"读取网关配置信息成功.[{DBShare.RouteInfo.Where(x => x != null).Sum(x => x.GateCount)}]");
            DBShare.MapList.Clear();
            if (File.Exists(_setting.MapFile))
            {
                loadList.Clear();
                loadList.LoadFromFile(_setting.MapFile);
                for (int i = 0; i < loadList.Count; i++)
                {
                    string sLineText = loadList[i];
                    if ((!string.IsNullOrEmpty(sLineText)) && (sLineText[0] == '['))
                    {
                        sLineText = HUtil32.ArrestStringEx(sLineText, "[", "]", ref sMapName);
                        sMapInfo = HUtil32.GetValidStr3(sMapName, ref sMapName, new[] { " ", "\09" });
                        sServerIndex = HUtil32.GetValidStr3(sMapInfo, ref sMapInfo, new[] { " ", "\09" });
                        int nServerIndex = HUtil32.StrToInt(sServerIndex, 0);
                        DBShare.MapList.Add(sMapName, nServerIndex);
                    }
                }
            }
            loadList = null;
        }

        private static void LoadChrNameList(string sFileName)
        {
            int i;
            if (File.Exists(sFileName))
            {
                DBShare.DenyChrNameList.LoadFromFile(sFileName);
                i = 0;
                while (true)
                {
                    if (DBShare.DenyChrNameList.Count <= i)
                    {
                        break;
                    }
                    if (string.IsNullOrEmpty(DBShare.DenyChrNameList[i].Trim()))
                    {
                        DBShare.DenyChrNameList.RemoveAt(i);
                        continue;
                    }
                    i++;
                }
            }
        }

        private static void LoadClearMakeIndexList(string sFileName)
        {
            if (File.Exists(sFileName))
            {
                DBShare.ClearMakeIndex.LoadFromFile(sFileName);
                int i = 0;
                while (true)
                {
                    if (DBShare.ClearMakeIndex.Count <= i)
                    {
                        break;
                    }
                    string sLineText = DBShare.ClearMakeIndex[i];
                    int nIndex = HUtil32.StrToInt(sLineText, -1);
                    if (nIndex < 0)
                    {
                        DBShare.ClearMakeIndex.RemoveAt(i);
                        continue;
                    }
                    DBShare.ClearMakeIndex[i] = nIndex.ToString();
                    i++;
                }
            }
        }
    }
}
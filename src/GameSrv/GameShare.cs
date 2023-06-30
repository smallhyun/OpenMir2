using GameSrv.DataSource;
using GameSrv.Maps;
using GameSrv.Module;
using GameSrv.Services;
using GameSrv.Word.Threads;
using M2Server;
using PlanesSystem;
using SystemModule;
using SystemModule.Data;

namespace GameSrv
{
    public static class GameShare
    {
        /// <summary>
        /// 服务器启动时间
        /// </summary>
        public static readonly long StartTime;
        public static readonly WordStatistics Statistics;
        public static readonly LocalDb LocalDb;
        public static readonly CommonDB CommonDb;
        public static readonly IPlanesService PlanesService;
        public static readonly NetworkMonitor NetworkMonitor;
        public static readonly SystemProcessor SystemProcess;
        public static readonly UserProcessor UserProcessor;
        public static readonly MerchantProcessor MerchantProcessor;
        public static readonly GeneratorProcessor GeneratorProcessor;
        public static readonly ActorBuffProcessor ActorBuffProcessor;
        public static readonly EventProcessor EventProcessor;
        public static readonly StorageProcessor StorageProcessor;
        public static readonly TimedRobotProcessor TimedRobotProcessor;
        public static readonly MapQuestManager QuestManager;
        public static readonly DataCacheThread DataServer;
        public static IList<ModuleInfo> Modules { get; set; } = new List<ModuleInfo>();

        static GameShare()
        {
            Statistics = new WordStatistics();
            LocalDb = new LocalDb();
            CommonDb = new CommonDB();
            NetworkMonitor = new NetworkMonitor();
            SystemProcess = new SystemProcessor();
            UserProcessor = new UserProcessor();
            MerchantProcessor = new MerchantProcessor();
            GeneratorProcessor = new GeneratorProcessor();
            EventProcessor = new EventProcessor();
            StorageProcessor = new StorageProcessor();
            TimedRobotProcessor = new TimedRobotProcessor();
            ActorBuffProcessor = new ActorBuffProcessor();
            PlanesService = new PlanesService();
            DataServer = new DataCacheThread();
            QuestManager = new MapQuestManager();
            StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 随机获取其他服务器
        /// </summary>
        /// <returns></returns>
        public static bool GetMultiServerAddrPort(byte serverIndex, ref string sIPaddr, ref int nPort)
        {
            var result = false;
            for (var i = 0; i < M2Share.ServerTableList.Length; i++)
            {
                var routeInfo = M2Share.ServerTableList[i];
                if (routeInfo == null)
                {
                    continue;
                }
                if (routeInfo.GateCount <= 0)
                {
                    continue;
                }
                if (routeInfo.ServerIdx == serverIndex)
                {
                    sIPaddr = GetRandpmRoute(routeInfo, ref nPort);
                    result = true;
                    break;
                }
            }
            return result;
        }

        private static string GetRandpmRoute(TRouteInfo routeInfo, ref int gatePort)
        {
            var random = RandomNumber.GetInstance().Random(routeInfo.GateCount);
            gatePort = routeInfo.GameGatePort[random];
            return routeInfo.GameGateIP[random];
        }

        public static void LoadConfig()
        {
            SystemShare.ServerConf.LoadConfig();
            SystemShare.StringConf.LoadString();
            SystemShare.ExpConf.LoadConfig();
            SystemShare.GlobalConf.LoadConfig();
            SystemShare.GameSetting.LoadConfig();
        }
    }
}
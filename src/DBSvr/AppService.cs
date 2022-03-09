using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DBSvr
{
    public class AppService : BackgroundService
    {
        private readonly ILogger<AppService> _logger;
        private readonly UserSocService _userSoc;
        private readonly LoginSocService _LoginSoc;
        private readonly HumDataService _dataService;
        private readonly ConfigManager _configManager;
        private Timer _threadTimer;

        public AppService(ILogger<AppService> logger,  UserSocService userSoc, LoginSocService idSoc, HumDataService dataService, ConfigManager configManager)
        {
            _logger = logger;
            _userSoc = userSoc;
            _LoginSoc = idSoc;
            _dataService = dataService;
            _configManager = configManager;
            _threadTimer = new Timer(ThreadServerTimer, null, 1000, 5000);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogDebug($"DBSvr is stopping."));
            _userSoc.Start();
            _LoginSoc.Start();
            _dataService.Start();
            await _userSoc.StartConsumer();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"DBSvr is starting.");
            DBShare.Initialization();
            _configManager.LoadConfig();
            DBShare.LoadConfig();
            DBShare.nHackerNewChrCount = 0;
            DBShare.nHackerDelChrCount = 0;
            DBShare.nHackerSelChrCount = 0;
            DBShare.n4ADC1C = 0;
            DBShare.n4ADC20 = 0;
            DBShare.n4ADC24 = 0;
            DBShare.n4ADC28 = 0;
            DBShare.MainOutMessage("服务器已启动...");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"DBSvr is stopping.");
            return base.StopAsync(cancellationToken);
        }

        private void ThreadServerTimer(object obj)
        {
            var userCount = _userSoc.GetUserCount();
            _LoginSoc.SendKeepAlivePacket(userCount);
            _LoginSoc.CheckConnection();
            _dataService.ClearTimeoutSession();
            //ServerState(userCount);
        }
        
        private void ServerState(int userCount)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UserCount:{userCount}");
            sb.AppendLine(DBShare.g_nClearIndex + "/(" + DBShare.g_nClearCount + "/" + DBShare.g_nClearItemIndexCount + ")/" + DBShare.g_nClearRecordCount);
            sb.AppendLine($"H-QyChr:{DBShare.g_nQueryChrCount} H-NwChr:{DBShare.nHackerNewChrCount} H-DlChr:{DBShare.nHackerDelChrCount} Dubb -Sl:{DBShare.nHackerSelChrCount}");
            sb.AppendLine($"H-Er-P1:{DBShare.n4ADC1C} Dubl-P2:{DBShare.n4ADC20} Dubl-P3:{DBShare.n4ADC24} Dubl-P4:{DBShare.n4ADC28}");
            DBShare.MainOutMessage(sb.ToString());
        }
    }
}
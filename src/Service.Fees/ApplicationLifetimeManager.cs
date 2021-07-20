using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;

namespace Service.Fees
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly MyNoSqlClientLifeTime _myNoSqlClientLifeTime;
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        
        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime,
            MyNoSqlClientLifeTime myNoSqlClientLifeTime,
            ILogger<ApplicationLifetimeManager> logger)
            : base(appLifetime)
        {
            _myNoSqlClientLifeTime = myNoSqlClientLifeTime;
            _logger = logger;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _myNoSqlClientLifeTime.Start();
            _logger.LogInformation("MyNoSqlTcpClient is started.");
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _myNoSqlClientLifeTime.Stop();
            _logger.LogInformation("MyNoSqlTcpClient is stopped.");
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
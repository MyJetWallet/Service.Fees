using Autofac;
using MyNoSqlServer.DataReader;
using Service.Fees.MyNoSql;

namespace Service.Fees.Client
{
    public static class FeesDictionaryAutofacHelper
    {
        /// <summary>
        /// Register interface:
        ///   * IFeesSettingsClient
        /// </summary>
        public static void RegisterFeesClients(this ContainerBuilder builder,
            IMyNoSqlSubscriber myNoSqlSubscriber)
        {
            var feesSettings = new MyNoSqlReadRepository<FeesSettingsNoSqlEntity>(myNoSqlSubscriber,
                FeesSettingsNoSqlEntity.TableName);
            
            var assetFees = new MyNoSqlReadRepository<AssetFeesNoSqlEntity>(myNoSqlSubscriber,
                AssetFeesNoSqlEntity.TableName);
            
            var depositFees = new MyNoSqlReadRepository<DepositFeesNoSqlEntity>(myNoSqlSubscriber,
                DepositFeesNoSqlEntity.TableName);
            
            var instrumentFees = new MyNoSqlReadRepository<SpotInstrumentFeesNoSqlEntity>(myNoSqlSubscriber,
                SpotInstrumentFeesNoSqlEntity.TableName);

            builder
                .RegisterInstance(new DepositFeesClient(depositFees))
                .As<IDepositFeesClient>()
                .AutoActivate()
                .SingleInstance();
            
            builder
                .RegisterInstance(new AssetFeesClient(assetFees, feesSettings))
                .As<IAssetFeesClient>()
                .AutoActivate()
                .SingleInstance();

            builder
                .RegisterInstance(new SpotInstrumentFeesClient(instrumentFees, feesSettings))
                .As<ISpotInstrumentFeesClient>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
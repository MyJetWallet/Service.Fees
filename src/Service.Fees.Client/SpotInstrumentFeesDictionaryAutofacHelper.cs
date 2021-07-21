using Autofac;
using MyNoSqlServer.DataReader;
using Service.Fees.MyNoSql;

namespace Service.Fees.Client
{
    public static class SpotInstrumentFeesDictionaryAutofacHelper
    {
        /// <summary>
        /// Register interface:
        ///   * IFeesSettingsClient
        /// </summary>
        public static void RegisterSpotInstrumentFeesClients(this ContainerBuilder builder,
            IMyNoSqlSubscriber myNoSqlSubscriber)
        {
            var instrumentFees = new MyNoSqlReadRepository<SpotInstrumentFeesNoSqlEntity>(myNoSqlSubscriber,
                SpotInstrumentFeesNoSqlEntity.TableName);
            
            var feesSettings = new MyNoSqlReadRepository<FeesSettingsNoSqlEntity>(myNoSqlSubscriber,
                FeesSettingsNoSqlEntity.TableName);

            builder
                .RegisterInstance(new SpotInstrumentFeesClient(instrumentFees, feesSettings))
                .As<ISpotInstrumentFeesClient>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
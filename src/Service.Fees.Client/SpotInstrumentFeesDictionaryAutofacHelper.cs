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
            var subs = new MyNoSqlReadRepository<SpotInstrumentFeesNoSqlEntity>(myNoSqlSubscriber,
                SpotInstrumentFeesNoSqlEntity.TableName);

            builder
                .RegisterInstance(new SpotInstrumentFeesClient(subs))
                .As<ISpotInstrumentFeesClient>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
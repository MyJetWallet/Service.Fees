using Autofac;
using MyNoSqlServer.DataReader;
using Service.Fees.MyNoSql;

namespace Service.Fees.Client
{
    public static class AssetFeesDictionaryAutofacHelper
    {
        /// <summary>
        /// Register interface:
        ///   * IFeesSettingsClient
        /// </summary>
        public static void RegisterFeesClients(this ContainerBuilder builder,
            IMyNoSqlSubscriber myNoSqlSubscriber)
        {
            var subs = new MyNoSqlReadRepository<AssetFeesNoSqlEntity>(myNoSqlSubscriber,
                AssetFeesNoSqlEntity.TableName);

            builder
                .RegisterInstance(new AssetFeesClient(subs))
                .As<IAssetFeesClient>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
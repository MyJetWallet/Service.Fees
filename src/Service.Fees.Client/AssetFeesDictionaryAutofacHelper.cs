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
        public static void RegisterAssetFeesClients(this ContainerBuilder builder,
            IMyNoSqlSubscriber myNoSqlSubscriber)
        {
            var assetFees = new MyNoSqlReadRepository<AssetFeesNoSqlEntity>(myNoSqlSubscriber,
                AssetFeesNoSqlEntity.TableName);
            
            var feesSettings = new MyNoSqlReadRepository<FeesSettingsNoSqlEntity>(myNoSqlSubscriber,
                FeesSettingsNoSqlEntity.TableName);

            builder
                .RegisterInstance(new AssetFeesClient(assetFees, feesSettings))
                .As<IAssetFeesClient>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
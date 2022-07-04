using Autofac;
using MyJetWallet.Sdk.NoSql;
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
            var feesSettings = builder.RegisterMyNoSqlReader<FeesSettingsNoSqlEntity>(myNoSqlSubscriber, FeesSettingsNoSqlEntity.TableName);

            var assetFees = builder.RegisterMyNoSqlReader<AssetFeesNoSqlEntity>(myNoSqlSubscriber, AssetFeesNoSqlEntity.TableName);

            var depositFees = builder.RegisterMyNoSqlReader<DepositFeesNoSqlEntity>(myNoSqlSubscriber, DepositFeesNoSqlEntity.TableName);

            var instrumentFees = builder.RegisterMyNoSqlReader<SpotInstrumentFeesNoSqlEntity>(myNoSqlSubscriber, SpotInstrumentFeesNoSqlEntity.TableName);

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
using Autofac;
using Service.Fees.Grpc;

namespace Service.Fees.Client.Grpc
{
    public static class FeesSettingsAutofacHelper
    {
        public static void RegisterFeesSettingsClients(this ContainerBuilder builder,
            string feesSettingsGrpcServiceUrl)
        {
            var factory = new FeesSettingsFactory(feesSettingsGrpcServiceUrl);

            builder.RegisterInstance(factory.GetFeesSettingsService())
                .As<IFeesSettingsService>().SingleInstance();
            builder.RegisterInstance(factory.GetAssetFeesSettingsService())
                .As<IAssetFeesSettingsService>().SingleInstance();
            builder.RegisterInstance(factory.GetSpotInstrumentFeesSettingsService())
                .As<ISpotInstrumentFeesSettingsService>().SingleInstance();
            builder.RegisterInstance(factory.GetGroupsService())
                .As<IFeeProfileService>().SingleInstance();
            builder.RegisterInstance(factory.GetDepositFeesSettingsService())
                .As<IDepositFeesSettingsService>().SingleInstance();
        }
    }
}
using Autofac;
using Service.Fees.Grpc;
using Service.Fees.Services;

namespace Service.Fees.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FeesSettingsService>()
                .As<IFeesSettingsService>();

            builder.RegisterType<AssetFeesService>()
                .As<IAssetFeesService>();

            builder.RegisterType<DepositFeesService>()
                .As<IDepositFeesService>();
            
            builder.RegisterType<SpotInstrumentFeesService>()
                .As<ISpotInstrumentFeesService>();
            
            builder.RegisterType<AssetFeesSettingsService>()
                .As<IAssetFeesSettingsService>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterType<SpotInstrumentFeesSettingsService>()
                .As<ISpotInstrumentFeesSettingsService>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.GrpcMetrics;
using ProtoBuf.Grpc.Client;
using Service.Fees.Grpc;

namespace Service.Fees.Client.Grpc
{
    [UsedImplicitly]
    public class FeesSettingsFactory
    {
        private readonly CallInvoker _channel;

        public FeesSettingsFactory(string feesSettingsGrpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(feesSettingsGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public IAssetFeesSettingsService GetAssetFeesSettingsService() =>
            _channel.CreateGrpcService<IAssetFeesSettingsService>();

        public ISpotInstrumentFeesSettingsService GetSpotInstrumentFeesSettingsService() =>
            _channel.CreateGrpcService<ISpotInstrumentFeesSettingsService>();

        public IFeesSettingsService GetFeesSettingsService() =>
            _channel.CreateGrpcService<IFeesSettingsService>();
        
        public IFeeProfileService GetGroupsService() => _channel.CreateGrpcService<IFeeProfileService>();

    }
}
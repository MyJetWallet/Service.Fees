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
    public class FeesClientFactory
    {
        private readonly CallInvoker _channel;

        public FeesClientFactory(string assetsDictionaryGrpcServiceUrl)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(assetsDictionaryGrpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public IFeesSettingsService GetFeesSettingsService() => _channel.CreateGrpcService<IFeesSettingsService>();
        public IAssetFeesService GetAssetFeesService() => _channel.CreateGrpcService<IAssetFeesService>();

        public ISpotInstrumentFeesService GetSpotInstrumentFeesService() =>
            _channel.CreateGrpcService<ISpotInstrumentFeesService>();
    }
}
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProtoBuf.Grpc.Client;
using Service.Fees.Client.Grpc;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            var factory = new FeesClientFactory("http://localhost:88");

            var settingsClient = factory.GetFeesSettingsService();

            await settingsClient.SetFeesSettingsAsync(new FeesSettings()
            {
                BrokerId = "test",
                AccountId = "OrangeDevAccount",
                WalletId = "OrangeDevWallet"
            });

            var settings = await settingsClient.GetFeesSettingsAsync(new GetFeesSettingsRequest()
                {
                    BrokerId = "test"
                }
            );

            Console.WriteLine(JsonConvert.SerializeObject(settings));

            var assetFeesClient = factory.GetAssetFeesService();
            await assetFeesClient.SetAssetFeesAsync(new AssetFees()
            {
                BrokerId = "test",
                AssetId = "BTC",
                FeeType = FeeType.ClientFee,
                OperationType = OperationType.Withdrawal,
                FeeSizeType = FeeSizeType.Absolute,
                FeeSize = 0.0001
            });

            var assetFees = await assetFeesClient.GetAssetFees(new GetAssetFeesRequest()
                {
                    BrokerId = "test", AssetId = "BTC", OperationType = OperationType.Withdrawal
                }
            );

            Console.WriteLine(JsonConvert.SerializeObject(assetFees));

            var instrumentClient = factory.GetSpotInstrumentFeesService();
            await instrumentClient.SetSpotInstrumentFeesAsync(new SpotInstrumentFees()
            {
                BrokerId = "test",
                SpotInstrumentId = "BTCUSD",
                FeeType = FeeType.ClientFee,
                MakerFeeSizeType = FeeSizeType.Percentage,
                MakerFeeSize = 0.0001,
                TakerFeeSizeType = FeeSizeType.Percentage,
                TakerFeeSize = 0.0002
            });

            var instrumentFees = await instrumentClient.GetSpotInstrumentFeesAsync(new GetSpotInstrumentFeesRequest()
                {BrokerId = "test", SpotInstrumentId = "BTCUSD"});

            Console.WriteLine(JsonConvert.SerializeObject(instrumentFees));

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
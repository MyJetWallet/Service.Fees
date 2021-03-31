using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain.Assets;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using Service.AssetsDictionary.Client;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc;
using Service.Fees.Grpc.Models;
using Service.Fees.MyNoSql;

namespace Service.Fees.Services
{
    public class AssetFeesService : IAssetFeesService
    {
        private readonly IMyNoSqlServerDataWriter<AssetFeesNoSqlEntity> _writer;
        private readonly IMyNoSqlServerDataWriter<FeesSettingsNoSqlEntity> _feesSettings;
        private readonly ILogger<AssetFeesService> _logger;
        private readonly IAssetsDictionaryClient _assetsClient;

        public AssetFeesService(IMyNoSqlServerDataWriter<AssetFeesNoSqlEntity> writer,
            IMyNoSqlServerDataWriter<FeesSettingsNoSqlEntity> feesSettings,
            ILogger<AssetFeesService> logger,
            IAssetsDictionaryClient assetsClient)
        {
            _writer = writer;
            _feesSettings = feesSettings;
            _logger = logger;
            _assetsClient = assetsClient;
        }

        public async Task<FeesResponse<AssetFees>> SetAssetFeesAsync(AssetFees assetFees)
        {
            _logger.LogInformation("Receive SetAssetFeesAsync request: {jsonText}",
                JsonConvert.SerializeObject(assetFees));

            if (string.IsNullOrEmpty(assetFees.BrokerId))
                return FeesResponse<AssetFees>.Error("Cannot create/update asset fees. BrokerId cannot be empty");
            if (string.IsNullOrEmpty(assetFees.AssetId))
                return FeesResponse<AssetFees>.Error("Cannot create/update asset fees. Symbol cannot be empty");

            var asset = _assetsClient.GetAssetById(new AssetIdentity()
                {BrokerId = assetFees.BrokerId, Symbol = assetFees.AssetId});

            if (asset == null)
                return FeesResponse<AssetFees>.Error("Cannot create asset fees. Asset do not found");

            var entity = AssetFeesNoSqlEntity.Create(assetFees);

            await _writer.InsertOrReplaceAsync(entity);

            _logger.LogInformation("Asset fees are created. BrokerId: {brokerId}, Asset: {asset}", entity.BrokerId,
                entity.AssetId);

            return FeesResponse<AssetFees>.Success(AssetFees.Create(entity.AssetFees));
        }

        public async Task<NullableValue<AssetFees>> GetAssetFees(GetAssetFeesRequest request)
        {
            try
            {
                var entity = await _writer.GetAsync(AssetFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                    AssetFeesNoSqlEntity.GenerateRowKey(request.AssetId, request.OperationType));

                if (entity == null)
                    return new NullableValue<AssetFees>();

                var assetFees = entity.AssetFees;

                if (string.IsNullOrEmpty(assetFees.AccountId) || string.IsNullOrEmpty(assetFees.WalletId))
                {
                    var settings = await _feesSettings.GetAsync(
                        FeesSettingsNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                        FeesSettingsNoSqlEntity.GenerateRowKey());
                    assetFees.AccountId = settings.FeesSettings.AccountId;
                    assetFees.WalletId = settings.FeesSettings.WalletId;
                }

                return new NullableValue<AssetFees>(assetFees);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown HTTP result CodeNotFound. Message: Row not found")
                    return new NullableValue<AssetFees>();
                throw;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc;
using Service.Fees.Grpc.Models;
using Service.Fees.MyNoSql;

// ReSharper disable InconsistentLogPropertyNaming

namespace Service.Fees.Services
{
    public class AssetFeesSettingsService : IAssetFeesSettingsService
    {
        private readonly ILogger<AssetFeesSettingsService> _logger;
        private readonly IMyNoSqlServerDataWriter<AssetFeesNoSqlEntity> _writer;

        public AssetFeesSettingsService(ILogger<AssetFeesSettingsService> logger,
            IMyNoSqlServerDataWriter<AssetFeesNoSqlEntity> writer)
        {
            _logger = logger;
            _writer = writer;
        }

        public async Task<List<AssetFees>> GetAssetFeesSettingsList()
        {
            var entities = await _writer.GetAsync();
            return entities.Select(e => e.AssetFees).ToList();
        }

        public async Task AddAssetFeesSettings(AssetFees settings)
        {
            using var action = MyTelemetry.StartActivity("Add Asset Fees Settings");
            settings.AddToActivityAsJsonTag("settings");
            try
            {
                _logger.LogInformation("Add Asset Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(settings));
                
                ValidateSettings(settings);
                
                var entity = AssetFeesNoSqlEntity.Create(settings);

                var existingItem = await _writer.GetAsync(entity.PartitionKey, entity.RowKey);
                if (existingItem != null) throw new Exception("Cannot add Asset Fees Settings. Already exist");

                await _writer.InsertAsync(entity);
                
                _logger.LogInformation("Added Asset Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(settings));
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot add ExternalMarketSettings: {requestJson}",
                    JsonConvert.SerializeObject(settings));
                ex.FailActivity();
                throw;
            }
        }

        public async Task UpdateAssetFeesSettings(AssetFees settings)
        {
            using var action = MyTelemetry.StartActivity("Update Asset Fees Settings");
            settings.AddToActivityAsJsonTag("settings");
            try
            {
                _logger.LogInformation("Update Asset Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(settings));
                
                ValidateSettings(settings);
                
                var entity = AssetFeesNoSqlEntity.Create(settings);

                var existingItem = await _writer.GetAsync(entity.PartitionKey, entity.RowKey);
                if (existingItem == null) throw new Exception("Cannot update Asset Fees Settings. Do not exist");

                await _writer.InsertOrReplaceAsync(entity);
                
                _logger.LogInformation("Updated Asset Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(settings));
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update ExternalMarketSettings: {requestJson}",
                    JsonConvert.SerializeObject(settings));
                ex.FailActivity();
                throw;
            }
        }

        public async Task RemoveAssetFeesSettings(RemoveAssetFeesRequest request)
        {
            using var action = MyTelemetry.StartActivity("Remove Asset Fees Settings");
            request.AddToActivityAsJsonTag("request");
            try
            {
                _logger.LogInformation("Remove Asset Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(request));

                var entity = await _writer.DeleteAsync(AssetFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                    AssetFeesNoSqlEntity.GenerateRowKey(request.AssetId, request.OperationType));
                
                if (entity != null)
                    _logger.LogInformation("Removed Asset Fees Settings: {jsonText}",
                        JsonConvert.SerializeObject(entity));
                else 
                    _logger.LogInformation("Unable to remove Asset Fees Setting, do not exist: {jsonText}",
                        JsonConvert.SerializeObject(request));
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot remove ExternalMarketSettings: {requestJson}",
                    JsonConvert.SerializeObject(request));
                ex.FailActivity();
                throw;
            }
        }

        private static void ValidateSettings(AssetFees settings)
        {
            if (string.IsNullOrEmpty(settings.BrokerId)) throw new Exception("Cannot add settings with empty broker");
            if (string.IsNullOrEmpty(settings.AccountId)) throw new Exception("Cannot add settings with empty account");
            if (string.IsNullOrEmpty(settings.WalletId)) throw new Exception("Cannot add settings with empty wallet");
            if (string.IsNullOrEmpty(settings.AssetId)) throw new Exception("Cannot add settings with empty asset");
            if (settings.FeeSize < 0) throw new Exception("Cannot add settings with negative fee size");
        }
    }
}
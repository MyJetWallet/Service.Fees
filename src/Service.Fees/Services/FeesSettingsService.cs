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

namespace Service.Fees.Services
{
    public class FeesSettingsService : IFeesSettingsService
    {
        private readonly IMyNoSqlServerDataWriter<FeesSettingsNoSqlEntity> _writer;
        private readonly ILogger<FeesSettingsService> _logger;

        public FeesSettingsService(IMyNoSqlServerDataWriter<FeesSettingsNoSqlEntity> writer,
            ILogger<FeesSettingsService> logger)
        {
            _writer = writer;
            _logger = logger;
        }

        public async Task<List<FeesSettings>> GetFeesSettingsList()
        {
            var entities = await _writer.GetAsync();
            return entities.Select(e => e.FeesSettings).ToList();
        }

        public async Task AddFeesSettings(FeesSettings settings)
        {
            using var action = MyTelemetry.StartActivity("Add Fees Settings Settings");
            settings.AddToActivityAsJsonTag("settings");
            try
            {
                _logger.LogInformation("Add Fees Settings Setting: {jsonText}",
                    JsonConvert.SerializeObject(settings));
                
                ValidateSettings(settings);
                
                var entity = FeesSettingsNoSqlEntity.Create(settings);

                var existingItem = await _writer.GetAsync(entity.PartitionKey, entity.RowKey);
                if (existingItem != null) throw new Exception("Cannot add Fees Settings Settings. Already exist");

                await _writer.InsertAsync(entity);
                
                _logger.LogInformation("Added Fees Settings Setting: {jsonText}",
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

        public async Task UpdateFeesSettings(FeesSettings settings)
        {
            using var action = MyTelemetry.StartActivity("Update Fees Settings Settings");
            settings.AddToActivityAsJsonTag("settings");
            try
            {
                _logger.LogInformation("Update Fees Settings Setting: {jsonText}",
                    JsonConvert.SerializeObject(settings));
                
                ValidateSettings(settings);
                
                var entity = FeesSettingsNoSqlEntity.Create(settings);

                var existingItem = await _writer.GetAsync(entity.PartitionKey, entity.RowKey);
                if (existingItem == null) throw new Exception("Cannot update Fees Settings Settings. Do not exist");

                await _writer.InsertOrReplaceAsync(entity);
                
                _logger.LogInformation("Updated Fees Settings Setting: {jsonText}",
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

        public async Task RemoveFeesSettings(RemoveFeesSettingsRequest request)
        {
            using var action = MyTelemetry.StartActivity("Remove Fees Settings Settings");
            request.AddToActivityAsJsonTag("request");
            try
            {
                _logger.LogInformation("Remove Fees Settings Setting: {jsonText}",
                    JsonConvert.SerializeObject(request));

                var entity = await _writer.DeleteAsync(FeesSettingsNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                    FeesSettingsNoSqlEntity.GenerateRowKey());
                
                if (entity != null)
                    _logger.LogInformation("Removed Fees Settings Settings: {jsonText}",
                        JsonConvert.SerializeObject(entity));
                else 
                    _logger.LogInformation("Unable to remove Fees Settings Setting, do not exist: {jsonText}",
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

        private static void ValidateSettings(FeesSettings settings)
        {
            if (string.IsNullOrEmpty(settings.BrokerId)) throw new Exception("Cannot add settings with empty broker");
            if (string.IsNullOrEmpty(settings.AccountId)) throw new Exception("Cannot add settings with empty account");
            if (string.IsNullOrEmpty(settings.WalletId)) throw new Exception("Cannot add settings with empty wallet");
        }
    }
}
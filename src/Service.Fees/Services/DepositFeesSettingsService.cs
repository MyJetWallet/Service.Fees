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
    public class DepositFeesSettingsService : IDepositFeesSettingsService
    {
        private readonly ILogger<DepositFeesSettingsService> _logger;
        private readonly IMyNoSqlServerDataWriter<DepositFeesNoSqlEntity> _writer;

        public DepositFeesSettingsService(ILogger<DepositFeesSettingsService> logger,
            IMyNoSqlServerDataWriter<DepositFeesNoSqlEntity> writer)
        {
            _logger = logger;
            _writer = writer;
        }

        public async Task<List<DepositFees>> GetDepositFeesSettingsList()
        {
            var entities = await _writer.GetAsync();
            return entities.Select(e => e.DepositFees).ToList();
        }

        public async Task AddDepositFeesSettings(DepositFees settings)
        {
            using var action = MyTelemetry.StartActivity("Add Deposit Fees Settings");
            settings.AddToActivityAsJsonTag("settings");
            try
            {
                _logger.LogInformation("Add Deposit Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(settings));
                
                ValidateSettings(settings);
                
                var entity = DepositFeesNoSqlEntity.Create(settings);

                var existingItem = await _writer.GetAsync(entity.PartitionKey, entity.RowKey);
                if (existingItem != null) throw new Exception("Cannot add Deposit Fees Settings. Already exist");

                await _writer.InsertAsync(entity);
                
                _logger.LogInformation("Added Deposit Fees Setting: {jsonText}",
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

        public async Task UpdateDepositFeesSettings(DepositFees settings)
        {
            using var action = MyTelemetry.StartActivity("Update Deposit Fees Settings");
            settings.AddToActivityAsJsonTag("settings");
            try
            {
                _logger.LogInformation("Update Deposit Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(settings));
                
                ValidateSettings(settings);
                
                var entity = DepositFeesNoSqlEntity.Create(settings);

                var existingItem = await _writer.GetAsync(entity.PartitionKey, entity.RowKey);
                if (existingItem == null) throw new Exception("Cannot update Deposit Fees Settings. Do not exist");

                await _writer.InsertOrReplaceAsync(entity);
                
                _logger.LogInformation("Updated Deposit Fees Setting: {jsonText}",
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

        public async Task RemoveDepositFeesSettings(RemoveDepositFeesRequest request)
        {
            using var action = MyTelemetry.StartActivity("Remove Deposit Fees Settings");
            request.AddToActivityAsJsonTag("request");
            try
            {
                _logger.LogInformation("Remove Deposit Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(request));

                var entity = await _writer.DeleteAsync(DepositFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId, request.ProfileId),
                    DepositFeesNoSqlEntity.GenerateRowKey(request.AssetId, request.AssetNetwork));
                
                if (entity != null)
                    _logger.LogInformation("Removed Deposit Fees Settings: {jsonText}",
                        JsonConvert.SerializeObject(entity));
                else 
                    _logger.LogInformation("Unable to remove Deposit Fees Setting, do not exist: {jsonText}",
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

        private static void ValidateSettings(DepositFees settings)
        {
            if (string.IsNullOrEmpty(settings.BrokerId)) throw new Exception("Cannot add settings with empty broker");
            if (string.IsNullOrEmpty(settings.ProfileId)) throw new Exception("Cannot add settings with empty profile");
            if (string.IsNullOrEmpty(settings.AssetId)) throw new Exception("Cannot add settings with empty asset");
            if (settings.FeeSizeRelative < 0 || settings.FeeSizeAbsolute < 0) throw new Exception("Cannot add settings with negative fee size");
        }
    }
}
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
    public class SpotInstrumentFeesSettingsService : ISpotInstrumentFeesSettingsService
    {
        private readonly ILogger<SpotInstrumentFeesSettingsService> _logger;
        private readonly IMyNoSqlServerDataWriter<SpotInstrumentFeesNoSqlEntity> _writer;

        public SpotInstrumentFeesSettingsService(ILogger<SpotInstrumentFeesSettingsService> logger,
            IMyNoSqlServerDataWriter<SpotInstrumentFeesNoSqlEntity> writer)
        {
            _logger = logger;
            _writer = writer;
        }

        public async Task<List<SpotInstrumentFees>> GetSpotInstrumentFeesSettingsList()
        {
            var entities = await _writer.GetAsync();
            return entities.Select(e => e.SpotInstrumentFees).ToList();
        }

        public async Task AddSpotInstrumentFeesSettings(SpotInstrumentFees settings)
        {
            using var action = MyTelemetry.StartActivity("Add Spot Instrument Fees Settings");
            settings.AddToActivityAsJsonTag("settings");
            try
            {
                _logger.LogInformation("Add Spot Instrument Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(settings));
                
                ValidateSettings(settings);
                
                var entity = SpotInstrumentFeesNoSqlEntity.Create(settings);

                var existingItem = await _writer.GetAsync(entity.PartitionKey, entity.RowKey);
                if (existingItem != null) throw new Exception("Cannot add Spot Instrument Fees Settings. Already exist");

                await _writer.InsertAsync(entity);
                
                _logger.LogInformation("Added Spot Instrument Fees Setting: {jsonText}",
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

        public async Task UpdateSpotInstrumentFeesSettings(SpotInstrumentFees settings)
        {
            using var action = MyTelemetry.StartActivity("Update Spot Instrument Fees Settings");
            settings.AddToActivityAsJsonTag("settings");
            try
            {
                _logger.LogInformation("Update Spot Instrument Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(settings));
                
                ValidateSettings(settings);
                
                var entity = SpotInstrumentFeesNoSqlEntity.Create(settings);

                var existingItem = await _writer.GetAsync(entity.PartitionKey, entity.RowKey);
                if (existingItem == null) throw new Exception("Cannot update Spot Instrument Fees Settings. Do not exist");

                await _writer.InsertOrReplaceAsync(entity);
                
                _logger.LogInformation("Updated Spot Instrument Fees Setting: {jsonText}",
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

        public async Task RemoveSpotInstrumentFeesSettings(RemoveSpotInstrumentFeesRequest request)
        {
            using var action = MyTelemetry.StartActivity("Remove Spot Instrument Fees Settings");
            request.AddToActivityAsJsonTag("request");
            try
            {
                _logger.LogInformation("Remove Spot Instrument Fees Setting: {jsonText}",
                    JsonConvert.SerializeObject(request));

                var entity = await _writer.DeleteAsync(SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                    SpotInstrumentFeesNoSqlEntity.GenerateRowKey(request.SpotInstrumentId));
                
                if (entity != null)
                    _logger.LogInformation("Removed Spot Instrument Fees Settings: {jsonText}",
                        JsonConvert.SerializeObject(entity));
                else 
                    _logger.LogInformation("Unable to remove Spot Instrument Fees Setting, do not exist: {jsonText}",
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

        private static void ValidateSettings(SpotInstrumentFees settings)
        {
            if (string.IsNullOrEmpty(settings.BrokerId)) throw new Exception("Cannot add settings with empty broker");
            if (string.IsNullOrEmpty(settings.SpotInstrumentId)) throw new Exception("Cannot add settings with empty instrument");
            if (settings.MakerFeeSize < 0) throw new Exception("Cannot add settings with negative maker fee size");
            if (settings.TakerFeeSize < 0) throw new Exception("Cannot add settings with negative taker fee size");
        }
    }
}
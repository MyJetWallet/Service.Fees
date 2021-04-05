using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain.Assets;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using Service.AssetsDictionary.Client;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc;
using Service.Fees.Grpc.Models;
using Service.Fees.MyNoSql;

namespace Service.Fees.Services
{
    public class SpotInstrumentFeesService : ISpotInstrumentFeesService
    {
        private readonly IMyNoSqlServerDataWriter<SpotInstrumentFeesNoSqlEntity> _writer;
        private readonly IMyNoSqlServerDataWriter<FeesSettingsNoSqlEntity> _feesSettings;
        private readonly ILogger<SpotInstrumentFeesService> _logger;
        private readonly ISpotInstrumentDictionaryClient _spotInstrumentDictionaryClient;

        public SpotInstrumentFeesService(IMyNoSqlServerDataWriter<SpotInstrumentFeesNoSqlEntity> writer,
            IMyNoSqlServerDataWriter<FeesSettingsNoSqlEntity> feesSettings,
            ILogger<SpotInstrumentFeesService> logger,
            ISpotInstrumentDictionaryClient spotInstrumentDictionaryClient)
        {
            _writer = writer;
            _feesSettings = feesSettings;
            _logger = logger;
            _spotInstrumentDictionaryClient = spotInstrumentDictionaryClient;
        }

        public async Task<FeesResponse<SpotInstrumentFees>> SetSpotInstrumentFeesAsync(
            SpotInstrumentFees spotInstrumentFees)
        {
            spotInstrumentFees.BrokerId.AddToActivityAsTag("brokerId");
            spotInstrumentFees.WalletId.AddToActivityAsTag("walletId");
            spotInstrumentFees.AccountId.AddToActivityAsTag("clientId");

            _logger.LogInformation("Receive SetSpotInstrumentFeesAsync request: {jsonText}",
                JsonConvert.SerializeObject(spotInstrumentFees));

            if (string.IsNullOrEmpty(spotInstrumentFees.BrokerId))
                return FeesResponse<SpotInstrumentFees>.Error(
                    "Cannot create/update spot instrument fees. BrokerId cannot be empty");
            if (string.IsNullOrEmpty(spotInstrumentFees.SpotInstrumentId))
                return FeesResponse<SpotInstrumentFees>.Error(
                    "Cannot create/update spot instrument fees. SpotInstrumentId cannot be empty");

            var instrument = _spotInstrumentDictionaryClient.GetSpotInstrumentById(new SpotInstrumentIdentity()
                {BrokerId = spotInstrumentFees.BrokerId, Symbol = spotInstrumentFees.SpotInstrumentId});

            if (instrument == null)
                return FeesResponse<SpotInstrumentFees>.Error(
                    "Cannot create spot instrument fees. Spot instrument do not found");

            var entity = SpotInstrumentFeesNoSqlEntity.Create(spotInstrumentFees);

            await _writer.InsertOrReplaceAsync(entity);

            _logger.LogInformation(
                "Spot Instrument fees are created. BrokerId: {brokerId}, Spot Instrument: {spotInstrument}",
                entity.BrokerId,
                entity.SpotInstrumentId);

            return FeesResponse<SpotInstrumentFees>.Success(SpotInstrumentFees.Create(entity.SpotInstrumentFees));
            
        }

        public async Task<NullableValue<SpotInstrumentFees>> GetSpotInstrumentFeesAsync(
            GetSpotInstrumentFeesRequest request)
        {
            try
            {
                var entity = await _writer.GetAsync(
                    SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                    SpotInstrumentFeesNoSqlEntity.GenerateRowKey(request.SpotInstrumentId));

                if (entity == null)
                    return new NullableValue<SpotInstrumentFees>();

                var instrumentFees = entity.SpotInstrumentFees;

                if (string.IsNullOrEmpty(instrumentFees.AccountId) || string.IsNullOrEmpty(instrumentFees.WalletId))
                {
                    var settings = await _feesSettings.GetAsync(
                        FeesSettingsNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                        FeesSettingsNoSqlEntity.GenerateRowKey());
                    instrumentFees.AccountId = settings.FeesSettings.AccountId;
                    instrumentFees.WalletId = settings.FeesSettings.WalletId;
                }

                return new NullableValue<SpotInstrumentFees>(SpotInstrumentFees.Create(entity.SpotInstrumentFees));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown HTTP result CodeNotFound. Message: Row not found")
                    return new NullableValue<SpotInstrumentFees>();
                throw;
            }
        }
    }
}
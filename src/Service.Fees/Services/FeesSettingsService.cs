using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        public async Task<FeesResponse<FeesSettings>> SetFeesSettingsAsync(FeesSettings feesSettings)
        {
            _logger.LogInformation("Receive SetFeesSettingsAsync request: {jsonText}",
                JsonConvert.SerializeObject(feesSettings));

            if (string.IsNullOrEmpty(feesSettings.BrokerId))
                return FeesResponse<FeesSettings>.Error("Cannot create/update fees settings. BrokerId cannot be empty");
            if (string.IsNullOrEmpty(feesSettings.AccountId))
                return FeesResponse<FeesSettings>.Error(
                    "Cannot create/update fees settings. AccountId cannot be empty");
            if (string.IsNullOrEmpty(feesSettings.WalletId))
                return FeesResponse<FeesSettings>.Error("Cannot create/update fees settings. WalletId cannot be empty");

            var entity = FeesSettingsNoSqlEntity.Create(feesSettings);

            await _writer.InsertOrReplaceAsync(entity);

            _logger.LogInformation(
                "Fees settings are saved. BrokerId: {brokerId}",
                entity.BrokerId);

            return FeesResponse<FeesSettings>.Success(FeesSettings.Create(entity.FeesSettings));
        }

        public async Task<NullableValue<FeesSettings>> GetFeesSettingsAsync(GetFeesSettingsRequest request)
        {
            try
            {
                var entity = await _writer.GetAsync(FeesSettingsNoSqlEntity.GeneratePartitionKey(request.BrokerId),
                    FeesSettingsNoSqlEntity.GenerateRowKey());

                if (entity == null)
                    return new NullableValue<FeesSettings>();

                return new NullableValue<FeesSettings>(FeesSettings.Create(entity.FeesSettings));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown HTTP result CodeNotFound. Message: Row not found")
                    return new NullableValue<FeesSettings>();
                throw;
            }
        }
    }
}
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.Fees.Domain.Models;
using Service.Fees.MyNoSql;

// ReSharper disable UnusedMember.Global

namespace Service.Fees.Client
{
    public interface ISpotInstrumentFeesClient
    {
        SpotInstrumentFees GetSpotInstrumentFees(string brokerId, string spotInstrumentId);
    }

    public class SpotInstrumentFeesClient : ISpotInstrumentFeesClient
    {
        private readonly IMyNoSqlServerDataReader<SpotInstrumentFeesNoSqlEntity> _spotInstrumentsReader;
        private readonly IMyNoSqlServerDataReader<FeesSettingsNoSqlEntity> _feesSettingsReader;

        public SpotInstrumentFeesClient(IMyNoSqlServerDataReader<SpotInstrumentFeesNoSqlEntity> spotInstrumentsReader,
            IMyNoSqlServerDataReader<FeesSettingsNoSqlEntity> feesSettingsReader)
        {
            _spotInstrumentsReader = spotInstrumentsReader;
            _feesSettingsReader = feesSettingsReader;
        }

        public SpotInstrumentFees GetSpotInstrumentFees(string brokerId,  string spotInstrumentId)
        {
            var entity = _spotInstrumentsReader.Get(SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey(brokerId),
                SpotInstrumentFeesNoSqlEntity.GenerateRowKey(spotInstrumentId));

            if (entity == null)
            {
                entity = _spotInstrumentsReader.Get(SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey(brokerId),
                    SpotInstrumentFeesNoSqlEntity.GenerateRowKey(SpotInstrumentFeesNoSqlEntity.DEFAULT_FEES));
                if (entity == null)
                {
                    return new SpotInstrumentFees {FeeType = FeeType.NoFee};
                }
            }

            var result = entity.SpotInstrumentFees;

            if (string.IsNullOrEmpty(result.AccountId) ||
                string.IsNullOrEmpty(result.WalletId))
            {
                var settings = _feesSettingsReader.Get(FeesSettingsNoSqlEntity.GeneratePartitionKey(brokerId),
                    FeesSettingsNoSqlEntity.GenerateRowKey());
                if (settings != null)
                {
                    result.BrokerId = settings.FeesSettings.BrokerId;
                    result.AccountId = settings.FeesSettings.AccountId;
                    result.WalletId = settings.FeesSettings.WalletId;
                }
            }

            return result;
        }
    }
}
using MyNoSqlServer.DataReader;
using Service.Fees.Domain.Models;
using Service.Fees.MyNoSql;

// ReSharper disable UnusedMember.Global

namespace Service.Fees.Client
{
    public interface ISpotInstrumentFeesClient
    {
        SpotInstrumentFees GetSpotInstrumentFees(string brokerId, string groupId, string spotInstrumentId);
    }

    public class SpotInstrumentFeesClient : ISpotInstrumentFeesClient
    {
        private readonly MyNoSqlReadRepository<SpotInstrumentFeesNoSqlEntity> _spotInstrumentsReader;
        private readonly MyNoSqlReadRepository<FeesSettingsNoSqlEntity> _feesSettingsReader;

        public SpotInstrumentFeesClient(MyNoSqlReadRepository<SpotInstrumentFeesNoSqlEntity> spotInstrumentsReader,
            MyNoSqlReadRepository<FeesSettingsNoSqlEntity> feesSettingsReader)
        {
            _spotInstrumentsReader = spotInstrumentsReader;
            _feesSettingsReader = feesSettingsReader;
        }

        public SpotInstrumentFees GetSpotInstrumentFees(string brokerId, string groupId, string spotInstrumentId)
        {
            var entity = _spotInstrumentsReader.Get(SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey(brokerId, groupId),
                SpotInstrumentFeesNoSqlEntity.GenerateRowKey(spotInstrumentId));

            if (entity == null)
            {
                entity = _spotInstrumentsReader.Get(SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey(brokerId, groupId),
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
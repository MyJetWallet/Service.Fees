using MyNoSqlServer.DataReader;
using Service.Fees.Domain.Models;
using Service.Fees.MyNoSql;

// ReSharper disable UnusedMember.Global

namespace Service.Fees.Client
{
    public interface IAssetFeesClient
    {
        AssetFees GetAssetFees(string brokerId, string groupId,  string assetId, OperationType operationType);
    }

    public class AssetFeesClient : IAssetFeesClient
    {
        private readonly MyNoSqlReadRepository<AssetFeesNoSqlEntity> _assetFeesReader;
        private readonly MyNoSqlReadRepository<FeesSettingsNoSqlEntity> _feesSettingsReader;


        public AssetFeesClient(MyNoSqlReadRepository<AssetFeesNoSqlEntity> assetFeesReader,
            MyNoSqlReadRepository<FeesSettingsNoSqlEntity> feesSettingsReader)
        {
            _assetFeesReader = assetFeesReader;
            _feesSettingsReader = feesSettingsReader;
        }

        public AssetFees GetAssetFees(string brokerId, string groupId, string assetId, OperationType operationType)
        {
            var entity = _assetFeesReader.Get(AssetFeesNoSqlEntity.GeneratePartitionKey(brokerId, groupId),
                AssetFeesNoSqlEntity.GenerateRowKey(assetId, operationType));

            if (entity == null)
            {
                return new AssetFees {FeeType = FeeType.NoFee};
            }

            var result = entity.AssetFees;

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
            
            return entity?.AssetFees;
        }
    }
}
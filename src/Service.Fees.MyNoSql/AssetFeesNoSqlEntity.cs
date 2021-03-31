using MyNoSqlServer.Abstractions;
using Service.Fees.Domain.Models;

namespace Service.Fees.MyNoSql
{
    public class AssetFeesNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-fees-assets";

        public static string GeneratePartitionKey(string brokerId) => $"broker:{brokerId}";

        public static string GenerateRowKey(string asset, OperationType operationType) =>
            $"{asset}:{operationType.ToString()}";

        public static AssetFeesNoSqlEntity Create(AssetFees assetFees)
        {
            return new AssetFeesNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(assetFees.BrokerId),
                RowKey = GenerateRowKey(assetFees.AssetId, assetFees.OperationType),
                BrokerId = assetFees.BrokerId,
                AssetId = assetFees.AssetId,
                OperationType = assetFees.OperationType,
                AssetFees = assetFees
            };
        }

        public string BrokerId { get; set; }
        public string AssetId { get; set; }
        public OperationType OperationType { get; set; }
        public AssetFees AssetFees { get; set; }
    }
}
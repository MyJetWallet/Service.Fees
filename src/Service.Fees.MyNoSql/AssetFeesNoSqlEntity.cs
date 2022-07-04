using MyNoSqlServer.Abstractions;
using Service.Fees.Domain.Models;
using System;

namespace Service.Fees.MyNoSql
{
    public class AssetFeesNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-fees-assets-v2";

        public static string GeneratePartitionKey(string brokerId, string profile) => $"broker:{brokerId}|profile:{profile}";

        [Obsolete]
        public static string GenerateRowKeyLegacy(string asset, OperationType operationType) =>
            $"{asset}:{operationType}";

        public static string GenerateRowKey(string asset, OperationType operationType, string assetNetwork) =>
            $"{asset}:{operationType}:{assetNetwork}";

        public static AssetFeesNoSqlEntity Create(AssetFees assetFees)
        {
            return new AssetFeesNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(assetFees.BrokerId, assetFees.ProfileId),
                RowKey = GenerateRowKey(assetFees.AssetId, assetFees.OperationType, assetFees.AssetNetwork),
                BrokerId = assetFees.BrokerId,
                AssetId = assetFees.AssetId,
                AssetNetwork = assetFees.AssetNetwork,
                OperationType = assetFees.OperationType,
                AssetFees = assetFees
            };
        }

        public string BrokerId { get; set; }
        public string AssetId { get; set; }
        public string AssetNetwork { get; set; }
        public OperationType OperationType { get; set; }
        public AssetFees AssetFees { get; set; }
    }
}
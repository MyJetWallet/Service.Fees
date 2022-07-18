using MyNoSqlServer.Abstractions;
using Service.Fees.Domain.Models;
using System;

namespace Service.Fees.MyNoSql
{
    public class DepositFeesNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-fees-deposits-v2";

        public static string GeneratePartitionKey(string brokerId, string profile) => $"broker:{brokerId}|profile:{profile}";

        public static string GenerateRowKey(string asset, string assetNetwork)
        {
            return string.IsNullOrEmpty(assetNetwork) ? $"{asset}" : $"{asset}:{assetNetwork}";
        }

        public static DepositFeesNoSqlEntity Create(DepositFees depositFees)
        {
            return new DepositFeesNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(depositFees.BrokerId, depositFees.ProfileId),
                RowKey = GenerateRowKey(depositFees.AssetId, depositFees.AssetNetwork),
                BrokerId = depositFees.BrokerId,
                AssetId = depositFees.AssetId,
                DepositFees = depositFees
            };
        }

        public string BrokerId { get; set; }
        public string AssetId { get; set; }
        public DepositFees DepositFees { get; set; }

        public static string GenerateRowKey(string assetId, object assetNetwork)
        {
            throw new NotImplementedException();
        }
    }
}
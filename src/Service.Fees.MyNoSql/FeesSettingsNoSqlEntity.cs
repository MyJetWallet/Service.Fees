using MyNoSqlServer.Abstractions;
using Service.Fees.Domain.Models;

namespace Service.Fees.MyNoSql
{
    public class FeesSettingsNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-fees-settings";

        public static string GeneratePartitionKey(string brokerId) => $"broker:{brokerId}";
        public static string GenerateRowKey() => "settings";

        public static FeesSettingsNoSqlEntity Create(FeesSettings feesSettings)
        {
            return new FeesSettingsNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(feesSettings.BrokerId),
                RowKey = GenerateRowKey(),
                BrokerId = feesSettings.BrokerId,
                FeesSettings = feesSettings
            };
        }

        public string BrokerId { get; set; }
        public FeesSettings FeesSettings { get; set; }
    }
}
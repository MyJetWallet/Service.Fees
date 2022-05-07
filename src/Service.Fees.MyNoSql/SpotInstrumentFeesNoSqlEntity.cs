using MyNoSqlServer.Abstractions;
using Service.Fees.Domain.Models;

namespace Service.Fees.MyNoSql
{
    public class SpotInstrumentFeesNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-fees-spot-instrument-v2";

        public static string GeneratePartitionKey(string brokerId, string group) => $"broker:{brokerId}|group:{group}";
        public static string GenerateRowKey(string spotInstrument) => $"{spotInstrument}";

        public static string DEFAULT_FEES = "default";

        public static SpotInstrumentFeesNoSqlEntity Create(SpotInstrumentFees spotInstrumentFees)
        {
            return new()
            {
                PartitionKey = GeneratePartitionKey(spotInstrumentFees.BrokerId, spotInstrumentFees.GroupId),
                RowKey = GenerateRowKey(spotInstrumentFees.SpotInstrumentId),
                BrokerId = spotInstrumentFees.BrokerId,
                SpotInstrumentId = spotInstrumentFees.SpotInstrumentId,
                SpotInstrumentFees = spotInstrumentFees
            };
        }

        public string BrokerId { get; set; }
        public string SpotInstrumentId { get; set; }
        public SpotInstrumentFees SpotInstrumentFees { get; set; }
    }
}
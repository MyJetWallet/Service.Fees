using System.Collections.Generic;
using MyNoSqlServer.Abstractions;
using Service.Fees.Domain.Models;

namespace Service.Fees.MyNoSql
{
    public class FeeProfilesNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-fees-profiles";

        public static string GeneratePartitionKey() => "FeeProfiles";

        public static string GenerateRowKey() => "FeeProfiles";

        public static FeeProfilesNoSqlEntity Create(List<string> profiles)
        {
            return new FeeProfilesNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(),
                Profiles = profiles
            };
        }

        public List<string> Profiles { get; set; }
    }
}
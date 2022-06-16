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

        public static FeeProfilesNoSqlEntity Create(List<string> profiles, List<string> depositProfiles)
        {
            return new FeeProfilesNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(),
                Profiles = profiles,
                DepositProfiles = depositProfiles
            };
        }

        public List<string> Profiles { get; set; }
        public List<string> DepositProfiles { get; set; }

    }
}
using System.Collections.Generic;
using MyNoSqlServer.Abstractions;
using Service.Fees.Domain.Models;

namespace Service.Fees.MyNoSql
{
    public class GroupsNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-fees-groups";

        public static string GeneratePartitionKey() => "FeeGroups";

        public static string GenerateRowKey() => "FeeGroups";

        public static GroupsNoSqlEntity Create(List<string> groups)
        {
            return new GroupsNoSqlEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(),
                Groups = groups
            };
        }

        public List<string> Groups { get; set; }
    }
}
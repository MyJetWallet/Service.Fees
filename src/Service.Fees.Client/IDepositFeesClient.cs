using System.Collections.Generic;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.Fees.Domain.Models;
using Service.Fees.MyNoSql;

// ReSharper disable UnusedMember.Global

namespace Service.Fees.Client
{
    public interface IDepositFeesClient
    {
        DepositFees GetDepositFees(string brokerId, string groupId,  string assetId);
    }

    public class DepositFeesClient : IDepositFeesClient
    {
        private readonly IMyNoSqlServerDataReader<DepositFeesNoSqlEntity> _depositFeesReader;

        private readonly Dictionary<string, DepositFees> _mock = new Dictionary<string, DepositFees>()
        {
            {
                "BTC", new DepositFees
                {
                    BrokerId = "jetwallet",
                    AssetId = "BTC",
                    FeeType = FeeType.ClientFee,
                    FeeSizeType = FeeSizeType.Percentage,
                    FeeSizeAbsolute = 0,
                    FeeSizeRelative = 10,
                    FeeAssetId = "BTC",
                    ProfileId = "DEFAULT"
                }
            },
            {
                "USDC", new DepositFees
                {
                    BrokerId = "jetwallet",
                    AssetId = "USDC",
                    FeeType = FeeType.ClientFee,
                    FeeSizeType = FeeSizeType.Absolute,
                    FeeSizeAbsolute = 1,
                    FeeSizeRelative = 0,
                    FeeAssetId = "USDC",
                    ProfileId = "DEFAULT"
                }
            },
            {
                "USDT", new DepositFees
                {
                    BrokerId = "jetwallet",
                    AssetId = "USDT",
                    FeeType = FeeType.ClientFee,
                    FeeSizeType = FeeSizeType.Composite,
                    FeeSizeAbsolute = 0.5m,
                    FeeSizeRelative = 3,
                    FeeAssetId = "USDT",
                    ProfileId = "DEFAULT"
                }
            },
        };

        public DepositFeesClient(IMyNoSqlServerDataReader<DepositFeesNoSqlEntity> depositFeesReader)
        {
            _depositFeesReader = depositFeesReader;
        }

        public DepositFees GetDepositFees(string brokerId, string groupId, string assetId)
        {
            var entity = _depositFeesReader.Get(DepositFeesNoSqlEntity.GeneratePartitionKey(brokerId, groupId),
                 DepositFeesNoSqlEntity.GenerateRowKey(assetId));

            if (entity == null)
            {
                return new DepositFees {FeeType = FeeType.NoFee};
            }
            
            return entity?.DepositFees;
        }
    }
}
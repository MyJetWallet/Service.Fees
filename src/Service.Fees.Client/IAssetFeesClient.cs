using System;
using MyNoSqlServer.DataReader;
using Service.Fees.Domain.Models;
using Service.Fees.MyNoSql;

// ReSharper disable UnusedMember.Global

namespace Service.Fees.Client
{
    public interface IAssetFeesClient
    {
        AssetFees GetAssetFees(string brokerId, string assetId, OperationType operationType);

        event Action OnChanged;
    }

    public class AssetFeesClient : IAssetFeesClient
    {
        private readonly MyNoSqlReadRepository<AssetFeesNoSqlEntity> _reader;

        public AssetFeesClient(MyNoSqlReadRepository<AssetFeesNoSqlEntity> reader)
        {
            _reader = reader;
            _reader.SubscribeToUpdateEvents(list => Changed(), list => Changed());
        }

        public AssetFees GetAssetFees(string brokerId, string assetId, OperationType operationType)
        {
            var entity = _reader.Get(AssetFeesNoSqlEntity.GeneratePartitionKey(brokerId),
                AssetFeesNoSqlEntity.GenerateRowKey(assetId, operationType));
            return entity?.AssetFees;
        }

        public event Action OnChanged;

        private void Changed()
        {
            OnChanged?.Invoke();
        }
    }
}
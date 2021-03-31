using System;
using MyNoSqlServer.DataReader;
using Service.Fees.Domain.Models;
using Service.Fees.MyNoSql;

// ReSharper disable UnusedMember.Global

namespace Service.Fees.Client
{
    public interface IFeesSettingsClient
    {
        FeesSettings GetFeesSettings(string brokerId);

        event Action OnChanged;
    }

    public class FeesSettingsClient : IFeesSettingsClient
    {
        private readonly MyNoSqlReadRepository<FeesSettingsNoSqlEntity> _reader;

        public FeesSettingsClient(MyNoSqlReadRepository<FeesSettingsNoSqlEntity> reader)
        {
            _reader = reader;
            _reader.SubscribeToUpdateEvents(list => Changed(), list => Changed());
        }

        public FeesSettings GetFeesSettings(string brokerId)
        {
            var entity = _reader.Get(FeesSettingsNoSqlEntity.GeneratePartitionKey(brokerId),
                FeesSettingsNoSqlEntity.GenerateRowKey());
            return entity?.FeesSettings;
        }

        public event Action OnChanged;

        private void Changed()
        {
            OnChanged?.Invoke();
        }
    }
}
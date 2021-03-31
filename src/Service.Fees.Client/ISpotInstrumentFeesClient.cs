using System;
using MyNoSqlServer.DataReader;
using Service.Fees.Domain.Models;
using Service.Fees.MyNoSql;

// ReSharper disable UnusedMember.Global

namespace Service.Fees.Client
{
    public interface ISpotInstrumentFeesClient
    {
        SpotInstrumentFees GetSpotInstrumentFees(string brokerId, string spotInstrumentId);

        event Action OnChanged;
    }

    public class SpotInstrumentFeesClient : ISpotInstrumentFeesClient
    {
        private readonly MyNoSqlReadRepository<SpotInstrumentFeesNoSqlEntity> _reader;

        public SpotInstrumentFeesClient(MyNoSqlReadRepository<SpotInstrumentFeesNoSqlEntity> reader)
        {
            _reader = reader;
            _reader.SubscribeToUpdateEvents(list => Changed(), list => Changed());
        }

        public SpotInstrumentFees GetSpotInstrumentFees(string brokerId, string spotInstrumentId)
        {
            var entity = _reader.Get(SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey(brokerId),
                SpotInstrumentFeesNoSqlEntity.GenerateRowKey(spotInstrumentId));
            return entity?.SpotInstrumentFees;
        }

        public event Action OnChanged;

        private void Changed()
        {
            OnChanged?.Invoke();
        }
    }
}
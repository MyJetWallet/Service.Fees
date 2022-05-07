using System;
using System.Threading.Tasks;
using Service.Fees.Client;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Services
{
    public class SpotInstrumentFeesService : ISpotInstrumentFeesService
    {
        private readonly ISpotInstrumentFeesClient _client;

        public SpotInstrumentFeesService(ISpotInstrumentFeesClient client)
        {
            _client = client;
        }

        public async Task<NullableValue<SpotInstrumentFees>> GetSpotInstrumentFeesAsync(
            GetSpotInstrumentFeesRequest request)
        {
            try
            {
                var fees = _client.GetSpotInstrumentFees(request.BrokerId, request.SpotInstrumentId, request.GroupId);

                return fees == null ? null : new NullableValue<SpotInstrumentFees>(SpotInstrumentFees.Create(fees));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown HTTP result CodeNotFound. Message: Row not found")
                    return new NullableValue<SpotInstrumentFees>();
                throw;
            }
        }
    }
}
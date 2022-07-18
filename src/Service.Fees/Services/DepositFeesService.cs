using System;
using System.Threading.Tasks;
using Service.Fees.Client;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Services
{
    public class DepositFeesService : IDepositFeesService
    {
        private readonly IDepositFeesClient _client;

        public DepositFeesService(IDepositFeesClient client)
        {
            _client = client;
        }

        public async Task<NullableValue<DepositFees>> GetDepositFees(GetDepositFeesRequest request)
        {
            try
            {
                var fees = _client.GetDepositFees(request.BrokerId, request.GroupId, request.AssetId, request.AssetNetwork);

                return fees == null ? null : new NullableValue<DepositFees>(DepositFees.Create(fees));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown HTTP result CodeNotFound. Message: Row not found")
                    return new NullableValue<DepositFees>();
                throw;
            }
        }
    }
}
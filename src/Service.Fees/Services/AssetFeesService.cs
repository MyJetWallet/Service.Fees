using System;
using System.Threading.Tasks;
using Service.Fees.Client;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Services
{
    public class AssetFeesService : IAssetFeesService
    {
        private readonly IAssetFeesClient _client;

        public AssetFeesService(IAssetFeesClient client)
        {
            _client = client;
        }

        public async Task<NullableValue<AssetFees>> GetAssetFeesWithNetwork(GetAssetFeesRequest request)
        {
            try
            {
                var fees = _client.GetAssetFeesWithNetwork(request.BrokerId, request.GroupId, request.AssetId, request.OperationType, request.AssetNetwork);

                return fees == null ? null : new NullableValue<AssetFees>(AssetFees.Create(fees));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown HTTP result CodeNotFound. Message: Row not found")
                    return new NullableValue<AssetFees>();
                throw;
            }
        }

        [Obsolete]
        public async Task<NullableValue<AssetFees>> GetAssetFees(GetAssetFeesRequest request)
        {
            try
            {
                var fees = _client.GetAssetFees(request.BrokerId, request.GroupId, request.AssetId, request.OperationType);

                return fees == null ? null : new NullableValue<AssetFees>(AssetFees.Create(fees));
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown HTTP result CodeNotFound. Message: Row not found")
                    return new NullableValue<AssetFees>();
                throw;
            }
        }
    }
}
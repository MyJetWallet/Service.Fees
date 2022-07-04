using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Grpc
{
    [ServiceContract]
    public interface IAssetFeesService
    {
        [OperationContract]
        Task<NullableValue<AssetFees>> GetAssetFeesWithNetwork(GetAssetFeesRequest request);

        [Obsolete]
        [OperationContract]
        Task<NullableValue<AssetFees>> GetAssetFees(GetAssetFeesRequest request);
    }
}
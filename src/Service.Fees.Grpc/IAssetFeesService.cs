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
        Task<FeesResponse<AssetFees>> SetAssetFeesAsync(AssetFees assetFees);

        [OperationContract]
        Task<NullableValue<AssetFees>> GetAssetFees(GetAssetFeesRequest request);
    }
}
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Grpc
{
    [ServiceContract]
    public interface IAssetFeesSettingsService
    {
        [OperationContract]
        Task<List<AssetFees>> GetAssetFeesSettingsList();

        [OperationContract]
        Task AddAssetFeesSettings(AssetFees settings);

        [OperationContract]
        Task UpdateAssetFeesSettings(AssetFees settings);

        [OperationContract]
        Task RemoveAssetFeesSettings(RemoveAssetFeesRequest request);
    }
}
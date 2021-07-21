using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Grpc
{
    [ServiceContract]
    public interface IFeesSettingsService
    {
        [OperationContract]
        Task<List<FeesSettings>> GetFeesSettingsList();
        
        [OperationContract]
        Task<FeesResponse<FeesSettings>> SetFeesSettingsAsync(FeesSettings feesSettings);

        [OperationContract]
        Task<NullableValue<FeesSettings>> GetFeesSettingsAsync(GetFeesSettingsRequest request);
    }
}
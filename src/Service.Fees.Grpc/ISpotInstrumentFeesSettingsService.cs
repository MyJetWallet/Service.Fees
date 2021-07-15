using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Grpc
{
    [ServiceContract]
    public interface ISpotInstrumentFeesSettingsService
    {
        [OperationContract]
        Task<List<SpotInstrumentFees>> GetSpotInstrumentFeesSettingsList();

        [OperationContract]
        Task AddSpotInstrumentFeesSettings(SpotInstrumentFees settings);

        [OperationContract]
        Task UpdateSpotInstrumentFeesSettings(SpotInstrumentFees settings);

        [OperationContract]
        Task RemoveSpotInstrumentFeesSettings(RemoveSpotInstrumentFeesRequest request);
    }
}
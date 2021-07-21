using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Grpc
{
    [ServiceContract]
    public interface ISpotInstrumentFeesService
    {
        [OperationContract]
        Task<NullableValue<SpotInstrumentFees>> GetSpotInstrumentFeesAsync(GetSpotInstrumentFeesRequest request);
    }
}
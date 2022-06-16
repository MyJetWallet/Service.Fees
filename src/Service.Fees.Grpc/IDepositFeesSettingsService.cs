using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Grpc
{
    [ServiceContract]
    public interface IDepositFeesSettingsService
    {
        [OperationContract]
        Task<List<DepositFees>> GetDepositFeesSettingsList();

        [OperationContract]
        Task AddDepositFeesSettings(DepositFees settings);

        [OperationContract]
        Task UpdateDepositFeesSettings(DepositFees settings);

        [OperationContract]
        Task RemoveDepositFeesSettings(RemoveDepositFeesRequest request);
    }
}
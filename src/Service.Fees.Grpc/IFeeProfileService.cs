using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Grpc;

[ServiceContract]
public interface IFeeProfileService
{
    [OperationContract] public Task<ProfilesResponse> GetAllProfiles();
    
    [OperationContract] public Task<OperationResponse> CreateWithdrawalProfile(CreateGroupRequest request);
    
    [OperationContract] public Task<OperationResponse> DeleteWithdrawalProfile(DeleteProfileRequest request);
    
    [OperationContract] public Task<OperationResponse> CreateDepositProfile(CreateGroupRequest request);
    
    [OperationContract] public Task<OperationResponse> DeleteDepositProfile(DeleteProfileRequest request);
}
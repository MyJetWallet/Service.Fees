using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Grpc;

[ServiceContract]
public interface IFeeProfileService
{
    [OperationContract] public Task<ProfilesResponse> GetAllProfiles();
    
    [OperationContract] public Task<OperationResponse> CreateProfile(CreateGroupRequest request);
    
    [OperationContract] public Task<OperationResponse> DeleteProfile(DeleteProfileRequest request);
}
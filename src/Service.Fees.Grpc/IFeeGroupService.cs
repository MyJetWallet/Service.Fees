using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fees.Grpc.Models;

namespace Service.Fees.Grpc;

[ServiceContract]
public interface IFeeGroupService
{
    [OperationContract] public Task<GroupsResponse> GetAllGroups();
    
    [OperationContract] public Task<OperationResposne> CreateGroup(CreateGroupRequest request);
    
    [OperationContract] public Task<OperationResposne> DeleteGroup(DeleteGroupRequest request);
}
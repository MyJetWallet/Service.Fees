using System.Runtime.Serialization;

namespace Service.Fees.Grpc.Models;

[DataContract]
public class CreateGroupRequest
{
    [DataMember (Order = 1)] public string ProfileId { get; set; }
    [DataMember (Order = 2)] public string CloneFromProfileId { get; set; }
    [DataMember (Order = 3)] public string BrokerId { get; set; }

}
using System.Runtime.Serialization;

namespace Service.Fees.Grpc.Models;

[DataContract]
public class DeleteGroupRequest
{
    [DataMember (Order = 1)] public string GroupId { get; set; }
    [DataMember (Order = 2)] public string BrokerId { get; set; }
}
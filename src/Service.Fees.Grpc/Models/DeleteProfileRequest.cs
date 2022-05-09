using System.Runtime.Serialization;

namespace Service.Fees.Grpc.Models;

[DataContract]
public class DeleteProfileRequest
{
    [DataMember (Order = 1)] public string ProfileId { get; set; }
    [DataMember (Order = 2)] public string BrokerId { get; set; }
}
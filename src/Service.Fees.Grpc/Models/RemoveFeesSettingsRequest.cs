using System.Runtime.Serialization;

namespace Service.Fees.Grpc.Models
{
    [DataContract]
    public class RemoveFeesSettingsRequest
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
    }
}
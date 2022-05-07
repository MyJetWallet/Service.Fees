using System.Runtime.Serialization;

namespace Service.Fees.Grpc.Models
{
    [DataContract]
    public class GetSpotInstrumentFeesRequest
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string SpotInstrumentId { get; set; }
        [DataMember(Order = 3)] public string GroupId { get; set; }

    }
}
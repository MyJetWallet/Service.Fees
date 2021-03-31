using System.Runtime.Serialization;

namespace Service.Fees.Domain.Models
{
    [DataContract]
    public enum FeeType
    {
        NoFee,
        ClientFee,
        ExternalFee
    }
}
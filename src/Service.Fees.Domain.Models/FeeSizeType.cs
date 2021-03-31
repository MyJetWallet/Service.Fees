using System.Runtime.Serialization;

namespace Service.Fees.Domain.Models
{
    [DataContract]
    public enum FeeSizeType
    {
        Percentage,
        Absolute
    }
}
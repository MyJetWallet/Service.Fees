using System.Runtime.Serialization;

namespace Service.Fees.Domain.Models
{
    [DataContract]
    public enum OperationType
    {
        Withdrawal,
        Order
    }
}
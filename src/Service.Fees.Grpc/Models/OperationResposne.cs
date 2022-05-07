using System.Runtime.Serialization;

namespace Service.Fees.Grpc.Models;

[DataContract]
public class OperationResposne
{
    [DataMember(Order = 1)] public bool IsSuccess { get; set; }
    
    [DataMember(Order = 2)] public string ErrorText { get; set; }
}
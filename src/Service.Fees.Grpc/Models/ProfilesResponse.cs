using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.Fees.Grpc.Models;

[DataContract]
public class ProfilesResponse
{
    [DataMember(Order = 1)] public List<string> WithdrawalProfiles { get; set; }
    [DataMember(Order = 2)] public List<string> DepositProfiles { get; set; }

}
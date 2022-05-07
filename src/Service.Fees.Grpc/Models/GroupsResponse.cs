using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.Fees.Grpc.Models;

[DataContract]
public class GroupsResponse
{
    [DataMember(Order = 1)] public List<string> Groups { get; set; }
}
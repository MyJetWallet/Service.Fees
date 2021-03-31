﻿using System.Runtime.Serialization;
using Service.Fees.Domain.Models;

namespace Service.Fees.Grpc.Models
{
    [DataContract]
    public class GetAssetFeesRequest
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string AssetId { get; set; }
        [DataMember(Order = 3)] public OperationType OperationType { get; set; }
    }
}
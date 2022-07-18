using System.Runtime.Serialization;

namespace Service.Fees.Domain.Models
{
    public interface IDepositFees
    {
        string BrokerId { get; }
        string AssetId { get; }
        string AssetNetwork { get; }
        string ProfileId { get; }
        FeeType FeeType { get; }
        FeeSizeType FeeSizeType { get; }
        decimal FeeSizeAbsolute { get; }
        decimal FeeSizeRelative { get; }
        string FeeAssetId { get; }
    }

    [DataContract]
    public class DepositFees : IDepositFees
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string AssetId { get; set; }
        [DataMember(Order = 3)] public FeeType FeeType { get; set; }
        [DataMember(Order = 4)] public FeeSizeType FeeSizeType { get; set; }
        [DataMember(Order = 5)] public decimal FeeSizeAbsolute { get; set; }
        [DataMember(Order = 6)] public decimal FeeSizeRelative { get; set; }
        [DataMember(Order = 7)] public string FeeAssetId { get; set; }
        [DataMember(Order = 8)] public string ProfileId { get; set; }

        [DataMember(Order = 9)] public string AssetNetwork { get; set; }

        public static DepositFees Create(IDepositFees assetFees)
        {
            return new DepositFees()
            {
                BrokerId = assetFees.BrokerId,
                AssetId = assetFees.AssetId,
                FeeType = assetFees.FeeType,
                FeeSizeType = assetFees.FeeSizeType,
                FeeSizeAbsolute = assetFees.FeeSizeAbsolute,
                FeeSizeRelative = assetFees.FeeSizeRelative,
                FeeAssetId = assetFees.FeeAssetId,
                ProfileId = assetFees.ProfileId,
                AssetNetwork = assetFees.AssetNetwork,
            };
        }
    }
}
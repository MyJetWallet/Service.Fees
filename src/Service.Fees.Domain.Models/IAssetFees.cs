using System.Runtime.Serialization;

namespace Service.Fees.Domain.Models
{
    public interface IAssetFees
    {
        string BrokerId { get; }
        string AssetId { get; }
        OperationType OperationType { get; }
        string ProfileId { get; }

        FeeType FeeType { get; }
        FeeSizeType FeeSizeType { get; }
        decimal FeeSize { get; }
        string FeeAssetId { get; }
        string AccountId { get; }
        string WalletId { get; }
    }

    [DataContract]
    public class AssetFees : IAssetFees
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string AssetId { get; set; }
        [DataMember(Order = 3)] public OperationType OperationType { get; set; }
        [DataMember(Order = 4)] public FeeType FeeType { get; set; }
        [DataMember(Order = 5)] public FeeSizeType FeeSizeType { get; set; }
        [DataMember(Order = 6)] public decimal FeeSize { get; set; }
        [DataMember(Order = 7)] public string FeeAssetId { get; set; }
        [DataMember(Order = 8)] public string AccountId { get; set; }
        [DataMember(Order = 9)] public string WalletId { get; set; }
        [DataMember(Order = 10)] public string ProfileId { get; set; }

        public static AssetFees Create(IAssetFees assetFees)
        {
            return new AssetFees()
            {
                BrokerId = assetFees.BrokerId,
                AssetId = assetFees.AssetId,
                OperationType = assetFees.OperationType,
                FeeType = assetFees.FeeType,
                FeeSizeType = assetFees.FeeSizeType,
                FeeSize = assetFees.FeeSize,
                FeeAssetId = assetFees.FeeAssetId,
                AccountId = assetFees.AccountId,
                WalletId = assetFees.WalletId,
                ProfileId = assetFees.ProfileId,
            };
        }
    }
}
using System.Runtime.Serialization;

namespace Service.Fees.Domain.Models
{
    public interface ISpotInstrumentFees
    {
        string BrokerId { get; }
        string SpotInstrumentId { get; }

        FeeType FeeType { get; }
        FeeSizeType MakerFeeSizeType { get; }
        double MakerFeeSize { get; }
        FeeSizeType TakerFeeSizeType { get; }
        double TakerFeeSize { get; }
        string FeeAssetId { get; }
        string AccountId { get; }
        string WalletId { get; }
    }

    [DataContract]
    public class SpotInstrumentFees : ISpotInstrumentFees
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string SpotInstrumentId { get; set; }
        [DataMember(Order = 3)] public FeeType FeeType { get; set; }
        [DataMember(Order = 4)] public FeeSizeType MakerFeeSizeType { get; set; }
        [DataMember(Order = 5)] public double MakerFeeSize { get; set; }
        [DataMember(Order = 6)] public FeeSizeType TakerFeeSizeType { get; set; }
        [DataMember(Order = 7)] public double TakerFeeSize { get; set; }
        [DataMember(Order = 8)] public string FeeAssetId { get; set; }
        [DataMember(Order = 9)] public string AccountId { get; set; }
        [DataMember(Order = 10)] public string WalletId { get; set; }

        public static SpotInstrumentFees Create(ISpotInstrumentFees spotInstrumentFees)
        {
            return new SpotInstrumentFees()
            {
                BrokerId = spotInstrumentFees.BrokerId,
                SpotInstrumentId = spotInstrumentFees.SpotInstrumentId,
                FeeType = spotInstrumentFees.FeeType,
                MakerFeeSizeType = spotInstrumentFees.MakerFeeSizeType,
                MakerFeeSize = spotInstrumentFees.MakerFeeSize,
                TakerFeeSizeType = spotInstrumentFees.TakerFeeSizeType,
                TakerFeeSize = spotInstrumentFees.TakerFeeSize,
                FeeAssetId = spotInstrumentFees.FeeAssetId,
                AccountId = spotInstrumentFees.AccountId,
                WalletId = spotInstrumentFees.WalletId
            };
        }
    }
}
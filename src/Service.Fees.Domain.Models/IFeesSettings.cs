using System.Runtime.Serialization;

namespace Service.Fees.Domain.Models
{
    public interface IFeesSettings
    {
        string BrokerId { get; }
        string AccountId { get; }
        string WalletId { get; }
    }

    [DataContract]
    public class FeesSettings : IFeesSettings
    {
        [DataMember(Order = 1)] public string BrokerId { get; set; }
        [DataMember(Order = 2)] public string AccountId { get; set; }
        [DataMember(Order = 3)] public string WalletId { get; set; }

        public static FeesSettings Create(IFeesSettings feesSettings)
        {
            return new FeesSettings()
            {
                BrokerId = feesSettings.BrokerId,
                AccountId = feesSettings.AccountId,
                WalletId = feesSettings.WalletId
            };
        }
    }
}
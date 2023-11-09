using Newtonsoft.Json;

namespace Chartboost
{
    /// <summary>
    /// Data class holding Adapter Info
    /// </summary>
    public struct ChartboostMediationAdapterInfo
    {
        [JsonProperty("adapterVersion")]
        public readonly string AdapterVersion;
        [JsonProperty("partnerVersion")]
        public readonly string PartnerVersion;
        [JsonProperty("partnerDisplayName")]
        public readonly string PartnerDisplayName;
        [JsonProperty("partnerIdentifier")]
        public readonly string PartnerIdentifier;

        public ChartboostMediationAdapterInfo(string adapterVersion, string partnerVersion, string partnerIdentifier, string partnerDisplayName)
        {
            AdapterVersion = adapterVersion;
            PartnerVersion = partnerVersion;
            PartnerDisplayName = partnerDisplayName;
            PartnerIdentifier = partnerIdentifier;
        }
    }
}

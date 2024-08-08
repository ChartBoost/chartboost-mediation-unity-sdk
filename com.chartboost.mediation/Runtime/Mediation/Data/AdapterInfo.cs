using Newtonsoft.Json;

namespace Chartboost.Mediation.Data
{
    /// <summary>
    /// Contains adapter specific information. Adapter must be integrated in order to information to be available.
    /// </summary>
    public struct AdapterInfo
    {
        /// <summary>
        /// The version of the adapter.
        /// </summary>
        [JsonProperty("adapterVersion")]
        public readonly string AdapterVersion;
        
        /// <summary>
        /// The version of the partner SDK.
        /// </summary>
        [JsonProperty("partnerVersion")]
        public readonly string PartnerVersion;
        
        /// <summary>
        /// The human-friendly partner name.
        /// </summary>
        [JsonProperty("partnerDisplayName")]
        public readonly string PartnerDisplayName;
        
        /// <summary>
        /// The partner’s unique identifier.
        /// </summary>
        [JsonProperty("partnerIdentifier")]
        public readonly string PartnerIdentifier;

        public AdapterInfo(string adapterVersion, string partnerVersion, string partnerIdentifier, string partnerDisplayName)
        {
            AdapterVersion = adapterVersion;
            PartnerVersion = partnerVersion;
            PartnerDisplayName = partnerDisplayName;
            PartnerIdentifier = partnerIdentifier;
        }
    }
}

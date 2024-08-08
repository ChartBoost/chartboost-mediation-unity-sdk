namespace Chartboost.Mediation.Adapters
{
    public interface IPartnerAdapterConfiguration
    {
        /// <summary>
        /// The partner adapter version.
        /// </summary>
        public string AdapterNativeVersion { get; }
        
        /// <summary>
        /// The partner SDK version.
        /// </summary>
        public string PartnerSDKVersion { get; }
        
        /// <summary>
        /// The partner ID for internal uses.
        /// </summary>
        public string PartnerIdentifier { get; }

        /// <summary>
        /// The partner name for external uses.
        /// </summary>
        public string PartnerDisplayName { get; }
    }
}

#if UNITY_IPHONE
using System.Runtime.InteropServices;

namespace Canary.Platforms
{
    /// <summary>
    /// iOS implementation of CanaryExeternal.
    /// </summary>
    public sealed class CanaryIOS : CanaryExternal
    {
        [DllImport("__Internal")]
        private static extern void _setSdkDomainName(string sdkDomainName);
        [DllImport("__Internal")]
        private static extern void _setRtbDomainName(string rtbDomainName);
        [DllImport("__Internal")]
        private static extern void _setAmazonPublisherServicesTestMode(bool value);
        [DllImport("__Internal")]
        private static extern void _setMetaAudienceNetworkTestMode(bool value);
        [DllImport("__Internal")]
        private static extern void _setAppLovinTestMode(bool value);

        public CanaryIOS()
        {
            LOGTag = "Canary(iOS)";
        }

        /// <summary>
        /// Set the domain for the SDK endpoint.
        /// </summary>
        /// <param name="sdkDomainName">The SDK domain name.</param>
        public override void SetSdkDomainName(string sdkDomainName)
        {
            base.SetSdkDomainName(sdkDomainName);
            _setSdkDomainName(sdkDomainName);
        }

        /// <summary>
        /// Set the domain for the realtime bidding endpoint.
        /// </summary>
        /// <param name="rtbDomainName">The realtime bidding endpoint.</param>
        public override void SetRtbDomainName(string rtbDomainName)
        {
            base.SetRtbDomainName(rtbDomainName);
            _setRtbDomainName(rtbDomainName);
        }

        /// <summary>
        /// Enable/disable test mode for Amazon Publisher Services.
        /// </summary>
        /// <param name="value">true to enable</param>
        public override void SetAmazonPublisherServicesTestModeEnabled(bool value)
        {
            base.SetAmazonPublisherServicesTestModeEnabled(value);
            _setAmazonPublisherServicesTestMode(value);
        }

        /// <summary>
        /// Enable/disable test mode for Facebook Audience Network.
        /// </summary>
        /// <param name="value">true to enable</param>
        public override void SetMetaAudienceNetworkTestModeEnabled(bool value)
        {
            base.SetMetaAudienceNetworkTestModeEnabled(value);
            _setMetaAudienceNetworkTestMode(value);
        }

        /// <summary>
        /// Enable/disable test mode for AppLovin.
        /// </summary>
        /// <param name="value">true to enable.</param>
        public override void SetApplovinTestModeEnabled(bool value)
        {
            base.SetApplovinTestModeEnabled(value);
            _setAppLovinTestMode(value);
        }
    }
}
#endif

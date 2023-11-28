#if UNITY_ANDROID
using UnityEngine;

namespace Canary.Platforms
{
    /// <summary>
    /// Android implementation of CanaryExternal.
    /// </summary>
    public sealed class CanaryAndroid : CanaryExternal
    {
        public CanaryAndroid()
        {
            LOGTag = "Canary(Android)";
        }

        // Initialize the Canary android bridge
        private static AndroidJavaObject GetCanaryBridge() => new AndroidJavaClass("com.chartboost.mediation.canary.unity.CanaryUnityBridge");

        /// <summary>
        /// Set the domain for the SDK endpoint.
        /// </summary>
        /// <param name="sdkDomainName">The SDK domain name.</param>
        public override void SetSdkDomainName(string sdkDomainName)
        {
            base.SetSdkDomainName(sdkDomainName);
            using var canaryBridge = GetCanaryBridge();
            canaryBridge.CallStatic("setSdkDomainName", sdkDomainName);
        }

        /// <summary>
        /// Set the domain for the realtime bidding endpoint.
        /// </summary>
        /// <param name="rtbDomainName">The realtime bidding endpoint.</param>
        public override void SetRtbDomainName(string rtbDomainName)
        {
            base.SetRtbDomainName(rtbDomainName);
            using var canaryBridge = GetCanaryBridge();
            canaryBridge.CallStatic("setRtbDomainName", rtbDomainName);
        }

        /// <summary>
        /// Enable/disable test mode for Amazon Publisher Services.
        /// </summary>
        /// <param name="value">true to enable</param>
        public override void SetAmazonPublisherServicesTestModeEnabled(bool value)
        {
            using var canaryBridge = GetCanaryBridge();
            canaryBridge.CallStatic("setAmazonPublisherServicesTestMode", value);
        }

        /// <summary>
        /// Enable/disable test mode for Facebook Audience Network.
        /// </summary>
        /// <param name="value">true to enable</param>
        public override void SetMetaAudienceNetworkTestModeEnabled(bool value)
        {
            base.SetMetaAudienceNetworkTestModeEnabled(value);
            using var canaryBridge = GetCanaryBridge();
            canaryBridge.CallStatic("setMetaAudienceNetworkTestMode", value);
        }

        /// <summary>
        /// Enable/disable test mode for AppLovin.
        /// </summary>
        /// <param name="value">true to enable.</param>
        public override void SetApplovinTestModeEnabled(bool value)
        {
            base.SetApplovinTestModeEnabled(value);
            using var canaryBridge = GetCanaryBridge();
            canaryBridge.CallStatic("setAppLovinTestMode", value);
        }

        /// <summary>
        /// Set the TCFv2 string. On Android, this value has to go to the
        /// default shared preferences. Since PlayerPrefs does not go to the
        /// default, it is necessary to do this in the bridge.
        /// </summary>
        /// <param name="value">TCFv2 string value.</param>
        public void SetTCString(string value)
        {
            base.SetTCString(value);
            using var canaryBridge = GetCanaryBridge();
            canaryBridge.CallStatic("setTcString", value);
        }
    }
}
#endif

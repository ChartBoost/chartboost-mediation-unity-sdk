using UnityEngine;

namespace Canary.Platforms
{
    /// <summary>
    /// Absract class for external native functionality needed by Canary. This
    /// functionality exercises things internal to the Helium SDK that are
    /// not made public to publishers and thus need their own private,
    /// non-distributed native bridging for each supported platform.
    /// </summary>
    public abstract class CanaryExternal
    {
        protected static string LOGTag = "ChartboostMediation (UnityCanary)";

        protected static void Log(string message)
        {
#if UNITY_EDITOR
            Debug.Log($"{LOGTag}/{message}");
#else
            System.Console.Out.WriteLine($"{LOGTag}/{message}");
#endif
        }

        /// <summary>
        /// Set the domain for the SDK endpoint.
        /// </summary>
        /// <param name="sdkDomainName">The SDK domain name.</param>
        public virtual void SetSdkDomainName(string sdkDomainName)
        {
            Log($"SetSdkDomainName {sdkDomainName}");
        }

        /// <summary>
        /// Set the domain for the realtime bidding endpoint.
        /// </summary>
        /// <param name="rtbDomainName">The realtime bidding endpoint.</param>
        public virtual void SetRtbDomainName(string rtbDomainName)
        {
            Log($"SetRtbDomainName {rtbDomainName}");
        }

        /// <summary>
        /// Enable/disable test mode for Amazon Publisher Services.
        /// </summary>
        /// <param name="value">true to enable</param>
        public virtual void SetAmazonPublisherServicesTestModeEnabled(bool value)
        {
            Log($"SetAmazonPublisherServicesTestModeEnabled {(value ? "YES" : "NO")}");
        }

        /// <summary>
        /// Enable/disable test mode for Facebook Audience Network.
        /// </summary>
        /// <param name="value">true to enable</param>
        public virtual void SetMetaAudienceNetworkTestModeEnabled(bool value)
        {
            Log($"SetFacebookAudienceNetworkTestModeEnabled {(value ? "YES" : "NO")}");
        }

        /// <summary>
        /// Enable/disable test mode for AppLovin.
        /// </summary>
        /// <param name="value">true to enable.</param>
        public virtual void SetApplovinTestModeEnabled(bool value)
        {
            Log($"SetApplovinTestModeEnabled {(value ? "YES" : "NO")}");
        }
        
        /// <summary>
        /// Set the TCFv2 string. On Android, this value has to go to the
        /// default shared preferences. Since PlayerPrefs does not go to the
        /// default, it is necessary to do this in the bridge.
        /// </summary>
        /// <param name="value">TCFv2 string value.</param>
        public virtual void SetTCString(string value)
        {
#if UNITY_ANDROID
            Log($"SetTCString {value}");
#endif
        }

    }
}

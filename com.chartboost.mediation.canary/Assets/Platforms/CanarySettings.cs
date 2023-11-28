using Canary.Platforms;

/// <summary>
/// A class that provides access to the native SDK functionality needed by
/// the Canary app.
/// </summary>
public class CanarySettings
{
    /// <summary>
    /// The platform-dependend instances of CanaryExternal API access.
    /// </summary>
    private static readonly CanaryExternal NativeCanarySettings;

    static CanarySettings()
    {
#if UNITY_EDITOR
        NativeCanarySettings = new CanaryUnsupported();
#elif UNITY_ANDROID
        NativeCanarySettings = new CanaryAndroid();
#elif UNITY_IPHONE
        NativeCanarySettings = new CanaryIOS();
#else
        NativeCanarySettings = new CanaryUnsupported();
#endif
    }
    
    public static void SetSdkDomainName(string sdkDomainName)
            => NativeCanarySettings.SetSdkDomainName(sdkDomainName);

    public static void SetRtbDomainName(string rtbDomainName)
            => NativeCanarySettings.SetRtbDomainName(rtbDomainName);

    public static void SetAmazonPublisherServicesTestModeEnabled(bool value)
            => NativeCanarySettings.SetAmazonPublisherServicesTestModeEnabled(value);

    public static void SetMetaAudienceNetworkTestModeEnabled(bool value)
            => NativeCanarySettings.SetMetaAudienceNetworkTestModeEnabled(value);

    public static void SetApplovinTestModeEnabled(bool value)
            => NativeCanarySettings.SetApplovinTestModeEnabled(value);

	public static void SetTCString(string value) {
#if UNITY_ANDROID
            // Android has to be bridged due to PlayerPrefs not saving to default shared preferences.
            // iOS does not have to be bridged since PlayerPrefs saves to standard user defaults.
            NativeCanarySettings.SetTCString(value);
#endif
	}
}

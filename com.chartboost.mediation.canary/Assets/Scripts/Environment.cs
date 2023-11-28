using System;
using Chartboost;
using UnityEngine;
using static System.String;

#nullable enable

/// <summary>
/// The environment properties for the Canary app.
/// </summary>
public class Environment
{
    public static readonly Environment Shared = new Environment();

    /// <summary>
    /// Constructor.
    /// </summary>
    public Environment()
    {
        Load();
    }

    /// <summary>
    /// The Helium application identifier.
    /// </summary>
    [NullablePlayerPrefsStringProperty(Key = "com.chartboost.helium.canary.app-identifier")]
    public string? AppIdentifier { get; set; }

    /// <summary>
    /// The Helium application signature.
    /// </summary>
    [NullablePlayerPrefsStringProperty(Key = "com.chartboost.helium.canary.app-signature")]
    public string? AppSignature { get; set; }

    /// <summary>
    /// The Helium Initialization behavior
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.automatic-initialization", Default = DefaultValue.AutomaticInitialization)]
    public bool AutomaticInitialization { get; set; } = DefaultValue.AutomaticInitialization;

    /// <summary>
    /// Auto-Load On Show setting.
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.auto-load-on-show", Default = false)]
    public bool AutoLoadOnShow { get; set; } = false;
    
    /// <summary>
    /// Keep fullscreen ad object until it has been shown and a new one has been loaded.  This allows for the following
    /// negative test cases:
    /// - Showing an ad while another is loading
    /// - Showing an ad multiple times
    /// - Showing an expired ad
    /// - Showing a destroyed ad
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.keep-fullscreen-ad-until-shown-then-load", Default = false)]
    public bool KeepFullscreenAdUntilShownThenLoad { get; set; } = false;
    
    /// <summary>
    /// Use New Fullscreen API setting
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.use-new-fullscreen-api", Default = true)]
    public bool UseNewFullscreenAPI { get; set; }

    /// <summary>
    /// Use New Banner API setting
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.use-new-banner-api", Default = true)]
    public bool UseNewBannerAPI { get; set; }
    
    /// <summary>
    /// Discard Oversized banner Ad setting.
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.discard-oversized-banner-ads", Default = false)]
    public bool DiscardOversizedBannerAds { get; set; } = false;

    /// <summary>
    /// The Helium backend to use.
    /// </summary>
    [PlayerPrefsStringProperty(Key = "com.chartboost.helium.canary.backend", Default = DefaultValue.Backend)]
    public string Backend
    {
        get => _backend;
        set
        {
            _backend = value;
            var backend = new Backend(_backend);
            CanarySettings.SetSdkDomainName(backend.SdkDomainName);
            #if !UNITY_IOS
            // IOS has backend APIs updated which no longer requires to set Rtb host 
            // https://github.com/ChartBoost/ios-helium-sdk/commit/265a9e6c21e104b0dc26125ae002e18b535038da
            // TODO: [HB-6559] Remove this call once both Android and IOS are updated to adopt new backend endpoints
            CanarySettings.SetRtbDomainName(backend.RtbDomainName);
            #endif
        }
    }
    
    private string _backend = DefaultValue.Backend;

    /// <summary>
    /// COPPA subject setting.
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.coppa-subject", Default = false)]
    public bool IsSubjectToCoppa { get; set; } = false;

    /// <summary>
    /// CCPA consent setting.
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.ccpa", Default = false)]
    public bool CcpaConsent { get; set; } = false;

    /// <summary>
    /// GDPR subject setting.
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.gdpr-subject", Default = false)]
    public bool IsSubjectToGdpr { get; set; } = false;

    /// <summary>
    /// GDPR consent setting.
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.gdpr", Default = false)]
    public bool GdprConsent { get; set; } = false;

    /// <summary>
    /// TCFv2 string.  Comes directly from UserDefaults from key `IABTCF_TCString` and is not
	/// a Chartboost/Helium namespaced property.
    /// </summary>
    [NullablePlayerPrefsStringProperty(Key = "IABTCF_TCString")]
    public string? TCString {
        get => _tcString;
        set
        {
            _tcString = value;
#if UNITY_ANDROID
            // Android has to be bridged due to PlayerPrefs not saving to default shared preferences.
            // iOS does not have to be bridged since PlayerPrefs saves to standard user defaults.
            CanarySettings.SetTCString(value);
#endif
        }
    }
    private string? _tcString;

    /// <summary>
    /// A name for the game engine.
    /// </summary>
    [NullablePlayerPrefsStringProperty(Key = "com.chartboost.helium.game-engine-name")]
    public string? GameEngineName
    {
        get => _gameEngineName;
        set
        {
            _gameEngineName = value;
            // Ability to change game engine name and version not yet available in the Unity SDK
            /*
            if (_gameEngineName != null && _gameEngineVersion != null)
            {
                HeliumSDK.SetGameEngine(_gameEngineName, _gameEngineVersion);
            }
            */
        }
    }
    private string? _gameEngineName;

    /// <summary>
    /// A version for the game engine.
    /// </summary>
    [NullablePlayerPrefsStringProperty(Key = "com.chartboost.helium.game-engine-version")]
    public string? GameEngineVersion
    {
        get => _gameEngineVersion;
        set
        {
            _gameEngineVersion = value;
            // Ability to change game engine name and version not yet available in the Unity SDK
            /*
            if (_gameEngineName != null && _gameEngineVersion != null)
            {
                HeliumSDK.SetGameEngine(_gameEngineName, _gameEngineVersion);
            }
            */
        }
    }
    private string? _gameEngineVersion;

    /// <summary>
    /// An identifier for the user.
    /// </summary>
    [NullablePlayerPrefsStringProperty(Key = "com.chartboost.helium.canary.user-identifier")]
    public string? UserIdentifier { get; set; }

    /// <summary>
    /// Test mode setting for Amazon Publisher Services.
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.enable-amazon-testmode", Default = true)]
    public bool IsAmazonTestModeEnabled
    {
        get => _isAmazonTestModeEnabled;
        set
        {
            _isAmazonTestModeEnabled = value;
            CanarySettings.SetAmazonPublisherServicesTestModeEnabled(value);
        }
    }
    private bool _isAmazonTestModeEnabled = true;

    /// <summary>
    /// Test mode setting for Facebook Audience Network.
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.enable-fan-testmode", Default = true)]
    public bool IsFacebookAudienceNetworkTestModeEnabled
    {
        get => _isMetaAudienceNetworkTestModeEnabled;
        set
        {
            _isMetaAudienceNetworkTestModeEnabled = value;
            CanarySettings.SetMetaAudienceNetworkTestModeEnabled(value);
        }
    }
    private bool _isMetaAudienceNetworkTestModeEnabled = true;

    /// <summary>
    /// Test mode setting for Helium.
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.enable-testmode", Default = true)]
    public bool IsChartboostMediationTestModeEnabled
    {
        get => _isChartboostMediationTestModeEnabled;
        set
        {
            _isChartboostMediationTestModeEnabled = value;
            switch (ChartboostMediationInitializer.Instance.Initialized)
            {
                case true:
                    ChartboostMediation.SetTestMode(_isChartboostMediationTestModeEnabled);
                    break;
                case false when !_subscribedToInit:
                    _subscribedToInit = true;
                    ChartboostMediation.DidStart += SetTestModeOnStart;
                    break;
            }
        }
    }

    private bool _subscribedToInit;
    private void SetTestModeOnStart(string error)
    {
        ChartboostMediation.DidStart -= SetTestModeOnStart;
        ChartboostMediation.SetTestMode(_isChartboostMediationTestModeEnabled);
    }
    private bool _isChartboostMediationTestModeEnabled = true;

    /// <summary>
    /// Test mode setting for AppLovin.
    /// </summary>
    [PlayerPrefsBooleanProperty(Key = "com.chartboost.helium.canary.enable-applovin-testmode", Default = true)]
    public bool IsAppLovinTestModeEnabled
    {
        get => _isAppLovinTestModeEnabled;
        set
        {
            _isAppLovinTestModeEnabled = value;
            CanarySettings.SetApplovinTestModeEnabled(value);
        }
    }
    private bool _isAppLovinTestModeEnabled = true;

    /// <summary>
    /// Indicator for the Sign In Controller that a sign-in change was initiated and that
    /// the app should force kill after sign in (since Chartboost Mediation SDK can't be
    /// re-initialized once initialized).
    ///
    /// This is not a persisted property.
    /// </summary>
    public bool ForceKillAfterSignIn { get; set; } = false;

    /// <summary>
    /// Load the environment values.
    /// </summary>
    private void Load()
    {
        Type[] attributeTypes =
        {
            typeof(PlayerPrefsStringProperty),
            typeof(NullablePlayerPrefsStringProperty),
            typeof(PlayerPrefsBooleanProperty),
            typeof(PlayerPrefsIntProperty)
        };
        var properties = typeof(Environment).GetProperties();
        foreach (var property in properties)
        {
            foreach (var attributeType in attributeTypes)
            {
                var attribute = Attribute.GetCustomAttribute(property, attributeType);
                if (attribute == null)
                    continue;
                switch (attribute)
                {
                    case PlayerPrefsStringProperty stringProperty:
                        property.SetValue(this, stringProperty.WrappedValue);
                        break;
                    case NullablePlayerPrefsStringProperty nullableStringProperty:
                        property.SetValue(this, nullableStringProperty.WrappedValue);
                        break;
                    case PlayerPrefsBooleanProperty boolProperty:
                        property.SetValue(this, boolProperty.WrappedValue);
                        break;
                    case PlayerPrefsIntProperty intProperty:
                        property.SetValue(this, intProperty.WrappedValue);
                        break;
                }
            }
        }
        
        // If the FIREBASE_DISTRIBUTION directive is set, set the AppIdentifier
        // and AppSignature properties to the default values. This will allow
        // the LaunchController to immediately load the root scene with the QA
        // automation app using the default app identifier and signature.
        #if FIREBASE_DISTRIBUTION
        AppIdentifier = DefaultValue.AppIdentifier;
        AppSignature = DefaultValue.AppSignature;
        #endif
    }

    /// <summary>
    /// Save the environment values.
    /// </summary>
    public void Save()
    {
        Type[] attributeTypes =
        {
            typeof(PlayerPrefsStringProperty),
            typeof(NullablePlayerPrefsStringProperty),
            typeof(PlayerPrefsBooleanProperty),
            typeof(PlayerPrefsIntProperty)
        };
        var properties = typeof(Environment).GetProperties();
        foreach (var property in properties)
        {
            foreach (var attributeType in attributeTypes)
            {
                var attribute = Attribute.GetCustomAttribute(property, attributeType);
                if (attribute == null)
                    continue;
                var value = property.GetValue(this, null);
                switch (attribute)
                {
                    case PlayerPrefsStringProperty stringProperty:
                        stringProperty.WrappedValue = value switch
                        {
                            string str => property.GetValue(this, null) as string,
                            _ => throw new TypeAccessException($"Not a supported type for value: {nameof(value)}"),
                        };
                        break;
                    case NullablePlayerPrefsStringProperty nullableStringProperty:
                        if (value == null)
                        {
                            nullableStringProperty.WrappedValue = null;
                        }
                        else
                        {
                            nullableStringProperty.WrappedValue = value switch
                            {
                                string str => property.GetValue(this, null) as string,
                                _ => throw new TypeAccessException($"Not a supported type for value: {nameof(value)}"),
                            };
                        }
                        break;
                    case PlayerPrefsBooleanProperty booleanProperty:
                        booleanProperty.WrappedValue = value switch
                        {
                            bool b => (bool)property.GetValue(this, null),
                            _ => throw new TypeAccessException($"Not a supported type for value: {nameof(value)}"),
                        };
                        break;

                    case PlayerPrefsIntProperty intProperty :
                        
                        intProperty.WrappedValue = value switch
                        {
                            int i => (int)property.GetValue(this, null),
                            
                            _ => throw new TypeAccessException($"Not a supported type for value: {nameof(value)}"),
                        };
                        break;
                }
            }
        }
    }
}

#nullable disable

/// <summary>
/// A property string attribute that is backed by PlayerPrefs as the data store.
/// </summary>
public class PlayerPrefsStringProperty : Attribute
{
    /// <summary>
    /// They key for the PlayerPrefs string.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The default value for the property if it does not exist within
    /// the PlayerPrefs for the Key.
    /// </summary>
    public string Default { get; set; }

    /// <summary>
    /// Management of the value within PlayerPrefs.
    /// </summary>
    public string WrappedValue
    {
        get => PlayerPrefs.GetString(Key, Default);
        set
        {
            if (IsNullOrEmpty(value))
            {
                PlayerPrefs.DeleteKey(Key);
            }
            else
            {
                PlayerPrefs.SetString(Key, value);
            }
        }
    }
}

/// <summary>
/// A property string attribute that is backed by PlayerPrefs as the data store.
/// </summary>
public class NullablePlayerPrefsStringProperty : Attribute
{
    /// <summary>
    /// They key for the PlayerPrefs string.
    /// </summary>
    public string Key { get; set; }

#nullable enable
    /// <summary>
    /// Management of the value within PlayerPrefs.
    /// </summary>
    public string? WrappedValue
    {
        get => PlayerPrefs.GetString(Key);
        set
        {
            if (IsNullOrEmpty(value))
            {
                PlayerPrefs.DeleteKey(Key);
            }
            else
            {
                PlayerPrefs.SetString(Key, value);
            }
        }
    }
#nullable disable
}

/// <summary>
/// A property boolean attribute that is backed by PlayerPrefs as the data store.
/// </summary>
public class PlayerPrefsBooleanProperty : Attribute
{
    /// <summary>
    /// They key for the PlayerPrefs string.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The default value for the property if it does not exist within
    /// the PlayerPrefs for the Key.
    /// </summary>
    public bool Default { get; set; }

    /// <summary>
    /// Management of the value within PlayerPrefs.
    /// </summary>
    public bool WrappedValue
    {
        get => PlayerPrefs.GetInt(Key, Default ? 1 : 0) != 0;
        set => PlayerPrefs.SetInt(Key, value ? 1 : 0);
    }
}

public class PlayerPrefsIntProperty : Attribute
{
    /// <summary>
    /// They key for the PlayerPrefs string.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The default value for the property if it does not exist within
    /// the PlayerPrefs for the Key.
    /// </summary>
    public int Default { get; set; }

    /// <summary>
    /// Management of the value within PlayerPrefs.
    /// </summary>
    public int WrappedValue
    {
        get => PlayerPrefs.GetInt(Key, Default);
        set => PlayerPrefs.SetInt(Key, value);
    }
}

# Chartboost Mediation Settings
Starting Chartboost Mediation Unity SDK 4.4.0 we have introduced a simple Editor Window where you can modify all of your Chartboost Mediation Settings. The window can be visited by opening `Chartboost Mediation/Configure`.

![Chartboost Mediation Settings](../images/chartboost-mediation-settings.png)

> **Note** \
> Make sure that these are the AppId and App Signature values that you obtain directly from your Chartboost Mediation Dashboard for your app as opposed to credentials from Chartboost or any other Ad Network.

## AppID & AppSignature

The Chartboost Mediation SDK AppId & AppSignature can be set in the window above, or through the available C# API as seen below:

```csharp
// Android
ChartboostMediationSettings.AndroidAppId = "YOUR_APPID";
ChartboostMediationSettings.AndroidAppSignature = "YOUR_APPSIGNATURE";

// IOS
ChartboostMediationSettings.IOSAppId = "YOUR_APPID";
ChartboostMediationSettings.IOSAppSignature = "YOUR_APPSIGNATURE";

var activePlatformAppId = ChartboostMediationSettings.AppId;
var activePlatformAppSignature = ChartboostMediationSettings.AppSignature;
```

Note that this fields will be required when calling `ChartboostMediation.StartWithOptions`, we provide this as an easy way to store and access such values.

## SDK Debugging

To enable SDK debugging toggle the field on the `SettingsWindow` or use the provided C# API as seen below:

```csharp
// enabled
ChartboostMediationSettings.IsLoggingEnabled = true;

// disabled (default state)
ChartboostMediationSettings.IsLoggingEnabled = false;
```

## Automatic Initialization

To enable Chartboost Mediation Unity SDK automatic SDK, toggle the field on the `SettingsWindow` or use the provided C# API as seen below:

```csharp
// enabled
ChartboostMediationSettings.IsAutomaticInitializationEnabled = true;

// disabled (default state)
ChartboostMediationSettings.IsAutomaticInitializationEnabled = false;
```

> **Note** \
> When using automatic initialization, the network kill switch is disabled by default. You are required to set the AppId and AppSignature into the ChartboostMediationSettings scriptable object in order for this feature to work.

# Build-Processing Tools

Starting Chartboost Mediation Unity SDK 4.4.0, we have introduced a set of build pre-processor and post-processor tools to help you ensure your integration has all the required fields or modifications needed for proper partner functionality.

## Google App Id 

> **Warning** \
> If you are implementing AdMob or GoogleBidding, you will need to use the tools provided below or create your approach to get the same result. Failing to include your Google App Id will result in crashes.

### Android

When modifiying this field with a Google App Id, the provided value will be utilized by the `ChartboostMediationPreprocessor` to modify your `AndroidManifest.xml` to ensure the `com.google.android.gms.ads.APPLICATION_ID` exists. If such element cannot be found, the preprocessor will make sure to add it as needed.

To enable this feature, toggle the field on the `SettingsWindow` or use the provided C# API as seen below:

```csharp
ChartboostMediationSettings.AndroidGoogleAppId = "ca-app-pub-...YOUR_ID_HERE";
```

### IOS
When modifiying this field with a Google App Id, the provided value will be utilized by the `ChartboostMediationPostprocessor` to modify your XCode project's `Info.plist` to ensure the `GADApplicationIdentifier` exists. If such element cannot be found, the postprocessor will make sure to add it as needed.

To enable this feature, toggle the field on the `SettingsWindow` or use the provided C# API as seen below:

```csharp
ChartboostMediationSettings.IOSGoogleAppId = "ca-app-pub-...YOUR_ID_HERE";
```

## AppLovin SDK Key

When modifying this field with an AppLovin SDK Key, the provided value will be utilized by the `ChartboostMediationPreprocessor` to modify your `AndroidManifest.xml` to ensure the `applovin.sdk.key` exists. If such element cannot be found, the preprocessor will make sure to add it as needed.

To enable this feature, toggle the field on the `SettingsWindow` or use the provided C# API as seen below:

```csharp
ChartboostMediationSettings.AppLovinSDKKey = "YOUR_APPLOVIN_SDK_KEY";
```

## SKAdNetwork Resolution

To add SKAdNetworks in your app, you need to add the `SKAdNetworkIdentifier` and their values in your app's `Info.plist` file. This process can be handled by totally controlled by you; however, after Chartboost Mediation 4.0.0, we have added a postprocessor to handle this process automatically if needed. 

To enable this feature, toggle the field on the `SettingsWindow` or use the provided C# API as seen below:

```csharp
// enabled
ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled = true;

// disabled (default state)
ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled = false;
```

## Disable Bitcode

Starting XCode 14 bitcode has been deprecated, for more information visit [Apple's official documentation](https://developer.apple.com/documentation/xcode-release-notes/xcode-14-release-notes#Deprecations). We have included a setting to modify the `BITCODE_ENABLE` flag into your XCode project.

To enable this feature, toggle the field on the `SettingsWindow` or use the provided C# API as seen below:

```csharp
// enabled
ChartboostMediationSettings.DisableBitcode = true;

// disabled (default state)
ChartboostMediationSettings.DisableBitcode = false;
```
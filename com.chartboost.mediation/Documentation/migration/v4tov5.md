# Migrating Chartboost Mediation 4.X to Chartboost Mediation 5.0.0

All code samples are provided in C# unless otherwise noted. The 5.X documentation is effective for v5.0.0\. Refer to the main documentation for the most up-to-date information with the latest 5.X version.

## New Features for the 5.X Series 🔗

### Chartboost Core SDK Required

The Chartboost Mediation 5.X series of SDKs introduces a new architecture centered around the Chartboost Core SDK. By moving the Mediation SDK's consent duties to the Core SDK, the Mediation SDK is able to more rapidly keep pace with the evolving privacy and consent landscape.

Review the [Core SDK integration guide](https://docs.chartboost.com/en/mediation/integrate/core/unity/get-started/) for more information.

### Chartboost Mediation API

The public APIs have been audited so that there is better parity between all of the platforms.

- The fixed and adapter banner APIs have been merged into a singular API.
- The fullscreen ads API architecture has been updated to match that of the banner ads API.
- The `ChartboostMediation` prefixing has been dropped in most APIs for brevity.

### Classes

Mediation 4.X                          | Mediation 5.X
-------------------------------------- | --------------------------------------
`ChartboostMediation`                  | `ChartboostMediation`
`ChartboostMediationInterstitialAd`    | <span style="color:red">Removed</span>
`ChartboostMediationRewardedAd`        | <span style="color:red">Removed</span>
`ChartboostMediationBannerAd`          | <span style="color:red">Removed</span>
`IChartboostMediationBannerView`       | `IBannerAd`
`ChartboostMediationUnityBannerAd`     | `UnityBannerAd`
`IChartboostMediationFullscreenAd`     | `IFullscreenAd`
`ChartboostMediationFullscreenAdQueue` | `IFullscreenAdQueue`
`ChartboostMediationAdapterInfo`       | `AdapterInfo`
`ChartboostMediationSettings`          | <span style="color:red">Removed</span>
`IPartnerConsent`                      | <span style="color:red">Removed</span>

### Delegates

Mediation 4.X                                              | Mediation 5.X
---------------------------------------------------------- | ------------------------------------------------------
`ChartboostMediationILRDEvent`                             | `ChartboostMediationImpressionLevelRevenueDataEvent`
`ChartboostMediationPartnerInitializationEvent`            | `ChartboostMediationPartnerAdapterInitializationEvent`
`ChartboostMediationEvent`                                 | <span style="color:red">Removed</span>
`ChartboostMediationPlacementEvent`                        | <span style="color:red">Removed</span>
`ChartboostMediationPlacementLoadEvent`                    | <span style="color:red">Removed</span>
`ChartboostMediationFullscreenAdEvent`                     | `FullscreenAdEvent`
`ChartboostMediationFullscreenAdEventWithError`            | `FullscreenAdEventWithError`
`ChartboostMediationFullscreenAdQueueEvent`                | `FullscreenAdQueueUpdateEvent`
`ChartboostMediationFullscreenAdQueueRemoveExpiredAdEvent` | `FullscreenAdQueueRemoveExpiredAdEvent`
`ChartboostMediationBannerEvent`                           | `BannerAdEvent`
`ChartboostMediationBannerDragEvent`                       | `BannerAdDragEvent`
`ChartboostMediationUnityBannerAdEvent`                    | `UnityBannerAdEvent`
`ChartboostMediationUnityBannerAdDragEvent`                | `UnityBannerAdDragEvent`

> **Note** The tables above covers most relevant developer facing APIs. As mentioned before, the `ChartboostMediation` prefixing has been dropped for brevity, APIs not mentioned above have been renamed accordingly.

## Integration

### Initialization

Starting with Mediation SDK 5.0.0, the Mediation SDK can only be initialized by Chartboost Core SDK. This architectural change was made so that Core can manage the initialization sequences of the current Mediation SDK and future SDKs on the horizon.

Review the [Core SDK integration guide](https://docs.chartboost.com/en/mediation/integrate/core/unity/integration/) for more information.

### Network Killswitch

Since Mediation SDK initialization is now handled by the Core SDK, the network killswitch feature has been moved to the static method `ChartboostMediation.SetPreInitializationConfiguration` that must be called before Core SDK initialization is initiated.

```csharp
// List of partners ids to skip initialization
HashSet<string> skippablePartnerIds = new HashSet<string>
{
    ChartboostAdapter.PartnerIdentifier,
    MetaAudienceNetworkAdapter.PartnerIdentifier
};

// Create ChartboostMediationPreInitializationConfiguration object
ChartboostMediationPreInitializationConfiguration preinitializatioOptions = new ChartboostMediationPreInitializationConfiguration(skippablePartnerIds);

// Set ChartboostMediationPreInitializationConfiguration object
ChartboostMediationError? ChartboostMediation.SetPreInitializationConfiguration(preinitializatioOptions);

// Report if failed to set ChartboostMediationPreInitializationConfiguration object
if (error.HasValue) 
    Debug.LogError($"Failed to set PreInitializationConfiguration: {JsonTools.SerializeObject(error.Value)}");
```

### Consent

All privacy and consent related APIs have been removed from the Mediation 5.X SDK and will be handled by the Core SDK.

The Mediation SDK will continue to utilize the TCFv2 string as part of auction requests, and will forward consent changes to the partner adapters. For partners that utilize complex non-IAB compliant privacy and consent signals that cannot be extrapolated from Core SDK consent signals, last resort APIs will be exposed via that partner adapter's `AdapterConfiguration` class and must be directly set on the adapter configuration.

For more information on supported Consent Mediation Platforms and their integration, review the [Core SDK integration guide](https://docs.chartboost.com/en/mediation/integrate/core/unity/integration/#integrating-consent-management-platform).

#### HyprMX

```csharp
// Given
HyprMXAdapter.SetConsentStatusOverride(HyprMXConsentStatus.ConsentGiven);

// Declined
HyprMXAdapter.SetConsentStatusOverride(HyprMXConsentStatus.ConsentDeclined);

// Unknown
HyprMXAdapter.SetConsentStatusOverride(HyprMXConsentStatus.ConsentDeclined);
```

#### Pangle

```csharp
// Consent
PangleAdapter.SetGDPRConsentOverride(PangleGDPRConsentType.PangleGDPRConsentConsent);

// No Consent
PangleAdapter.SetGDPRConsentOverride(PangleGDPRConsentType.PangleGDPRConsentNoConsent);

// Default
PangleAdapter.SetGDPRConsentOverride(PangleGDPRConsentType.PangleGDPRConsentTypeDefault);

// Sell
PangleAdapter.SetDoNotSellOverride(PangleDoNotSellType.PangleGDoNotSellTypeSell);

// Not Sell
PangleAdapter.SetDoNotSellOverride(PangleDoNotSellType.PangleDoNotSellTypeNotSell);

// Default
PangleAdapter.SetDoNotSellOverride(PangleDoNotSellType.PangleDoNotSellTypeDefault);
```

#### UnityAds

```csharp
// Consent
UnityAdsAdapter.SetGDPRConsentOverride(true);


// No Consent
UnityAdsAdapter.SetGDPRConsentOverride(false);

// Consent
UnityAdsAdapter.SetPrivacyConsentOverride(true);

// No Consent
UnityAdsAdapter.SetPrivacyConsentOverride(false);
```

#### Vungle

```csharp
// Consent
VungleAdapter.SetGDPRStatusOverride(true);


// No Consent
VungleAdapter.SetGDPRStatusOverride(false);

// Consent
VungleAdapter.SetCCPAStatusOverride(true);

// No Consent
VungleAdapter.SetCCPAStatusOverride(false);
```

### Publisher Metadata

Publisher-specified data used to improve auction requests have been moved to the Core SDK, and will still be available to the Mediation 5.X auction requests.

Mediation 4.X                                | Mediation 5.X
-------------------------------------------- | ----------------------------------------------------------------------------------
`ChartboostMediation.SetUserIdentifier`      | `ChartboostCore.PublisherMetadata.SetPlayerIdentifier(frameworkname:)`
<span style="color:red">No Equivalent</span> | `ChartboostCore.PublisherMetadata.SetFramework(frameworkName:, frameworkVersion:)`
`ChartboostMediation.SetSubjectToCoppa`      | `ChartboostCore.PublisherMetadata.SetIsUserUnderage(isUserUnderage:)`

### Adaptive and Fixed Banners

The changes to adaptive banner ads APIs revolved around naming convention parity and architectural alignment with the banner ads APIs.

```csharp
// Get a bannerAd
IBannerAd bannerAd = ChartboostMediation.GetBannerAd();

// Place it at the center of screen
bannerAd.Position = new Vector2(
    DensityConverters.PixelsToNative(Screen.width/2f),
    DensityConverters.PixelsToNative(Screen.height/2f)
);

// Set pivot
bannerAd.Pivot = new Vector2(0.5f, 0.5f);

// Set banner ad callbacks
bannerAd.WillAppear += ad => Debug.Log($"BannerAd: {ad.LoadId} will appear.");
bannerAd.DidClick += ad => Debug.Log($"BannerAd: {ad.LoadId} was clicked.");
bannerAd.DidDrag += (ad,x,y) => Debug.Log($"BannerAd: {ad.LoadId} was dragged x:{x}/y:{y}.");
bannerAd.DidRecordImpression += ad => Debug.Log($"BannerAd: {ad.LoadId} was clicked.");


// create a load request with size and your placementName
var loadRequest = new BannerAdLoadRequest(
    "BANNER_PLACEMENT_NAME",
    BannerSize.Adaptive6X1(width)    // This can be any other size or the old non-adaptive size like `BannerSize.Standard`
);

// Check if IBannerAd failed to load
var error = adShowResult.Error;

// Failed to load
if (error.HasValue)
{
    // Report load failure
    Debug.LogError($"`IBannerAd` Load failed with error: {JsonTools.SerializeObject(error.Value, Formatting.Indented)}");
    return;
}

// Load succeeded

// Report metrics and show success
var loadId = loadResult.LoadId;
var metricsJson = JsonTools.SerializeObject(loadResult.Metrics, Formatting.Indented);
var winningBidInfo = JsonTools.SerializeObject(loadResult.WinningBidInfo, Formatting.Indented);
Debug.Log($"`IBannerAd` loaded successfully with:\n" +
          $"LoadId: {loadId}\n" +
          $"Metrics: {metricsJson}\n" +
          $"Winning Bid Info: {winningBidInfo}");

```

### Banner Ads for Unity GameObject

```csharp
// Determine the maximum size to load using width and height
BannerSize size = BannerSize.Adaptive(100, 100);
BannerAdLoadRequest loadRequest = new BannerAdLoadRequest("BANNER_PLACEMENT_NAME", size);

// Get reference to `UnityBannerAd` from scene
public UnityBannerAd unityBannerAd;

// Or create one at runtime
var unityBannerAd = ChartboostMediation.GetUnityBannerAd("PLACEMENT_NAME", FindObjectOfType<Canvas>().transform);

// Place this at the top-right corner of screen
unityBannerAd.transform.position = new Vector2(Screen.width, Screen.height);
unityBannerAd.GetComponent<RectTransform>().pivot = new Vector2(1, 1);

// Set callbacks
unityBannerAd.WillAppear += ad => Debug.Log($"UnityBannerAd: {ad.LoadId} will appear.");
unityBannerAd.DidClick += ad => Debug.Log($"UnityBannerAd: {ad.LoadId} was clicked.");
unityBannerAd.DidDrag += (ad,x,y) => Debug.Log($"UnityBannerAd: {ad.LoadId} was dragged x:{x}/y:{y}.");
unityBannerAd.DidRecordImpression += ad => Debug.Log($"UnityBannerAd: {ad.LoadId} was clicked.");

// Load with this gameobject's rect size as request size for banner
var loadResult = await unityBannerAd.Load();

// Or use a custom load request

// Create load request
var loadRequest = new BannerAdLoadRequest(
    "PLACEMENT_NAME",
    BannerSize.Adaptive6X1(100)    // This can be any other size or the old non-adaptive size like `BannerSize.Standard`
);
var loadResult = await unityBannerAd.Load(loadRequest);
if(!loadResult.Error.HasValue)
{
    // loaded successfully
}

// Check if UnityBannerAd failed to load
var error = adShowResult.Error;

// Failed to load
if (error.HasValue)
{
    // Report load failure
    Debug.LogError($"`UnityBannerAd` Load failed with error: {JsonTools.SerializeObject(error.Value, Formatting.Indented)}");
    return;
}

// Load succeeded

// Report metrics and show success
var loadId = loadResult.LoadId;
var metricsJson = JsonTools.SerializeObject(loadResult.Metrics, Formatting.Indented);
var winningBidInfo = JsonTools.SerializeObject(loadResult.WinningBidInfo, Formatting.Indented);
Debug.Log($"`UnityBannerAd` loaded successfully with:\n" +
          $"LoadId: {loadId}\n" +
          $"Metrics: {metricsJson}\n" +
          $"Winning Bid Info: {winningBidInfo}");
```

### Resizing Adaptive Banners

Resizing adaptive banners have been removed and replaced with `bannerAd.ContainerSize = ContainerSize.WrapContent();`.

### Fullscreen Ads

The changes to the fullscreen ads APIs revolved around naming convention parity and architectural alignment with the banner ads APIs.

Review the [Fullscreen Ads integration guide](../integration/fullscreen.md) for more details.

```csharp
// Keywords to pass for fullscreen ad
var keywords = new Dictionary<string, string>
{
    { "key", "value" },
    { "key_2", "value_2" }
};

// Create FullscreenAdLoadRequest with keywords
FullscreenAdLoadRequest fullscreenAdRequest = new FullscreenAdLoadRequest("FULLSCREEN_PLACEMENT_ID", keywords);

// Load `IFullscreenAd` using async approach and our previously built FullscreenAdLoadRequest
FullscreenAdLoadResult fullscreenAdLoadResult = await ChartboostMediation.LoadFullscreenAd(fullscreenAdRequest);

// Check if an error occurred
ChartboostMediationError? error = fullscreenAdLoadResult.Error;

// Load failed
if (error.HasValue)
{
    // Report load failure
    Debug.LogError($"`IFullscreenAd` Load failed with error: {JsonTools.SerializeObject(error.Value, Formatting.Indented)}");
    return;
}

// Load succeeded 

// Parse load result data
var loadId = fullscreenAdLoadResult.LoadId;
var winningBidInfoJson = JsonTools.SerializeObject(fullscreenAdLoadResult.WinningBidInfo, Formatting.Indented);
var metricsJson = JsonTools.SerializeObject(fullscreenAdLoadResult.Metrics, Formatting.Indented);

// Report fullscreen ad load result information
Debug.Log($"`IFullscreenAd` load completed with: LoadId{loadId} WinningBidInfo: {winningBidInfoJson}, Metrics: {metricsJson}");

// Obtain IFullscreenAd
IFullscreenAd ad = fullscreenAdLoadResult.Ad;

// Subscribing IFullscreenAd Instance Delegates
ad.DidClick += fullscreenAd => Log($"DidClick Name: {fullscreenAd.Request.PlacementName}");

ad.DidClose += (fullscreenAd, error) => Debug.Log(!error.HasValue ? $"DidClose Name: {fullscreenAd.Request.PlacementName}" : $"DidClose with Error. Name: {fullscreenAd.Request.PlacementName}, Code: {error?.Code}, Message: {error?.Message}");

ad.DidReward += fullscreenAd => Log($"DidReward Name: {fullscreenAd.Request.PlacementName}");

ad.DidRecordImpression += fullscreenAd => Log($"DidImpressionRecorded Name: {fullscreenAd.Request.PlacementName}");

ad.DidExpire += fullscreenAd => Log($"DidExpire Name: {fullscreenAd.Request.PlacementName}");

// Show `IFullscreenAd` using async approach
AdShowResult adShowResult = await ad.Show();

// Check if IFullscreenAd failed to show
var error = adShowResult.Error;

// Failed to show
if (error.HasValue)
{
    // Report show failure
    Debug.LogError($"`IFullscreenAd` Show failed with error: {JsonTools.SerializeObject(error.Value, Formatting.Indented)}");
    return;
}

// Show succeeded

// Report metrics and show sucess
var metricsJson = JsonTools.SerializeObject(adShowResult.Metrics, Formatting.Indented);
Debug.Log($"`IFullscreenAd` show completed with: Metrics: {metricsJson}");
```

### Discarding Oversized Ads

The changes to the discarding oversized ads revolved around naming convention parity.

```csharp
ChartboostMediation.DiscardOverSizedAds = true;
// or
ChartboostMediation.DiscardOverSizedAds = false;
```

### Disposing Loaded Ads

The changes to disposing loaded ads revolved around naming convention parity and architectural alignment. Starting Mediation 5.X the Ad objects implement the [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-8.0) interface to properly dispose of managed and unmanaged resources.

```csharp
fullscreenAd.Dispose();
bannerAd.Dispose();
```

## Configuring Mediation

### Test Mode

The changes to the test mode revolved around naming convention parity.

```csharp
ChartboostMediation.TestMode = true;
// or
ChartboostMediation.TestMode = false;
```

### Privacy Methods

All privacy methods have been removed from 5.X and migrated to Chartboost Core SDK.

```csharp
// COPPA
ChartboostMediation.SetSubjectToCoppa(true);
// or
ChartboostMediation.SetSubjectToCoppa(false);

//- By sending `SetSubjectToCoppa(true)`, you indicate that you want your content treated as child-directed for purposes of COPPA. We will take steps to disable interest-based advertising for such ad requests.
//- By sending `SetSubjectToCoppa(false)`, you indicate that you don't want your content treated as child-directed for purposes of COPPA. You represent and warrant that your applications and services are not directed towards children and that you will not provide any information to Mediation from a user under the age of 13.

// GDPR
ChartboostMediation.SetSubjectToGDPR(true);
// or
ChartboostMediation.SetSubjectToGDPR(false);

//- By sending `setSubjectToGDPR(true)`, you indicate that GDPR is applied to this user from your application.
//- By sending `setSubjectToGDPR(false)`, you indicate that GDPR is not applied to this user from your application.

// User Given Consent
ChartboostMediation.SetUserHasGivenConsent(true)
// or
ChartboostMediation.SetUserHasGivenConsent(false)

//- By sending `SetUserHasGivenConsent(true)`, you indicate that this user from your application has given consent to share personal data for behavior-targeted advertising.
//- By sending `SetUserHasGivenConsent(false)`, you indicate that this user from your application has not given consent to use its personal data for behavior-targeted advertising, so only contextual advertising is allowed.

// CCPA
ChartboostMediation.SetCCPAConsent(true)
// or
ChartboostMediation.SetCCPAConsent(false)

//- By sending `SetCCPAConsent(true)`, you indicate that this user from your application has given consent to share personal data for behavior-targeted advertising under CCPA regulation.
//- By sending `SetCCPAConsent(false)`, you indicate that this user from your application has not given consent to allow sharing personal data for behavior-targeted advertising under CCPA regulation.
```

## Mediation Start / Module Observer

Initialization of the Mediation SDK is now done by the [Chartboost Core SDK](https://docs.chartboost.com/en/mediation/integrate/core/unity/integration/#initializing-core-with-modules). To receive initialization callbacks, utilize the module observer.

```csharp
ChartboostCore.ModuleInitializationCompleted += result =>
{
    var moduleName = result.ModuleId == ChartboostMediation.CoreModuleId ? "Chartboost Mediation" : $"Chartboost Core module {result.ModuleId}";

    Debug.Log($"Received initialization result for: {moduleName} start:{result.Start}, end:{result.End} with duration: {result.Duration}");

    // Module failed to initialize module
    if (result.Error.HasValue) 
        Debug.LogError($"Module: {moduleName} failed to initialize with error: {JsonTools.SerializeObject(result.Error.Value)}");
    // Modue succeeded to initialize, add to list of modules to skip to pass on the next ChartboostCore.Initialize call.
    else
        modulesToSkip.Add(result.ModuleId);
};
```

## Error Codes

5.X will continue to use codes from 4.X with minor changes in the descriptions. Any new error codes introduced in 5.X will follow the same naming convention.

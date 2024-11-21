# Changelog
All notable changes to this project will be documented in this file using the standards as defined at [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0).

### Version 5.2.0 *(2024-11-14)*

Improvements:
- Added a new `BannerVisualElement` API which allows creating and loading of banner ads using Unity UI Toolkit.

### Version 5.1.0 *(2024-10-03)*

Improvements:
- Added internal logic to cache and retrieve `ILRD` events when application context is lost.
- Added internal logging logic to report debug information from native wrappers.
- Added the following events to `BannderAd` and `UnityBannerAd` API:
  - `DidBeginDrag` : Triggered when a BannerAd container begins dragging on the screen.
  - `DidEndDrag` : Triggered when a BannerAd container finishes dragging on the screen.
  
### Version 5.0.0 *(2024-08-08)*
Improvements:
- Completed rebranding all APIs to Chartboost Mediation.
- Initialization of Chartboost Mediation now utilizes Chartboost Core SDK. Please review the [Initialize Mediation](https://docs.chartboost.com/en/mediation/integrate/unity/initialize-mediation/) and [Core SDK](https://docs.chartboost.com/en/mediation/integrate/core/unity/) documentation.
- Privacy and consent signals are now set through Chartboost Core SDK.
- Ad Adapter are now distributed in separate independent packages through npm & NuGet.
- `ChartboostMediationBannerView` is now named `IBannerAd`.
- `ChartboostMediationUnityBannerAd` is now named `UnityBannerAd`.
- `ChartboostMediationFullscreenAd` is now named `IFullscreenAd`.

Removed:
- `ChartboostMediationInterstitialAd`.
- `ChartboostMediationRewardedAd`.
- Explicit `ChartboostMediation.DidStart` delegate.
- Explicit `ChartboostMediation.UnexpectedErrorOccurred` delegate.
- `AdaptersWindow` type and distribution method of adapters.

Review our [Migration from Mediation 4.x to 5.x](https://docs.chartboost.com/en/mediation/integrate/unity/migration-from-4x-to-5x/) documentation for more information.

### Version 4.9.0 *(2024-03-21)*
The following new dependencies are added :

`"com.chartboost.unity.threading": "1.0.1"`<br>
`"com.chartboost.unity.utilities": "1.0.1"`

Improvements:
- Added support for Ad Queueing. Ad Queueing is a new feature that builds upon the existing Fullscreen ad experience that allows Publishers to queue up multiple Fullscreen ads and show them in succession.

API Changes:
- Added Fullscreen Ad related events to `IChartboostMediationFullscreenAd` object.
- Marked events in `ChartboostMediationFullscreenAdLoadRequest` as deprecated. Use events from `IChartboostMediationFullscreenAd` instead.


### Version 4.8.0 *(2024-02-08)*
The following ad networks have been deprecated:
- AdColony
- TapJoy
- Yahoo

Improvements:
- New Chartboost Mediation Unity SDK demo app.
- Add `Size` to `ChartboostMediationBannerAdLoadResult`.

Bug Fixes:
- Fixed an issue with incorrect order of callbacks when `ContinueWithOnMainThread` Task extension is used.
- Fixed an issue where `CustomData` would always be null for iOS fullscreen placements.
- Fixed an issue where `SKAdNetwork` utilities would fail to account for JSON parsing issues.
- Fixed an issue where `AdapterWindows` utilities would fail to account for deprecated ad networks.
  
### Version 4.7.0 *(2023-11-14)*
Chartboost Mediation Unity SDK 4.7.0 is the last release for 2023. As such, we strive to make as many improvements and bug fixes as possible. However, there are a few breaking changes with this release in preperation for an easier transition to Chartboost Mediation 5.X in 2024.

Improvements:
- Added `AdaptersInfo` data class to Chartboost Mediation Unity SDK. Integrated ad adapter information can be fetched using `ChartboostMediation.AdaptersInfo`.
- Added per partner consent API. Ad partner consent can be set individually utilizing the new consent API.
- `Adapters.cs` has been deprecated and replaced by `Partners.cs`.
- Added `ContainerSize` property to the new `IChartboostMediationBannerView`, allowing developers to get the size of the banner container.
- Added a setter to `ChartboostMediationSettings` which allow developers to set their own instance of the scriptable object.
- Converted hard coded strings into constants across the codebase.
- Editor Windows `Adapters` and `Settings` have been modularized and can be enabled or disabled as needed.
- Added a main thread task continuation extension. Please use this if you would like to call asynchronous APIs from a synchronous code and want to handle results.
- Added distribution of package through Nuget.

The following improvements will cause breaking changes:
- `ChartboostMediationUnityBannerAdEvent` has been modified and now passes the associated instance into delegates as a parameter.
- Moved `ChartboostMediationFullscreenAdLoadResult.cs` from `Chartboost.Results` to `Chartboost.Requests` namespace.
- Removed `Chartboost.Results` namespace.

Bug Fixes:
- Fixed compiler warnings for deprecated API calls on all platforms. Compiler flag `-warnaserror` has been added to test projects to avoid future similar issues.
- Updated Usage of Getting Screen Scale on iOS 17+. `UIScreen.Main.Scale` has been deprecated.
- Fixed the issue on Android where banner ads utilizing the new `BannerView API` would always load on the `top-left` location regardless of the selected location.

### Version 4.6.0 *(2023-10-12)*
Improvements:
- Added a new `ChartboostMediationBannerView` API for banner ads which supports adaptive banners.
- Added a new `ChartboostMediationUnityBannerAd` API which allows creating and loading of banner ads using Unity gameobjects.
- EDM updated to version 1.2.177.

Bug Fixes:
- Initialization on unsupported platform now triggers `DidStart` with error.
- Logger.Logerror now calls Debug.Logerror instead of Debug.Log.

API Changes:
- Marked `ChartboostMediationBannerAd`as deprecated. Use `ChartboostMediationBannerView` instead.

### Version 4.5.0 *(2023-08-31)*
- Fixed a typo in ad-adapters.md
- Updated native dependencies to support native SDKs for Chartboost Mediation SDK.

### Version 4.4.0 *(2023-07-27)*
Improvements:
- Added new SDK initialization method `ChartboostMediation.StartWithOptions`.
- Fixed garbage collection ad disposal and invalidation logic.
- Changed `ChartboostMediation.UnexpectedSystemErrorDidOccur` to be sent into the Unity main-thread.
- Added a new `EditorWindow` and `SettingsWindow.cs` to unify the Chartboost Mediation integration into a single location. It can be accessed through the menu item `Chartboost Mediation/Configure`.
The following feature improvements can be enabled through the new settings window:
- Added build postprocessing capabilities to automatically add `GADApplicationIdentifier` to `Info.plist`. 
- Added build postprocessing capabilities to disable bitcode for XCode projects.
- Added build preprocessing capabilities to automatically add `com.google.android.gms.ads.APPLICATION_ID` to the `AndroidManifest.xml`.
- Added build preprocessing capabilities to automatically add `applovin.sdk.key` to the `AndroidManifest.xml`. 

API Changes:
- Marked `ChartboostMediationPartners` enum as deprecated. Use `ChartboostMediation.StartWithOptions` instead.

### Version 4.3.0 *(2023-06-22)*
Improvements:
- Added support for Rewarded Interstitials. This is available via `ChartboostMediation.LoadFullscreenAd()` and supported only in the latest adapters. Please check each adapter's changelog to confirm which partners support rewarded interstitials.
- Added new `ChartboostMediationFullscreenAd` APIs which combine and improve the interstitial and rewarded ad APIs. Previous interstitial and rewarded ad APIs are now deprecated.
- Added `LineItemName` and `LineItemId` to `BidInfo`.
- Added the ability to set consent and debug flags before calling `start`. Only the last change is applied and will only be updated after the SDK has finished initializing.

Bug Fixes:
- Added newly released networks into partner killswitch enum.

## Version 4.2.3 *(2023-05-18)*
Bug Fixes:
- Fixed an issue with Unity-iOS Rewarded Ads hanging onto the ad instances for too long, potentially causing duplicate callback events.

## Version 4.2.2 *(2023-05-10)*
Improvements:
- Updated `AdaptersWindow.cs` to utilize a more comprehensive JSON schema for better handling of network dependency changes.

Bug Fixes:
- Fixed an issue with InMobi SDK dependencies not being properly defined due to versioning differences.

## Version 4.2.1 *(2023-05-05)*
Bug Fixes:
- Fixed an issue where package information failed to load if projects had a large amount of packages.
- Fixed an issue where main SDK dependency could not be generated through UPM implementation.

## Version 4.2.0 *(2023-05-04)*
Added:
- Editor Window, `AdaptersWindow.cs`, for Chartboost Mediation adapters handling. For capabilities, see below:
  * Ad Adapters are now decoupled from the Chartboost Mediation UPM package.
  * Ad Adapters can be fetched, selected, and upgraded through the new Adapters Window.
  * `Adapters Window` can be accessed through the `Chartboost Mediation/Adapters` menu.
  * Public C# API is available to automate tasks such as upgrading, adding, loading, and saving Ad Adapter selections.
    *Adapters information is automatically fetched on startup and manually through the `Adapters Window`.

Improvements:
- Fixed standardized error reporting of `ChartboostMediationError` components in iOS.

API Changes:
- Marked `ChartboostMediationIntegrationChecker.cs` as deprecated. Please utilize `AdaptersWindow.cs` instead.

## Version 4.1.0 *(2023-03-30)*
Improvements:
- Simplified AppId & AppSignature accessors in `ChartboostMediationSettings`.
- Added TestMode call to `ChartboostMediation.cs`.
- Made Chartboost Mediation errors readable from iOS native layer to Unity.
- Refactored native bridge projects to follow rebranding & newer APIs.
- Fixed usage of placements for unsupported platforms.
- Updated documentation with AdMob & AppLovin AndroidManifest special cases.
 
Bug Fixes:
- Fixed placements `clearLoaded` in native implementations to return void instead of boolean.
- Fixed an issue for iOS banners crashing on removal.

## Version 4.0.0 *(2023-02-03)*
As part of the Marketing team’s efforts to clearly articulate the use cases and customers we support by being more descriptive with our product branding, Helium is being rebranded as Chartboost Mediation.

Starting in 4.0.0, the Chartboost Mediation brand will be used in place of Helium for new additions. In the coming 4.X releases, the old Helium branding will be deprecated and the new Chartboost Mediation branding will be used to give publishers a smoother transition.

Improvements:
- Revamped Helium delegates & callbacks API.
- Added optional PostProcessor to simplify SKAdNetwork integration process.
- Refactored Ad Placement objects so they return empty values in unsupported platforms.
- Cleanup and Refactoring of iOS Dependencies, native SDKs are now explicitly stated.
- Fixed LogError method in HeliumExternal.
- Banners `ClearLoaded` method no longer returns a boolean value.
- Removed `reward` parameter from Reward callback since value was not being utilized.
- Minimum iOS version changed to 12.0 when using Amazon Publisher Services adapter.

Deletions:
- Removed obsolete APIs in `HeliumSettings.cs` new accessors are available.

API Changes & Rebranding:

- New npm package distribution under `com.chartboost.mediation`
- Namespace `Helium` deprecated and replaced with `Chartboost`.
- `HeliumIntegrationChecker` deprecated and replaced with `ChartboostMediationIntegration Checker`.
- `HeliumError` removed and replaced with descriptive `string` values.
- `HeliumBidInfo` renamed to `BidInfo`
- Delegates have been renamed from `Helium` to `ChartboostMediation`:
  - `ChartboostMediationPlacementLoadEvent` added, it provides bid info on load.
  - `HeliumRewardEvent` converted to standard `ChartboostMediationPlacementEvent`.
  - `HeliumBidEvent` merged into Load delegate mentioned above.
- `HeliumInterstitialAd` renamed to `ChartboostMediationInterstitialAd`
- `HeliumRewardedAd` renamed to `ChartboostMediationRewardedAd`
- `HeliumBannerAd` renamed to `ChartboostMediationBannerAd`
- `HeliumBannerAdSize` renamed to `ChartboostMediationBannerAdSize`
- `HeliumBannerAdScreenLocation` renamed to `ChartboostMediationBannerAdScreenLocation`
- `HeliumSettings` renamed with `ChartboostMediationSettings`
- `HeliumPartners` renamed to `ChartboostMediationPartners`

For details on how to migrate from Helium 3.X to Chartboost Mediation 4.0.0, visit the corresponding documentation.

## Version 3.3.2 *(2023-01-26)*
Bug Fixes:
- Downgraded Yahoo 1.3.0 to Verizon 1.14.0.

## Version 3.3.1 *(2022-12-06)*
Bug Fixes:
- Updated FAN Dependency from 6.9.0 to 6.12.0

## Version 3.3.0 *(2022-12-01)*
Improvements:
- Refactoring of HeliumSettings Scriptable Object API.
- Network Kill Switch.
- Provide a Publisher API for Partner Initialization Metrics.

Bug Fixes:
- `NullReferenceException` after initialization without callback subscription.
- Credentials warning spamming
- Detect Misconfigured Unity setup & utility methods.

### Version 3.2.0 *(2022-10-20)*
----------------------------
Improvements:
- Converted Android Java bridge to Kotlin.

Documentation:
- Improved UnityAds integration guide and requirements.

### Version 3.1.0 *(2022-09-15)*
----------------------------
Improvements:
- Removed proguard for Android/Unity integration.

Bug Fixes:
- Clear Ad requests return proper clear status. Banners always return true when cleared

### Version 3.0.0 *(2022-08-18)*
----------------------------
Improvements:
- Unity Package Manager distribution of the Helium SDK.
- Helium Unity APIs updated to follow Unity/C# standard coding conventions.
- Banner API updated from a load-show to a load-only paradigm.
- Helium impression events are now separate from partner network impression events.
- Sequential waterfall ad loading for all ad formats.
- Removal of UnitySendMessage callbacks for proper Native callbacks on Android & iOS
- `HeliumBannerAd`   
  - `Show` removed. Banners are ready to show upon successful load.
  - `ReadyToShow` removed since it is no longer necessary.
- `HeliumSDK`   
  - `DidShowBanner` callback removed since it is no longer necessary.
  - Added the following callbacks for when Helium determines an ad to be visible on screen.
    - `DidRecordImpressionInterstitial`
    - `DidRecordImpressionRewarded`
    - `DidRecordImpressionBanner`   

Bug Fixes:
- Ad load requests to the native Helium SDKs are dispatched to a background thread.

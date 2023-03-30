# Changelog
All notable changes to this project will be documented in this file using the standards as defined at [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0).

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

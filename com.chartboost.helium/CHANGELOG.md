# Changelog
All notable changes to this project will be documented in this file using the standards as defined at [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0).

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

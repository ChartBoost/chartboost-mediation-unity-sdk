# Changelog
All notable changes to this project will be documented in this file using the standards as defined at [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0).

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

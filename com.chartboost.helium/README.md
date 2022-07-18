# Helium Unity SDK

[![Helium Unity SDK](https://github.com/ChartBoost/helium-unity-sdk/actions/workflows/helium-unity.yml/badge.svg?branch=develop)](https://github.com/ChartBoost/helium-unity-sdk/actions/workflows/helium-unity.yml)

Helium Unity SDK as a compliant Unity Package.

## Summary

Helium Unity SDK provides support for Unity based games to easily integrate the Helium SDK for Android & iOS platforms. This guide will contain step by step instructions on how to integrate the SDK, as well as recommended practices to make best use of all the features offered by the Helium SDK.

### Minimum Supporter Development Tools <a name="dev_tools"></a>

| Software                                                              | Version             |
| :---                                                                  | :---                |
| [XCode](https://developer.apple.com/xcode/)                           | 13.1                |
| [Android Studio](https://developer.android.com/studio)                | 2020.3.1+           |
| [iOS](https://www.apple.com/ios)                                      | 10.0+               |
| [Android OS](https://developer.android.com/studio/releases/platforms) | 5.0+ (API level 21) |

### Integrating the Helium Unity SDK

Helium Unity SDK is distributed using the publich [npm registry](https://www.npmjs.com/search?q=com.chartboost) as such it is compatible with the Unity Package Manager (UPM). In order to add the Helium Unity SDK to your project, just add the following to your Unity Project's ***manifest.json*** file.

```json
  "dependencies": {
    "com.chartboost.helium": "3.0.0",
    ...
  },
```

In order to better understand the Helium Unity SDK, documentation has been split between Setup and Integration Steps:

#### Setup
1. Ad Networks Adapters
2. [Helium App ID & App Signature](com.chartboost.helium/Documentation/integration/helium-ids.md)
3. [Using Proguard](com.chartboost.helium/Documentation/integration/using-proguard.md)
4. [Samples  & Google External Dependency Manager (EDM)](com.chartboost.helium/Documentation/integration/edm.md)

#### Integration

1. [Initialization](com.chartboost.helium/Documentation/integration/initialization.md)
2. [Loading Ads](com.chartboost.helium/Documentation/integration/loading-ads.md)
3. [Showing Ads](com.chartboost.helium/Documentation/integration/showing-ads.md)
4. [Delegate Usage](com.chartboost.helium/Documentation/integration/delegate-usage.md)
5. [Error Codes](com.chartboost.helium/Documentation/integration/error-codes.md)
6. [Unit Testing](com.chartboost.helium/Documentation/integration/unit-testing.md)

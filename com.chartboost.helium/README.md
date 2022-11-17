# Helium Unity SDK

[![Helium Unity SDK](https://github.com/ChartBoost/helium-unity-sdk/actions/workflows/helium-unity.yml/badge.svg?branch=develop)](https://github.com/ChartBoost/helium-unity-sdk/actions/workflows/helium-unity.yml)

## Summary

Helium Unity SDK provides support for Unity based games to easily integrate the Helium SDK for Android & iOS platforms. This guide will contain step by step instructions on how to integrate the SDK, as well as recommended practices to make best use of all the features offered by the Helium SDK.

### Minimum Supporter Development Tools <a name="dev_tools"></a>

| Software                                                              | Version              |
| :---                                                                  |:---------------------|
| [XCode](https://developer.apple.com/xcode/)                           | 14.1                 |
| [Android Studio](https://developer.android.com/studio)                | 2020.3.1+            |
| [iOS](https://www.apple.com/ios)                                      | 10.0+                |
| [Minimum Android API Level](https://developer.android.com/studio/releases/platforms#5.0) | 5.0+ (API level 21)  |
| [Target Android API Level](https://developer.android.com/studio/releases/platforms#12) | 12.0+ (API level 31) |

### CHANGELOG
Visit the [CHANGELOG](com.chartboost.helium/CHANGELOG.md) to reference changes to each version of the Helium Unity SDK.

### Integrating the Helium Unity SDK

Helium Unity SDK is distributed using the public [npm registry](https://www.npmjs.com/search?q=com.chartboost) as such it is compatible with the Unity Package Manager (UPM). In order to add the Helium Unity SDK to your project, just add the following to your Unity Project's ***manifest.json*** file. The scoped registry section is required in order to fetch packages from the NpmJS registry.

```json
  "dependencies": {
    "com.chartboost.helium": "3.3.0",
    ...
  },
  "scopedRegistries": [
    {
      "name": "NpmJS",
      "url": "https://registry.npmjs.org",
      "scopes": [
        "com.chartboost"
      ]
    }
  ]
```

In order to better understand the Helium Unity SDK, documentation has been split between Setup and Integration Steps:

#### Setup
1. [Ad Network Adapters](com.chartboost.helium/Documentation/setup/ad-adapters.md)
2. [Android Manifest](com.chartboost.helium/Documentation/setup/androidmanifest.md)
3. [Google External Dependency Manager (EDM)](com.chartboost.helium/Documentation/setup/edm.md)
4. [Multidex](com.chartboost.helium/Documentation/setup/multidex.md)
5. [Helium Integration Checker](com.chartboost.helium/Documentation/integration-checker.md)

#### Integration

1. [Initialization](com.chartboost.helium/Documentation/integration/initialization.md)
2. [Configure Helium](com.chartboost.helium/Documentation/integration/configure.md)
3. [Loading Ads](com.chartboost.helium/Documentation/integration/loading-ads.md)
4. [Showing Ads](com.chartboost.helium/Documentation/integration/showing-ads.md)
5. [Delegate Usage](com.chartboost.helium/Documentation/integration/delegate-usage.md)
6. [Error Codes](com.chartboost.helium/Documentation/integration/error-codes.md)
7. [Unit Testing](com.chartboost.helium/Documentation/integration/unit-testing.md)

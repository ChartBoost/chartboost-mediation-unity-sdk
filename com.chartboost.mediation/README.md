# Chartboost Mediation Unity SDK

[![Chartboost Mediation Unity SDK](https://github.com/ChartBoost/helium-unity-sdk/actions/workflows/status.yml/badge.svg?branch=develop)](https://github.com/ChartBoost/helium-unity-sdk/actions/workflows/status.yml)

## Summary

Chartboost Mediation Unity SDK provides support for Unity based games to easily integrate the Chartboost Mediation for Android & iOS platforms. This guide will contain step by step instructions on how to integrate the SDK, as well as recommended practices to make best use of all the features offered by the Chartboost Mediation SDK.

### Minimum Supporter Development Tools

| Software                                                              | Version              |
| :---                                                                  |:---------------------|
| [XCode](https://developer.apple.com/xcode/)                           | 14.1                 |
| [Android Studio](https://developer.android.com/studio)                | 2020.3.1+            |
| [iOS](https://www.apple.com/ios)                                      | 10.0+                |
| [Minimum Android API Level](https://developer.android.com/studio/releases/platforms#5.0) | 5.0+ (API level 21)  |
| [Target Android API Level](https://developer.android.com/studio/releases/platforms#12) | 12.0+ (API level 31) |

### CHANGELOG
Visit the [CHANGELOG](com.chartboost.mediation/CHANGELOG.md) to reference changes to each version of the Chartboost Mediation Unity SDK.

### Integrating the Chartboost Mediation Unity SDK

Chartboost Mediation Unity SDK is distributed using the public [npm registry](https://www.npmjs.com/search?q=com.chartboost.mediation) as such it is compatible with the Unity Package Manager (UPM). In order to add the Chartboost Mediation Unity SDK to your project, just add the following to your Unity Project's ***manifest.json*** file. The scoped registry section is required in order to fetch packages from the NpmJS registry.

```json
  "dependencies": {
    "com.chartboost.mediation": "4.0.0",
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

In order to better understand the Chartboost Mediation Unity SDK, documentation has been split between Setup and Integration Steps:

#### Setup
1. [Ad Network Adapters](com.chartboost.mediation/Documentation/setup/ad-adapters.md)
2. [Android Manifest](com.chartboost.mediation/Documentation/setup/androidmanifest.md)
3. [Google External Dependency Manager (EDM)](com.chartboost.mediation/Documentation/setup/edm.md)
4. [Multidex](com.chartboost.mediation/Documentation/setup/multidex.md)
5. [Chartboost Mediation Integration Checker](com.chartboost.mediation/Documentation/setup/integration-checker.md)
6. [Enabling SKAdNetwork](com.chartboost.mediation/Documentation/setup/skadnetwork.md)

#### Integration

1. [Initialization](com.chartboost.mediation/Documentation/integration/initialization.md)
2. [Configure Chartboost Mediation](com.chartboost.mediation/Documentation/integration/configure.md)
3. [Loading Ads](com.chartboost.mediation/Documentation/integration/loading-ads.md)
4. [Showing Ads](com.chartboost.mediation/Documentation/integration/showing-ads.md)
5. [Delegate Usage](com.chartboost.mediation/Documentation/integration/delegate-usage.md)
6. [Error Codes](com.chartboost.mediation/Documentation/integration/error-codes.md)
7. [Unit Testing](com.chartboost.mediation/Documentation/integration/unit-testing.md)

### [FAQ](com.chartboost.mediation/Documentation/faq.md)

### [Migrating Helium 3.X to Chartboost Mediation 4.0.0](com.chartboost.mediation/Documentation/integration/helium-to-chartboost-mediation.md)
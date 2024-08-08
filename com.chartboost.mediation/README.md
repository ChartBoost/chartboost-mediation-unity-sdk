# Chartboost Mediation Unity SDK

[![Chartboost Mediation Unity SDK](https://github.com/ChartBoost/helium-unity-sdk/actions/workflows/status.yml/badge.svg?branch=develop)](https://github.com/ChartBoost/helium-unity-sdk/actions/workflows/status.yml)

## Summary

Chartboost Mediation Unity SDK provides support for Unity based games to easily integrate the Chartboost Mediation for Android & iOS platforms. This guide will contain step by step instructions on how to integrate the SDK, as well as recommended practices to make best use of all the features offered by the Chartboost Mediation SDK.

## Minimum Requirements

| Plugin | Version |
| ------ | ------ |
| Cocoapods | 1.11.3+ |
| iOS | 11.0+ |
| Xcode | 14.1+ |
| Android API | 21+ |
| Unity | 2022.3.+ |

# Integration

## Using the public [npm registry](https://www.npmjs.com/search?q=com.chartboost.mediation)

In order to add the Chartboost Core Unity SDK to your project using the npm package, add the following to your Unity Project's ***manifest.json*** file. The scoped registry section is required in order to fetch packages from the NpmJS registry.

```json
"dependencies": {
    "com.chartboost.mediation": "5.0.0",
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
## Using the public [NuGet package](https://www.nuget.org/packages/Chartboost.CSharp.Mediation.Unity)

To add the Chartboost Mediation Unity SDK to your project using the NuGet package, you will first need to add the [NugetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) package into your Unity Project.

This can be done by adding the following to your Unity Project's ***manifest.json***

```json
  "dependencies": {
    "com.github-glitchenzo.nugetforunity": "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity",
    ...
  },
```

Once <code>NugetForUnity</code> is installed, search for `Chartboost.CSharp.Mediation.Unity` in the search bar of Nuget Explorer window(Nuget -> Manage Nuget Packages).
You should be able to see the `Chartboost.CSharp.Mediation.Unity` package. Choose the appropriate version and install.

# Setup
1. [Ad Network Adapters](com.chartboost.mediation/Documentation/setup/ad-adapters.md)
2. [Android Manifest](com.chartboost.mediation/Documentation/setup/androidmanifest.md)
3. [Google External Dependency Manager (EDM)](com.chartboost.mediation/Documentation/setup/edm.md)
4. [Multidex](com.chartboost.mediation/Documentation/setup/multidex.md)

# Integration

1. [Initialization](com.chartboost.mediation/Documentation/integration/initialization.md)
2. [Configure](com.chartboost.mediation/Documentation/integration/configure.md)
3. [Fullscreen Ad](com.chartboost.mediation/Documentation/integration/fullscreen.md)
4. [Fullscreen Ad Queue](com.chartboost.mediation/Documentation/integration/fullscreenadqueue.md)
5. [Banner Ad](com.chartboost.mediation/Documentation/integration/bannerad.md)
6. [Error Codes](com.chartboost.mediation/Documentation/integration/error-codes.md)
7. [Unit Testing](com.chartboost.mediation/Documentation/integration/unit-testing.md)
8. [FAQ](com.chartboost.mediation/Documentation/faq.md)

# Migration Guides

1. [Migration from v4 to v5](com.chartboost.mediation/Documentation/migration/v4tov5.md)
2. [Migration from v3 to v4](com.chartboost.mediation/Documentation/migration/v3tov4.md)
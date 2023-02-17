# Migrating Helium 3.X to Chartboost Mediation 4.0.0

As part of the Marketing team’s efforts to clearly articulate the use cases and customers we support by being more descriptive with our product branding, `Helium` is being rebranded as `Chartboost Mediation`.

Starting in 4.0.0, the `Chartboost Mediation` brand will be used in place of `Helium` for new additions. In the coming 4.X releases, the old `Helium` branding will be deprecated and the new `Chartboost Mediation` branding will be used to give publishers a smoother transition.

This document exist to specifically address all possible issues that developers might encounter when migrating from `Helium 3.X` integrations to `Chartboost Mediation 4.0.0`

## npm Package & UPM
As part of Chartboost’s rebranding efforts, Chartboost Mediation 4.0.0 will be using a new npmjs package for distribution of the Chartboost Mediation Unity SDK. 

| Helium 3.X | Chartboost Mediation 4.0.0 |
| :--- |:--- |
| [com.chartboost.helium](https://www.npmjs.com/package/com.chartboost.helium) | [com.chartboost.mediation](www.npmjs.com/package/com.chartboost.mediation) |

The new manifest.json integration for Unity should look like the following: 

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

## API Changes


### Helium Name

The `Helium` name has been deprecated and replaced by `ChartboostMediation`. We will delete this during the 4.x series. 

Unity will produce any `warnings` whenever any deprecated APIs are in use, detailed descriptions on how to address and fix such warnings are available for each scenario.

Here is an example scenario on how to address such issues

```csharp
    // This will produce a warning as it has been deprecated
    HeliumSDK.SetUserIdentifier("DEFAULT");

    // New 4.0.0 API
    ChartboostMediation.SetUserIdentifier("DEFAULT");
```

In most scenarios, a find and replace from `HeliumSDK` to `ChartboostMediation` should be enough to fix the warnings. However, namespaces have also been modified, look at the section below for more details


### Namespace

The `Helium` namespace has been deprecated and removed from most places in the `Chartboost Mediation Unity SDK`. It is still available for most deprecated classes, but it is highly recommended to transition to the new `Chartboost` namespace. All rebranded and refactored APIs available under the Chartboost namespace.

In the past the code would look something like this: 
```csharp
    using Helium;

    // This will produce a warning as it has been deprecated
    HeliumSDK.SetUserIdentifier("DEFAULT");
```

The new APIs will utilize the `Chartboost` namespace as follows:

```csharp
    using Chartboost;

    // New 4.0.0 API
    ChartboostMediation.SetUserIdentifier("DEFAULT");
```

### Editor Classes
All `Chartboost Mediation Unity SDK` classes have been rebranded. Obsolete APIs are still available but as mentioned above, it is highly recommended to transition to the new existing APIs.

#### HeliumIntegrationChecker

The `HeliumIntegrationChecker` has been replaced with the `ChartboostMediationIntegrationChecker`, all functionality remained the same, and this is purely just a rebranding transition. The `HeliumIntegrationChecker` API is still available; however, it will be removed in future versions.


`HeliumIntegrationChecker` deprecated API:
```csharp
  // location Helium.Editor.HeliumIntegrationChecker

  ReimportExistingHeliumSamples(ICollection<string> existingSamples, string version)

  ReimportExistingAdapters()
```
 
`ChartboostMediationIntegrationChecker` new API:

```csharp
  // Chartboost.Editor.ChartboostMediationIntegrationChecker

  ReimportExistingSamplesSet(ICollection<string> existingSamples, string version)

  ReimportAllExistingAdapters()
```

### Runtime Classes

Most of the `Chartboost Mediation Unity SDK` Runtime classes have been rebranded or modified to adapt Chartboost's latest rebranding efforts. As such, most references to `Helium` have been removed or replaced in one way or another.

#### HeliumError

The `HeliumError` class has been entirely removed and replaced with a simple descriptive `string`. Visit [Error Codes](com.chartboost.mediation/Documentation/integration/error-codes.md) for more details.

#### HeliumBidInfo

The `HeliumBidInfo` struct has been rebranded as `BidInfo`, you can easily fix this by finding and replacing all references to `HeliumBidInfo` with `BidInfo`.

#### Delegates

Delegates have been modified to adopt the latest rebranding efforts as well as API changes.

Old delegates
```csharp
    public delegate void HeliumEvent(HeliumError error);

    public delegate void HeliumILRDEvent(string placement, Hashtable impressionData);

    public delegate void HeliumPartnerInitializationEvent(string partnerInitializationEventData);

    public delegate void HeliumPlacementEvent(string placement, HeliumError error);
    
    public delegate void HeliumBidEvent(string placement, HeliumBidInfo bid);

    public delegate void HeliumRewardEvent(string placement, int reward);
```

New delegates

```csharp
    public delegate void ChartboostMediationEvent(string error);

    public delegate void ChartboostMediationILRDEvent(string placement, Hashtable impressionData);

    public delegate void ChartboostMediationPartnerInitializationEvent(string partnerInitializationEventData);

    public delegate void ChartboostMediationPlacementEvent(string placement, string error);

    public delegate void ChartboostMediationPlacementLoadEvent(string placement, string loadId, BidInfo bidInfo, string error);
```

As seen above, the `HeliumError` references have been replaced with descriptive `strings`. Additionally, the `HeliumBidEvent` has merged with a new `LoadEvent`.

#### Interstitials

The `HeliumInterstitialAd` class has been deprecated and replaced with `ChartboostMediationInterstitialAd`, to facilitate the migration process you can find and replace all references to `HeliumInterstitialAd` with `ChartboostMediationInterstitialAd`. Below there is a list of all changes:

`HeliumInterstitialAd` old API:
```csharp
  // location Helium.HeliumInterstitialAd

  bool ClearLoaded()
```
 
`ChartboostMediationInterstitialAd` new API:

```csharp
  // Chartboost.FullScreen.Interstitial.ChartboostMediationInterstitialAd

  ReimportExistingSamplesSet(ICollection<string> existingSamples, string version)

  // ClearLoaded no longer returns a value
  void ClearLoaded()
```

#### Rewarded Videos

The `HeliumRewardedAd` class has been deprecated and replaced with `ChartboostMediationRewardedAd`, to facilitate the migration process you can find and replace all references to `HeliumRewardedAd` with `ChartboostMediationRewardedAd`. Below there is a list of all changes:

`HeliumRewardedAd` old API:
```csharp
  // location Helium.HeliumRewardedAd

  bool ClearLoaded()
```
 
`ChartboostMediationRewardedAd` new API:

```csharp
  // Chartboost.FullScreen.Rewarded.ChartboostMediationRewardedAd

  // ClearLoaded no longer returns a value
  void ClearLoaded()
```

#### Banners

The `HeliumBannerAd` class has been deprecated and replaced with `ChartboostMediationBannerAd`, to facilitate the migration process you can find and replace all references to `HeliumBannerAd` with `ChartboostMediationBannerAd`. Below there is a list of all changes:

`HeliumBannerAd` old API:
```csharp
  // location Helium.HeliumBannerAd

  bool ClearLoaded()
```
 
`ChartboostMediationBannerAd` new API:

```csharp
  // Chartboost.Banner.ChartboostMediationBannerAd

  // ClearLoaded no longer returns a value
  void ClearLoaded()
```

#### HeliumSettings
The `HeliumSettings` class has been deprecated and replaced with the `ChartboostMediationSettings` class. To update, just remove your old settings and create a new one. This will make sure your new `ChartboostMediationSettings` Scriptable Object is properly located.

Additionally, the enum `HeliumPartners` has been renamed to `ChartboostMediationPartners`.

### Partner Adapters

The process of updating partner adapters remains the same as before. Use the Unity Editor menu under `Chartboost Mediation/Integration/Status Check`, this will make sure your integration meets our baseline requirements for proper functionality and it will ask you to update the adapters as well. Additionally, if you want to forcefully update all adapters, you can use `Chartboost Mediation/Integration/Force Reimport Adapters`. 

Some adapters have been renamed due to Chartboost rebranding efforts, as seen below.

| Helium 3.X | Chartboost Mediation 4.0.0 |
| :--- |:--- |
| Helium | Chartboost Mediation |
| Chartboost | Chartboost Monetization |
| Facebook | Meta Audience Network |
| Fyber | Digital Turbine Exchange |



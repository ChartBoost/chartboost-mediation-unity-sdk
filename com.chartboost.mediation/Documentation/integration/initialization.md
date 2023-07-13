# Initialization

## Adding the import header

Add the following import header to the top of any class file that will be using a Chartboost Mediation class.

```c#
using Chartboost;
```

## Initializing Chartboost Mediation Unity SDK

In order to initialize the Chartboost Mediation Unity SDK, you will need your Chartboost Mediation App ID & App Signature. This can be obtained in your [Chartboost Mediation Dashboard](https://helium.chartboost.com).
There are 2 ways you can go about providing your App IDs to the SDK.

### Chartboost Mediation Settings & Automatic Initialization

Visit [Chartboost Mediation Settings](../setup/settings.md) for more information on how to utilize the `ChartboostMediationSettings` `ScriptableObject` settings.

### Manual Initialization

If you would like to have more control over when to initialize the Chartboost Mediation SDK, call the following on your Awake method.

```c#
// New Manual Initialization after 4.1.0
ChartboostMediation.StartWithOptions(ChartboostMediationSettings.AppId, ChartboostMediationSettings.AppSignature);

// Old Style of Manual Initialization Not Using ChartboostMediationSettings Scritable Object
var appId = "";
var appSignature = "";

#if UNITY_ANDROID
appId = "ANDROID_SAMPLE_APP_ID";
appSignature = "ANDROID_SAMPLE_APP_SIGNATURE";
#elif UNITY_IOS
appId = "IOS_SAMPLE_APP_ID";
appSignature = "IOS_SAMPLE_APP_SIGNATURE";
#endif

ChartboostMediation.StartWithOptions(appID, appSignature);
```

This will start the Chartboost Mediation Unity SDK. For delegate information see section [Delegate Usage](delegate-usage.md)

> **Warning** \
> Failing to remove default values will result in an error.

Once the Chartboost Mediation SDK has successfully started, you can start requesting ads.

### Partner Kill Switch
The Chartboost Mediation Unity SDK initialization method has been expanded to take in optional initialization parameters. One of those parameters is a set of network adapter identifiers to skip initialization for the session.


```c#
var options = new[]{"network_identifier", "network_identifier2"};
ChartboostMediation.StartWithOptions(ChartboostMediationSettings.AppId, ChartboostMediationSettings.AppSignature, options);
```

For more information on how to corroborate partner initialization data visit [Delegate Usage](delegate-usage.md)

#### Network Adapter Identifiers

| Network                     | Identifier            |
|-----------------------------|-----------------------|
| AdColony                    | adcolony              |
| AdMob                       | admob                 |
| Amazon Publisher Services   | amazon_aps            |
| AppLovin                    | applovin              |
| Meta Audience Network       | facebook              |
| Digital Turbine Exchange    | fyber                 |
| Google Bidding              | google_googlebidding  |
| InMobi                      | inmobi                |
| IronSource                  | ironsource            |
| Mintegral                   | mintegral             |
| Pangle                      | pangle                |
| Tapjoy                      | tapjoy                |
| Unity                       | unity                 |
| Vungle                      | vungle                |
| Yahoo                       | yahoo                 |
| MobileFuse                  | mobilefuse            |
| Verve                       | verve                 |
| HyprMX                      | hyprmx                |

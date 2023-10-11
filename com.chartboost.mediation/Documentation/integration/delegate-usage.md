# Delegate Usage

## Overview

Chartboost Mediation's delegate methods allow you to exercise a greater degree of control over your integration. For example, you can:

* Log debug messages when your game attempts to load an interstitial
* Prevent ads from showing the first time a user plays your game
* Determine whether a user has closed an ad
* Get the price of the bid that won the auction

## Delegates Setup

The Chartboost Mediation Unity SDK implements its delegate functionality using C# style delegates and events. Before using any of the delegate methods, you should first subscribe to the relevant SDK events in your MonoBehaviour as demonstrated:

### Subscribing Static Delegates
```c#
private void OnEnable() {
    // Start Delegate
    ChartboostMediationSDK.DidStart += DidStart;

    // ILRD Delegate
    ChartboostMediationSDK.DidReceiveImpressionLevelRevenueData += DidReceiveImpressionLevelRevenueData;

    // Partner Initialization Data Delegate
    ChartboostMediation.DidReceivePartnerInitializationData += DidReceivePartnerInitializationData;

    // Error Handling Delegate
    ChartboostMediation.UnexpectedSystemErrorDidOccur += UnexpectedSystemErrorDidOccur;
}
```

> **Warning** \
> Do not make cache or show calls inside a delegate indicating that an ad has just failed to load.

> **Note** \
> Notice that banner ads do not have a `DidCloseBanner` delegate. Please keep that in mind when implementing Banners.

> **Note** \
> Not all of the partner SDKs have support for the DidClick delegate.

### Unsubscribing Delegates

You should also make sure to unsubscribe to those same events when appropriate:

```c#
private void OnDisable() {
    // Remove event handlers
    ChartboostMediation.DidStart -= DidStart;

    // ILRD Delegate
    ChartboostMediation.DidReceiveImpressionLevelRevenueData -= DidReceiveImpressionLevelRevenueData;

    // Partner Initialization Data Delegate
    ChartboostMediation.DidReceivePartnerInitializationData -= DidReceivePartnerInitializationData;

    // Error Handling Delegate
    ChartboostMediation.UnexpectedSystemErrorDidOccur -= UnexpectedSystemErrorDidOccur;
}
```

### Subscribing Delegates

### Lifecycle Delegates
```c#
// Start Delegate
private void DidStart(string error)
{
    Debug.Log($"DidStart: {error}");
    // Logic goes here
}

// ILRD Delegate
private void DidReceiveImpressionLevelRevenueData(string placement, Hashtable impressionData)
{
    var json =  JsonTools.Serialize(impressionData);
    Debug.Log($"DidReceiveImpressionLevelRevenueData {placement}: {json}");
}

// Partner Initialization Data Delegate
private void DidReceivePartnerInitializationData(string partnerInitializationData)
{
    Debug.Log($"DidReceivePartnerInitializationData: ${partnerInitializationData}");
}

// Error Handling Delegate
private static void UnexpectedSystemErrorDidOccur(string error)
{
    Debug.LogErrorFormat(error);
}
```


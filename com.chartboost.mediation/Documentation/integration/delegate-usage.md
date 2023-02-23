# Delegate Usage

## Overview

The Chartboost Mediaiton delegate methods allow you to exercise a greater degree of control over your integration. For example, you can:

* Log debug messages when your game attempts to load an interstitial
* Prevent ads from showing the first time a user plays your game
* Determine whether a user has closed an ad
* Get the price of the bid that won the auction

## Delegates Setup

The Chartboost Mediation SDK implements its delegate functionality using C# style delegates and events. Before using any of the delegate methods, you should first subscribe to the relevant SDK events in your MonoBehaviour as demonstrated:

### Subscribing Delegates
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

    // Interstitial Ad Delegates
    ChartboostMediation.DidLoadInterstitial += DidLoadInterstitial;
    ChartboostMediation.DidShowInterstitial += DidShowInterstitial;
    ChartboostMediation.DidCloseInterstitial += DidCloseInterstitial;
    ChartboostMediation.DidClickInterstitial += DidClickInterstitial;
    ChartboostMediation.DidRecordImpressionInterstitial += DidRecordImpressionInterstitial;

    // Rewarded Ad Delegates
    ChartboostMediation.DidLoadRewarded += DidLoadRewarded;
    ChartboostMediation.DidShowRewarded += DidShowRewarded;
    ChartboostMediation.DidCloseRewarded += DidCloseRewarded;
    ChartboostMediation.DidReceiveReward += DidReceiveReward;
    ChartboostMediation.DidClickRewarded += DidClickRewarded;
    ChartboostMediation.DidRecordImpressionRewarded += DidRecordImpressionRewarded;

    // Banner Ad Delegates
    ChartboostMediation.DidLoadBanner += DidLoadBanner;
    ChartboostMediation.DidClickBanner += DidClickBanner;
    ChartboostMediation.DidRecordImpressionBanner += DidRecordImpressionBanner;
}
```

> **_NOTE 1:_** Do not make cache or show calls inside a delegate indicating that an ad has just failed to load.

> **_NOTE 2:_** Notice that banner ads do not have a DidCloseBanner delegate. Please keep that in mind when implementing Banners.

> **_NOTE 3:_** Not all of the partner SDKs have support for the DidClick delegate.

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

    // Interstitial Ad Delegates
    ChartboostMediation.DidLoadInterstitial -= DidLoadInterstitial;
    ChartboostMediation.DidShowInterstitial -= DidShowInterstitial;
    ChartboostMediation.DidCloseInterstitial -= DidCloseInterstitial;
    ChartboostMediation.DidClickInterstitial -= DidClickInterstitial;
    ChartboostMediation.DidRecordImpressionInterstitial -= DidRecordImpressionInterstitial;

    // Rewarded Ad Delegates
    ChartboostMediation.DidLoadRewarded -= DidLoadRewarded;
    ChartboostMediation.DidShowRewarded -= DidShowRewarded;
    ChartboostMediation.DidCloseRewarded -= DidCloseRewarded;
    ChartboostMediation.DidReceiveReward -= DidReceiveReward;
    ChartboostMediation.DidClickRewarded -= DidClickRewarded;
    ChartboostMediation.DidRecordImpressionRewarded -= DidRecordImpressionRewarded;

    // Banner Ad Delegates
    ChartboostMediation.DidLoadBanner -= DidLoadBanner;
    ChartboostMediation.DidClickBanner -= DidClickBanner;
    ChartboostMediation.DidRecordImpressionBanner -= DidRecordImpressionBanner;
}
```

## Example Delegate Methods

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

### Interstitial Ad Delegates
```c#
private void DidLoadInterstitial(string placementName, string loadId, BidInfo info, string error)
{
    Debug.Log($"DidLoadInterstitial {placementName}, Price: ${info.Price:F4}, Auction Id: {info.AuctionId}, Partner Id: {info.PartnerId}. {error}");
}

private void DidShowInterstitial(string placementName, string error)
{
    Debug.Log($"DidShowInterstitial {placementName}: {error}");
}

private void DidCloseInterstitial(string placementName, string error)
{
    Debug.Log($"DidCloseInterstitial {placementName}: {error}");
}

private void DidClickInterstitial(string placementName, string error)
{
    Debug.Log($"DidClickInterstitial {placementName}: {error}");
}

private void DidRecordImpressionInterstitial(string placementName, string error)
{
    Log($"DidRecordImpressionInterstitial {placementName}: {error}");
}
```

### Rewarded Ad Delegates
```c#
private void DidLoadRewarded(string placementName, string loadId, BidInfo info, string error)
{
    Debug.Log($"DidLoadRewarded {placementName}, Price: ${info.Price:F4}, Auction Id: {info.AuctionId}, Partner Id: {info.PartnerId}. {error}");
}

private void DidShowRewarded(string placementName, string error)
{
    Debug.Log($"DidShowRewarded {placementName}: {error}");
}

private void DidCloseRewarded(string placementName, string error)
{
    Debug.Log($"DidCloseRewarded {placementName}: {error}");
}

private void DidClickRewarded(string placementName, string error)
{
    Debug.Log($"DidClickRewarded {placementName}: {error}");
}

private void DidReceiveReward(string placementName, string error)
{
    Debug.Log($"DidReceiveReward {placementName}: {error}");
}

private void DidRecordImpressionRewarded(string placementName, string error)
{
    Log($"DidRecordImpressionRewarded {placementName}: {error}");
}
```

### Banner Ad delegates
```c#
private void DidLoadBanner(string placementName, string loadId, BidInfo info, string error)
{
    Debug.Log($"DidLoadBanner{placementName}, {placementName}, Price: ${info.Price:F4}, Auction Id: {info.AuctionId}, Partner Id: {info.PartnerId}. {error}");
}

private void DidClickBanner(string placementName, string error)
{
    Debug.Log($"DidClickBanner {placementName}: {error}");
}

private void DidRecordImpressionBanner(string placementName, string error)
{
    Log($"DidRecordImpressionBanner {placementName}: {error}");
}
```

# Delegate Usage

## Overview

The Helium SDK’s delegate methods allow you to exercise a greater degree of control over your integration. For example, you can:

* Log debug messages when your game attempts to load an interstitial
* Prevent ads from showing the first time a user plays your game
* Determine whether a user has closed an ad
* Get the price of the bid that won the auction

## Delegates Setup

The Helium SDK implements its delegate functionality using C# style delegates and events. Before using any of the delegate methods, you should first subscribe to the relevant SDK events in your MonoBehaviour as demonstrated:

### Subscribing Delegates
```c#
private void OnEnable() {
    // Helium Start Delegate
    HeliumSDK.DidStart += DidStart;

    // Helium ILRD Delegate
    HeliumSDK.DidReceiveImpressionLevelRevenueData += DidReceiveImpressionLevelRevenueData;

    // Helium Partner Initialization Data Delegate
    HeliumSDK.DidReceivePartnerInitializationData += DidReceivePartnerInitializationData;

    // Helium Error Handling Delegate
    HeliumSDK.UnexpectedSystemErrorDidOccur += UnexpectedSystemErrorDidOccur;

    // Helium Interstitial Ad Delegates
    HeliumSDK.DidLoadInterstitial += DidLoadInterstitial;
    HeliumSDK.DidShowInterstitial += DidShowInterstitial;
    HeliumSDK.DidCloseInterstitial += DidCloseInterstitial;
    HeliumSDK.DidWinBidInterstitial += DidWinBidInterstitial;
    HeliumSDK.DidClickInterstitial += DidClickInterstitial;
    HeliumSDK.DidRecordImpressionInterstitial += DidRecordImpressionInterstitial;

    // Helium Rewarded Ad Delegates
    HeliumSDK.DidLoadRewarded += DidLoadRewarded;
    HeliumSDK.DidShowRewarded += DidShowRewarded;
    HeliumSDK.DidCloseRewarded += DidCloseRewarded;
    HeliumSDK.DidReceiveReward += DidReceiveReward;
    HeliumSDK.DidWinBidRewarded += DidWinBidRewarded;
    HeliumSDK.DidClickRewarded += DidClickRewarded;
    HeliumSDK.DidRecordImpressionRewarded += DidRecordImpressionRewarded;

    // Helium Banner Ad Delegates
    HeliumSDK.DidLoadBanner += DidLoadBanner;
    HeliumSDK.DidClickBanner += DidClickBanner;
    HeliumSDK.DidRecordImpressionBanner += DidRecordImpressionBanner;
    HeliumSDK.DidWinBidBanner += DidWinBidBanner;
}
```

> **_NOTE 1:_** Do not make cache or show calls inside a delegate indicating that an ad has just failed to load.

> **_NOTE 2:_** Notice that Helium banner ads do not have a DidCloseBanner delegate. Please keep that in mind when implementing Helium Banners.

> **_NOTE 3:_** Not all of the partner SDKs have support for the DidClick delegate.

### Unsubscribing Delegates

You should also make sure to unsubscribe to those same events when appropriate:

```c#
private void OnDisable() {
    // Remove event handlers
    HeliumSDK.DidStart -= DidStart;

    // Helium ILRD Delegate
    HeliumSDK.DidReceiveImpressionLevelRevenueData -= DidReceiveImpressionLevelRevenueData;

    // Helium Partner Initialization Data Delegate
    HeliumSDK.DidReceivePartnerInitializationData -= DidReceivePartnerInitializationData;

    // Helium Error Handling Delegate
    HeliumSDK.UnexpectedSystemErrorDidOccur -= UnexpectedSystemErrorDidOccur;

    HeliumSDK.DidLoadInterstitial -= DidLoadInterstitial;
    HeliumSDK.DidShowInterstitial -= DidShowInterstitial;
    HeliumSDK.DidCloseInterstitial -= DidCloseInterstitial;
    HeliumSDK.DidWinBidInterstitial -= DidWinBidInterstitial;
    HeliumSDK.DidClickInterstitial -= DidClickInterstitial;
    HeliumSDK.DidRecordImpressionInterstitial -= DidRecordImpressionInterstitial;

    HeliumSDK.DidLoadRewarded -= DidLoadRewarded;
    HeliumSDK.DidShowRewarded -= DidShowRewarded;
    HeliumSDK.DidCloseRewarded -= DidCloseRewarded;
    HeliumSDK.DidReceiveReward -= DidReceiveReward;
    HeliumSDK.DidWinBidRewarded -= DidWinBidRewarded;
    HeliumSDK.DidClickRewarded -= DidClickRewarded;
    HeliumSDK.DidRecordImpressionRewarded -= DidRecordImpressionRewarded;

    //Helium Banner Ad Delegates
    HeliumSDK.DidLoadBanner -= DidLoadBanner;
    HeliumSDK.DidClickBanner -= DidClickBanner;
    HeliumSDK.DidRecordImpressionBanner -= DidRecordImpressionBanner;
    HeliumSDK.DidWinBidBanner -= DidWinBidBanner;
}
```

## Example Delegate Methods

### Lifecycle Delegates
```c#
// Helium Start Delegate
private void DidStartHelium(HeliumError error)
{
    Debug.Log($"DidStart: {error}");
    if (error != null) return;
    // Logic goes here
}

// Helium ILRD Delegate
private void DidReceiveImpressionLevelRevenueData(string placement, Hashtable impressionData)
{
    var json =  HeliumJSON.Serialize(impressionData);
    Debug.Log($"DidReceiveImpressionLevelRevenueData {placement}: {json}");
}

// Helium Partner Initialization Data Delegate
private void DidReceivePartnerInitializationData(string partnerInitializationData)
{
    Debug.Log($"DidReceivePartnerInitializationData: ${partnerInitializationData}");
}

// Helium Error Handling Delegate
private static void UnexpectedSystemErrorDidOccur(HeliumError error)
{
    Debug.LogErrorFormat(error.ErrorDescription);
}
```

### Interstitial Ad Delegates
```c#
private void DidLoadInterstitial(string placementName, HeliumError error)
{
    Debug.Log($"DidLoadInterstitial {placementName}: {error}");
}

private void DidShowInterstitial(string placementName, HeliumError error)
{
    Debug.Log($"DidShowInterstitial {placementName}: {error}");
}

private void DidCloseInterstitial(string placementName, HeliumError error)
{
    Debug.Log($"DidCloseInterstitial {placementName}: {error}");
}

private void DidWinBidInterstitial(string placementName, HeliumBidInfo info)
{
    Debug.Log($"DidWinBidInterstitial {placementName}: ${info.Price:F4}, Auction Id: {info.AuctionId}, Partner Id: {info.PartnerId}");
}

private void DidClickInterstitial(string placementName, HeliumError error)
{
    Debug.Log($"DidClickInterstitial {placementName}: {error}");
}

private void DidRecordImpressionInterstitial(string placementName, HeliumError error)
{
    Log($"DidRecordImpressionInterstitial {placementName}: {error}");
}
```

### Rewarded Ad Delegates
```c#
private void DidLoadRewarded(string placementName, HeliumError error)
{
    Debug.Log($"DidLoadRewarded {placementName}: {error}");
}

private void DidShowRewarded(string placementName, HeliumError error)
{
    Debug.Log($"DidShowRewarded {placementName}: {error}");
}

private void DidCloseRewarded(string placementName, HeliumError error)
{
    Debug.Log($"DidCloseRewarded {placementName}: {error}");
}

private void DidClickRewarded(string placementName, HeliumError error)
{
    Debug.Log($"DidClickRewarded {placementName}: {error}");
}

private void DidReceiveReward(string placementName, int reward)
{
    Debug.Log($"DidReceiveReward {placementName}: {reward}");
}

private void DidRecordImpressionRewarded(string placementName, HeliumError error)
{
    Log($"DidRecordImpressionRewarded {placementName}: {error}");
}

private void DidWinBidRewarded(string placementName, HeliumBidInfo info)
{
    Debug.Log($"DidWinBidRewarded {placementName}: {placementName}: ${info.Price:F4}, Auction Id: {info.AuctionId}, Partner Id: {info.PartnerId}");
}
```

### Banner Ad delegates
```c#
private void DidLoadBanner(string placementName, HeliumError error)
{
    Debug.Log($"DidLoadBanner{placementName}: {error}");
}

private void DidShowBanner(string placementName, HeliumError error)
{
    Debug.Log($"DidShowBanner {placementName}: {error}");
}

private void DidWinBidBanner(string placementName, HeliumBidInfo info)
{
    Debug.Log($"DidWinBidBanner {placementName}: {placementName}: ${info.Price:F4}, Auction Id: {info.AuctionId}, Partner Id: {info.PartnerId}");
}

private void DidClickBanner(string placementName, HeliumError error)
{
    Debug.Log($"DidClickBanner {placementName}: {error}");
}

private void DidRecordImpressionBanner(string placementName, HeliumError error)
{
    IsBannerVisible = true;
    Log($"DidRecordImpressionBanner {placementName}: {error}");
}
```

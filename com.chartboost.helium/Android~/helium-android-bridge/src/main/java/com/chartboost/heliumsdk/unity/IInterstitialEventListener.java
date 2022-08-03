package com.chartboost.heliumsdk.unity;

/**
 * Interface for sending real-time Interstitial events
 */
public interface IInterstitialEventListener {
    void DidLoadInterstitial(String placementName, int errorCode, String errorDescription);
    void DidClickInterstitial(String placementName, int errorCode, String errorDescription);
    void DidCloseInterstitial(String placementName, int errorCode, String errorDescription);
    void DidShowInterstitial(String placementName, int errorCode, String errorDescription);
    void DidRecordImpression(String placementName, int errorCode, String errorDescription);
    void DidWinBidInterstitial(String placementName, String auctionId, String partnerId, double price);
}

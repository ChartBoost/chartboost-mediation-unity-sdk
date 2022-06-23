package com.chartboost.heliumsdk.unity;

public interface IInterstitialEventListener {
    void DidLoadInterstitial(String placementName, int errorCode, String errorDescription);
    void DidClickInterstitial(String placementName, int errorCode, String errorDescription);
    void DidCloseInterstitial(String placementName, int errorCode, String errorDescription);
    void DidShowInterstitial(String placementName, int errorCode, String errorDescription);
    void DidWinBidInterstitial(String placementName, String partnerPlacementName,  String auctionId, double price, String seat);
}

package com.chartboost.heliumsdk.unity;

/**
 * Interface for sending real-time Banner events
 */
public interface IBannerEventListener {
    void DidLoadBanner(String placementName, int errorCode, String errorDescription);
    void DidClickBanner(String placementName, int errorCode, String errorDescription);
    void DidRecordImpression(String placementName, int errorCode, String errorDescription);
    void DidWinBidBanner(String placementName, String auctionId, String partnerId, double price);
}

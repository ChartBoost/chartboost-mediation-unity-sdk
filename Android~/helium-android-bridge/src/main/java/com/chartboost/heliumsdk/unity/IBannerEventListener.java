package com.chartboost.heliumsdk.unity;

public interface IBannerEventListener {
    void DidLoadBanner(String placementName, int errorCode, String errorDescription);
    void DidShowBanner(String placementName, int errorCode, String errorDescription);
    void DidClickBanner(String placementName, int errorCode, String errorDescription);
    void DidWinBidBanner(String placementName, String partnerPlacementName, String auctionId, double price, String seat);
}

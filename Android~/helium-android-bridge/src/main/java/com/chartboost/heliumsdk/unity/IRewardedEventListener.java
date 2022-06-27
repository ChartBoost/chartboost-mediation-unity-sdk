package com.chartboost.heliumsdk.unity;

/**
 * Interface for sending real-time rewarded events
 */
public interface IRewardedEventListener {
    void DidLoadRewarded(String placementName, int errorCode, String errorDescription);
    void DidShowRewarded(String placementName, int errorCode, String errorDescription);
    void DidCloseRewarded(String placementName, int errorCode, String errorDescription);
    void DidClickRewarded(String placementName, int errorCode, String errorDescription);
    void DidWinBidRewarded(String placementName, String auctionId, String partnerId, double price);
    void DidReceiveReward(String placementName, int reward);
}

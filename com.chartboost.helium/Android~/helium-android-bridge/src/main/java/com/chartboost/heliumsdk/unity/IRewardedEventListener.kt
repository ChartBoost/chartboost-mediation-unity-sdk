package com.chartboost.heliumsdk.unity

/**
 * Interface for sending real-time rewarded events
 */
interface IRewardedEventListener {
    fun DidLoadRewarded(placementName: String, auctionId: String?, partnerId: String, price: Double, error: String)
    fun DidShowRewarded(placementName: String, error: String)
    fun DidCloseRewarded(placementName: String, error: String)
    fun DidClickRewarded(placementName: String, error: String)
    fun DidRecordImpression(placementName: String, error: String)
    fun DidReceiveReward(placementName: String)
}

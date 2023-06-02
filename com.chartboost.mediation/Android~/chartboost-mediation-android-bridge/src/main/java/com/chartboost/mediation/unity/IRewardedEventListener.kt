package com.chartboost.mediation.unity

/**
 * Interface for sending real-time rewarded events
 */
@Deprecated("IRewardedEventListener has been deprecated, utilize the new fullscreen API instead.")
interface IRewardedEventListener {
    fun DidLoadRewarded(placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, lineItemId: String, error: String)
    fun DidShowRewarded(placementName: String, error: String)
    fun DidCloseRewarded(placementName: String, error: String)
    fun DidClickRewarded(placementName: String)
    fun DidRecordImpression(placementName: String)
    fun DidReceiveReward(placementName: String)
}

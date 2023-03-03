package com.chartboost.mediation.unity

/**
 * Interface for sending real-time rewarded events
 */
interface IRewardedEventListener {
    fun DidLoadRewarded(placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, error: String)
    fun DidShowRewarded(placementName: String, error: String)
    fun DidCloseRewarded(placementName: String, error: String)
    fun DidClickRewarded(placementName: String)
    fun DidRecordImpression(placementName: String)
    fun DidReceiveReward(placementName: String)
}

package com.chartboost.heliumsdk.unity

/**
 * Interface for sending real-time rewarded events
 */
interface IRewardedEventListener {
    fun DidLoadRewarded(placementName: String, errorCode: Int, errorDescription: String)
    fun DidShowRewarded(placementName: String, errorCode: Int, errorDescription: String)
    fun DidCloseRewarded(placementName: String, errorCode: Int, errorDescription: String)
    fun DidClickRewarded(placementName: String, errorCode: Int, errorDescription: String)
    fun DidRecordImpression(placementName: String, errorCode: Int, errorDescription: String)
    fun DidWinBidRewarded(placementName: String, auctionId: String, partnerId: String, price: Double)
    fun DidReceiveReward(placementName: String)
}

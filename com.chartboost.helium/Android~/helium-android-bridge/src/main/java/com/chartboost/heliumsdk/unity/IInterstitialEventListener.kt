package com.chartboost.heliumsdk.unity

/**
 * Interface for sending real-time Interstitial events
 */
interface IInterstitialEventListener {
    fun DidLoadInterstitial(placementName: String, loadId: String, error: String)
    fun DidShowInterstitial(placementName: String, error: String)
    fun DidCloseInterstitial(placementName: String, error: String)
    fun DidClickInterstitial(placementName: String)
    fun DidRecordImpression(placementName: String)
    fun DidReceiveReward(placementName: String)
    fun DidWinBidInterstitial(placementName: String, auctionId: String, partnerId: String, price: Double, error: String)
}

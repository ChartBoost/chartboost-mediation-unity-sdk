package com.chartboost.mediation.unity

/**
 * Interface for sending real-time Interstitial events
 */
@Deprecated("IInterstitialEventListener has been deprecated, utilize the new Fullscreen API instead")
interface IInterstitialEventListener {
    fun DidLoadInterstitial(placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, lineItemId: String, error: String)
    fun DidShowInterstitial(placementName: String, error: String)
    fun DidCloseInterstitial(placementName: String, error: String)
    fun DidClickInterstitial(placementName: String)
    fun DidRecordImpression(placementName: String)
    fun DidReceiveReward(placementName: String)
}

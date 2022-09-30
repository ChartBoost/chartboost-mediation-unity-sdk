package com.chartboost.heliumsdk.unity

/**
 * Interface for sending real-time Interstitial events
 */
interface IInterstitialEventListener {
    fun DidLoadInterstitial(placementName: String, errorCode: Int, errorDescription: String)
    fun DidClickInterstitial(placementName: String, errorCode: Int, errorDescription: String)
    fun DidCloseInterstitial(placementName: String, errorCode: Int, errorDescription: String)
    fun DidShowInterstitial(placementName: String, errorCode: Int, errorDescription: String)
    fun DidRecordImpression(placementName: String, errorCode: Int, errorDescription: String)
    fun DidWinBidInterstitial(placementName: String?, auctionId: String?, partnerId: String, price: Double)
}

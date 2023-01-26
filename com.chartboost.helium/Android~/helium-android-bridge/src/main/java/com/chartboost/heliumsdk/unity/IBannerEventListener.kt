package com.chartboost.heliumsdk.unity

/**
 * Interface for sending real-time Banner events
 */
interface IBannerEventListener {
    fun DidLoadBanner(placementName: String, loadId: String, error: String)
    fun DidClickBanner(placementName: String)
    fun DidRecordImpression(placementName: String)
    fun DidWinBidBanner(placementName: String, auctionId: String, partnerId: String, price: Double, error: String)
}

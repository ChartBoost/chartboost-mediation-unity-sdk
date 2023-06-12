package com.chartboost.mediation.unity

/**
 * Interface for sending real-time Banner events
 */
interface IBannerEventListener {
    fun DidLoadBanner(placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, lineItemName: String, lineItemId: String, error: String)
    fun DidClickBanner(placementName: String)
    fun DidRecordImpression(placementName: String)
}

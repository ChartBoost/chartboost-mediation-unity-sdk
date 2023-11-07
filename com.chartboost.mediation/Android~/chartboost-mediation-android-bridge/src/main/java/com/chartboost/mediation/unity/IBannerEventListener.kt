package com.chartboost.mediation.unity

/**
 * Interface for sending real-time Banner events
 */
@Deprecated("IBannerEventListener has been deprecated, utilize the new Banner API instead")
interface IBannerEventListener {
    fun DidLoadBanner(placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, lineItemName: String, lineItemId: String, error: String)
    fun DidClickBanner(placementName: String)
    fun DidRecordImpression(placementName: String)
}

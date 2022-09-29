package com.chartboost.heliumsdk.unity

/**
 * Interface for sending real-time Banner events
 */
interface IBannerEventListener {
    fun DidLoadBanner(placementName: String?, errorCode: Int, errorDescription: String?)
    fun DidClickBanner(placementName: String?, errorCode: Int, errorDescription: String?)
    fun DidRecordImpression(placementName: String?, errorCode: Int, errorDescription: String?)
    fun DidWinBidBanner(
        placementName: String?,
        auctionId: String?,
        partnerId: String?,
        price: Double
    )
}

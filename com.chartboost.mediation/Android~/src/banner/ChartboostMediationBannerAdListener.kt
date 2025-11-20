@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.banner

/**
 * Listener interface for banner ad lifecycle and interaction events.
 * Provides callbacks for ad view changes, user interactions, and drag events.
 * Implementations of this interface are typically used to forward events to Unity C# layer.
 */
interface ChartboostMediationBannerAdListener {

    /**
     * Called when a partner ad view is added to the banner container.
     * This occurs on initial load and on each ad refresh.
     *
     * @param ad The banner ad wrapper that received a new ad view
     */
    fun onAdViewAdded(ad: BannerAdWrapper)

    /**
     * Called when the user clicks on the banner ad.
     *
     * @param ad The banner ad wrapper that was clicked
     */
    fun onAdClicked(ad: BannerAdWrapper)

    /**
     * Called when an ad impression is recorded.
     * An impression is typically counted when the ad becomes visible to the user.
     *
     * @param ad The banner ad wrapper that recorded an impression
     */
    fun onAdImpressionRecorded(ad: BannerAdWrapper)

    /**
     * Called when the user begins dragging the banner ad.
     * Only triggered when draggability is enabled on the banner.
     *
     * @param ad The banner ad wrapper being dragged
     * @param x The X coordinate of the banner's origin in pixels (scaled)
     * @param y The Y coordinate of the banner's origin in pixels (scaled)
     */
    fun onAdDragBegin(ad: BannerAdWrapper, x: Float, y: Float)

    /**
     * Called continuously while the user is dragging the banner ad.
     * Only triggered when draggability is enabled on the banner.
     *
     * @param ad The banner ad wrapper being dragged
     * @param x The current X coordinate of the banner's origin in pixels (scaled)
     * @param y The current Y coordinate of the banner's origin in pixels (scaled)
     */
    fun onAdDrag(ad: BannerAdWrapper, x: Float, y: Float)

    /**
     * Called when the user finishes dragging the banner ad.
     * Only triggered when draggability is enabled on the banner.
     *
     * @param ad The banner ad wrapper that was dragged
     * @param x The final X coordinate of the banner's origin in pixels (scaled)
     * @param y The final Y coordinate of the banner's origin in pixels (scaled)
     */
    fun onAdDragEnd(ad: BannerAdWrapper, x: Float, y: Float)
}

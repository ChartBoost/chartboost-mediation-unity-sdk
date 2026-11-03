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
}

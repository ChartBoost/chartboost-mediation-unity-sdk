@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.banner

/**
 * Internal listener interface for banner drag events.
 * Used by [BannerLayout] to communicate drag gestures to [BannerAdWrapper].
 *
 * This interface provides low-level drag event callbacks that are forwarded
 * to [ChartboostMediationBannerAdListener] with additional banner context.
 */
interface IBannerDragListener {

    /**
     * Called when a drag gesture begins on the banner.
     * Triggered after the user's touch movement exceeds the drag threshold.
     *
     * @param x The X coordinate of the banner's origin in pixels
     * @param y The Y coordinate of the banner's origin in pixels
     */
    fun onDragBegin(x: Float, y: Float)

    /**
     * Called continuously while the banner is being dragged.
     * This method is invoked for every touch move event during an active drag.
     *
     * @param x The current X coordinate of the banner's origin in pixels
     * @param y The current Y coordinate of the banner's origin in pixels
     */
    fun onDrag(x: Float, y: Float)

    /**
     * Called when a drag gesture ends.
     * Triggered when the user lifts their finger or the drag is cancelled.
     *
     * @param x The final X coordinate of the banner's origin in pixels
     * @param y The final Y coordinate of the banner's origin in pixels
     */
    fun onDragEnd(x: Float, y: Float)
}

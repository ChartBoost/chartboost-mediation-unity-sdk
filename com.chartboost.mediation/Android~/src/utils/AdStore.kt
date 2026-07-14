@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.utils

import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationFullscreenAd
import com.chartboost.mediation.unity.banner.BannerAdWrapper
import com.chartboost.mediation.unity.logging.LogLevel
import com.chartboost.mediation.unity.logging.UnityLoggingBridge
import java.util.concurrent.ConcurrentHashMap

/**
 * Centralized storage for managing ad instances across the Unity-Android bridge.
 *
 * This object maintains references to both fullscreen and banner ads to prevent premature
 * garbage collection and enable lifecycle management from the Unity C# layer.
 *
 * Thread Safety: All operations are thread-safe using ConcurrentHashMap.
 * Memory Management: Callers must explicitly call release methods to prevent memory leaks.
 * Use [clearAll] during application shutdown to release all tracked ads.
 *
 * @see ChartboostMediationFullscreenAd
 * @see BannerAdWrapper
 */
object AdStore {
    /**
     * Tag used for logging operations related to AdStore.
     */
    private val TAG = AdStore::class.java.simpleName

    /**
     * Thread-safe storage for fullscreen ad instances keyed by their hash codes.
     * Stores references to prevent garbage collection while ads are in use.
     */
    private val fullscreenAdStore: ConcurrentHashMap<Int, ChartboostMediationFullscreenAd> = ConcurrentHashMap()

    /**
     * Thread-safe storage for banner ad wrapper instances keyed by their hash codes.
     * Stores references to prevent garbage collection while ads are in use.
     */
    private val bannerAdStore: ConcurrentHashMap<Int, BannerAdWrapper> = ConcurrentHashMap()

    /**
     * Maximum number of ads allowed in each store to prevent memory leaks.
     */
    private const val MAX_STORE_SIZE = 100

    /**
     * Tracks a fullscreen ad instance in the store.
     *
     * This method stores a reference to the fullscreen ad to prevent garbage collection
     * and enables retrieval via the ad's hash code from Unity.
     *
     * @param fullscreenAd The fullscreen ad instance to track
     * @return The unique hash code assigned to this ad for future reference
     */
    @JvmStatic
    fun trackFullscreenAd(fullscreenAd: ChartboostMediationFullscreenAd): Int {
        if (fullscreenAdStore.size >= MAX_STORE_SIZE) {
            UnityLoggingBridge.log(
                TAG,
                "Fullscreen ad store exceeded maximum size ($MAX_STORE_SIZE). Possible memory leak.",
                LogLevel.ERROR
            )
        }

        val hashCode = fullscreenAd.hashCode()
        fullscreenAdStore[hashCode] = fullscreenAd
        UnityLoggingBridge.log(
            TAG,
            "Tracking FullscreenAd with Id: $hashCode (Total fullscreen ads: ${fullscreenAdStore.size})",
            LogLevel.VERBOSE
        )
        return hashCode
    }

    /**
     * Releases a tracked fullscreen ad and cleans up its resources.
     *
     * This method invalidates the ad and removes it from the store, allowing
     * garbage collection. Should be called when the ad is no longer needed.
     *
     * @param hashCode The hash code identifying the fullscreen ad to release
     */
    @JvmStatic
    fun releaseFullscreenAd(hashCode: Int) {
        try {
            val fullscreenAd = fullscreenAdStore.remove(hashCode)
            if (fullscreenAd != null) {
                fullscreenAd.invalidate()
                UnityLoggingBridge.log(
                    TAG,
                    "Released FullscreenAd with Id: $hashCode (Remaining fullscreen ads: ${fullscreenAdStore.size})",
                    LogLevel.VERBOSE
                )
            } else {
                UnityLoggingBridge.log(
                    TAG,
                    "Attempted to release non-existent FullscreenAd with Id: $hashCode",
                    LogLevel.WARNING
                )
            }
        } catch (e: Exception) {
            UnityLoggingBridge.log(
                TAG,
                "Error releasing FullscreenAd with Id: $hashCode - ${e.message}",
                LogLevel.ERROR
            )
        }
    }

    /**
     * Retrieves a fullscreen ad from the store without releasing it.
     *
     * @param hashCode The hash code identifying the fullscreen ad
     * @return The fullscreen ad instance, or null if not found
     */
    @JvmStatic
    fun getFullscreenAd(hashCode: Int): ChartboostMediationFullscreenAd? {
        return fullscreenAdStore[hashCode]
    }

    /**
     * Tracks a banner ad wrapper instance in the store.
     *
     * This method stores a reference to the banner ad wrapper to prevent garbage collection
     * and enables retrieval via the wrapper's hash code from Unity.
     *
     * @param bannerAd The banner ad wrapper instance to track
     * @return The unique hash code assigned to this ad for future reference
     */
    @JvmStatic
    fun trackBannerAd(bannerAd: BannerAdWrapper): Int {
        if (bannerAdStore.size >= MAX_STORE_SIZE) {
            UnityLoggingBridge.log(
                TAG,
                "Banner ad store exceeded maximum size ($MAX_STORE_SIZE). Possible memory leak.",
                LogLevel.ERROR
            )
        }

        val hashCode = bannerAd.hashCode()
        bannerAdStore[hashCode] = bannerAd
        UnityLoggingBridge.log(
            TAG,
            "Tracking BannerAd with Id: $hashCode (Total banner ads: ${bannerAdStore.size})",
            LogLevel.VERBOSE
        )
        return hashCode
    }

    /**
     * Releases a tracked banner ad and cleans up its resources.
     *
     * This method destroys the banner ad and removes it from the store, allowing
     * garbage collection. Should be called when the ad is no longer needed.
     *
     * @param hashCode The hash code identifying the banner ad to release
     */
    @JvmStatic
    fun releaseBannerAd(hashCode: Int) {
        try {
            val bannerAd = bannerAdStore.remove(hashCode)
            if (bannerAd != null) {
                bannerAd.destroy()
                UnityLoggingBridge.log(
                    TAG,
                    "Released BannerAd with Id: $hashCode (Remaining banner ads: ${bannerAdStore.size})",
                    LogLevel.VERBOSE
                )
            } else {
                UnityLoggingBridge.log(
                    TAG,
                    "Attempted to release non-existent BannerAd with Id: $hashCode",
                    LogLevel.WARNING
                )
            }
        } catch (e: Exception) {
            UnityLoggingBridge.log(
                TAG,
                "Error releasing BannerAd with Id: $hashCode - ${e.message}",
                LogLevel.ERROR
            )
        }
    }

    /**
     * Clears all tracked ads and releases their resources.
     *
     * This method should be called during application shutdown or reset to prevent
     * memory leaks. It invalidates all fullscreen ads and destroys all banner ads.
     */
    @JvmStatic
    fun clearAll() {
        try {
            val fullscreenCount = fullscreenAdStore.size
            val bannerCount = bannerAdStore.size

            for (ad in fullscreenAdStore.values) { ad.invalidate() }
            fullscreenAdStore.clear()

            for (ad in bannerAdStore.values) { ad.destroy() }
            bannerAdStore.clear()

            UnityLoggingBridge.log(
                TAG,
                "Cleared all ad stores (Released $fullscreenCount fullscreen ads, $bannerCount banner ads)",
                LogLevel.INFO
            )
        } catch (e: Exception) {
            UnityLoggingBridge.log(
                TAG,
                "Error clearing ad stores - ${e.message}",
                LogLevel.ERROR
            )
        }
    }

    /**
     * Returns diagnostic information about the current state of the ad store.
     *
     * Provides counts of tracked fullscreen and banner ads for debugging and monitoring.
     *
     * @return A formatted string containing ad store statistics
     */
    @JvmStatic
    fun adStoreInfo(): String {
        return "$TAG Fullscreen AdStore Count: ${fullscreenAdStore.size}, Banner AdStore Count: ${bannerAdStore.size}"
    }
}

@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.bridge

import android.app.Activity
import android.util.DisplayMetrics
import com.chartboost.chartboostmediationsdk.ChartboostMediationPreinitializationConfiguration
import com.chartboost.chartboostmediationsdk.ChartboostMediationSdk
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdView
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationFullscreenAd
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationFullscreenAdListener
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationFullscreenAdLoadListener
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationFullscreenAdLoadRequest
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationFullscreenAdQueue
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationFullscreenAdQueueListener
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationFullscreenAdQueueManager
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationFullscreenAdShowListener
import com.chartboost.chartboostmediationsdk.domain.ChartboostMediationAdException
import com.chartboost.chartboostmediationsdk.utils.LogController
import com.chartboost.mediation.unity.banner.BannerAdWrapper
import com.chartboost.mediation.unity.banner.ChartboostMediationBannerAdListener
import com.chartboost.mediation.unity.logging.LogLevel
import com.chartboost.mediation.unity.logging.UnityLoggingBridge
import com.chartboost.mediation.unity.utils.AdStore
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers.Main
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.cancel
import kotlinx.coroutines.launch

/**
 * JNI bridge class for Chartboost Mediation SDK operations in Unity.
 *
 * Provides static methods called from Unity C# layer to interact with the native Android SDK.
 * Handles ad loading, showing, configuration, and lifecycle management with proper threading
 * and resource cleanup.
 */
@Suppress("unused")
class BridgeCBM {

    companion object {
        private val TAG = BridgeCBM::class.java.simpleName

        // Coroutine scope tied to the bridge lifecycle
        private val scope = CoroutineScope(Main + SupervisorJob())

        //region Activity Management

        /**
         * Gets the current Unity activity. Does not retain a reference to avoid memory leaks.
         *
         * @return The current Unity activity, or null if not available
         */
        private fun getActivity(): Activity? = UnityPlayer.currentActivity

        /**
         * Gets the current display density of the device.
         *
         * @return The density value, or DENSITY_DEFAULT if activity is unavailable
         */
        private val displayDensity: Float
            get() {
                val activity = getActivity()
                if (activity == null) {
                    UnityLoggingBridge.log(TAG, "Activity not available, using default density", LogLevel.DEBUG)
                    return DisplayMetrics.DENSITY_DEFAULT.toFloat()
                }
                return activity.resources?.displayMetrics?.density ?: DisplayMetrics.DENSITY_DEFAULT.toFloat()
            }

        //endregion

        //region Fullscreen Ad Operations

        /**
         * Loads a fullscreen ad asynchronously.
         *
         * This method launches a coroutine to load the ad on the main thread. The result is
         * delivered via the provided callback handler.
         *
         * @param adRequest The load request containing placement and targeting information
         * @param adLoadResultHandler Callback invoked when the load operation completes
         * @param fullscreenAdListener Listener for fullscreen ad lifecycle events
         */
        @JvmStatic
        @Suppress("unused") // Called from C# JNI layer
        fun loadFullscreenAd(
            adRequest: ChartboostMediationFullscreenAdLoadRequest,
            adLoadResultHandler: ChartboostMediationFullscreenAdLoadListener,
            fullscreenAdListener: ChartboostMediationFullscreenAdListener
        ) {
            UnityLoggingBridge.log(TAG, "Loading fullscreen ad for placement: ${adRequest.placement}", LogLevel.VERBOSE)
            scope.launch {
                val activity = getActivity()
                if (activity == null) {
                    UnityLoggingBridge.log(TAG, "Failed to load fullscreen ad: Activity not available", LogLevel.DEBUG)
                    return@launch
                }

                val adLoadResult = ChartboostMediationFullscreenAd.loadFullscreenAd(
                    activity, adRequest, fullscreenAdListener
                )

                adLoadResult.ad?.let {
                    AdStore.trackFullscreenAd(it)
                    UnityLoggingBridge.log(TAG, "Fullscreen ad loaded successfully", LogLevel.VERBOSE)
                } ?: UnityLoggingBridge.log(TAG, "Fullscreen ad load returned null ad: ${adLoadResult.error?.message}", LogLevel.DEBUG)

                adLoadResultHandler.onAdLoaded(adLoadResult)
            }
        }

        /**
         * Shows a fullscreen ad asynchronously.
         *
         * This method launches a coroutine to show the ad on the main thread. The result is
         * delivered via the provided callback handler.
         *
         * @param fullscreenAd The fullscreen ad to show
         * @param adShowResultHandler Callback invoked when the show operation completes
         */
        @JvmStatic
        @Suppress("unused") // Called from C# JNI layer
        fun showFullscreenAd(
            fullscreenAd: ChartboostMediationFullscreenAd,
            adShowResultHandler: ChartboostMediationFullscreenAdShowListener
        ) {
            UnityLoggingBridge.log(TAG, "Showing fullscreen ad", LogLevel.VERBOSE)
            scope.launch {
                val activity = getActivity()
                if (activity == null) {
                    UnityLoggingBridge.log(TAG, "Failed to show fullscreen ad: Activity not available", LogLevel.DEBUG)
                    return@launch
                }

                val adShowResult = fullscreenAd.show(activity)
                UnityLoggingBridge.log(TAG, "Fullscreen ad show completed with error: ${adShowResult.error?.message ?: "none"}", LogLevel.VERBOSE)
                adShowResultHandler.onAdShown(adShowResult)
            }
        }

        /**
         * Gets or creates a fullscreen ad queue for the specified placement.
         *
         * Ad queues automatically manage ad loading and caching for improved performance.
         *
         * @param placementName The placement name for the ad queue
         * @param fullscreenAdQueueListener Listener for ad queue events
         * @return The fullscreen ad queue instance
         */
        @JvmStatic
        @Suppress("unused") // Called from C# JNI layer
        fun getFullscreenAdQueue(
            placementName: String,
            fullscreenAdQueueListener: ChartboostMediationFullscreenAdQueueListener
        ): ChartboostMediationFullscreenAdQueue? {
            val activity = getActivity()
            if (activity == null) {
                UnityLoggingBridge.log(TAG, "Failed to create fullscreen ad queue: Activity not available", LogLevel.DEBUG)
                return null
            }

            val queue = ChartboostMediationFullscreenAdQueueManager.queue(activity, placementName)
            queue.adQueueListener = fullscreenAdQueueListener
            UnityLoggingBridge.log(TAG, "Created FullscreenAdQueue for placement: $placementName", LogLevel.VERBOSE)
            return queue
        }

        //endregion

        //region Banner Ad Operations

        /**
         * Creates and loads a banner ad.
         *
         * **Important**: This method creates an Android View and must be called from the UI thread
         * or wrapped in a UI thread dispatch. The banner view creation is synchronous.
         *
         * @param listener The listener for banner ad lifecycle and interaction events
         * @return The banner ad wrapper instance, or null if activity is unavailable
         */
        @JvmStatic
        @Suppress("unused") // Called from C# JNI layer
        fun loadBannerAd(listener: ChartboostMediationBannerAdListener): BannerAdWrapper? {
            val activity = getActivity()
            if (activity == null) {
                UnityLoggingBridge.log(TAG, "Failed to create banner ad: Activity not available", LogLevel.DEBUG)
                return null
            }

            UnityLoggingBridge.log(TAG, "Creating banner ad", LogLevel.VERBOSE)
            val bannerView = ChartboostMediationBannerAdView(activity)
            val bannerAdWrapper = BannerAdWrapper.wrap(bannerView)
            bannerAdWrapper.setListener(listener)
            AdStore.trackBannerAd(bannerAdWrapper)
            return bannerAdWrapper
        }

        //endregion

        //region SDK Configuration

        /**
         * Sets the pre-initialization configuration for the Chartboost Mediation SDK.
         *
         * This must be called before SDK initialization to take effect.
         *
         * @param configuration The pre-initialization configuration, or null to clear
         * @return An error if the configuration is invalid, null otherwise
         */
        @JvmStatic
        @Suppress("unused") // Called from C# JNI layer
        fun setPreinitializationConfiguration(configuration: ChartboostMediationPreinitializationConfiguration?): ChartboostMediationAdException? {
            UnityLoggingBridge.log(TAG, "Setting ChartboostMediationPreinitializationConfiguration", LogLevel.VERBOSE)
            return ChartboostMediationSdk.setPreinitializationConfiguration(configuration)
        }

        /**
         * Gets the current SDK log level.
         *
         * @return The log level value as an integer
         */
        @JvmStatic
        @Suppress("unused") // Called from C# JNI layer
        fun getLogLevel(): Int {
            ChartboostMediationSdk.logLevel.let {
                UnityLoggingBridge.log(TAG, "Current LogLevel is $it", LogLevel.VERBOSE)
                return it.value
            }
        }

        /**
         * Sets the SDK log level.
         *
         * @param value The log level value as an integer
         */
        @JvmStatic
        @Suppress("unused") // Called from C# JNI layer
        fun setLogLevel(value: Int) {
            val logLevel = LogController.LogLevel.fromInt(value)
            ChartboostMediationSdk.logLevel = logLevel
            UnityLoggingBridge.log(TAG, "LogLevel set to $logLevel", LogLevel.VERBOSE)
        }

        //endregion

        //region Utilities

        /**
         * Gets the UI scale factor (display density) for the current device.
         *
         * This value is used to convert between density-independent pixels (dp) and actual pixels.
         *
         * @return The display density, or DENSITY_DEFAULT if activity is unavailable
         */
        @JvmStatic
        @Suppress("unused") // Called from C# JNI layer
        fun getUIScaleFactor(): Float = displayDensity

        //endregion

        //region Lifecycle Management

        /**
         * Cleans up resources and cancels all pending operations.
         *
         * Should be called during app shutdown or Unity activity destruction to prevent
         * memory leaks and ensure proper cleanup of coroutines and tracked ads.
         */
        @JvmStatic
        @Suppress("unused") // Called from C# JNI layer
        fun cleanup() {
            UnityLoggingBridge.log(TAG, "BridgeCBM cleanup initiated", LogLevel.VERBOSE)
            AdStore.clearAll()
            scope.cancel()
            UnityLoggingBridge.log(TAG, "BridgeCBM cleanup completed", LogLevel.VERBOSE)
        }

        //endregion
    }
}

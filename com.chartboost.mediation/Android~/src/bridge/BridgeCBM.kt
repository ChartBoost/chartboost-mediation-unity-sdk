@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.bridge

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
import com.chartboost.core.ChartboostCore
import com.chartboost.core.ChartboostCoreLogLevel
import com.chartboost.mediation.unity.banner.BannerAdWrapper
import com.chartboost.mediation.unity.banner.ChartboostMediationBannerAdListener
import com.chartboost.mediation.unity.logging.LogLevel
import com.chartboost.mediation.unity.logging.UnityLoggingBridge
import com.chartboost.mediation.unity.utils.AdStore
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers.Main
import kotlinx.coroutines.launch
import kotlin.math.log

@Suppress("unused")
class BridgeCBM {

    companion object {
        private val TAG = BridgeCBM::class.java.simpleName

        @JvmStatic
        fun loadFullscreenAd(adRequest: ChartboostMediationFullscreenAdLoadRequest, adLoadResultHandler: ChartboostMediationFullscreenAdLoadListener, fullscreenAdListener: ChartboostMediationFullscreenAdListener) {
            CoroutineScope(Main).launch {
                val adLoadResult = ChartboostMediationFullscreenAd.loadFullscreenAd(UnityPlayer.currentActivity, adRequest, fullscreenAdListener)
                adLoadResult.ad?.let { AdStore.trackFullscreenAd(it) }
                adLoadResultHandler.onAdLoaded(adLoadResult)
            }
        }

        @JvmStatic
        fun showFullscreenAd(fullscreenAd: ChartboostMediationFullscreenAd, adShowResultHandler: ChartboostMediationFullscreenAdShowListener) {
            CoroutineScope(Main).launch {
                val adShowResult =  fullscreenAd.show(UnityPlayer.currentActivity)
                adShowResultHandler.onAdShown(adShowResult)
            }
        }

        @JvmStatic
        fun loadBannerAd(listener: ChartboostMediationBannerAdListener): BannerAdWrapper {
            val bannerView = ChartboostMediationBannerAdView(UnityPlayer.currentActivity)
            val bannerAdWrapper = BannerAdWrapper.wrap(bannerView)
            bannerAdWrapper.setListener(listener)
            AdStore.trackBannerAd(bannerAdWrapper)
            return bannerAdWrapper
        }

        @JvmStatic
        fun getFullscreenAdQueue(placementName: String, fullscreenAdQueueListener: ChartboostMediationFullscreenAdQueueListener) : ChartboostMediationFullscreenAdQueue {
            val queue = ChartboostMediationFullscreenAdQueueManager.queue(UnityPlayer.currentActivity, placementName)
            queue.adQueueListener = fullscreenAdQueueListener
            UnityLoggingBridge.log(TAG, "Created FullscreenAdQueue with Listener", LogLevel.VERBOSE)
            return queue
        }

        @JvmStatic
        fun setPreinitializationConfiguration(configuration: ChartboostMediationPreinitializationConfiguration?): ChartboostMediationAdException? {
            UnityLoggingBridge.log(TAG, "Setting ChartboostMediationPreinitializationConfiguration", LogLevel.VERBOSE)
            return ChartboostMediationSdk.setPreinitializationConfiguration(configuration)
        }

        @JvmStatic
        fun getUIScaleFactor(): Float {
            return UnityPlayer.currentActivity.resources?.displayMetrics?.density ?: DisplayMetrics.DENSITY_DEFAULT.toFloat()
        }

        @JvmStatic
        fun getLogLevel() : Int {
            ChartboostMediationSdk.logLevel.let {
                UnityLoggingBridge.log(TAG, "LogLevel is $it", LogLevel.VERBOSE)
                return it.value
            }
        }

        @JvmStatic
        fun setLogLevel(value: Int) {
            val logLevel = LogController.LogLevel.fromInt(value)
            ChartboostMediationSdk.logLevel = logLevel
            UnityLoggingBridge.log(TAG, "LogLevel set to $logLevel", LogLevel.VERBOSE)
        }
    }
}

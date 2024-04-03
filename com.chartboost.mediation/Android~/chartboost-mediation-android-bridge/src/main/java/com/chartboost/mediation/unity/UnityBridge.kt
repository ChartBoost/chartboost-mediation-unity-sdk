package com.chartboost.mediation.unity

import android.util.DisplayMetrics
import com.chartboost.heliumsdk.HeliumSdk
import com.chartboost.heliumsdk.ad.ChartboostMediationAdLoadRequest
import com.chartboost.heliumsdk.ad.ChartboostMediationFullscreenAd
import com.chartboost.heliumsdk.ad.ChartboostMediationFullscreenAdListener
import com.chartboost.heliumsdk.ad.ChartboostMediationFullscreenAdLoadListener
import com.chartboost.heliumsdk.ad.ChartboostMediationFullscreenAdQueue
import com.chartboost.heliumsdk.ad.ChartboostMediationFullscreenAdQueueListener
import com.chartboost.heliumsdk.ad.ChartboostMediationFullscreenAdQueueManager
import com.chartboost.heliumsdk.ad.ChartboostMediationFullscreenAdShowListener
import com.chartboost.heliumsdk.ad.HeliumBannerAd
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers.Main
import kotlinx.coroutines.launch

class UnityBridge {

    companion object {
        private val TAG = UnityBridge::class.java.simpleName

        @JvmStatic
        fun loadFullscreenAd(adRequest: ChartboostMediationAdLoadRequest, adLoadResultHandler: ChartboostMediationFullscreenAdLoadListener, fullscreenAdListener: ChartboostMediationFullscreenAdListener) {
            CoroutineScope(Main).launch {
                val adLoadResult = HeliumSdk.loadFullscreenAd(UnityPlayer.currentActivity, adRequest, fullscreenAdListener)
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
        fun loadBannerAd(listener: ChartboostMediationBannerViewListener): BannerAdWrapper {
            val size = HeliumBannerAd.HeliumBannerSize.STANDARD
            val bannerView = HeliumBannerAd(UnityPlayer.currentActivity, "", size, null)
            val bannerAdWrapper = BannerAdWrapper.wrap(bannerView)
            bannerAdWrapper.setListener(listener)
            AdStore.trackBannerAd(bannerAdWrapper);
            return bannerAdWrapper
        }

        @JvmStatic
        fun getFullscreenAdQueue(placementName: String, fullscreenAdQueueListener: ChartboostMediationFullscreenAdQueueListener) : ChartboostMediationFullscreenAdQueue {
            val queue = ChartboostMediationFullscreenAdQueueManager.queue(UnityPlayer.currentActivity, placementName)
            queue.adQueueListener = fullscreenAdQueueListener
            return queue
        }

        @JvmStatic
        fun getUIScaleFactor(): Float {
            return UnityPlayer.currentActivity.resources?.displayMetrics?.density ?: DisplayMetrics.DENSITY_DEFAULT.toFloat()
        }
    }
}

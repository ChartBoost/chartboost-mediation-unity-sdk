package com.chartboost.mediation.unity.utils

import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationFullscreenAd
import com.chartboost.mediation.unity.banner.BannerAdWrapper
import com.chartboost.mediation.unity.logging.LogLevel
import com.chartboost.mediation.unity.logging.UnityLoggingBridge

class AdStore {
    companion object {
        private val TAG = AdStore::class.java.simpleName
        private var fullscreenAdStore: MutableMap<Int, ChartboostMediationFullscreenAd> = mutableMapOf()
        private var bannerAdStore: MutableMap<Int, BannerAdWrapper> = mutableMapOf()

        @JvmStatic
        fun trackFullscreenAd(fullscreenAd: ChartboostMediationFullscreenAd)
        {
            val hashCode = fullscreenAd.hashCode()
            fullscreenAdStore[hashCode] = fullscreenAd
            UnityLoggingBridge.log(TAG, "Tracking FullscreenAd with Id: $hashCode", LogLevel.VERBOSE)
        }

        @JvmStatic
        fun releaseFullscreenAd(hashCode: Int)
        {
            val fullscreenAd = fullscreenAdStore[hashCode]
            fullscreenAd?.invalidate()
            fullscreenAdStore.remove(hashCode)
            UnityLoggingBridge.log(TAG, "Releasing FullscreenAd with Id: $hashCode", LogLevel.VERBOSE)
        }

        @JvmStatic
        fun trackBannerAd(bannerAd: BannerAdWrapper)
        {
            val hashCode = bannerAd.hashCode()
            bannerAdStore[hashCode] = bannerAd;
            UnityLoggingBridge.log(TAG, "Tracking BannerAd Ad with Id: $hashCode", LogLevel.VERBOSE)
        }

        @JvmStatic
        fun releaseBannerAd(hashCode: Int)
        {
            val bannerAd = bannerAdStore[hashCode]
            bannerAd?.destroy();
            bannerAdStore.remove(hashCode)
            UnityLoggingBridge.log(TAG, "Releasing BannerAd with Id: $hashCode", LogLevel.VERBOSE)
        }

        @JvmStatic
        fun adStoreInfo() : String {
            return "$TAG Fullscreen AdStore Count: ${fullscreenAdStore.count()}, Banner AdStore Count: \${bannerAdStore.count()}\n" +
                    "Legacy AdStore Count: \${legacyAdStore.count()}";
        }
    }
}

package com.chartboost.mediation.unity

import com.chartboost.heliumsdk.ad.ChartboostMediationFullscreenAd

class AdStore {
    companion object {
        private val TAG = AdStore::class.java.simpleName
        private var fullscreenAdStore: MutableMap<Int, ChartboostMediationFullscreenAd> = mutableMapOf()
        private var bannerAdStore: MutableMap<Int, BannerAdWrapper> = mutableMapOf()

        @JvmStatic
        fun trackFullscreenAd(fullscreenAd: ChartboostMediationFullscreenAd)
        {
            val hashCode = fullscreenAd.hashCode()
            fullscreenAdStore[hashCode] = fullscreenAd;
        }

        @JvmStatic
        fun releaseFullscreenAd(hashCode: Int)
        {
            val fullscreenAd = fullscreenAdStore[hashCode]
            fullscreenAd?.invalidate()
            fullscreenAdStore.remove(hashCode)
        }

        @JvmStatic
        fun trackBannerAd(bannerAd: BannerAdWrapper)
        {
            val hashCode = bannerAd.hashCode()
            bannerAdStore[hashCode] = bannerAd;
        }

        @JvmStatic
        fun releaseBannerAd(hashCode: Int)
        {
            bannerAdStore.remove(hashCode)
        }

        @JvmStatic
        fun storeInfo() : String {
            return "$TAG Fullscreen AdStore Count: ${fullscreenAdStore.count()}, Banner AdStore Count: \${bannerAdStore.count()}\n" +
                    "Legacy AdStore Count: \${legacyAdStore.count()}";
        }
    }
}

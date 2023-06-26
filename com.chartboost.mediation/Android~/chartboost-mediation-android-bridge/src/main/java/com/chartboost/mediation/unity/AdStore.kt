package com.chartboost.mediation.unity

import com.chartboost.heliumsdk.ad.ChartboostMediationFullscreenAd

class AdStore {
    companion object {
        private val TAG = AdStore::class.java.simpleName
        private var fullscreenAdStore: MutableMap<Int, ChartboostMediationFullscreenAd> = mutableMapOf()
        private var legacyAdStore: MutableMap<Int, AdWrapper> = mutableMapOf()

        @JvmStatic
        fun trackLegacyAd(legacyAd: AdWrapper)
        {
            val hashCode = legacyAd.hashCode()
            legacyAdStore[hashCode] = legacyAd
        }

        @JvmStatic
        fun releaseLegacyAd(hashCode: Int)
        {
            val legacyAd = legacyAdStore[hashCode]
            legacyAd?.destroy()
            legacyAdStore.remove(hashCode)
        }

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
        fun storeInfo() : String {
            return "$TAG Fullscreen AdStore Count: ${fullscreenAdStore.count()}, Legacy AdStore Count: ${legacyAdStore.count()}";
        }
    }
}

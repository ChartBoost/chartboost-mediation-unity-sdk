package com.chartboost.mediation.unity

import com.chartboost.heliumsdk.*
import com.chartboost.heliumsdk.ad.*
import com.chartboost.heliumsdk.ad.HeliumBannerAd.HeliumBannerSize
import com.chartboost.heliumsdk.domain.ChartboostMediationAdException
import com.chartboost.mediation.unity.AdWrapper.Companion.wrap
import com.chartboost.mediation.unity.EventProcessor.EventConsumer
import com.chartboost.mediation.unity.EventProcessor.EventWithErrorConsumer
import com.chartboost.mediation.unity.EventProcessor.LoadEventConsumer
import com.chartboost.mediation.unity.EventProcessor.serializeEvent
import com.chartboost.mediation.unity.EventProcessor.serializeEventWithException
import com.chartboost.mediation.unity.EventProcessor.serializeLoadEvent
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Dispatchers.Main
import kotlinx.coroutines.launch

class UnityBridge {

    companion object {
        private val TAG = UnityBridge::class.java.simpleName
        private var bannerEventsListener: IBannerEventListener? = null
        private var interstitialEventsListener: IInterstitialEventListener? = null
        private var rewardedEventListener: IRewardedEventListener? = null

        @JvmStatic
        fun setupEventListeners(
            interstitialListener: IInterstitialEventListener,
            rewardedListener: IRewardedEventListener,
            bannerListener: IBannerEventListener,
        ) {
            interstitialEventsListener = interstitialListener
            rewardedEventListener = rewardedListener
            bannerEventsListener = bannerListener
        }

        @JvmStatic
        fun toInitializationOptions(default: String, options: Array<String>) : HeliumInitializationOptions =
            HeliumInitializationOptions(options.toSet())

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

        @Deprecated("getInterstitialAd has been deprecated, utilize getFullscreenAd instead.")
        @JvmStatic
        fun getInterstitialAd(placementName: String): AdWrapper {
            val interstitialAd = HeliumInterstitialAd(UnityPlayer.currentActivity, placementName, object : HeliumFullscreenAdListener {
                override fun onAdCached(placementName: String, loadId: String, winningBidInfo: Map<String, String>, error: ChartboostMediationAdException?) {
                    serializeLoadEvent(placementName, loadId, winningBidInfo, error,
                        LoadEventConsumer { eventPlacementName: String, eventLoadId: String, auctionId: String, partnerId: String, price: Double, lineItemName:String, lineItemId:String, eventError: String ->
                            interstitialEventsListener?.DidLoadInterstitial(eventPlacementName, eventLoadId, auctionId, partnerId, price, lineItemName, lineItemId, eventError)
                        }
                    )
                }

                override fun onAdShown(placementName: String, error: ChartboostMediationAdException?) {
                    serializeEventWithException(placementName, error,
                        EventWithErrorConsumer { eventPlacementName: String, eventError: String ->
                            interstitialEventsListener?.DidShowInterstitial(eventPlacementName, eventError)
                        })
                }

                override fun onAdClosed(placementName: String, error: ChartboostMediationAdException?) {
                    serializeEventWithException(placementName, error,
                        EventWithErrorConsumer { eventPlacementName: String, eventError: String ->
                            interstitialEventsListener?.DidCloseInterstitial(eventPlacementName, eventError)
                        })
                }

                override fun onAdClicked(placementName: String) {
                    serializeEvent(placementName,
                        EventConsumer { eventPlacementName: String ->
                            interstitialEventsListener?.DidClickInterstitial(eventPlacementName)
                        })
                }

                override fun onAdImpressionRecorded(placementName: String) {
                    serializeEvent(placementName,
                        EventConsumer { eventPlacementName: String ->
                            interstitialEventsListener?.DidRecordImpression(eventPlacementName)
                        })
                }

                override fun onAdRewarded(placementName: String) {
//                TODO("Not yet implemented")
                }
            })

            return trackLegacy(interstitialAd)
        }

        @Deprecated("getInterstitialAd has been deprecated, utilize getFullscreenAd instead.")
        @JvmStatic
        fun getRewardedAd(placementName: String): AdWrapper {
            val rewardedAd = HeliumRewardedAd(UnityPlayer.currentActivity, placementName, object : HeliumFullscreenAdListener {
                override fun onAdCached(placementName: String, loadId: String, winningBidInfo: Map<String, String>, error: ChartboostMediationAdException?) {
                    serializeLoadEvent(placementName, loadId, winningBidInfo, error,
                        LoadEventConsumer { eventPlacementName: String, eventLoadId: String, auctionId: String, partnerId: String, price: Double, lineItemName:String, lineItemId:String, eventError: String ->
                            rewardedEventListener?.DidLoadRewarded(eventPlacementName, eventLoadId, auctionId, partnerId, price, lineItemName, lineItemId, eventError)
                        }
                    )
                }

                override fun onAdShown(placementName: String, error: ChartboostMediationAdException?) {
                    serializeEventWithException(placementName, error,
                        EventWithErrorConsumer { eventPlacementName: String, eventError: String ->
                            rewardedEventListener?.DidShowRewarded(eventPlacementName, eventError)
                        })
                }

                override fun onAdClosed(placementName: String, error: ChartboostMediationAdException?) {
                    serializeEventWithException(placementName, error,
                        EventWithErrorConsumer { eventPlacementName: String, eventError: String ->
                            rewardedEventListener?.DidCloseRewarded(eventPlacementName, eventError)
                        })
                }

                override fun onAdClicked(placementName: String) {
                    serializeEvent(placementName,
                        EventConsumer { eventPlacementName: String ->
                            rewardedEventListener?.DidClickRewarded(eventPlacementName)
                        })
                }

                override fun onAdImpressionRecorded(placementName: String) {
                    serializeEvent(placementName,
                        EventConsumer { eventPlacementName: String ->
                            rewardedEventListener?.DidRecordImpression(eventPlacementName)
                        })
                }

                override fun onAdRewarded(placementName: String) {
                    serializeEvent(placementName,
                        EventConsumer { eventPlacementName: String->
                            rewardedEventListener?.DidReceiveReward(eventPlacementName)
                        })
                }
            })

            return trackLegacy(rewardedAd)
        }

        @JvmStatic
        fun getBannerAd(placementName: String, size: HeliumBannerSize): AdWrapper {
            val bannerAd = HeliumBannerAd(UnityPlayer.currentActivity, placementName, size, object : HeliumBannerAdListener {
                override fun onAdCached(placementName: String, loadId: String, winningBidInfo: Map<String, String>, error: ChartboostMediationAdException?) {
                    serializeLoadEvent(placementName, loadId, winningBidInfo, error,
                        LoadEventConsumer { eventPlacementName: String, eventLoadId: String, auctionId: String, partnerId: String, price: Double, lineItemName:String, lineItemId:String, eventError: String ->
                            bannerEventsListener?.DidLoadBanner(eventPlacementName, eventLoadId, auctionId, partnerId, price, lineItemName, lineItemId, eventError)
                        }
                    )
                }

                override fun onAdClicked(placementName: String) {
                    serializeEvent(
                        placementName,
                        EventConsumer { eventPlacementName: String ->
                            bannerEventsListener?.DidClickBanner(eventPlacementName)
                        })
                }

                override fun onAdImpressionRecorded(placementName: String) {
                    serializeEvent(
                        placementName,
                        EventConsumer { eventPlacementName: String ->
                            bannerEventsListener?.DidRecordImpression(eventPlacementName)
                        })
                }
            })
            return trackLegacy(bannerAd)
        }

        private fun trackLegacy(ad: HeliumAd): AdWrapper {
            val wrapped = wrap(ad)
            AdStore.trackLegacyAd(wrapped)
            return wrapped;
        }
    }
}

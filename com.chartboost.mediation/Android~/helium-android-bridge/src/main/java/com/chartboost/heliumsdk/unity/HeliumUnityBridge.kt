package com.chartboost.heliumsdk.unity

import android.util.Log
import com.chartboost.heliumsdk.*
import com.chartboost.heliumsdk.ad.*
import com.chartboost.heliumsdk.ad.HeliumBannerAd.HeliumBannerSize
import com.chartboost.heliumsdk.domain.HeliumAdException
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.HeliumLoadEventConsumer
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.HeliumEventConsumerWithError
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.HeliumEventConsumer
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumEvent
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumEventWithError
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumLoadEvent
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializePlacementIlrdData
import com.chartboost.heliumsdk.unity.HeliumUnityAdWrapper.Companion.wrap
import com.unity3d.player.UnityPlayer

@Suppress("NAME_SHADOWING")
class HeliumUnityBridge {
    private var lifeCycleEventListener: ILifeCycleEventListener? = null
    private var bannerEventsListener: IBannerEventListener? = null
    private var interstitialEventsListener: IInterstitialEventListener? = null
    private var rewardedEventListener: IRewardedEventListener? = null
    private var ilrdObserver: HeliumIlrdObserver? = null
    private var initResultsObserver: PartnerInitializationResultsObserver? = null

    fun setupEventListeners(
        lifeCycleListener: ILifeCycleEventListener,
        interstitialListener: IInterstitialEventListener,
        rewardedListener: IRewardedEventListener,
        bannerListener: IBannerEventListener
    ) {
        lifeCycleEventListener = lifeCycleListener
        interstitialEventsListener = interstitialListener
        rewardedEventListener = rewardedListener
        bannerEventsListener = bannerListener
    }

    fun setSubjectToCoppa(isSubject: Boolean) {
        HeliumSdk.setSubjectToCoppa(isSubject)
    }

    fun setSubjectToGDPR(isSubject: Boolean) {
        HeliumSdk.setSubjectToGDPR(isSubject)
    }

    fun setCCPAConsent(hasGivenConsent: Boolean) {
        HeliumSdk.setCCPAConsent(hasGivenConsent)
    }

    fun setUserHasGivenConsent(hasGivenConsent: Boolean) {
        HeliumSdk.setUserHasGivenConsent(hasGivenConsent)
    }

    var userIdentifier: String?
        get() = HeliumSdk.getUserIdentifier()
        set(userIdentifier) {
            HeliumSdk.setUserIdentifier(userIdentifier)
        }

    fun start(appId: String, appSignature: String, unityVersion: String, initializationOptions: Array<String>) {
        ilrdObserver = object : HeliumIlrdObserver {
            override fun onImpression(impData: HeliumImpressionData) {
                val json = serializePlacementIlrdData(impData.placementId, impData.ilrdInfo)
                lifeCycleEventListener?.DidReceiveILRD(json)
            }
        }

        initResultsObserver = object : PartnerInitializationResultsObserver {
            override fun onPartnerInitializationResultsReady(data : PartnerInitializationResultsData) {
                val json = data.data.toString()
                lifeCycleEventListener?.DidReceivePartnerInitializationData(json)
            }
        }

        runTaskOnUiThread {
            UnityPlayer.currentActivity.let { activity ->
                HeliumSdk.start(activity, appId, appSignature,  HeliumInitializationOptions(initializationOptions.toSet()))  { error ->
                    error?.let {
                        Log.d("Unity", "HeliumUnityBridge: Plugin failed to initialize: $it.")
                    } ?: run {
                        HeliumSdk.setGameEngine("unity", unityVersion)
                        ilrdObserver?.let { observer ->
                            HeliumSdk.subscribeIlrd(observer)
                        }
                        initResultsObserver?.let {
                                observer ->
                            HeliumSdk.subscribeInitializationResults(observer)
                        }
                        Log.d("Unity", "HeliumUnityBridge: Plugin initialized.")
                    }
                    lifeCycleEventListener?.DidStart(error?.toString() ?: "")
                }
            }
        }
    }

    fun setTestMode(mode: Boolean) {
        HeliumSdk.setTestMode(mode)
    }

    fun destroy() {
        ilrdObserver?.let {
            HeliumSdk.unsubscribeIlrd(it)
            ilrdObserver = null
        }
    }

    fun getInterstitialAd(placementName: String): HeliumUnityAdWrapper {
        val interstitialAd = HeliumInterstitialAd(UnityPlayer.currentActivity, placementName, object : HeliumFullscreenAdListener {
            override fun onAdCached(placementName: String, loadId: String, bidInfo: Map<String, String>, error: HeliumAdException?) {
                serializeHeliumLoadEvent(placementName, loadId, bidInfo, error,
                    HeliumLoadEventConsumer { placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, error: String ->
                        interstitialEventsListener?.DidLoadInterstitial(placementName, loadId, auctionId, partnerId, price, error)
                    }
                )
            }

            override fun onAdShown(placementName: String, error: HeliumAdException?) {
                serializeHeliumEventWithError(placementName, error,
                    HeliumEventConsumerWithError { placementName: String, error: String ->
                        interstitialEventsListener?.DidShowInterstitial(placementName, error)
                    })
            }

            override fun onAdClosed(placementName: String, error: HeliumAdException?) {
                serializeHeliumEventWithError(placementName, error,
                    HeliumEventConsumerWithError { placementName: String, error: String ->
                        interstitialEventsListener?.DidCloseInterstitial(placementName, error)
                    })
            }

            override fun onAdClicked(placementName: String) {
                serializeHeliumEvent(placementName,
                    HeliumEventConsumer { placementName: String ->
                        interstitialEventsListener?.DidClickInterstitial(placementName)
                    })
            }

            override fun onAdImpressionRecorded(placementName: String) {
                serializeHeliumEvent(placementName,
                    HeliumEventConsumer { placementName: String ->
                        interstitialEventsListener?.DidRecordImpression(placementName)
                    })
            }

            override fun onAdRewarded(placementName: String) {
//                TODO("Not yet implemented")
            }
        })
        return wrap(interstitialAd)
    }

    fun getRewardedAd(placementName: String): HeliumUnityAdWrapper {
        val rewardedAd = HeliumRewardedAd(UnityPlayer.currentActivity, placementName, object : HeliumFullscreenAdListener {
            override fun onAdCached(placementName: String, loadId: String, bidInfo: Map<String, String>, error: HeliumAdException?) {
                serializeHeliumLoadEvent(placementName, loadId, bidInfo, error,
                    HeliumLoadEventConsumer { placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, error: String ->
                        rewardedEventListener?.DidLoadRewarded(placementName, loadId, auctionId, partnerId, price, error)
                    }
                )
            }

            override fun onAdShown(placementName: String, error: HeliumAdException?) {
                serializeHeliumEventWithError(placementName, error,
                    HeliumEventConsumerWithError { placementName: String, error: String ->
                        rewardedEventListener?.DidShowRewarded(placementName, error)
                    })
            }

            override fun onAdClosed(placementName: String, error: HeliumAdException?) {
                serializeHeliumEventWithError(placementName, error,
                    HeliumEventConsumerWithError { placementName: String, error: String ->
                        rewardedEventListener?.DidCloseRewarded(placementName, error)
                    })
            }

            override fun onAdClicked(placementName: String) {
                serializeHeliumEvent(placementName,
                    HeliumEventConsumer { placementName: String ->
                        rewardedEventListener?.DidClickRewarded(placementName)
                    })
            }

            override fun onAdImpressionRecorded(placementName: String) {
                serializeHeliumEvent(placementName,
                    HeliumEventConsumer { placementName: String ->
                        rewardedEventListener?.DidRecordImpression(placementName)
                    })
            }

            override fun onAdRewarded(placementName: String) {
                serializeHeliumEvent(placementName,
                    HeliumEventConsumer { placementName: String->
                        rewardedEventListener?.DidReceiveReward(placementName)
                    })
            }
        })
        return wrap(rewardedAd)
    }

    fun getBannerAd(placementName: String, size: Int): HeliumUnityAdWrapper {
        // default to standard
        var wantedSize = HeliumBannerSize.STANDARD
        when (size) {
            0 -> wantedSize = HeliumBannerSize.STANDARD
            1 -> wantedSize = HeliumBannerSize.MEDIUM
            2 -> wantedSize = HeliumBannerSize.LEADERBOARD
        }
        val bannerAd = HeliumBannerAd(UnityPlayer.currentActivity, placementName, wantedSize, object : HeliumBannerAdListener {
            override fun onAdCached(placementName: String, loadId: String, bidInfo: Map<String, String>, error: HeliumAdException?) {
                serializeHeliumLoadEvent(placementName, loadId, bidInfo, error,
                    HeliumLoadEventConsumer { placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, error: String ->
                        bannerEventsListener?.DidLoadBanner(placementName, loadId, auctionId, partnerId, price, error)
                    }
                )
            }

            override fun onAdClicked(placementName: String) {
                serializeHeliumEvent(
                    placementName,
                    HeliumEventConsumer { placementName: String ->
                        bannerEventsListener?.DidClickBanner(placementName)
                    })
            }

            override fun onAdImpressionRecorded(placementName: String) {
                serializeHeliumEvent(
                    placementName,
                    HeliumEventConsumer { placementName: String ->
                        bannerEventsListener?.DidRecordImpression(placementName)
                    })
            }
        })
        return wrap(bannerAd)
    }

    companion object {
        private val TAG = HeliumUnityBridge::class.java.simpleName

        // Stores a static instance of the HeliumPlugin class for easy access
        // from Unity
        @JvmStatic
        fun instance(): HeliumUnityBridge {
            return HeliumUnityBridge()
        }

        // Method whenever work needs to be done on the UI thread.
        fun runTaskOnUiThread(runnable: Runnable) {
            UnityPlayer.currentActivity.runOnUiThread {
                try {
                    runnable.run()
                } catch (ex: Exception) {
                    Log.w(TAG, "Exception found when running on UI Thread: ${ex.message}")
                }
            }
        }
    }
}

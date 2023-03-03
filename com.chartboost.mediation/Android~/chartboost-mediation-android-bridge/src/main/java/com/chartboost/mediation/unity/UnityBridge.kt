package com.chartboost.mediation.unity

import android.util.Log
import com.chartboost.heliumsdk.*
import com.chartboost.heliumsdk.ad.*
import com.chartboost.heliumsdk.ad.HeliumBannerAd.HeliumBannerSize
import com.chartboost.heliumsdk.domain.ChartboostMediationAdException
import com.chartboost.mediation.unity.EventProcessor.LoadEventConsumer
import com.chartboost.mediation.unity.EventProcessor.EventWithErrorConsumer
import com.chartboost.mediation.unity.EventProcessor.EventConsumer
import com.chartboost.mediation.unity.EventProcessor.serializeEvent
import com.chartboost.mediation.unity.EventProcessor.serializeEventWithError
import com.chartboost.mediation.unity.EventProcessor.serializeLoadEvent
import com.chartboost.mediation.unity.EventProcessor.serializePlacementIlrdData
import com.chartboost.mediation.unity.AdWrapper.Companion.wrap
import com.unity3d.player.UnityPlayer
@Suppress("NAME_SHADOWING")
class UnityBridge {
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

    fun setTestMode(mode: Boolean) {
        HeliumSdk.setTestMode(mode)
    }

    fun destroy() {
        ilrdObserver?.let {
            HeliumSdk.unsubscribeIlrd(it)
            ilrdObserver = null
        }
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
    fun getInterstitialAd(placementName: String): AdWrapper {
        val interstitialAd = HeliumInterstitialAd(UnityPlayer.currentActivity, placementName, object : HeliumFullscreenAdListener {
            override fun onAdCached(placementName: String, loadId: String, winningBidInfo: Map<String, String>, error: ChartboostMediationAdException?) {
                serializeLoadEvent(placementName, loadId, winningBidInfo, error,
                    LoadEventConsumer { placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, error: String ->
                        interstitialEventsListener?.DidLoadInterstitial(placementName, loadId, auctionId, partnerId, price, error)
                    }
                )
            }

            override fun onAdShown(placementName: String, error: ChartboostMediationAdException?) {
                serializeEventWithError(placementName, error,
                    EventWithErrorConsumer { placementName: String, error: String ->
                        interstitialEventsListener?.DidShowInterstitial(placementName, error)
                    })
            }

            override fun onAdClosed(placementName: String, error: ChartboostMediationAdException?) {
                serializeEventWithError(placementName, error,
                    EventWithErrorConsumer { placementName: String, error: String ->
                        interstitialEventsListener?.DidCloseInterstitial(placementName, error)
                    })
            }

            override fun onAdClicked(placementName: String) {
                serializeEvent(placementName,
                    EventConsumer { placementName: String ->
                        interstitialEventsListener?.DidClickInterstitial(placementName)
                    })
            }

            override fun onAdImpressionRecorded(placementName: String) {
                serializeEvent(placementName,
                    EventConsumer { placementName: String ->
                        interstitialEventsListener?.DidRecordImpression(placementName)
                    })
            }

            override fun onAdRewarded(placementName: String) {
//                TODO("Not yet implemented")
            }
        })
        return wrap(interstitialAd)
    }
    fun getRewardedAd(placementName: String): AdWrapper {
        val rewardedAd = HeliumRewardedAd(UnityPlayer.currentActivity, placementName, object : HeliumFullscreenAdListener {
            override fun onAdCached(placementName: String, loadId: String, winningBidInfo: Map<String, String>, error: ChartboostMediationAdException?) {
                serializeLoadEvent(placementName, loadId, winningBidInfo, error,
                    LoadEventConsumer { placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, error: String ->
                        rewardedEventListener?.DidLoadRewarded(placementName, loadId, auctionId, partnerId, price, error)
                    }
                )
            }

            override fun onAdShown(placementName: String, error: ChartboostMediationAdException?) {
                serializeEventWithError(placementName, error,
                    EventWithErrorConsumer { placementName: String, error: String ->
                        rewardedEventListener?.DidShowRewarded(placementName, error)
                    })
            }

            override fun onAdClosed(placementName: String, error: ChartboostMediationAdException?) {
                serializeEventWithError(placementName, error,
                    EventWithErrorConsumer { placementName: String, error: String ->
                        rewardedEventListener?.DidCloseRewarded(placementName, error)
                    })
            }

            override fun onAdClicked(placementName: String) {
                serializeEvent(placementName,
                    EventConsumer { placementName: String ->
                        rewardedEventListener?.DidClickRewarded(placementName)
                    })
            }

            override fun onAdImpressionRecorded(placementName: String) {
                serializeEvent(placementName,
                    EventConsumer { placementName: String ->
                        rewardedEventListener?.DidRecordImpression(placementName)
                    })
            }

            override fun onAdRewarded(placementName: String) {
                serializeEvent(placementName,
                    EventConsumer { placementName: String->
                        rewardedEventListener?.DidReceiveReward(placementName)
                    })
            }
        })
        return wrap(rewardedAd)
    }
    fun getBannerAd(placementName: String, size: Int): AdWrapper {
        // default to standard
        var wantedSize = HeliumBannerSize.STANDARD
        when (size) {
            0 -> wantedSize = HeliumBannerSize.STANDARD
            1 -> wantedSize = HeliumBannerSize.MEDIUM
            2 -> wantedSize = HeliumBannerSize.LEADERBOARD
        }
        val bannerAd = HeliumBannerAd(UnityPlayer.currentActivity, placementName, wantedSize, object : HeliumBannerAdListener {
            override fun onAdCached(placementName: String, loadId: String, winningBidInfo: Map<String, String>, error: ChartboostMediationAdException?) {
                serializeLoadEvent(placementName, loadId, winningBidInfo, error,
                    LoadEventConsumer { placementName: String, loadId: String, auctionId: String, partnerId: String, price: Double, error: String ->
                        bannerEventsListener?.DidLoadBanner(placementName, loadId, auctionId, partnerId, price, error)
                    }
                )
            }

            override fun onAdClicked(placementName: String) {
                serializeEvent(
                    placementName,
                    EventConsumer { placementName: String ->
                        bannerEventsListener?.DidClickBanner(placementName)
                    })
            }

            override fun onAdImpressionRecorded(placementName: String) {
                serializeEvent(
                    placementName,
                    EventConsumer { placementName: String ->
                        bannerEventsListener?.DidRecordImpression(placementName)
                    })
            }
        })
        return wrap(bannerAd)
    }
    companion object {
        private val TAG = UnityBridge::class.java.simpleName

        // Creates a static instance of the class for easy access from Unity, reference is hold in Unity layer.
        @JvmStatic
        fun instance(): UnityBridge {
            return UnityBridge()
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

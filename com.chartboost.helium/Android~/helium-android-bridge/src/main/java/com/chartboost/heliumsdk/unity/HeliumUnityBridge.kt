package com.chartboost.heliumsdk.unity

import android.util.Log
import com.chartboost.heliumsdk.*
import com.chartboost.heliumsdk.ad.*
import com.chartboost.heliumsdk.ad.HeliumBannerAd.HeliumBannerSize
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.HeliumBidEventConsumer
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.HeliumEventConsumer
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.HeliumRewardEventConsumer
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumBidEvent
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumEvent
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumRewardEvent
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

    fun onBackPressed(): Boolean {
        return HeliumSdk.onBackPressed()
    }

    var userIdentifier: String?
        get() = HeliumSdk.getUserIdentifier()
        set(userIdentifier) {
            HeliumSdk.setUserIdentifier(userIdentifier)
        }

    fun start(appId: String, appSignature: String, unityVersion: String) {
        ilrdObserver = object : HeliumIlrdObserver {
            override fun onImpression(impData: HeliumImpressionData) {
                val json = serializePlacementIlrdData(impData.placementId, impData.ilrdInfo)
                lifeCycleEventListener?.DidReceiveILRD(json)
            }
        }

        initResultsObserver = object : PartnerInitializationResultsObserver {
            override fun onPartnerInitializationResultsReady(data : PartnerInitializationResultsData) {
                val json = data.toString()
                lifeCycleEventListener?.DidReceivePartnerInitializationData(json)
            }
        }

        runTaskOnUiThread {
            // This call initializes the Helium SDK. This might change in the future with two ID parameters and we'll get rid of the logControllerListener
            HeliumSdk.start(UnityPlayer.currentActivity, appId, appSignature) { error ->
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
                lifeCycleEventListener?.DidStart(error?.hashCode() ?: -1, error?.toString() ?: "")
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
        return wrap(
            HeliumInterstitialAd(
                placementName,
                object : HeliumInterstitialAdListener {
                    override fun didReceiveWinningBid(
                        placementName: String,
                        hashMap: HashMap<String, String>
                    ) {
                        serializeHeliumBidEvent(
                            placementName,
                            hashMap,
                            HeliumBidEventConsumer { placementName: String, auctionId: String, partnerId: String, price: Double ->
                                interstitialEventsListener?.DidWinBidInterstitial(
                                    placementName,
                                    auctionId,
                                    partnerId,
                                    price
                                )
                            })
                    }

                    override fun didCache(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                interstitialEventsListener?.DidLoadInterstitial(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }

                    override fun didShow(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                interstitialEventsListener?.DidShowInterstitial(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }

                    override fun didClose(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                interstitialEventsListener?.DidCloseInterstitial(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }

                    override fun didClick(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                interstitialEventsListener?.DidClickInterstitial(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }

                    override fun didRecordImpression(placementName: String) {
                        serializeHeliumEvent(
                            placementName,
                            null,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                interstitialEventsListener?.DidRecordImpression(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }
                })
        )
    }

    fun getRewardedAd(placementName: String): HeliumUnityAdWrapper {
        return wrap(
            HeliumRewardedAd(
                placementName,
                object : HeliumRewardedAdListener {
                    override fun didReceiveWinningBid(
                        placementName: String,
                        hashMap: HashMap<String, String>
                    ) {
                        serializeHeliumBidEvent(
                            placementName,
                            hashMap,
                            HeliumBidEventConsumer { placementName: String, auctionId: String, partnerId: String, price: Double ->
                                rewardedEventListener?.DidWinBidRewarded(
                                    placementName,
                                    auctionId,
                                    partnerId,
                                    price
                                )
                            })
                    }

                    override fun didCache(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                rewardedEventListener?.DidLoadRewarded(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }

                    override fun didShow(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                rewardedEventListener?.DidShowRewarded(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }

                    override fun didClose(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                rewardedEventListener?.DidCloseRewarded(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }

                    override fun didReceiveReward(placementName: String, reward: String) {
                        serializeHeliumRewardEvent(
                            placementName,
                            reward,
                            HeliumRewardEventConsumer { placementName: String, reward: Int ->
                                rewardedEventListener?.DidReceiveReward(
                                    placementName,
                                    reward
                                )
                            })
                    }

                    override fun didClick(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                rewardedEventListener?.DidClickRewarded(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }

                    override fun didRecordImpression(placementName: String) {
                        serializeHeliumEvent(
                            placementName,
                            null,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                rewardedEventListener?.DidRecordImpression(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }
                })
        )
    }

    fun getBannerAd(placementName: String, size: Int): HeliumUnityAdWrapper {
        // default to standard
        var wantedSize = HeliumBannerSize.STANDARD

        when (size) {
            0 -> wantedSize = HeliumBannerSize.STANDARD
            1 -> wantedSize = HeliumBannerSize.MEDIUM
            2 -> wantedSize = HeliumBannerSize.LEADERBOARD
        }
        return wrap(
            HeliumBannerAd(
                UnityPlayer.currentActivity,
                placementName,
                wantedSize,
                object : HeliumBannerAdListener {
                    override fun didReceiveWinningBid(
                        placementName: String,
                        bidInfo: HashMap<String, String>
                    ) {
                        serializeHeliumBidEvent(placementName, bidInfo,
                            HeliumBidEventConsumer { placementName: String, auctionId: String, partnerId: String, price: Double ->
                                bannerEventsListener?.DidWinBidBanner(
                                    placementName,
                                    auctionId,
                                    partnerId,
                                    price
                                )
                            })
                    }

                    override fun didCache(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                bannerEventsListener?.DidLoadBanner(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }

                    override fun didClick(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                bannerEventsListener?.DidClickBanner(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }

                    override fun didRecordImpression(placementName: String) {
                        serializeHeliumEvent(
                            placementName,
                            null,
                            HeliumEventConsumer { placementName: String, errorCode: Int, errorDescription: String ->
                                bannerEventsListener?.DidRecordImpression(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }
                })
        )
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

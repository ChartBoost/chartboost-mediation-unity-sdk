package com.chartboost.heliumsdk.unity

import android.app.Activity
import android.util.Log
import com.chartboost.heliumsdk.HeliumIlrdObserver
import com.chartboost.heliumsdk.HeliumImpressionData
import com.chartboost.heliumsdk.HeliumSdk
import com.chartboost.heliumsdk.ad.*
import com.chartboost.heliumsdk.ad.HeliumBannerAd.HeliumBannerSize
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.HeliumBidEventConsumer
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.HeliumEventConsumer
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.HeliumRewardEventConsumer
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumBidEvent
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumEvent
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumRewardEvent
import com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializePlacementILRDData
import com.chartboost.heliumsdk.unity.HeliumUnityAdWrapper.Companion.wrap
import com.unity3d.player.UnityPlayer

@Suppress("NAME_SHADOWING")
class HeliumUnityBridge {
    private lateinit var _activity: Activity
    private var lifeCycleEventListener: ILifeCycleEventListener? = null
    private var bannerEventsListener: IBannerEventListener? = null
    private var interstitialEventsListener: IInterstitialEventListener? = null
    private var rewardedEventListener: IRewardedEventListener? = null
    private var ilrdObserver: HeliumIlrdObserver? = null
    fun setupEventListeners(
        lifeCycleListener: ILifeCycleEventListener?,
        interstitialListener: IInterstitialEventListener?,
        rewardedListener: IRewardedEventListener?,
        bannerListener: IBannerEventListener?
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

    fun onBackPressed(): Boolean {
        return HeliumSdk.onBackPressed()
    }

    fun start(appId: String?, appSignature: String?, unityVersion: String?) {
        _activity = UnityPlayer.currentActivity

        ilrdObserver = object : HeliumIlrdObserver {
            override fun onImpression(impData: HeliumImpressionData) {
                val json = serializePlacementILRDData(impData.placementId, impData.ilrdInfo)
                lifeCycleEventListener!!.DidReceiveILRD(json)
            }
        }
        runTaskOnUiThread {
            // This call initializes the Helium SDK. This might change in the future with two ID parameters and we'll get rid of the logControllerListener
            HeliumSdk.start(_activity, appId!!, appSignature!!) { error: Error? ->
                val errorNotFound = error == null
                if (errorNotFound) {
                    Log.d("Unity", "HeliumUnityBridge: Plugin Initialized")
                    HeliumSdk.setGameEngine("unity", unityVersion)
                    HeliumSdk.subscribeIlrd(ilrdObserver!!)
                } else {
                    Log.d("Unity", "HeliumUnityBridge: Plugin failed to initialize: $error")
                }
                val errorCode = if (errorNotFound) -1 else 1
                val errorDescription = if (errorNotFound) EMPTY_STRING else error.toString()
                lifeCycleEventListener!!.DidStart(errorCode, errorDescription)
            }
        }
    }

    fun setTestMode(mode: Boolean) {
        HeliumSdk.setTestMode(mode)
    }

    fun destroy() {
        if (ilrdObserver != null) {
            HeliumSdk.unsubscribeIlrd(ilrdObserver!!)
            ilrdObserver = null
        }
    }

    fun getInterstitialAd(placementName: String?): HeliumUnityAdWrapper? {
        return if (placementName == null) null else wrap(
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
                            HeliumBidEventConsumer { placementName: String?, auctionId: String?, partnerId: String?, price: Double ->
                                interstitialEventsListener!!.DidWinBidInterstitial(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                interstitialEventsListener!!.DidLoadInterstitial(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                interstitialEventsListener!!.DidShowInterstitial(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                interstitialEventsListener!!.DidCloseInterstitial(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                interstitialEventsListener!!.DidClickInterstitial(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                interstitialEventsListener!!.DidRecordImpression(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }
                })
        )
    }

    fun getRewardedAd(placementName: String?): HeliumUnityAdWrapper? {
        return if (placementName == null) null else wrap(
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
                            HeliumBidEventConsumer { placementName: String?, auctionId: String?, partnerId: String?, price: Double ->
                                rewardedEventListener!!.DidWinBidRewarded(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                rewardedEventListener!!.DidLoadRewarded(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                rewardedEventListener!!.DidShowRewarded(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                rewardedEventListener!!.DidCloseRewarded(
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
                            HeliumRewardEventConsumer { placementName: String?, reward: Int ->
                                rewardedEventListener!!.DidReceiveReward(
                                    placementName,
                                    reward
                                )
                            })
                    }

                    override fun didClick(placementName: String, error: HeliumAdError?) {
                        serializeHeliumEvent(
                            placementName,
                            error,
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                rewardedEventListener!!.DidClickRewarded(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                rewardedEventListener!!.DidRecordImpression(
                                    placementName,
                                    errorCode,
                                    errorDescription
                                )
                            })
                    }
                })
        )
    }

    fun getBannerAd(placementName: String?, size: Int): HeliumUnityAdWrapper? {
        if (placementName == null) return null
        var wantedSize: HeliumBannerSize? = null
        when (size) {
            0 -> wantedSize = HeliumBannerSize.STANDARD
            1 -> wantedSize = HeliumBannerSize.MEDIUM
            2 -> wantedSize = HeliumBannerSize.LEADERBOARD
        }
        val finalWantedSize = wantedSize
        return wrap(
            HeliumBannerAd(
                _activity,
                placementName,
                finalWantedSize!!,
                object : HeliumBannerAdListener {
                    override fun didReceiveWinningBid(
                        placementName: String,
                        bidInfo: HashMap<String, String>
                    ) {
                        serializeHeliumBidEvent(placementName, bidInfo,
                            HeliumBidEventConsumer { placementName: String?, auctionId: String?, partnerId: String?, price: Double ->
                                bannerEventsListener!!.DidWinBidBanner(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                bannerEventsListener!!.DidLoadBanner(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                bannerEventsListener!!.DidClickBanner(
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
                            HeliumEventConsumer { placementName: String?, errorCode: Int, errorDescription: String? ->
                                bannerEventsListener!!.DidRecordImpression(
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
        private const val TAG = "HeliumUnityBridge"
        private const val EMPTY_STRING = ""

        // Stores a static instance of the HeliumPlugin class for easy access
        // from Unity
        fun instance(): HeliumUnityBridge {
            return HeliumUnityBridge()
        }

        // Method whenever work needs to be done on the UI thread.
        fun runTaskOnUiThread(runnable: Runnable) {
            UnityPlayer.currentActivity.runOnUiThread {
                try {
                    runnable.run()
                } catch (ex: Exception) {
                    Log.w(TAG, "Exception found when running on UI Thread" + ex.message)
                }
            }
        }
    }
}

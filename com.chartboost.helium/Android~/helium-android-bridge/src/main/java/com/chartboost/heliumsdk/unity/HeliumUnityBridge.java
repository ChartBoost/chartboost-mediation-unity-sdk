package com.chartboost.heliumsdk.unity;

import static com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumBidEvent;
import static com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumEvent;
import static com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializeHeliumRewardEvent;
import static com.chartboost.heliumsdk.unity.HeliumEventProcessor.serializePlacementILRDData;
import static com.chartboost.heliumsdk.unity.HeliumUnityAdWrapper.Wrap;
import android.app.Activity;
import android.util.Log;
import com.chartboost.heliumsdk.HeliumSdk;
import com.chartboost.heliumsdk.HeliumIlrdObserver;
import com.chartboost.heliumsdk.ad.HeliumAdError;
import com.chartboost.heliumsdk.ad.HeliumBannerAd;
import com.chartboost.heliumsdk.ad.HeliumBannerAdListener;
import com.chartboost.heliumsdk.ad.HeliumInterstitialAd;
import com.chartboost.heliumsdk.ad.HeliumInterstitialAdListener;
import com.chartboost.heliumsdk.ad.HeliumRewardedAd;
import com.chartboost.heliumsdk.ad.HeliumRewardedAdListener;
import com.unity3d.player.UnityPlayer;
import androidx.annotation.NonNull;
import java.util.HashMap;

public class HeliumUnityBridge {
    private static final String TAG = "HeliumUnityBridge";
    private static final String EMPTY_STRING = "";

    private Activity _activity;

    private ILifeCycleEventListener lifeCycleEventListener;
    private IBannerEventListener bannerEventsListener;
    private IInterstitialEventListener interstitialEventsListener;
    private IRewardedEventListener rewardedEventListener;
    private HeliumIlrdObserver ilrdObserver;

    // Stores a static instance of the HeliumPlugin class for easy access
    // from Unity
    @SuppressWarnings("unused")
    public static HeliumUnityBridge instance() {
        return new HeliumUnityBridge();
    }

    @SuppressWarnings("unused")
    public void setupEventListeners(final ILifeCycleEventListener lifeCycleListener,
                                    final IInterstitialEventListener interstitialListener,
                                    final IRewardedEventListener rewardedListener,
                                    final IBannerEventListener bannerListener)
    {
        lifeCycleEventListener = lifeCycleListener;
        interstitialEventsListener = interstitialListener;
        rewardedEventListener = rewardedListener;
        bannerEventsListener = bannerListener;
    }

    @SuppressWarnings("unused")
    public void setSubjectToCoppa(boolean isSubject) {
        HeliumSdk.setSubjectToCoppa(isSubject);
    }

    @SuppressWarnings("unused")
    public void setSubjectToGDPR(boolean isSubject) {
        HeliumSdk.setSubjectToGDPR(isSubject);
    }

    @SuppressWarnings("unused")
    public void setCCPAConsent(boolean hasGivenConsent) { HeliumSdk.setCCPAConsent(hasGivenConsent); }

    @SuppressWarnings("unused")
    public void setUserHasGivenConsent(boolean hasGivenConsent) { HeliumSdk.setUserHasGivenConsent(hasGivenConsent); }

    @SuppressWarnings("unused")
    public void setUserIdentifier(final String userIdentifier) { HeliumSdk.setUserIdentifier(userIdentifier); }

    @SuppressWarnings("unused")
    public String getUserIdentifier() {
        return HeliumSdk.getUserIdentifier();
    }

    @SuppressWarnings("unused")
    public boolean onBackPressed() {
       return HeliumSdk.onBackPressed();
    }

    @SuppressWarnings("unused")
    public void start(final String appId, final String appSignature, final String unityVersion) {
        _activity = UnityPlayer.currentActivity;
        ilrdObserver = impData -> {
            String json = serializePlacementILRDData(impData.getPlacementId(), impData.getIlrdInfo());
            lifeCycleEventListener.DidReceiveILRD(json);
        };
        runTaskOnUiThread(() -> {
            // This call initializes the Helium SDK. This might change in the future with two ID parameters and we'll get rid of the logControllerListener
            HeliumSdk.start(_activity, appId, appSignature, error -> {
                boolean errorNotFound = error == null;
                if (errorNotFound) {
                    Log.d("Unity", "HeliumUnityBridge: Plugin Initialized");
                    HeliumSdk.setGameEngine("unity", unityVersion);
                    HeliumSdk.subscribeIlrd(ilrdObserver);
                } else {
                    Log.d("Unity", "HeliumUnityBridge: Plugin failed to initialize: " + error);
                }

                int errorCode = errorNotFound ? -1 : 1;
                String errorDescription = errorNotFound ? EMPTY_STRING : error.toString();
                lifeCycleEventListener.DidStart(errorCode, errorDescription);
            });
        });
    }

    @SuppressWarnings("unused")
    public void setTestMode(boolean mode) {
        HeliumSdk.setTestMode(mode);
    }

    @SuppressWarnings("unused")
    public void destroy() {
        if (ilrdObserver != null) {
            HeliumSdk.unsubscribeIlrd(ilrdObserver);
            ilrdObserver = null;
        }
    }

    @SuppressWarnings("unused")
    public HeliumUnityAdWrapper getInterstitialAd(final String placementName) {
        if (placementName == null)
            return null;

        return Wrap(new HeliumInterstitialAd(placementName, new HeliumInterstitialAdListener() {
            @Override
            public void didReceiveWinningBid(@NonNull String placementName, @NonNull HashMap<String, String> hashMap) {
                serializeHeliumBidEvent(placementName, hashMap, interstitialEventsListener::DidWinBidInterstitial);
            }

            @Override
            public void didCache(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, interstitialEventsListener::DidLoadInterstitial);
            }

            @Override
            public void didShow(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, interstitialEventsListener::DidShowInterstitial);
            }

            @Override
            public void didClose(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, interstitialEventsListener::DidCloseInterstitial);
            }

            @Override
            public void didClick(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, interstitialEventsListener::DidClickInterstitial);
            }

            @Override
            public void didRecordImpression(@NonNull String placementName) {
                serializeHeliumEvent(placementName, null, interstitialEventsListener::DidRecordImpression);
            }
        }));
    }

    @SuppressWarnings("unused")
    public HeliumUnityAdWrapper getRewardedAd(final String placementName) {
        if (placementName == null)
            return null;

        return Wrap(new HeliumRewardedAd(placementName, new HeliumRewardedAdListener() {
            @Override
            public void didReceiveWinningBid(@NonNull String placementName, @NonNull HashMap<String, String> hashMap) {
                serializeHeliumBidEvent(placementName, hashMap, rewardedEventListener::DidWinBidRewarded);
            }

            @Override
            public void didCache(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, rewardedEventListener::DidLoadRewarded);
            }

            @Override
            public void didShow(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, rewardedEventListener::DidShowRewarded);
            }

            @Override
            public void didClose(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, rewardedEventListener::DidCloseRewarded);
            }

            @Override
            public void didReceiveReward(@NonNull String placementName, @NonNull String reward) {
                serializeHeliumRewardEvent(placementName,  reward, rewardedEventListener::DidReceiveReward);
            }

            @Override
            public void didClick(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, rewardedEventListener::DidClickRewarded);
            }

            @Override
            public void didRecordImpression(@NonNull String placementName){
                serializeHeliumEvent(placementName, null, rewardedEventListener::DidRecordImpression);
            }
        }));
    }

    @SuppressWarnings("unused")
    public HeliumUnityAdWrapper getBannerAd(final String placementName, final int size) {
        if (placementName == null)
            return null;

        HeliumBannerAd.HeliumBannerSize wantedSize = null;

        switch (size) {
            case 0:
                wantedSize = HeliumBannerAd.HeliumBannerSize.STANDARD;
                break;
            case 1:
                wantedSize = HeliumBannerAd.HeliumBannerSize.MEDIUM;
                break;
            case 2:
                wantedSize = HeliumBannerAd.HeliumBannerSize.LEADERBOARD;
                break;
        }
        HeliumBannerAd.HeliumBannerSize finalWantedSize = wantedSize;

        return Wrap(new HeliumBannerAd(_activity, placementName, finalWantedSize, new HeliumBannerAdListener() {
            @Override
            public void didReceiveWinningBid(@NonNull String placementName, @NonNull HashMap<String, String> hashMap) {
                serializeHeliumBidEvent(placementName, hashMap, bannerEventsListener::DidWinBidBanner);
            }

            @Override
            public void didCache(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, bannerEventsListener::DidLoadBanner);
            }

            @Override
            public void didClick(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, bannerEventsListener::DidClickBanner);
            }

            @Override
            public void didRecordImpression(@NonNull String placementName) {
                serializeHeliumEvent(placementName, null, bannerEventsListener::DidRecordImpression);
            }
        }));
    }

    // Method whenever work needs to be done on the UI thread.
    public static void runTaskOnUiThread(final Runnable runnable) {
        UnityPlayer.currentActivity.runOnUiThread(() -> {
            try {
                runnable.run();
            } catch (Exception ex) {
                Log.w(TAG, "Exception found when running on UI Thread" + ex.getMessage());
            }
        });
    }
}

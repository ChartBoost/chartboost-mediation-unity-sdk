package com.chartboost.heliumsdk.unity;

import android.app.Activity;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.util.DisplayMetrics;
import android.widget.FrameLayout;
import android.widget.RelativeLayout;
import android.graphics.Color;

import com.chartboost.heliumsdk.HeliumSdk;
import com.chartboost.heliumsdk.HeliumIlrdObserver;
import com.chartboost.heliumsdk.ad.HeliumAd;
import com.chartboost.heliumsdk.ad.HeliumAdError;
import com.chartboost.heliumsdk.ad.HeliumBannerAd;
import com.chartboost.heliumsdk.ad.HeliumBannerAdListener;
import com.chartboost.heliumsdk.ad.HeliumInterstitialAd;
import com.chartboost.heliumsdk.ad.HeliumInterstitialAdListener;
import com.chartboost.heliumsdk.ad.HeliumRewardedAd;
import com.chartboost.heliumsdk.ad.HeliumRewardedAdListener;
import com.chartboost.heliumsdk.domain.Keywords;
import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import androidx.annotation.NonNull;

import java.util.HashMap;

public class HeliumUnityBridge {
    private static final String CB_UNITY_SDK_VERSION_STRING = "0.0.0";
    private static final int LEADERBOARD_WIDTH = 728;
    private static final int LEADERBOARD_HEIGHT = 90;
    private static final int MEDIUM_WIDTH = 300;
    private static final int MEDIUM_HEIGHT = 250;
    private static final int STANDARD_WIDTH = 320;
    private static final int STANDARD_HEIGHT = 50;

    private static final String TAG = "HeliumUnity-android";
    private static final String EMPTY_STRING = "";

    private HeliumUnityBridge _instance;
    // this field is only useful when doing direct tests outside of Unity
    public Activity _activity;
    private volatile boolean isHeliumInitialized = false;

    // This is the container for the banner.  We need a relative layout to position the banner in one of the 7
    // possible positions.  HeliumBannerAd is just a FrameLayout.
    private RelativeLayout mBannerLayout;

    private ILifeCycleEventListener lifeCycleEventListener;
    private IBannerEventListener bannerEventsListener;
    private IInterstitialEventListener interstitialEventsListener;
    private IRewardedEventListener rewardedEventListener;
    private HeliumIlrdObserver ilrdObserver;

    // Stores a static instance of the HeliumPlugin class for easy access
    // from Unity
    public static Object instance() {
        return new HeliumUnityBridge();
    }

    // Fetches the current Activity that the Unity player is using
    private Activity getActivity() {
        if (_activity != null)
            return _activity;

        return UnityPlayer.currentActivity;
    }

    @FunctionalInterface
    public interface HeliumEventConsumer<T, V, S> {
        void accept(T placementName, V errorCode, S errorDescription);
    }

    private void serializeHeliumEvent(String placementName, HeliumAdError heliumAdError, HeliumEventConsumer<String, Integer, String> eventConsumer){
        if (placementName == null)
            placementName = EMPTY_STRING;

        int errorCode = -1;
        String errorDescription = EMPTY_STRING;

        if (heliumAdError != null) {
            errorCode = heliumAdError.getCode();
            errorDescription = heliumAdError.getMessage();
        }

        eventConsumer.accept(placementName, errorCode, errorDescription);
    }

    @FunctionalInterface
    public interface HeliumBidEventConsumer<T, V, S, X, Z>
    {
        void accept(T placementName, V partnerPlacementName, S auctionId, X price, Z seat);
    }

    private void serializeHeliumBidEvent(String placementName, HashMap<String, String> dataMap, HeliumBidEventConsumer<String, String, String, Double, String> eventConsumer)
    {
        try {
            if (placementName == null)
                placementName = EMPTY_STRING;

            String PARTNER_PLACEMENT_NAME = "partner-placement-name";
            String partnerPlacementName = dataMap.get(PARTNER_PLACEMENT_NAME);
            partnerPlacementName = partnerPlacementName == null ? EMPTY_STRING : partnerPlacementName;
            String AUCTION_ID = "auction-id";
            String auctionId = dataMap.get(AUCTION_ID);
            auctionId = auctionId == null ? EMPTY_STRING : auctionId;
            String PRICE = "price";
            String priceAsString = dataMap.get(PRICE);
            priceAsString = priceAsString == null ? "0" : priceAsString;
            double price = Double.parseDouble(priceAsString);
            String SEAT = "seat";
            String seat = dataMap.get(SEAT);
            seat = seat == null ? EMPTY_STRING : auctionId;
            eventConsumer.accept(placementName, partnerPlacementName, auctionId, price, seat);
        }
        catch (Exception e) {
            Log.d(TAG, "bidFetchingInformationError", e);
        }
    }

    @FunctionalInterface
    public interface HeliumRewardEventConsumer<T, V>
    {
        void accept(T placementName, V reward);
    }

    private void serializeHeliumRewardEvent(String placementName, String reward, HeliumRewardEventConsumer<String, String> eventConsumer) {
        if (placementName == null)
            placementName = EMPTY_STRING;

        if (reward == null)
            reward = EMPTY_STRING;

        eventConsumer.accept(placementName, reward);
    }

    private String serializePlacementILRDData(String placementName, JSONObject ilrdInfo) {
        JSONObject serializedString = new JSONObject();
        JSONObject infoObj = new JSONObject();
        try {
            serializedString.put("placementName", placementName);
            serializedString.put("ilrd", ilrdInfo);
        } catch (JSONException e) {
            Log.d(TAG, "serializeError", e);
        }
        return serializedString.toString();
    }


    // ##### ##### ##### ##### ##### ##### ##### #####
    // Public API
    // ##### ##### ##### ##### ##### ##### ##### #####

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

    public void setSubjectToCoppa(boolean isSubject) {
        HeliumSdk.setSubjectToCoppa(isSubject);
    }

    public void setSubjectToGDPR(boolean isSubject) {
        HeliumSdk.setSubjectToGDPR(isSubject);
    }

    public void setCCPAConsent(boolean hasGivenConsent) { HeliumSdk.setCCPAConsent(hasGivenConsent); }

    public void setUserHasGivenConsent(boolean hasGivenConsent) { HeliumSdk.setUserHasGivenConsent(hasGivenConsent); }

    public void setUserIdentifier(final String userIdentifier) { HeliumSdk.setUserIdentifier(userIdentifier); }

    public String getUserIdentifier() {
        return HeliumSdk.getUserIdentifier();
    }

    public boolean onBackPressed() {
       return HeliumSdk.onBackPressed();
    }

    public void start(final String appId, final String appSignature, final String unityVersion) {
        _activity = UnityPlayer.currentActivity;
        ilrdObserver = impData -> {
            String json = serializePlacementILRDData(impData.getPlacementId(), impData.getIlrdInfo());
            lifeCycleEventListener.DidReceiveILRD(json);
        };
        runTaskOnUiThread(() -> {
            // This call initializes the Helium SDK. This might change in the future with two ID parameters and we'll get rid of the logControllerListener
            HeliumSdk.start(getActivity(), appId, appSignature, error -> {
                boolean errorNotFound = error == null;
                if (errorNotFound) {
                    Log.d("Unity", "HeliumUnityBridge: Plugin Initialized");
                    HeliumSdk.setGameEngine("unity", unityVersion);
                    HeliumUnityBridge.this.isHeliumInitialized = true;
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

    public void setTestMode(boolean mode) {
        HeliumSdk.setTestMode(mode);
    }

    public void pause(final boolean pause) {

    }

    public void destroy() {
        if (ilrdObserver != null) {
            HeliumSdk.unsubscribeIlrd(ilrdObserver);
            ilrdObserver = null;
        }
    }

    public HeliumAdWrapper getInterstitialAd(final String placementName) {
        if (placementName == null)
            return null;

        return wrapAd(() -> new HeliumInterstitialAd(placementName, new HeliumInterstitialAdListener() {
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
        }));
    }

    public HeliumAdWrapper getRewardedAd(final String placementName) {
        if (placementName == null)
            return null;

        return wrapAd(() -> new HeliumRewardedAd(placementName, new HeliumRewardedAdListener() {
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
        }));
    }


    public HeliumAdWrapper getBannerAd(final String placementName, final int size) {
        if (placementName == null)
            return null;

        HeliumBannerAd.Size wantedSize = null;

        switch (size) {
            case 0:
                wantedSize = HeliumBannerAd.Size.STANDARD;
                break;
            case 1:
                wantedSize = HeliumBannerAd.Size.MEDIUM;
                break;
            case 2:
                wantedSize = HeliumBannerAd.Size.LEADERBOARD;
                break;
        }
        HeliumBannerAd.Size finalWantedSize = wantedSize;

        return wrapAd(() -> new HeliumBannerAd(getActivity(), placementName, finalWantedSize, new HeliumBannerAdListener() {
            @Override
            public void didReceiveWinningBid(@NonNull String placementName, @NonNull HashMap<String, String> hashMap) {
                serializeHeliumBidEvent(placementName, hashMap, bannerEventsListener::DidWinBidBanner);
            }

            @Override
            public void didCache(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, bannerEventsListener::DidLoadBanner);
            }

            @Override
            public void didShow(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, bannerEventsListener::DidShowBanner);
            }

            @Override
            public void didClose(@NonNull String placementName, HeliumAdError error) {
                // Not all partners support it, keeping from being triggered at the Unity side.
                // UnitySendMessage(gameObjectName, "DidCloseBannerEvent", serializePlacementError(placementName, error));
            }

            @Override
            public void didClick(@NonNull String placementName, HeliumAdError error) {
                serializeHeliumEvent(placementName,  error, bannerEventsListener::DidClickBanner);
            }
        }));
    }

    private void createBannerLayout(int position) {

        // Check if there is an already existing banner layout. If so, remove it. Otherwise,
        // create a new one.
        if (mBannerLayout != null && mBannerLayout.getChildCount() >= 0) {
            FrameLayout bannerParent = (FrameLayout) mBannerLayout.getParent();
            if (bannerParent != null) {
                bannerParent.removeView(mBannerLayout);
            }
        } else {
            mBannerLayout = new RelativeLayout(getActivity());
            mBannerLayout.setBackgroundColor(Color.TRANSPARENT);
        }

        /*
            //     TopLeft = 0,
            //     TopCenter = 1,
            //     TopRight = 2,
            //     Center = 3,
            //     BottomLeft = 4,
            //     BottomCenter = 5,
            //     BottomRight = 6
        */
        int bannerGravityPosition = 0;
        switch (position) {
            case 0:
                bannerGravityPosition = Gravity.TOP | Gravity.LEFT;
                break;
            case 1:
                bannerGravityPosition = Gravity.TOP | Gravity.CENTER_HORIZONTAL;
                break;
            case 2:
                bannerGravityPosition = Gravity.TOP | Gravity.RIGHT;
                break;
            case 3:
                bannerGravityPosition = Gravity.CENTER_VERTICAL | Gravity.CENTER_HORIZONTAL;
                break;
            case 4:
                bannerGravityPosition = Gravity.BOTTOM | Gravity.LEFT;
                break;
            case 5:
                bannerGravityPosition = Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
                break;
            case 6:
                bannerGravityPosition = Gravity.BOTTOM | Gravity.RIGHT;
                break;
        }
        mBannerLayout.setGravity(bannerGravityPosition);
    }

    private HeliumAdWrapper wrapAd(AdCreator ad) {
        return new HeliumAdWrapper(ad);
    }

    // Method whenever work needs to be done on the UI thread.
    private void runTaskOnUiThread(final Runnable runnable) {
        getActivity().runOnUiThread(() -> {
            try {
                runnable.run();
            } catch (Exception ex) {
                Log.w(TAG, "Exception found when running on UI Thread" + ex.getMessage());
            }
        });
    }

    private interface AdCreator {
        HeliumAd createAd();
    }

    private class HeliumAdWrapper {
        private final AdCreator adCreator;
        private HeliumAd ad;
        private boolean startedLoad = false;

        public HeliumAdWrapper(AdCreator adCreator) {
            this.adCreator = adCreator;
        }

        public void load() {
            ad().load();
            startedLoad = true;
        }

        public boolean setKeyword(final String keyword, final String value) {
            HeliumAd ad = ad();
            Keywords keywords = ad.getKeywords(); // not null by design
            return keywords.set(keyword, value);
        }

        public String removeKeyword(final String keyword) {
            HeliumAd ad = ad();
            Keywords keywords = ad.getKeywords();  // not null by design
            return keywords.remove(keyword);
        }

        // Generally used for banners on unity with a given screen location.
        public void show(int screenLocation) {
            runTaskOnUiThread(() -> {
                // Check if the helium banner already has a child. If so, remove it.
                if (mBannerLayout != null && mBannerLayout.getChildCount() >= 0) {
                    mBannerLayout.removeAllViews();
                }

                // Create the banner layout on the given position.
                createBannerLayout(screenLocation);

                // Attach the banner layout to the activity.
                float pixels = getDisplayDensity();
                try {
                    HeliumBannerAd bannerAd = (HeliumBannerAd) ad();
                    switch (bannerAd.getSize() != null ? bannerAd.getSize() : HeliumBannerAd.Size.STANDARD) {
                        case LEADERBOARD:
                            bannerAd.setLayoutParams(getBannerLayoutParams(pixels, LEADERBOARD_WIDTH, LEADERBOARD_HEIGHT));
                            break;
                        case MEDIUM:
                            bannerAd.setLayoutParams(getBannerLayoutParams(pixels, MEDIUM_WIDTH, MEDIUM_HEIGHT));
                            break;
                        case STANDARD:
                            bannerAd.setLayoutParams(getBannerLayoutParams(pixels, STANDARD_WIDTH, STANDARD_HEIGHT));
                    }

                    // Attach the banner to the banner layout.
                    mBannerLayout.addView(bannerAd);

                    getActivity().addContentView(mBannerLayout,
                            new FrameLayout.LayoutParams(
                                    FrameLayout.LayoutParams.MATCH_PARENT,
                                    FrameLayout.LayoutParams.MATCH_PARENT));

                    // Show the banner
                    ad().show();
                    // This immediately sets the visibility of this banner. If this doesn't happen
                    // here, it is impossible to set the visibility later.
                    bannerAd.setVisibility(View.VISIBLE);

                    // This affects future visibility of the banner layout. Despite it never being
                    // set invisible, not setting this to visible here makes the banner not visible.
                    mBannerLayout.setVisibility(View.VISIBLE);
                } catch(Exception ex) {
                    Log.w(TAG, "Helium encountered an error calling banner show() - " + ex.getMessage());
                }
            });
        }

        private FrameLayout.LayoutParams getBannerLayoutParams(float pixels, int width, int height) {
            return new FrameLayout.LayoutParams((int)(pixels * width), (int)(pixels * height));
        }

        private float getDisplayDensity() {
            try {
                return getActivity().getResources().getDisplayMetrics().density;
            } catch(Exception ex) {
                Log.w(TAG, "Helium encountered an error calling getDisplayDensity() - " + ex.getMessage());
            }
            return DisplayMetrics.DENSITY_DEFAULT;
        }

        // For other ad types.
        public void show() {
            runTaskOnUiThread(() -> ad().show());
        }

        public void setCustomData(final String customData) {
            HeliumAd ad = ad();
            if (ad instanceof HeliumRewardedAd) {
                HeliumRewardedAd rewardedAd = (HeliumRewardedAd)ad;
                rewardedAd.setCustomData(customData);
            }
            else {
                throw new RuntimeException("custom data can only be set on a rewarded ad");
            }
        }

        public boolean clearLoaded() {
            return ad().clearLoaded();
        }

        public boolean readyToShow() {
            if (!isHeliumInitialized || !startedLoad)
                return false;
            try {
                return ad().readyToShow();
            } catch (Exception ex) {
                Log.w(TAG, "Helium encountered an error calling Ad.readyToShow() - " + ex.getMessage());
                return false;
            }
        }

        public void destroy() {
            runTaskOnUiThread(() -> {
                // If the ad is a banner, let's remove it first.
                if (ad() instanceof HeliumBannerAd && mBannerLayout != null) {
                    mBannerLayout.removeAllViews();
                    mBannerLayout.setVisibility(View.GONE);
                }
                ad().destroy();
            });
        }

        public void setBannerVisibility(boolean isVisible) {
            runTaskOnUiThread(() -> {
                HeliumAd ad = ad();
                if (ad instanceof HeliumBannerAd && mBannerLayout != null) {
                    HeliumBannerAd bannerAd = (HeliumBannerAd)ad;
                    int visibility = isVisible ? View.VISIBLE : View.INVISIBLE;
                    mBannerLayout.setVisibility(visibility);
                    bannerAd.setVisibility(visibility);
                }
            });
        }

        private HeliumAd ad() {
            if (!isHeliumInitialized)
                throw new RuntimeException("cannot interact with Helium ad as Helium is not yet initialized");
            if (ad == null)
                ad = adCreator.createAd();
            return ad;
        }
    }
}

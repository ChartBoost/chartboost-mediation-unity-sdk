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
import com.chartboost.heliumsdk.HeliumImpressionData;
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
    private static HeliumUnityBridge _instance;
    // this field is only useful when doing direct tests outside of Unity
    public Activity _activity;
    private volatile boolean isHeliumInitialized = false;
    private String gameObjectName = "";

    // This is the container for the banner.  We need a relative layout to position the banner in one of the 7
    // possible positions.  HeliumBannerAd is just a FrameLayout.
    private RelativeLayout mBannerLayout;

    /**
     * Interface for sending real-time background events while the Unity Player is paused (which will happen when
     * a fullscreen ad is being displayed).
     */
    public interface IBackgroundEventListener
    {
        void onBackgroundEvent(String event, String json);
    }
    private static IBackgroundEventListener bgEventListener;
    private HeliumIlrdObserver ilrdObserver;

    // Stores a static instance of the HeliumPlugin class for easy access
    // from Unity
    public static Object instance() {
        if (_instance == null) {
            _instance = new HeliumUnityBridge();
        }
        return _instance;
    }

    // Fetches the current Activity that the Unity player is using
    private Activity getActivity() {
        if (_activity != null)
            return _activity;

        return UnityPlayer.currentActivity;
    }

    // Used for callbacks from Java to Unity
    private void UnitySendMessage(String go, String m, String p) {
        UnitySendMessage(go, m, p, false);
    }

    private void UnitySendMessage(String go, String m, String p, boolean background) {
        try {
            Log.d("HeliumUnityBridge", "Send message " + go + " / " + m + " / " + p);
            if (background && bgEventListener != null) {
                bgEventListener.onBackgroundEvent(m, p);
            }
            else {
                UnityPlayer.UnitySendMessage(go, m, p);
            }
        } catch (Exception e) {
            Log.i(TAG, "UnitySendMessage error: " + e.getMessage());
            Log.i(TAG, "UnitySendMessage: " + go + ", " + m + ", " + p);
        }
    }

    private String serializeError(Error error) {
        JSONObject errorMessage = new JSONObject();
        try {
            errorMessage.put("errorCode", error != null ? 1 : -1);
            errorMessage.put("errorDescription", error != null ? error.getMessage() : null);
        } catch (JSONException e) {
            Log.d(TAG, "serializeError", e);
        }
        return errorMessage.toString();
    }

    private String serializePlacementError(String placementName, HeliumAdError error) {
        JSONObject errorMessage = new JSONObject();
        try {
            errorMessage.put("placementName", placementName);
            errorMessage.put("errorCode", error != null ? error.getCode() : -1);
            errorMessage.put("errorDescription", error != null ? error.getMessage() : null);
        } catch (JSONException e) {
            Log.d(TAG, "serializeError", e);
        }
        return errorMessage.toString();
    }

    private String serializePlacementReward(String placementName, String reward) {
        JSONObject errorMessage = new JSONObject();
        try {
            errorMessage.put("placementName", placementName);
            errorMessage.put("reward", reward);
        } catch (JSONException e) {
            Log.d(TAG, "serializeError", e);
        }
        return errorMessage.toString();
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

    private String serializeWinningBid(String placementName, HashMap<String, String> hashMap) {
        JSONObject serializedString = new JSONObject();
        JSONObject infoObj = new JSONObject();
        try {
            serializedString.put("placementName", placementName);
            infoObj.put("seat", hashMap.get("seat"));
            infoObj.put("partner-placement-name", hashMap.get("partner-placement-name"));
            infoObj.put("auction-id", hashMap.get("auction-id"));
            infoObj.put("price", Double.parseDouble(hashMap.get("price")));
            serializedString.put("info", infoObj);
        } catch (JSONException e) {
            Log.d(TAG, "serializeError", e);
        }
        return serializedString.toString();
    }

    // ##### ##### ##### ##### ##### ##### ##### #####
    // Public API
    // ##### ##### ##### ##### ##### ##### ##### #####

    public void setSubjectToCoppa(boolean isSubject) {
        HeliumSdk.setSubjectToCoppa(isSubject);
    }

    public void setSubjectToGDPR(boolean isSubject) {
        HeliumSdk.setSubjectToGDPR(isSubject);
    }

    public void setCCPAConsent(boolean hasGivenConsent) {
        HeliumSdk.setCCPAConsent(hasGivenConsent);
    }

    public void setUserHasGivenConsent(boolean hasGivenConsent) {
        HeliumSdk.setUserHasGivenConsent(hasGivenConsent);
    }

    public void setUserIdentifier(final String userIdentifier) {
        HeliumSdk.setUserIdentifier(userIdentifier);
    }

    public String getUserIdentifier() {
        return HeliumSdk.getUserIdentifier();
    }

    public void start(final String appId, final String appSignature, final String unityVersion, final IBackgroundEventListener backgroundEventListener) {
        _activity = UnityPlayer.currentActivity;
        HeliumUnityBridge.bgEventListener = backgroundEventListener;
        ilrdObserver = new HeliumIlrdObserver() {
            @Override
            public void onImpression(HeliumImpressionData impData) {
                if (impData != null) {
                    String json = serializePlacementILRDData(impData.getPlacementId(), impData.getIlrdInfo());
                    UnitySendMessage(gameObjectName, "DidReceiveILRD", json, true);
                }
            }
        };
        runTaskOnUiThread(new Runnable() {
            @Override
            public void run() {
                // This call initializes the Helium SDK. This might change in the future with two ID parameters and we'll get rid of the logControllerListener
                HeliumSdk.start(getActivity(), appId, appSignature, new HeliumSdk.HeliumSdkListener() {
                    @Override
                    public void didInitialize(Error error) {
                        if (error == null) {
                            Log.d("Unity", "HeliumUnityBridge: Plugin Initialized");
                            HeliumSdk.setGameEngine("unity", unityVersion);
                            HeliumUnityBridge.this.isHeliumInitialized = true;
                            HeliumSdk.subscribeIlrd(ilrdObserver);
                        } else {
                            Log.d("Unity", "HeliumUnityBridge: Plugin failed to initialize: " + error.toString());
                        }
                        UnitySendMessage(gameObjectName, "DidStartEvent", serializeError(error));
                    }
                });
            }
        });
    }

    public void setTestMode(boolean mode) {
        HeliumSdk.setTestMode(mode);
    }

    public void pause(final boolean pause) {
        runTaskOnUiThread(new Runnable() {
            @Override
            public void run() {
                // Leaving this code here in case we need to use lifecycle callbacks again in the future
                if (!isHeliumInitialized)
                    return;
                if (pause) {

                } else {

                }
            }
        });
    }

    public void destroy() {
        if (ilrdObserver != null) {
            HeliumSdk.unsubscribeIlrd(ilrdObserver);
            ilrdObserver = null;
        }
        runTaskOnUiThread(new Runnable() {
            @Override
            public void run() {
                // Leaving this code here in case we need to use lifecycle callbacks again in the future
                if (!isHeliumInitialized)
                    return;
            }
        });
    }

    public boolean isAnyViewVisible() {
        if (!isHeliumInitialized)
            return false;
        return false; //Chartboost.isAnyViewVisible();
    }

    public HeliumAdWrapper getInterstitialAd(final String placementName) {
        if (placementName == null)
            return null;

        return wrapAd(new AdCreator() {
            @Override
            public HeliumAd createAd() {
                return new HeliumInterstitialAd(placementName, new HeliumInterstitialAdListener() {
                    @Override
                    public void didReceiveWinningBid(String placementName, HashMap<String, String> hashMap) {
                        UnitySendMessage(gameObjectName, "DidWinBidInterstitialEvent", serializeWinningBid(placementName, hashMap));
                    }

                    @Override
                    public void didCache(String placementName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidLoadInterstitialEvent", serializePlacementError(placementName, error));
                    }

                    @Override
                    public void didShow(String placementName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidShowInterstitialEvent", serializePlacementError(placementName, error));
                    }

                    @Override
                    public void didClose(String placementName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidCloseInterstitialEvent", serializePlacementError(placementName, error));
                    }

                    @Override
                    public void didClick(String placmenetName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidClickInterstitialEvent", serializePlacementError(placementName, error));
                    }
                });
            }
        });
    }

    public HeliumAdWrapper getRewardedAd(final String placementName) {
        if (placementName == null)
            return null;

        return wrapAd(new AdCreator() {
            @Override
            public HeliumAd createAd() {
                return new HeliumRewardedAd(placementName, new HeliumRewardedAdListener() {
                    @Override
                    public void didReceiveWinningBid(String placementName, HashMap<String, String> hashMap) {
                        UnitySendMessage(gameObjectName, "DidWinBidRewardedEvent", serializeWinningBid(placementName, hashMap));
                    }

                    @Override
                    public void didCache(String placementName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidLoadRewardedEvent", serializePlacementError(placementName, error));
                    }

                    @Override
                    public void didShow(String placementName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidShowRewardedEvent", serializePlacementError(placementName, error));
                    }

                    @Override
                    public void didClose(String placementName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidCloseRewardedEvent", serializePlacementError(placementName, error));
                    }

                    @Override
                    public void didReceiveReward(String placementName, String reward) {
                        UnitySendMessage(gameObjectName, "DidReceiveRewardEvent", serializePlacementReward(placementName, reward), true);
                    }

                    @Override
                    public void didClick(String placmenetName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidClickRewardedEvent", serializePlacementError(placementName, error));
                    }
                });
            }
        });
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

        return wrapAd(new AdCreator() {
            @Override
            public HeliumAd createAd() {

                return new HeliumBannerAd(getActivity(), placementName, finalWantedSize, new HeliumBannerAdListener() {
                    @Override
                    public void didReceiveWinningBid(String s, HashMap<String, String> hashMap) {
                        UnitySendMessage(gameObjectName, "DidWinBidBannerEvent", serializeWinningBid(placementName, hashMap));
                    }

                    @Override
                    public void didCache(String placementName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidLoadBannerEvent", serializePlacementError(placementName, error));
                    }

                    @Override
                    public void didShow(String placementName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidShowBannerEvent", serializePlacementError(placementName, error));
                    }

                    @Override
                    public void didClose(String placementName, HeliumAdError error) {
                        // Not all partners support it, keeping from being triggered at the Unity side.
                        // UnitySendMessage(gameObjectName, "DidCloseBannerEvent", serializePlacementError(placementName, error));
                    }

                    @Override
                    public void didClick(String placmenetName, HeliumAdError error) {
                        UnitySendMessage(gameObjectName, "DidClickBannerEvent", serializePlacementError(placementName, error));
                    }
                });
            }
        });
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

    public void setGameObjectName(String name) {
        gameObjectName = name;
    }

    private HeliumAdWrapper wrapAd(AdCreator ad) {
        return new HeliumAdWrapper(ad);
    }

    // Method whenever work needs to be done on the UI thread.
    private void runTaskOnUiThread(final Runnable runnable) {
        getActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                try {
                    runnable.run();
                } catch (Exception ex) {
                    Log.w(TAG, "Exception found when running on UI Thread" + ex.getMessage());
                }
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
            runTaskOnUiThread(new Runnable() {
                @Override
                public void run() {
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
                        switch (bannerAd.getSize()) {
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
            runTaskOnUiThread(new Runnable() {
                @Override
                public void run() {
                    ad().show();
                }
            });
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
            runTaskOnUiThread(new Runnable() {
                @Override
                public void run() {
                    // If the ad is a banner, let's remove it first.
                    if (ad() instanceof HeliumBannerAd && mBannerLayout != null) {
                        mBannerLayout.removeAllViews();
                        mBannerLayout.setVisibility(View.GONE);
                    }
                    ad().destroy();
                }
            });
        }

        public void setBannerVisibility(boolean isVisible) {
            runTaskOnUiThread(new Runnable() {
                @Override
                public void run() {
                    HeliumAd ad = ad();
                    if (ad instanceof HeliumBannerAd && mBannerLayout != null) {
                        HeliumBannerAd bannerAd = (HeliumBannerAd)ad;
                        int visibility = isVisible ? View.VISIBLE : View.INVISIBLE;
                        mBannerLayout.setVisibility(visibility);
                        bannerAd.setVisibility(visibility);
                    }
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

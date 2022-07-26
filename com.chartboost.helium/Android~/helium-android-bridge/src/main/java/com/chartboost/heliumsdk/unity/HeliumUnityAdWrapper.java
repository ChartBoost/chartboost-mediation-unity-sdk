package com.chartboost.heliumsdk.unity;

import static com.chartboost.heliumsdk.unity.HeliumUnityBridge.runTaskOnUiThread;
import android.app.Activity;
import android.graphics.Color;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.RelativeLayout;
import com.chartboost.heliumsdk.ad.HeliumAd;
import com.chartboost.heliumsdk.ad.HeliumBannerAd;
import com.chartboost.heliumsdk.ad.HeliumFullscreenAd;
import com.chartboost.heliumsdk.ad.HeliumRewardedAd;
import com.chartboost.heliumsdk.domain.Keywords;
import com.unity3d.player.UnityPlayer;
import java.util.concurrent.atomic.AtomicBoolean;

public class HeliumUnityAdWrapper {
    private static final String TAG = "HeliumUnityAdWrapper";

    private static final int LEADERBOARD_WIDTH = 728;
    private static final int LEADERBOARD_HEIGHT = 90;
    private static final int MEDIUM_WIDTH = 300;
    private static final int MEDIUM_HEIGHT = 250;
    private static final int STANDARD_WIDTH = 320;
    private static final int STANDARD_HEIGHT = 50;

    // This is the container for the banner.  We need a relative layout to position the banner in one of the 7
    // possible positions.  HeliumBannerAd is just a FrameLayout.
    private RelativeLayout mBannerLayout;

    private final HeliumAd _ad;
    private Activity _activity;

    public static HeliumUnityAdWrapper Wrap(HeliumAd ad){
        return new HeliumUnityAdWrapper(ad);
    }

    private HeliumAd ad() {
        _activity = UnityPlayer.currentActivity;
        if (_ad == null)
            throw new RuntimeException("cannot interact with Helium ad as ad was not created");
        return _ad;
    }

    private boolean startedLoad = false;

    public HeliumUnityAdWrapper(HeliumAd ad) {
        _ad = ad;
    }

    public boolean isBanner() {
        return ad() instanceof HeliumBannerAd;
    }

    public boolean isFullScreen() {
        return ad() instanceof HeliumFullscreenAd;
    }

    public HeliumBannerAd asBanner() {
        return (HeliumBannerAd)ad();
    }

    public HeliumFullscreenAd asFullScreen() {
        return (HeliumFullscreenAd)ad();
    }

    public void load() {
        runTaskOnUiThread(() -> {
            ad().load();
            startedLoad = true;
        });
    }

    @SuppressWarnings("unused")
    public void load(int screenLocation) {
        runTaskOnUiThread(() -> {
            createBannerLayout(screenLocation);
            load();
        });
    }

    @SuppressWarnings("unused")
    public void show() {
        runTaskOnUiThread(() ->  {
            if (isFullScreen())
                asFullScreen().show();
        });
    }

    @SuppressWarnings("unused")
    public boolean setKeyword(final String keyword, final String value) {
        Keywords keywords = ad().getKeywords(); // not null by design
        return keywords.set(keyword, value);
    }

    @SuppressWarnings("unused")
    public String removeKeyword(final String keyword) {
        Keywords keywords = ad().getKeywords();  // not null by design
        return keywords.remove(keyword);
    }

    @SuppressWarnings("unused")
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

    @SuppressWarnings("unused")
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

    @SuppressWarnings("unused")
    public boolean clearLoaded() {
        AtomicBoolean ret = new AtomicBoolean(false);
        runTaskOnUiThread(() -> {
            if (isFullScreen())
                ret.set(asFullScreen().clearLoaded());
            else if (isBanner())
                ret.set(asBanner().clearAd());
        });
        return ret.get();
    }

    @SuppressWarnings("unused")
    public boolean readyToShow() {
        if (!startedLoad)
            return false;
        try {
            if (isFullScreen())
                return asFullScreen().readyToShow();
            else
                return false;
        } catch (Exception ex) {
            Log.w(TAG, "Helium encountered an error calling Ad.readyToShow() - " + ex.getMessage());
            return false;
        }
    }

    @SuppressWarnings("unused")
    public void destroy() {
        runTaskOnUiThread(() -> {
            destroyBannerLayout();
            ad().destroy();
        });
    }

    private void createBannerLayout(int screenLocation) {
        if (!isBanner())
            return;
        if (mBannerLayout != null && mBannerLayout.getChildCount() >= 0)
            mBannerLayout.removeAllViews();

        // Create the banner layout on the given position.
        // Check if there is an already existing banner layout. If so, remove it. Otherwise,
        // create a new one.
        if (mBannerLayout != null && mBannerLayout.getChildCount() >= 0) {
            FrameLayout bannerParent = (FrameLayout) mBannerLayout.getParent();
            if (bannerParent != null) {
                bannerParent.removeView(mBannerLayout);
            }
        } else {
            mBannerLayout = new RelativeLayout(_activity);
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
        switch (screenLocation) {
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

        // Attach the banner layout to the activity.
        float pixels = getDisplayDensity();
        try {
            HeliumBannerAd bannerAd = asBanner();
            switch (bannerAd.getSize() != null ? bannerAd.getSize() : HeliumBannerAd.HeliumBannerSize.STANDARD) {
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

            _activity.addContentView(mBannerLayout,
                    new FrameLayout.LayoutParams(
                            FrameLayout.LayoutParams.MATCH_PARENT,
                            FrameLayout.LayoutParams.MATCH_PARENT));

            // This immediately sets the visibility of this banner. If this doesn't happen
            // here, it is impossible to set the visibility later.
            bannerAd.setVisibility(View.VISIBLE);

            // This affects future visibility of the banner layout. Despite it never being
            // set invisible, not setting this to visible here makes the banner not visible.
            mBannerLayout.setVisibility(View.VISIBLE);
        } catch(Exception ex) {
            Log.w(TAG, "Helium encountered an error calling banner load() - " + ex.getMessage());
        }
    }

    private void destroyBannerLayout(){
        if (isBanner() && mBannerLayout != null) {
            mBannerLayout.removeAllViews();
            mBannerLayout.setVisibility(View.GONE);
        }
    }

    private FrameLayout.LayoutParams getBannerLayoutParams(float pixels, int width, int height) {
        return new FrameLayout.LayoutParams((int)(pixels * width), (int)(pixels * height));
    }

    private float getDisplayDensity() {
        try {
            return _activity.getResources().getDisplayMetrics().density;
        } catch(Exception ex) {
            Log.w(TAG, "Helium encountered an error calling getDisplayDensity() - " + ex.getMessage());
        }
        return DisplayMetrics.DENSITY_DEFAULT;
    }
}

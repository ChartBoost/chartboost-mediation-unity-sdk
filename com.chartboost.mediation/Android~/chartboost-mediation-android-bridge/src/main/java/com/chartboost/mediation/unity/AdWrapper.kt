package com.chartboost.mediation.unity

import android.app.Activity
import android.graphics.Color
import android.util.DisplayMetrics
import android.util.Log
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.widget.RelativeLayout
import com.chartboost.heliumsdk.ad.HeliumAd
import com.chartboost.heliumsdk.ad.HeliumBannerAd
import com.chartboost.heliumsdk.ad.HeliumBannerAd.HeliumBannerSize
import com.chartboost.heliumsdk.ad.HeliumFullscreenAd
import com.chartboost.heliumsdk.ad.HeliumRewardedAd
import com.unity3d.player.UnityPlayer
import org.json.JSONObject

@Deprecated("AdWrapper utilizes deprecated APIs and will be removed in the future.")
class AdWrapper(private val ad: HeliumAd) {
    /**
     * This is the container for the banner.
     * We need a relative layout to position the banner in one of the 7 possible positions.
     * ChartboostMediationBannerAd is just a ViewGroup.
     * */
    private var bannerLayout: RelativeLayout? = null
    private val activity: Activity? = UnityPlayer.currentActivity

    fun load() {
        runTaskOnUiThread {
            ad.load()
        }
    }

    fun load(screenLocation: Int) {
        runTaskOnUiThread {
            createBannerLayout(screenLocation)
            load()
        }
    }

    fun show() {
        runTaskOnUiThread {
            if (ad is HeliumFullscreenAd) {
                ad.show()
            }
        }
    }

    fun setKeyword(keyword: String, value: String): Boolean {
        return ad.keywords.set(keyword, value)
    }

    fun removeKeyword(keyword: String): String {
        return ad.keywords.remove(keyword) ?: ""
    }

    fun setCustomData(customData: String) {
        if (ad is HeliumRewardedAd) {
            ad.customData = customData
        } else {
            Log.d(TAG, "custom data can only be set on a rewarded ad")
        }
    }

    fun getSize(): String? {
        if(ad !is HeliumBannerAd){
            Log.w(TAG, "getSize should only be called on banner ads")
            return null
        }

        val size = ad.getSize();
        val json = JSONObject();
        json.put("name", size?.name);
        json.put("aspectRatio", size?.aspectRatio);
        json.put("width", size?.width);
        json.put("height", size?.height);
        json.put("type", size?.isAdaptive);

        return json.toString();
    }

    fun setHorizontalAlignment(horizontalAlignment: Int) {
        if(ad !is HeliumBannerAd){
            Log.w(TAG, "setHorizontalAlignment should only be called on banner ads")
            return
        }

        val bannerAd:HeliumBannerAd = ad;
        when(horizontalAlignment){
            0 -> bannerAd.foregroundGravity =  Gravity.LEFT
            1 -> bannerAd.foregroundGravity =  Gravity.CENTER_HORIZONTAL
            2 -> bannerAd.foregroundGravity =  Gravity.RIGHT
        }
    }

    fun setVerticalAlignment(verticalAlignment: Int) {
        if(ad !is HeliumBannerAd){
            Log.w(TAG, "setVerticalAlignment should only be called on banner ads")
            return
        }
        val bannerAd:HeliumBannerAd = ad;
        when(verticalAlignment){
            0 -> bannerAd.foregroundGravity =  Gravity.TOP
            1 -> bannerAd.foregroundGravity =  Gravity.CENTER_VERTICAL
            2 -> bannerAd.foregroundGravity =  Gravity.BOTTOM
        }
    }

    fun setBannerVisibility(isVisible: Boolean) {
        runTaskOnUiThread {
            if (ad is HeliumBannerAd && bannerLayout != null) {
                val visibility = if (isVisible) View.VISIBLE else View.INVISIBLE
                bannerLayout?.visibility = visibility
                ad.visibility = visibility
            }
        }
    }

    fun clearLoaded() {
        when(ad) {
            is HeliumFullscreenAd -> { ad.clearLoaded() }
            is HeliumBannerAd -> {
                runTaskOnUiThread { ad.clearAd() }
            }
        }
    }

    fun readyToShow(): Boolean {
        return when (ad) {
            is HeliumFullscreenAd -> try {
                ad.readyToShow()
            } catch (ex: Exception) {
                Log.w(TAG, "Helium encountered an error calling Ad.readyToShow() - ${ex.message}")
                false
            }
            is HeliumBannerAd -> {
                Log.w(TAG, "This should never be called, banners do not have readyToShow")
                false
            }
            else -> false
        }
    }

    fun destroy() {
        runTaskOnUiThread {
            destroyBannerLayout()
            ad.destroy()
        }
    }

    private fun createBannerLayout(screenLocation: Int) {
        if (ad !is HeliumBannerAd) {
            Log.w(TAG, "createBannerLayout should only be called on Banner Ads")
            return
        }

        if (activity == null) {
            Log.w(TAG, "Activity not found")
            return
        }

        var layout = bannerLayout

        // Create the banner layout on the given position.
        // Check if there is an already existing banner layout. If so, remove it. Otherwise,
        // create a new one.

        layout?.let {
            it.removeAllViews()
            val bannerParent = it.parent as ViewGroup
            bannerParent.removeView(it)
        }

        layout = RelativeLayout(activity)
        layout.setBackgroundColor(Color.TRANSPARENT)

        /*
            //     TopLeft = 0,
            //     TopCenter = 1,
            //     TopRight = 2,
            //     Center = 3,
            //     BottomLeft = 4,
            //     BottomCenter = 5,
            //     BottomRight = 6
        */

        val bannerGravityPosition = when (screenLocation) {
            0 -> Gravity.TOP or Gravity.LEFT
            1 -> Gravity.TOP or Gravity.CENTER_HORIZONTAL
            2 -> Gravity.TOP or Gravity.RIGHT
            3 -> Gravity.CENTER_VERTICAL or Gravity.CENTER_HORIZONTAL
            4 -> Gravity.BOTTOM or Gravity.LEFT
            5 -> Gravity.BOTTOM or Gravity.CENTER_HORIZONTAL
            6 -> Gravity.BOTTOM or Gravity.RIGHT
            // Other cases
            else -> Gravity.TOP or Gravity.CENTER_HORIZONTAL
        }

        layout.gravity = bannerGravityPosition

        // Attach the banner layout to the activity.
        val density = displayDensity
        try {
            when (ad.getSize() ?: HeliumBannerSize.STANDARD) {
                HeliumBannerSize.LEADERBOARD -> ad.layoutParams = getBannerLayoutParams(density, LEADERBOARD.first, LEADERBOARD.second)
                HeliumBannerSize.MEDIUM -> ad.layoutParams = getBannerLayoutParams(density, MEDIUM.first, MEDIUM.second)
                HeliumBannerSize.STANDARD -> ad.layoutParams = getBannerLayoutParams(density, STANDARD.first, STANDARD.second)
            }

            // Attach the banner to the banner layout.
            layout.addView(ad)
            activity.addContentView(layout, ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT))

            // This immediately sets the visibility of this banner. If this doesn't happen
            // here, it is impossible to set the visibility later.
            layout.visibility = View.VISIBLE

            // This affects future visibility of the banner layout. Despite it never being
            // set invisible, not setting this to visible here makes the banner not visible.
            layout.visibility = View.VISIBLE
        } catch (ex: Exception) {
            Log.w(TAG, "Helium encountered an error calling banner load() - ${ex.message}")
        }
        bannerLayout = layout
    }

    private fun destroyBannerLayout() {
        bannerLayout?.let {
            it.removeAllViews()
            it.visibility = View.GONE
        }
    }

    private fun getBannerLayoutParams(pixels: Float, width: Int, height: Int): ViewGroup.LayoutParams {
        return ViewGroup.LayoutParams((pixels * width).toInt(), (pixels * height).toInt())
    }

    private val displayDensity: Float
        get() {
            return activity?.resources?.displayMetrics?.density ?: DisplayMetrics.DENSITY_DEFAULT.toFloat()
        }

    companion object {
        private val TAG = AdWrapper::class.java.simpleName
        private val STANDARD = Pair(320, 50)
        private val MEDIUM = Pair(300, 250)
        private val LEADERBOARD = Pair(728, 90)

        @JvmStatic
        fun wrap(ad: HeliumAd): AdWrapper {
            return AdWrapper(ad)
        }

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

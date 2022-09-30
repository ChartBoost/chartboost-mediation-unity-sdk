package com.chartboost.heliumsdk.unity

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

class HeliumUnityAdWrapper(private val ad: HeliumAd) {
    //
    private val activity: Activity? = UnityPlayer.currentActivity

    /**
     * This is the container for the banner.
     * We need a relative layout to position the banner in one of the 7 possible positions.
     * HeliumBannerAd is just a ViewGroup.
     * */
    private var bannerLayout: RelativeLayout? = null
    private var startedLoad = false

    fun load() {
        HeliumUnityBridge.runTaskOnUiThread {
            ad.load()
            startedLoad = true
        }
    }

    fun load(screenLocation: Int) {
        HeliumUnityBridge.runTaskOnUiThread {
            createBannerLayout(screenLocation)
            load()
        }
    }

    fun show() {
        HeliumUnityBridge.runTaskOnUiThread {
            if (ad is HeliumFullscreenAd)
                ad.show()
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
            Log.d(TAG,"custom data can only be set on a rewarded ad")
        }
    }

    fun setBannerVisibility(isVisible: Boolean) {
        HeliumUnityBridge.runTaskOnUiThread {
            if (ad is HeliumBannerAd && bannerLayout != null) {
                val visibility = if (isVisible) View.VISIBLE else View.INVISIBLE
                bannerLayout?.visibility = visibility
                ad.visibility = visibility
            }
        }
    }

    fun clearLoaded(): Boolean {
        if (ad is HeliumFullscreenAd)
            return ad.clearLoaded()
        if (ad is HeliumBannerAd) {
            HeliumUnityBridge.runTaskOnUiThread { ad.clearAd() }
            return true
        }
        return false
    }

    fun readyToShow(): Boolean {
        return startedLoad && try {
            when (ad) {
                is HeliumFullscreenAd -> ad.readyToShow()
                is HeliumBannerAd -> {
                    Log.w(TAG, "This should never be called, banners do not have readyToShow")
                    false
                }
                else -> false
            }
        } catch (ex: Exception) {
            Log.w(TAG, "Helium encountered an error calling Ad.readyToShow() - ${ex.message}")
            false
        }
    }

    fun destroy() {
        HeliumUnityBridge.runTaskOnUiThread {
            destroyBannerLayout()
            ad.destroy()
        }
    }

    private fun createBannerLayout(screenLocation: Int) {
        if (ad !is HeliumBannerAd)
            return

        // Create the banner layout on the given position.
        // Check if there is an already existing banner layout. If so, remove it. Otherwise,
        // create a new one.
        bannerLayout?.let {
            it.removeAllViews()
            val bannerParent = it.parent as ViewGroup
            bannerParent.removeView(it)
        } ?: run {
            bannerLayout = RelativeLayout(activity)
            bannerLayout?.setBackgroundColor(Color.TRANSPARENT)
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
        var bannerGravityPosition = 0
        when (screenLocation) {
            0 -> bannerGravityPosition = Gravity.TOP or Gravity.LEFT
            1 -> bannerGravityPosition = Gravity.TOP or Gravity.CENTER_HORIZONTAL
            2 -> bannerGravityPosition = Gravity.TOP or Gravity.RIGHT
            3 -> bannerGravityPosition = Gravity.CENTER_VERTICAL or Gravity.CENTER_HORIZONTAL
            4 -> bannerGravityPosition = Gravity.BOTTOM or Gravity.LEFT
            5 -> bannerGravityPosition = Gravity.BOTTOM or Gravity.CENTER_HORIZONTAL
            6 -> bannerGravityPosition = Gravity.BOTTOM or Gravity.RIGHT
        }
        bannerLayout?.gravity = bannerGravityPosition

        // Attach the banner layout to the activity.
        val density = displayDensity
        try {
            when (ad.getSize() ?: HeliumBannerSize.STANDARD) {
                HeliumBannerSize.LEADERBOARD -> ad.layoutParams = density?.let { getBannerLayoutParams(it, LEADERBOARD.first, LEADERBOARD.second) }
                HeliumBannerSize.MEDIUM -> ad.layoutParams = density?.let { getBannerLayoutParams(it, MEDIUM.first, MEDIUM.second) }
                HeliumBannerSize.STANDARD -> ad.layoutParams = density?.let { getBannerLayoutParams(it, STANDARD.first, STANDARD.second) }
            }

            // Attach the banner to the banner layout.
            bannerLayout?.addView(ad)
            activity?.addContentView(bannerLayout, ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT))

            // This immediately sets the visibility of this banner. If this doesn't happen
            // here, it is impossible to set the visibility later.
            ad.visibility = View.VISIBLE

            // This affects future visibility of the banner layout. Despite it never being
            // set invisible, not setting this to visible here makes the banner not visible.
            bannerLayout?.visibility = View.VISIBLE
        } catch (ex: Exception) {
            Log.w(TAG, "Helium encountered an error calling banner load() - ${ex.message}")
        }
    }

    private fun destroyBannerLayout() {
        if (ad is HeliumBannerAd && bannerLayout != null) {
            bannerLayout?.removeAllViews()
            bannerLayout?.visibility = View.GONE
        }
    }

    private fun getBannerLayoutParams(pixels: Float, width: Int, height: Int): ViewGroup.LayoutParams {
        return ViewGroup.LayoutParams((pixels * width).toInt(), (pixels * height).toInt())
    }

    private val displayDensity: Float?
        get() {
            return try {
                activity?.resources?.displayMetrics?.density
            } catch (ex: Exception) {
                Log.w(TAG, "Helium encountered an error calling getDisplayDensity() - ${ex.message}")
                DisplayMetrics.DENSITY_DEFAULT.toFloat()
            }
        }

    companion object {
        private const val TAG = "HeliumUnityAdWrapper"
        private val STANDARD = Pair(320, 50)
        private val MEDIUM = Pair(300, 250)
        private val LEADERBOARD = Pair(728, 90)

        @JvmStatic
        fun wrap(ad: HeliumAd): HeliumUnityAdWrapper {
            return HeliumUnityAdWrapper(ad)
        }
    }
}

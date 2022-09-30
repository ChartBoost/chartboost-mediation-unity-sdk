package com.chartboost.heliumsdk.unity

import android.app.Activity
import android.graphics.Color
import android.util.DisplayMetrics
import android.util.Log
import android.view.Gravity
import android.view.View
import android.widget.FrameLayout
import android.widget.RelativeLayout
import com.chartboost.heliumsdk.ad.HeliumAd
import com.chartboost.heliumsdk.ad.HeliumBannerAd
import com.chartboost.heliumsdk.ad.HeliumBannerAd.HeliumBannerSize
import com.chartboost.heliumsdk.ad.HeliumFullscreenAd
import com.chartboost.heliumsdk.ad.HeliumRewardedAd
import com.unity3d.player.UnityPlayer

class HeliumUnityAdWrapper(private val ad: HeliumAd) {
    // This is the container for the banner.  We need a relative layout to position the banner in one of the 7
    // possible positions.  HeliumBannerAd is just a FrameLayout.
    private val activity: Activity = UnityPlayer.currentActivity
    private var bannerLayout: RelativeLayout? = null
    private var startedLoad = false

    private fun asBanner(): HeliumBannerAd? {
        return ad as HeliumBannerAd?
    }

    private fun asFullScreen(): HeliumFullscreenAd? {
        return ad as HeliumFullscreenAd?
    }

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
                asFullScreen()?.show()
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
                asBanner()?.visibility = visibility
            }
        }
    }

    fun clearLoaded(): Boolean {
        if (ad is HeliumFullscreenAd)
            return asFullScreen()?.clearLoaded() ?: false
        if (ad is HeliumBannerAd) {
            HeliumUnityBridge.runTaskOnUiThread { asBanner()?.clearAd() }
            return true
        }
        return false
    }

    fun readyToShow(): Boolean {
        return if (!startedLoad) false else try {
            when (ad) {
                is HeliumFullscreenAd -> asFullScreen()?.readyToShow() ?: false
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

        bannerLayout?.let {
            it.removeAllViews()
        }

        // Create the banner layout on the given position.
        // Check if there is an already existing banner layout. If so, remove it. Otherwise,
        // create a new one.
        bannerLayout?.let {
            val bannerParent = it.parent as FrameLayout
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
        val pixels = displayDensity
        try {
            val bannerAd = asBanner()
            when (if (bannerAd?.getSize() != null) bannerAd.getSize() else HeliumBannerSize.STANDARD) {
                HeliumBannerSize.LEADERBOARD -> bannerAd?.layoutParams = getBannerLayoutParams(pixels, LEADERBOARD_WIDTH, LEADERBOARD_HEIGHT)
                HeliumBannerSize.MEDIUM -> bannerAd?.layoutParams = getBannerLayoutParams(pixels, MEDIUM_WIDTH, MEDIUM_HEIGHT)
                HeliumBannerSize.STANDARD -> bannerAd?.layoutParams = getBannerLayoutParams(pixels, STANDARD_WIDTH, STANDARD_HEIGHT)
                null -> TODO()
            }

            // Attach the banner to the banner layout.
            bannerLayout?.addView(bannerAd)
            activity.addContentView(
                bannerLayout,
                FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MATCH_PARENT,
                    FrameLayout.LayoutParams.MATCH_PARENT
                )
            )

            // This immediately sets the visibility of this banner. If this doesn't happen
            // here, it is impossible to set the visibility later.
            bannerAd?.visibility = View.VISIBLE

            // This affects future visibility of the banner layout. Despite it never being
            // set invisible, not setting this to visible here makes the banner not visible.
            bannerLayout?.visibility = View.VISIBLE
        } catch (ex: Exception) {
            Log.w(TAG, "Helium encountered an error calling banner load() - " + ex.message)
        }
    }

    private fun destroyBannerLayout() {
        if (ad is HeliumBannerAd && bannerLayout != null) {
            bannerLayout?.removeAllViews()
            bannerLayout?.visibility = View.GONE
        }
    }

    private fun getBannerLayoutParams(
        pixels: Float,
        width: Int,
        height: Int
    ): FrameLayout.LayoutParams {
        return FrameLayout.LayoutParams((pixels * width).toInt(), (pixels * height).toInt())
    }

    private val displayDensity: Float
        get() {
            try {
                return activity.resources.displayMetrics.density
            } catch (ex: Exception) {
                Log.w(TAG, "Helium encountered an error calling getDisplayDensity() - ${ex.message}")
            }
            return DisplayMetrics.DENSITY_DEFAULT.toFloat()
        }

    companion object {
        private const val TAG = "HeliumUnityAdWrapper"
        private const val LEADERBOARD_WIDTH = 728
        private const val LEADERBOARD_HEIGHT = 90
        private const val MEDIUM_WIDTH = 300
        private const val MEDIUM_HEIGHT = 250
        private const val STANDARD_WIDTH = 320
        private const val STANDARD_HEIGHT = 50

        @JvmStatic
        fun wrap(ad: HeliumAd): HeliumUnityAdWrapper {
            return HeliumUnityAdWrapper(ad)
        }
    }
}

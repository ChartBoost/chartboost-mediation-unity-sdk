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

class HeliumUnityAdWrapper(private val _ad: HeliumAd?) {
    // This is the container for the banner.  We need a relative layout to position the banner in one of the 7
    // possible positions.  HeliumBannerAd is just a FrameLayout.
    private var mBannerLayout: RelativeLayout? = null
    private val _activity: Activity = UnityPlayer.currentActivity
    private val isBanner: Boolean = _ad is HeliumBannerAd
    private val isFullScreen: Boolean = _ad is HeliumFullscreenAd
    private var _asBanner: HeliumBannerAd? = null
    private var _asFullScreen: HeliumFullscreenAd? = null
    private var startedLoad = false

    private fun ad(): HeliumAd {
        if (_ad == null) throw RuntimeException("$TAG cannot interact with Helium ad as ad was not created")
        return _ad
    }

    init {
        if (isBanner) _asBanner = _ad as HeliumBannerAd?
        if (isFullScreen) _asFullScreen = _ad as HeliumFullscreenAd?
    }

    private fun asBanner(): HeliumBannerAd? {
        return _asBanner
    }

    private fun asFullScreen(): HeliumFullscreenAd? {
        return _asFullScreen
    }

    fun load() {
        HeliumUnityBridge.runTaskOnUiThread {
            ad().load()
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
        HeliumUnityBridge.runTaskOnUiThread { if (isFullScreen) asFullScreen()!!.show() }
    }

    fun setKeyword(keyword: String?, value: String?): Boolean {
        val keywords = ad().keywords // not null by design
        return keywords.set(keyword!!, value!!)
    }

    fun removeKeyword(keyword: String?): String? {
        val keywords = ad().keywords // not null by design
        return keywords.remove(keyword!!)
    }

    fun setCustomData(customData: String?) {
        val ad = ad()
        if (ad is HeliumRewardedAd) {
            ad.customData = customData
        } else {
            throw RuntimeException("$TAG custom data can only be set on a rewarded ad")
        }
    }

    fun setBannerVisibility(isVisible: Boolean) {
        HeliumUnityBridge.runTaskOnUiThread {
            if (isBanner && mBannerLayout != null) {
                val visibility = if (isVisible) View.VISIBLE else View.INVISIBLE
                mBannerLayout!!.visibility = visibility
                asBanner()!!.visibility = visibility
            }
        }
    }

    fun clearLoaded(): Boolean {
        if (isFullScreen) return asFullScreen()!!.clearLoaded()
        if (isBanner) {
            HeliumUnityBridge.runTaskOnUiThread { asBanner()!!.clearAd() }
            return true
        }
        return false
    }

    fun readyToShow(): Boolean {
        return if (!startedLoad) false else try {
            if (isFullScreen) asFullScreen()!!.readyToShow() else if (isBanner) {
                Log.w(TAG, "This should never be called, banners do not have readyToShow")
                false
            } else false
        } catch (ex: Exception) {
            Log.w(TAG, "Helium encountered an error calling Ad.readyToShow() - " + ex.message)
            false
        }
    }

    fun destroy() {
        HeliumUnityBridge.runTaskOnUiThread {
            destroyBannerLayout()
            ad().destroy()
        }
    }

    private fun createBannerLayout(screenLocation: Int) {
        if (!isBanner) return
        if (mBannerLayout != null && mBannerLayout!!.childCount >= 0) mBannerLayout!!.removeAllViews()

        // Create the banner layout on the given position.
        // Check if there is an already existing banner layout. If so, remove it. Otherwise,
        // create a new one.
        if (mBannerLayout != null && mBannerLayout!!.childCount >= 0) {
            val bannerParent = mBannerLayout!!.parent as FrameLayout
            bannerParent.removeView(mBannerLayout)
        } else {
            mBannerLayout = RelativeLayout(_activity)
            mBannerLayout!!.setBackgroundColor(Color.TRANSPARENT)
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
        mBannerLayout!!.gravity = bannerGravityPosition

        // Attach the banner layout to the activity.
        val pixels = displayDensity
        try {
            val bannerAd = asBanner()
            when (if (bannerAd!!.getSize() != null) bannerAd.getSize() else HeliumBannerSize.STANDARD) {
                HeliumBannerSize.LEADERBOARD -> bannerAd.layoutParams =
                    getBannerLayoutParams(pixels, LEADERBOARD_WIDTH, LEADERBOARD_HEIGHT)
                HeliumBannerSize.MEDIUM -> bannerAd.layoutParams =
                    getBannerLayoutParams(pixels, MEDIUM_WIDTH, MEDIUM_HEIGHT)
                HeliumBannerSize.STANDARD -> bannerAd.layoutParams =
                    getBannerLayoutParams(pixels, STANDARD_WIDTH, STANDARD_HEIGHT)
                null -> TODO()
            }

            // Attach the banner to the banner layout.
            mBannerLayout!!.addView(bannerAd)
            _activity.addContentView(
                mBannerLayout,
                FrameLayout.LayoutParams(
                    FrameLayout.LayoutParams.MATCH_PARENT,
                    FrameLayout.LayoutParams.MATCH_PARENT
                )
            )

            // This immediately sets the visibility of this banner. If this doesn't happen
            // here, it is impossible to set the visibility later.
            bannerAd.visibility = View.VISIBLE

            // This affects future visibility of the banner layout. Despite it never being
            // set invisible, not setting this to visible here makes the banner not visible.
            mBannerLayout!!.visibility = View.VISIBLE
        } catch (ex: Exception) {
            Log.w(TAG, "Helium encountered an error calling banner load() - " + ex.message)
        }
    }

    private fun destroyBannerLayout() {
        if (isBanner && mBannerLayout != null) {
            mBannerLayout!!.removeAllViews()
            mBannerLayout!!.visibility = View.GONE
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
                return _activity.resources.displayMetrics.density
            } catch (ex: Exception) {
                Log.w(
                    TAG,
                    "Helium encountered an error calling getDisplayDensity() - " + ex.message
                )
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
        fun wrap(ad: HeliumAd?): HeliumUnityAdWrapper {
            return HeliumUnityAdWrapper(ad)
        }
    }
}

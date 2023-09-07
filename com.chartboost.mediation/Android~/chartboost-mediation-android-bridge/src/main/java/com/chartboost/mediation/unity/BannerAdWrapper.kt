package com.chartboost.mediation.unity

import android.app.Activity
import android.graphics.Color
import android.util.DisplayMetrics
import android.util.Log
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.widget.RelativeLayout
import com.chartboost.heliumsdk.ad.HeliumBannerAd
import com.chartboost.heliumsdk.ad.HeliumBannerAdListener
import com.chartboost.heliumsdk.domain.ChartboostMediationAdException
import com.chartboost.heliumsdk.domain.Keywords
import com.unity3d.player.UnityPlayer
import org.json.JSONObject
import kotlin.math.roundToInt

class BannerAdWrapper(private val ad:HeliumBannerAd) {

    var winningBidInfo:Map<String, String>? = null
    var loadId:String = ""
    private var bannerLayout: RelativeLayout? = null
    private val activity: Activity? = UnityPlayer.currentActivity

    fun setListener(bannerViewListener: ChartboostMediationBannerViewListener){
        ad.heliumBannerAdListener = object : HeliumBannerAdListener {
            override fun onAdCached(
                placementName: String,
                loadId: String,
                winningBidInfo: Map<String, String>,
                error: ChartboostMediationAdException?
            ) {
                this@BannerAdWrapper.loadId = loadId;
                this@BannerAdWrapper.winningBidInfo = winningBidInfo;

                error?.let { err ->
                    bannerViewListener.onAdCached(this@BannerAdWrapper, err.message)
                } ?: run {
                    bannerViewListener.onAdCached(this@BannerAdWrapper, "");
                }
            }

            override fun onAdViewAdded(placementName: String) {
                bannerViewListener.onAdViewAdded(this@BannerAdWrapper);
            }

            override fun onAdClicked(placementName: String) {
                bannerViewListener.onAdClicked(this@BannerAdWrapper);
            }

            override fun onAdImpressionRecorded(placementName: String) {
                bannerViewListener.onAdImpressionRecorded(this@BannerAdWrapper);
            }
        }
    }

    fun load(placementName:String, sizeName: String, sizeWidth:Float, sizeHeight:Float, screenLocation: Int){
        runTaskOnUiThread {
            var size = HeliumBannerAd.HeliumBannerSize.STANDARD // default
            when (sizeName) {
                "ADAPTIVE" -> size = HeliumBannerAd.HeliumBannerSize.bannerSize(
                    sizeWidth.roundToInt(),
                    sizeHeight.roundToInt()
                )

                "STANDARD" -> size = HeliumBannerAd.HeliumBannerSize.STANDARD
                "MEDIUM" -> size = HeliumBannerAd.HeliumBannerSize.MEDIUM
                "LEADERBOARD" -> size = HeliumBannerAd.HeliumBannerSize.LEADERBOARD
            }

            createBannerLayout(size, screenLocation);
            //TODO: createBannerLayout(size, x,y); => for gameobject banner

            ad.load(placementName, size);
        }
    }

    fun setKeywords(keywords: Keywords) {
        for (kvp in keywords.get()){
            ad.keywords[kvp.key] = kvp.value
        }
    }
    
    fun setHorizontalAlignment(horizontalAlignment:Int) {
        runTaskOnUiThread {
            ad.foregroundGravity = when (horizontalAlignment) {
                0 -> Gravity.LEFT
                1 -> Gravity.CENTER_HORIZONTAL
                2 -> Gravity.RIGHT
                else -> Gravity.CENTER_HORIZONTAL
            }
        }
    }

    fun getHorizontalAlignment():Int {
        return when (ad.foregroundGravity) {
            Gravity.LEFT -> 0
            Gravity.CENTER_HORIZONTAL -> 1
            Gravity.RIGHT -> 2
            else -> 1
        }
    }

    fun setVerticalAlignment(verticalAlignment:Int) {
        runTaskOnUiThread {
            ad.foregroundGravity = when (verticalAlignment) {
                0 -> Gravity.TOP
                1 -> Gravity.CENTER_VERTICAL
                2 -> Gravity.BOTTOM
                else -> Gravity.CENTER_VERTICAL
            }
        }
    }

    fun getVerticalAlignment():Int {
        return when (ad.foregroundGravity) {
            Gravity.TOP -> 0
            Gravity.CENTER_VERTICAL -> 1
            Gravity.BOTTOM -> 2
            else -> 1
        }
    }

    fun getAdSize(): String? {
        val size = ad.getSize();
        val creativeSize = ad.getCreativeSizeDips();
        
        val json = JSONObject();
        json.put("name", size?.name);
        json.put("aspectRatio", size?.aspectRatio);
        json.put("width", creativeSize.width);
        json.put("height", creativeSize.height);
        json.put("type", size?.isAdaptive);

        return json.toString();
    }

    fun reset() {
        runTaskOnUiThread { ad.clearAd() }
    }

    private fun createBannerLayout(size: HeliumBannerAd.HeliumBannerSize, screenLocation: Int) {
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
            ad.layoutParams = getBannerLayoutParams(displayDensity, size.width, size.height);

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

    private fun getBannerLayoutParams(pixels: Float, width: Int, height: Int): ViewGroup.LayoutParams {
        return ViewGroup.LayoutParams((pixels * width).toInt(), (pixels * height).toInt())
    }

    private val displayDensity: Float
        get() {
            return activity?.resources?.displayMetrics?.density ?: DisplayMetrics.DENSITY_DEFAULT.toFloat()
        }

    companion object {
        private val TAG = BannerAdWrapper::class.java.simpleName
        private val STANDARD = Pair(320, 50)
        private val MEDIUM = Pair(300, 250)
        private val LEADERBOARD = Pair(728, 90)

        @JvmStatic
        fun wrap(ad: HeliumBannerAd): BannerAdWrapper {
            return BannerAdWrapper(ad)
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
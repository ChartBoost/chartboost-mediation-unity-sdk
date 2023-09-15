package com.chartboost.mediation.unity

import android.app.Activity
import android.graphics.Color
import android.graphics.PointF
import android.os.Build
import android.util.DisplayMetrics
import android.util.Log
import android.util.Size
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
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
    private var bannerLayout: BannerLayout? = null
    private var bannerViewListener:ChartboostMediationBannerViewListener? = null;

    private var usesGravity = false;
    private var bannerRequestContainer:BannerRequestContainer? = null;
    private val activity: Activity? = UnityPlayer.currentActivity

    fun setListener(bannerViewListener: ChartboostMediationBannerViewListener){
        this.bannerViewListener = bannerViewListener;
        val thisWrapper = this@BannerAdWrapper;
        val thisListener = this@BannerAdWrapper.bannerViewListener;
        ad.heliumBannerAdListener = object : HeliumBannerAdListener {
            override fun onAdCached(
                placementName: String,
                loadId: String,
                winningBidInfo: Map<String, String>,
                error: ChartboostMediationAdException?
            ) {
                thisWrapper.loadId = loadId;
                thisWrapper.winningBidInfo = winningBidInfo;
                error?.let { err ->
                    thisListener?.onAdCached(thisWrapper, err.message)
                } ?: run {
                    thisListener?.onAdCached(thisWrapper, "");
                }
            }

            override fun onAdViewAdded(placementName: String) {
                thisListener?.onAdViewAdded(thisWrapper);
            }

            override fun onAdClicked(placementName: String) {
                thisWrapper.bannerViewListener?.onAdClicked(thisWrapper);
            }

            override fun onAdImpressionRecorded(placementName: String) {
                thisListener?.onAdImpressionRecorded(thisWrapper);
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
            ad.load(placementName, size);
        }
    }

    fun load(placementName:String, sizeName: String, sizeWidth:Float, sizeHeight:Float, x:Float, y:Float){
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
            createBannerLayout(size, x,y);
            Log.d("Unity", "Before load => placement name : ${ad.placementName}, size : ${ad.getSize()?.name}")
            Log.d("Unity", "Updating placement to $placementName, size : ${size.name}")
            ad.load(placementName, size);
            Log.d("Unity", "After load => placement name : ${ad.placementName}, size : ${ad.getSize()?.name}")
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

    fun resizeToFit(axis:Int, pivotX:Float, pivotY:Float){
        runTaskOnUiThread {

            val width = displayDensity * ad.getCreativeSizeDips().width
            val height = displayDensity * ad.getCreativeSizeDips().height
            val newSize = Size(width.roundToInt(), height.roundToInt())

            // if container is positioned based on gravity then pivot and gravity are pretty much the same
            // so we don't make any adjustments in container's position
            if(usesGravity) {
                when (axis) {
                    0 -> ad.layoutParams =
                        ViewGroup.LayoutParams(newSize.width, ad.layoutParams.height)

                    1 -> ad.layoutParams =
                        ViewGroup.LayoutParams(ad.layoutParams.width, newSize.height)

                    else -> ad.layoutParams = ViewGroup.LayoutParams(newSize.width, newSize.height)
                }
            }
            // if container is not positioned based on gravity then we have to manually position it
            // by moving it around its pivot
            else{
                val containerSize = Size(ad.layoutParams.width, ad.layoutParams.height);
                val containerPivot = PointF(ad.x + (containerSize.width * pivotX), ad.y + (containerSize.height * pivotY));
                Log.d("Unity","Container pivot : (${containerPivot.x},${containerPivot.y})" )

                // Find top-left corner of newSize w.r.t pivot
                val left =  pivotX * newSize.width
                val top = pivotY * newSize.height
                Log.d("Unity", "left : $left, top : $top")

                // Resize and move container to top-left of new size
                val topLeft= PointF(containerPivot.x - left, containerPivot.y - top)
                Log.d("Unity", "top-left : $topLeft")

                when(axis){
                    0 -> {
                        ad.layoutParams = ViewGroup.LayoutParams(newSize.width, ad.layoutParams.height)
                        ad.x = topLeft.x
                    }
                    1 -> {
                        ad.layoutParams = ViewGroup.LayoutParams(ad.layoutParams.width, newSize.height)
                        ad.y = topLeft.y
                    }
                    else -> {
                        ad.layoutParams = ViewGroup.LayoutParams(newSize.width, newSize.height)
                        ad.x = topLeft.x
                        ad.y = topLeft.y
                    }
                }
            }
        }
    }

    fun setDraggability(canDrag:Boolean) {
        runTaskOnUiThread{
            bannerLayout?.canDrag = canDrag
        }
    }

    fun setVisibility(isVisible: Boolean) {
        runTaskOnUiThread {
            if (bannerLayout != null) {
                val visibility = if (isVisible) View.VISIBLE else View.INVISIBLE
                bannerLayout?.visibility = visibility
                ad.visibility = visibility
            }
        }
    }

    fun reset() {
        runTaskOnUiThread { ad.clearAd() }
    }

    fun destroy() {
        runTaskOnUiThread {
            destroyBannerLayout()
            ad.destroy()
        }
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

        layout = BannerLayout(activity, ad, object : IBannerDragListener {
            override fun onDrag(x: Float, y: Float) {
                bannerViewListener?.onAdDrag(this@BannerAdWrapper, x,y)
            }
        } )
        
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
        usesGravity = true;

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

    private fun createBannerLayout(size:HeliumBannerAd.HeliumBannerSize, x:Float, y:Float){
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

        layout = BannerLayout(activity, ad, object : IBannerDragListener {
            override fun onDrag(x: Float, y: Float) {
                bannerViewListener?.onAdDrag(this@BannerAdWrapper, x,y)
            }
        } )
        usesGravity = false;

        // Attach the banner layout to the activity.
        val density = displayDensity
        try {
            ad.layoutParams = getBannerLayoutParams(displayDensity, size.width, size.height);
            ad.x = displayDensity * x
            ad.y = displayDensity * y
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
                ad.setBackgroundColor(Color.argb(0.3f,0f,0f, 1f))
            };
            // Attach the banner to the banner layout.
            layout.addView(ad)
            activity.addContentView(layout, ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT))

            // This immediately sets the visibility of this banner. If this doesn't happen
            // here, it is impossible to set the visibility later.
            layout.visibility = View.VISIBLE

            // This affects future visibility of the banner layout. Despite it never being
            // set invisible, not setting this to visible here makes the banner not visible.
            layout.visibility = View.VISIBLE

            // create bannerRequestContainer to be used later for resizing
            bannerRequestContainer = BannerRequestContainer(displayDensity*x, displayDensity*y, ad)
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
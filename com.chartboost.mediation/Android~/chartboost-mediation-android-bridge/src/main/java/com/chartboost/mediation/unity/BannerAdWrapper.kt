package com.chartboost.mediation.unity

import android.app.Activity
import android.graphics.Color
import android.graphics.PointF
import android.os.Build
import android.util.DisplayMetrics
import android.util.Log
import android.util.Size
import android.view.DisplayCutout
import android.view.Gravity
import android.view.View
import android.view.View.OnLayoutChangeListener
import android.view.ViewGroup
import android.widget.FrameLayout
import android.widget.RelativeLayout
import com.chartboost.heliumsdk.ad.HeliumBannerAd
import com.chartboost.heliumsdk.ad.HeliumBannerAdListener
import com.chartboost.heliumsdk.domain.ChartboostMediationAdException
import com.chartboost.heliumsdk.domain.Keywords
import com.unity3d.player.UnityPlayer
import org.json.JSONObject
import kotlin.math.roundToInt

class BannerAdWrapper(private val ad: HeliumBannerAd) {

    var winningBidInfo: Map<String, String>? = null
    var loadId: String = ""
    var horizontalGravity = Gravity.CENTER_HORIZONTAL
    var verticalGravity = Gravity.CENTER_VERTICAL

    private var usesGravity = false
    private var partnerAd: View? = null
    private var bannerLayout: BannerLayout? = null
    private var bannerViewListener: ChartboostMediationBannerViewListener? = null
    private val activity: Activity? = UnityPlayer.currentActivity

    fun setListener(bannerViewListener: ChartboostMediationBannerViewListener) {
        this.bannerViewListener = bannerViewListener
        val thisWrapper = this@BannerAdWrapper
        val thisListener = this@BannerAdWrapper.bannerViewListener
        ad.heliumBannerAdListener = object : HeliumBannerAdListener {
            override fun onAdCached(
                placementName: String,
                loadId: String,
                winningBidInfo: Map<String, String>,
                error: ChartboostMediationAdException?
            ) {
                thisWrapper.loadId = loadId
                thisWrapper.winningBidInfo = winningBidInfo

                error?.let { err ->
                    thisListener?.onAdCached(thisWrapper, err.message)
                } ?: run {
                    thisListener?.onAdCached(thisWrapper, "")
                }
            }

            override fun onAdViewAdded(placementName: String, child: View?) {
                thisWrapper.partnerAd = child

                // Wait till partnerAd is lay out
                child?.addOnLayoutChangeListener(object : OnLayoutChangeListener {
                    override fun onLayoutChange(
                        p0: View?,
                        p1: Int,
                        p2: Int,
                        p3: Int,
                        p4: Int,
                        p5: Int,
                        p6: Int,
                        p7: Int,
                        p8: Int
                    ) {
                        // FrameLayout cannot set gravity for its children, each child has to
                        // set its own gravity.
                        runTaskOnUiThread {
                            partnerAd?.let {
                                val horizontalGravity = this@BannerAdWrapper.horizontalGravity
                                val verticalGravity = this@BannerAdWrapper.verticalGravity
                                val layoutParams = it.layoutParams as FrameLayout.LayoutParams
                                layoutParams.gravity = horizontalGravity or verticalGravity
                                partnerAd?.layoutParams = layoutParams
                            }
                        }

                        thisListener?.onAdViewAdded(thisWrapper)
                        child.removeOnLayoutChangeListener(this)
                    }
                })
            }

            override fun onAdClicked(placementName: String) {
                thisListener?.onAdClicked(thisWrapper)
            }

            override fun onAdImpressionRecorded(placementName: String) {
                thisListener?.onAdImpressionRecorded(thisWrapper)
            }
        }
    }

    fun load(
        placementName: String,
        sizeType: Int,
        sizeWidth: Float,
        sizeHeight: Float,
        screenLocation: Int
    ) {
        runTaskOnUiThread {
            val size = getSizeFromSizeType(sizeType, sizeWidth, sizeHeight)
            createBannerLayout(size, screenLocation)
            ad.load(placementName, size)
        }
    }

    fun load(
        placementName: String,
        sizeType: Int,
        sizeWidth: Float,
        sizeHeight: Float,
        x: Float,
        y: Float
    ) {
        runTaskOnUiThread {
            val size = getSizeFromSizeType(sizeType, sizeWidth, sizeHeight)
            createBannerLayout(size, x, y)
            ad.load(placementName, size)
        }
    }

    fun setKeywords(keywords: Keywords) {
        for (kvp in keywords.get()) {
            ad.keywords[kvp.key] = kvp.value
        }
    }

    fun setHorizontalAlignment(horizontalAlignment: Int) {
        runTaskOnUiThread {
            this.horizontalGravity = when (horizontalAlignment) {
                0 -> Gravity.LEFT
                1 -> Gravity.CENTER_HORIZONTAL
                2 -> Gravity.RIGHT
                else -> Gravity.CENTER_HORIZONTAL
            }
            Log.d(TAG, "Setting horizontal alignment as ${this.horizontalGravity}")

            partnerAd?.let {
                val layoutParams = it.layoutParams as FrameLayout.LayoutParams

                // apply both since we don't want to overwrite previously set verticalAlignment by
                // only setting horizontalAlignment
                layoutParams.gravity = this.horizontalGravity or this.verticalGravity
                partnerAd?.layoutParams = layoutParams
            }
        }
    }

    fun getHorizontalAlignment(): Int {
        return when (horizontalGravity) {
            Gravity.LEFT -> 0
            Gravity.CENTER_HORIZONTAL -> 1
            Gravity.RIGHT -> 2
            else -> 1
        }
    }

    fun setVerticalAlignment(verticalAlignment: Int) {
        runTaskOnUiThread {
            this.verticalGravity = when (verticalAlignment) {
                0 -> Gravity.TOP
                1 -> Gravity.CENTER_VERTICAL
                2 -> Gravity.BOTTOM
                else -> Gravity.CENTER_VERTICAL
            }
            Log.d(TAG, "Setting vertical alignment as ${this.verticalGravity}")

            partnerAd?.let {
                val layoutParams = it.layoutParams as FrameLayout.LayoutParams

                // apply both since we don't want to overwrite previously set horizontalAlignment by
                // only setting verticalAlignment
                layoutParams.gravity = this.horizontalGravity or this.verticalGravity
                partnerAd?.layoutParams = layoutParams
            }
        }
    }

    fun getVerticalAlignment(): Int {
        return when (verticalGravity) {
            Gravity.TOP -> 0
            Gravity.CENTER_VERTICAL -> 1
            Gravity.BOTTOM -> 2
            else -> 1
        }
    }

    fun getAdSize(): String {
        val size = ad.getSize()
        val creativeSize = partnerAd?.let {
            Size((it.width / displayDensity).toInt(), (it.height / displayDensity).toInt())
        }

        // if partnerAd is not available then adSize is unknown
        val sizeType = partnerAd?.let {
            when (size?.name) {
                "STANDARD" -> 0
                "MEDIUM" -> 1
                "LEADERBOARD" -> 2
                "ADAPTIVE" -> 3
                else -> -1
            }
        }?: run { -1 }  // -1 => Unknown

        val json = JSONObject()
        json.put("sizeType", sizeType)
        json.put("aspectRatio", size?.aspectRatio)
        json.put("width", creativeSize?.width ?: 0)
        json.put("height", creativeSize?.height ?: 0)
        json.put("type", size?.isAdaptive)

        return json.toString()
    }

    fun resizeToFit(axis: Int, pivotX: Float, pivotY: Float) {
        runTaskOnUiThread {
            partnerAd?.let {
                val newSize = Size(it.width, it.height)

                // if container is positioned based on gravity then pivot and gravity are pretty much the same
                // so we don't make any adjustments in container's position
                if (usesGravity) {
                    when (axis) {
                        0 -> ad.layoutParams = RelativeLayout.LayoutParams(newSize.width, ad.layoutParams.height)
                        1 -> ad.layoutParams = RelativeLayout.LayoutParams(ad.layoutParams.width, newSize.height)
                        else -> ad.layoutParams = RelativeLayout.LayoutParams(newSize.width, newSize.height)
                    }
                    return@runTaskOnUiThread
                }
                // if container is not positioned based on gravity then we have to manually position it
                // by moving it around its pivot
                val containerSize = Size(ad.layoutParams.width, ad.layoutParams.height)
                val containerPivot = PointF(
                    ad.x + (containerSize.width * pivotX),
                    ad.y + (containerSize.height * pivotY)
                )

                // Find top-left corner of newSize w.r.t pivot
                val left = pivotX * newSize.width
                val top = pivotY * newSize.height

                // Resize and move container to top-left of new size
                val topLeft = PointF(containerPivot.x - left, containerPivot.y - top)

                when (axis) {
                    0 -> {
                        ad.layoutParams = RelativeLayout.LayoutParams(newSize.width, ad.layoutParams.height)
                        ad.x = topLeft.x
                    }

                    1 -> {
                        ad.layoutParams = RelativeLayout.LayoutParams(ad.layoutParams.width, newSize.height)
                        ad.y = topLeft.y
                    }

                    else -> {
                        ad.layoutParams = RelativeLayout.LayoutParams(newSize.width, newSize.height)
                        ad.x = topLeft.x
                        ad.y = topLeft.y
                    }
                }
            }
                ?: run {
                    Log.d(TAG, "Cannot resize. No partner ad available")
                }
        }
    }
    

    fun setDraggability(canDrag: Boolean) {
        runTaskOnUiThread {
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
                bannerViewListener?.onAdDrag(this@BannerAdWrapper, x, y)
            }
        })

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
        usesGravity = true

        keepWithinSafeArea(screenLocation)

        // Attach the banner layout to the activity.
        val density = displayDensity
        try {
            ad.layoutParams = getBannerLayoutParams(displayDensity, size.width, size.height)

            // Attach the banner to the banner layout.
            layout.addView(ad)
            activity.addContentView(layout, ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT))

            // This immediately sets the visibility of this banner. If this doesn't happen
            // here, it is impossible to set the visibility later.
            // This also affects future visibility of the banner layout. Despite it never being
            // set invisible, not setting this to visible here makes the banner not visible.
            layout.visibility = View.VISIBLE

        } catch (ex: Exception) {
            Log.w(TAG, "Helium encountered an error calling banner load() - ${ex.message}")
        }
        bannerLayout = layout
    }

    private fun createBannerLayout(size: HeliumBannerAd.HeliumBannerSize, x: Float, y: Float) {
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
                bannerViewListener?.onAdDrag(this@BannerAdWrapper, x, y)
            }
        })
        layout.setBackgroundColor(Color.TRANSPARENT)
        usesGravity = false

        // Attach the banner layout to the activity.
        val density = displayDensity
        try {
            ad.layoutParams = getBannerLayoutParams(displayDensity, size.width, size.height)
            ad.x = displayDensity * x
            ad.y = displayDensity * y

            // Attach the banner to the banner layout.
            layout.addView(ad)
            activity.addContentView(layout, ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT))

            // This immediately sets the visibility of this banner. If this doesn't happen
            // here, it is impossible to set the visibility later.
            // This also affects future visibility of the banner layout. Despite it never being
            // set invisible, not setting this to visible here makes the banner not visible.
            layout.visibility = View.VISIBLE

        } catch (ex: Exception) {
            Log.w(TAG, "Helium encountered an error calling banner load() - ${ex.message}")
        }
        bannerLayout = layout
    }
    
    private fun getSizeFromSizeType(sizeType: Int, sizeWidth: Float, sizeHeight: Float): HeliumBannerAd.HeliumBannerSize {
        var size:HeliumBannerAd.HeliumBannerSize = HeliumBannerAd.HeliumBannerSize.bannerSize(0,0);
        when (sizeType) {
            0 -> size = HeliumBannerAd.HeliumBannerSize.STANDARD
            1 -> size = HeliumBannerAd.HeliumBannerSize.MEDIUM
            2 -> size = HeliumBannerAd.HeliumBannerSize.LEADERBOARD
            3 -> size = HeliumBannerAd.HeliumBannerSize.bannerSize(
                sizeWidth.roundToInt(),
                sizeHeight.roundToInt()
            )
        }
        return size;
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

    private fun keepWithinSafeArea(screenLocation: Int) {
        ad.setOnApplyWindowInsetsListener { _, windowInsets ->
            run {
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.P) {
                    val displayCutout: DisplayCutout? = windowInsets.displayCutout
                    if (displayCutout != null) {
                        var x = 0
                        var y = 0
                        when(screenLocation) {
                            // Top-left
                            0 -> {
                                x= displayCutout.safeInsetLeft
                                y = displayCutout.safeInsetTop
                            }
                            // Top-center
                            1 -> {
                                y = displayCutout.safeInsetTop
                            }
                            // Top-right
                            2 -> {
                                x= displayCutout.safeInsetRight
                                y = displayCutout.safeInsetTop
                            }
                            // center
                            3 -> {}
                            // bottom-left
                            4 -> {
                                x= displayCutout.safeInsetLeft
                                y = displayCutout.safeInsetBottom
                            }
                            // bottom-center
                            5 -> {
                                y = displayCutout.safeInsetBottom
                            }
                            // bottom-right
                            6 -> {
                                x= displayCutout.safeInsetRight
                                y = displayCutout.safeInsetBottom
                            }
                        }
                        ad.x = x.toFloat()
                        ad.y = y.toFloat()
                    }
                }
                windowInsets
            }
        }
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

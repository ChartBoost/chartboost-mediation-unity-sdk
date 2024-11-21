@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.banner

import android.app.Activity
import android.graphics.Color
import android.graphics.Point
import android.graphics.PointF
import android.os.Build
import android.util.DisplayMetrics
import android.util.Log
import android.util.Size
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.widget.FrameLayout
import android.widget.RelativeLayout
import androidx.annotation.RequiresApi
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdLoadListener
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdLoadRequest
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdView
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdViewListener
import com.chartboost.chartboostmediationsdk.domain.Keywords
import com.chartboost.mediation.unity.logging.LogLevel
import com.chartboost.mediation.unity.logging.UnityLoggingBridge
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers.Main
import kotlinx.coroutines.launch
import org.json.JSONObject
import kotlin.math.roundToInt

class BannerAdWrapper(private val ad: ChartboostMediationBannerAdView) {
    private var _winningBidInfo: Map<String, String>? = null
    private var _loadId: String = ""
    private var _metrics: JSONObject? = null
    private var _position:PointF
    private var _pivot:PointF

    private var horizontalGravity = Gravity.CENTER_HORIZONTAL
    private var verticalGravity = Gravity.CENTER_VERTICAL

    private var partnerAd: View? = null
    private var bannerLayout: BannerLayout? = null
    private var bannerAdListener: ChartboostMediationBannerAdListener? = null
    private val activity: Activity? = UnityPlayer.currentActivity

    val winningBidInfo: Map<String, String>? = _winningBidInfo
    val loadId: String = _loadId
    val metrics: JSONObject? = _metrics

    fun getKeywords(): Keywords {
        return ad.keywords
    }

    fun setKeywords(keywords: Keywords) {
        ad.keywords = keywords;
    }

    fun setPosition(x: Float, y: Float){
        _position = PointF(x,y)
        runTaskOnUiThread {
            ad.translationX = x * displayDensity
            ad.translationY = y * displayDensity
        }
    }

    fun getPosition():PointF{
        return _position
    }

    fun setPivot(x: Float, y: Float){
        _pivot = PointF(x,y)
        updateMargins()
    }

    fun getPivot(): PointF {
        return _pivot
    }

    fun setHorizontalAlignment(horizontalAlignment: Int) {
        runTaskOnUiThread {
            this.horizontalGravity = when (horizontalAlignment) {
                0 -> Gravity.LEFT
                1 -> Gravity.CENTER_HORIZONTAL
                2 -> Gravity.RIGHT
                else -> Gravity.CENTER_HORIZONTAL
            }
            UnityLoggingBridge.log(TAG, "Setting horizontal alignment as ${this.horizontalGravity}", LogLevel.DEBUG)
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
            UnityLoggingBridge.log(TAG, "Setting vertical alignment as ${this.verticalGravity}", LogLevel.DEBUG)
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

    fun getBannerSize(): ChartboostMediationBannerAdView.ChartboostMediationBannerSize {
        val width = ad.getCreativeSizeDips().width
        val height = ad.getCreativeSizeDips().height
        return  when (ad.getSize()?.name) {
            "STANDARD" -> ChartboostMediationBannerAdView.ChartboostMediationBannerSize.STANDARD
            "MEDIUM" -> ChartboostMediationBannerAdView.ChartboostMediationBannerSize.MEDIUM
            "LEADERBOARD" -> ChartboostMediationBannerAdView.ChartboostMediationBannerSize.LEADERBOARD
            "ADAPTIVE" -> ChartboostMediationBannerAdView.ChartboostMediationBannerSize.bannerSize(width, height)
            else -> {
                UnityLoggingBridge.log(TAG, "Size not defined, set to ADAPTIVE(0x0) by default", LogLevel.WARNING)
                ChartboostMediationBannerAdView.ChartboostMediationBannerSize.bannerSize(0,0)
            }
        }
    }

    fun setContainerSize(width: Int, height: Int){
        runTaskOnUiThread {

            // Fit Both
            if(width == -1 && height == -1){
                UnityLoggingBridge.log(TAG, "Setting container size to wrap content", LogLevel.DEBUG)
                ad.layoutParams = RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WRAP_CONTENT,
                    ViewGroup.LayoutParams.WRAP_CONTENT
                )
            }
            // Fit Horizontal

            else if(width == -1){
                UnityLoggingBridge.log(TAG,"Setting container size to wrap horizontal", LogLevel.DEBUG)
                ad.layoutParams = RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WRAP_CONTENT,
                    (height * displayDensity).toInt()
                )
            }

            // Fit Vertical
            else if(height == -1){
                UnityLoggingBridge.log(TAG,"Setting container size to wrap vertical", LogLevel.DEBUG)
                ad.layoutParams = RelativeLayout.LayoutParams(
                    (width * displayDensity).toInt(),
                    ViewGroup.LayoutParams.WRAP_CONTENT
                )
            }

            // Fixed size
            else{
                UnityLoggingBridge.log(TAG, "Setting container size to fixed size ($width, $height)", LogLevel.DEBUG)
                ad.layoutParams = RelativeLayout.LayoutParams(
                    (width * displayDensity).toInt(),
                    (height * displayDensity).toInt()
                )
            }
        }
    }

    fun getContainerSize(): Size = Size((ad.width/displayDensity).toInt(), (ad.height/displayDensity).toInt())

    fun getDraggability(): Boolean {
        return bannerLayout?.canDrag ?: false;
    }

    fun setDraggability(canDrag: Boolean) {
        runTaskOnUiThread {
            bannerLayout?.canDrag = canDrag
        }
    }

    fun getVisibility(): Boolean {
        return ad.visibility == View.VISIBLE;
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

    fun load(placementName: String, sizeType: Int, sizeWidth: Float, sizeHeight: Float, adLoadResultHandler: ChartboostMediationBannerAdLoadListener) {
        CoroutineScope(Main).launch {
            val size = getSizeFromSizeType(sizeType, sizeWidth, sizeHeight)
            val loadRequest = ChartboostMediationBannerAdLoadRequest(placementName, ad.keywords, size)
            val adLoadResult = ad.load(loadRequest)

            // Note: These variables are not required for this class as they are only used during
            // the load request and can be accessed directly in the Unity C# layer. However, since
            // these variables are included in the BannerView on iOS, we maintain the same design
            // here for consistency.
            _loadId = adLoadResult.loadId
            _winningBidInfo = adLoadResult.winningBidInfo
            _metrics = adLoadResult.metrics

            adLoadResultHandler.onAdLoaded(adLoadResult)
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

    fun setListener(bannerViewListener: ChartboostMediationBannerAdListener) {
        bannerAdListener = bannerViewListener
        ad.chartboostMediationBannerAdViewListener = object : ChartboostMediationBannerAdViewListener {
            // called on each refresh
            override fun onAdViewAdded(placement: String, child: View?) {
                bannerAdListener?.onAdViewAdded(this@BannerAdWrapper)
                partnerAd = child
                updateGravity()
            }

            override fun onAdClicked(placement: String) {
                bannerAdListener?.onAdClicked(this@BannerAdWrapper)
            }

            override fun onAdImpressionRecorded(placement: String) {
                bannerAdListener?.onAdImpressionRecorded(this@BannerAdWrapper)
            }
        }
    }

    @RequiresApi(Build.VERSION_CODES.O)
    fun setContainerBackgroundColor(colorArray: FloatArray)
    {
        if (colorArray.size == 4) {
            val r = colorArray[0]
            val g = colorArray[1]
            val b = colorArray[2]
            val a = colorArray[3]

            UnityLoggingBridge.log(TAG, "Setting container background color to - R: $r, G: $g, B: $b, A: $a", LogLevel.DEBUG)
            runTaskOnUiThread { ad.setBackgroundColor(Color.argb(a, r, g, b)) }
        }
        else {
            UnityLoggingBridge.log(TAG, "Invalid Color Array Length", LogLevel.DEBUG)
        }
    }

    @RequiresApi(Build.VERSION_CODES.O)
    fun setAdBackgroundColor(colorArray: FloatArray) {
        if (colorArray.size == 4) {
            val r = colorArray[0]
            val g = colorArray[1]
            val b = colorArray[2]
            val a = colorArray[3]

            UnityLoggingBridge.log(TAG, "Setting ad background color to - R: $r, G: $g, B: $b, A: $a", LogLevel.DEBUG)
            if(partnerAd == null){
                partnerAd = View(activity)
                ad.addView(partnerAd)
            }
            runTaskOnUiThread { partnerAd?.setBackgroundColor(Color.argb(a, r, g, b)) }
        }
        else {
            UnityLoggingBridge.log(TAG, "Invalid Color Array Length", LogLevel.DEBUG)
        }
    }

    fun setAdRelativePosition(x: Float, y: Float){
        runTaskOnUiThread {
            partnerAd?.let {
                it.x = x * displayDensity
                it.y = y * displayDensity
            }
        }
    }

    fun getAdRelativePosition(): PointF {
        partnerAd?.let { return PointF(it.x, it.y) }
        return PointF(0F,0F)
    }

    private fun createBannerLayout() {
        if (activity == null) {
            UnityLoggingBridge.log(TAG, "Activity not found to create banner layout", LogLevel.WARNING)
            return
        }

        runTaskOnUiThread {
            var layout = bannerLayout

            // Create the banner layout on the given position.
            // Check if there is an already existing banner layout. If so, remove it. Otherwise,
            // create a new one.

            layout?.let {
                it.removeAllViews()
                val bannerParent = it.parent as ViewGroup
                bannerParent.removeView(it)
            }

            // Listen to drag events on BannerLayout
            layout = BannerLayout(activity, ad, dragListener)
            layout.setBackgroundColor(Color.TRANSPARENT)

            // Attach the banner layout to the activity.
            val density = displayDensity
            try {

                // Default is wrap content
                ad.layoutParams = RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WRAP_CONTENT,
                    ViewGroup.LayoutParams.WRAP_CONTENT
                )

                // Update layout based on pivot
                ad.viewTreeObserver.addOnGlobalLayoutListener {
                    updateMargins()
                }

                // Attach the banner to the banner layout.
                layout.addView(ad)
                activity.addContentView(
                    layout,
                    ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MATCH_PARENT,
                        ViewGroup.LayoutParams.MATCH_PARENT
                    )
                )

                // This immediately sets the visibility of this banner. If this doesn't happen
                // here, it is impossible to set the visibility later.
                // This also affects future visibility of the banner layout. Despite it never being
                // set invisible, not setting this to visible here makes the banner not visible.
                layout.visibility = View.VISIBLE
            } catch (ex: Exception) {
                UnityLoggingBridge.logException(TAG, "Chartboost Mediation encountered an error while creating banner layout - $ex")
            }
            bannerLayout = layout
        }
    }

    private val dragListener: IBannerDragListener = object : IBannerDragListener {

        override fun onDragBegin(x: Float, y: Float) {
            bannerAdListener?.onAdDragBegin(this@BannerAdWrapper, x, y)
        }

        override fun onDrag(x: Float, y: Float) {
            bannerAdListener?.onAdDrag(this@BannerAdWrapper, x, y)
        }
        override fun onDragEnd(x: Float, y: Float) {
            bannerAdListener?.onAdDragEnd(this@BannerAdWrapper, x, y)
        }
    }

    private fun updateMargins() {
        runTaskOnUiThread {
            // Since there is no concept of pivot in Android, we negate the pivot values
            // and assign them as margins. This adjustment sets the left and top margins
            // to move the view based on the pivot point.
            // Note: We only do this for BannerAd API. For UnityBannerAd the pivot is
            // always assumed to be at (0,0) since positioning and resizing is handled by the gameobject

            val layoutParams = ad.layoutParams as RelativeLayout.LayoutParams
            layoutParams.leftMargin = (ad.width * -(_pivot.x)).toInt()
            layoutParams.topMargin = (ad.height * -(_pivot.y)).toInt()

            ad.layoutParams = layoutParams
        }
    }

    private fun updateGravity(){
        runTaskOnUiThread {
            // FrameLayout cannot set gravity for its children, each child has to
            // set its own gravity.
            partnerAd?.let {
                val layoutParams = it.layoutParams as FrameLayout.LayoutParams
                layoutParams.gravity = horizontalGravity or verticalGravity
                it.layoutParams = layoutParams
            }
        }
    }

    private fun getSizeFromSizeType(sizeType: Int, sizeWidth: Float, sizeHeight: Float): ChartboostMediationBannerAdView.ChartboostMediationBannerSize {
        var size:ChartboostMediationBannerAdView.ChartboostMediationBannerSize = ChartboostMediationBannerAdView.ChartboostMediationBannerSize.bannerSize(0,0)
        when (sizeType) {
            0 -> size = ChartboostMediationBannerAdView.ChartboostMediationBannerSize.STANDARD
            1 -> size = ChartboostMediationBannerAdView.ChartboostMediationBannerSize.MEDIUM
            2 -> size = ChartboostMediationBannerAdView.ChartboostMediationBannerSize.LEADERBOARD
            3 -> size = ChartboostMediationBannerAdView.ChartboostMediationBannerSize.bannerSize(
                sizeWidth.roundToInt(),
                sizeHeight.roundToInt()
            )
        }
        return size
    }

    private fun destroyBannerLayout() {
        bannerLayout?.let {
            it.removeAllViews()
            it.visibility = View.GONE
        }
    }

    private fun getBannerLayoutParams(pixels: Float, width: Int, height: Int): RelativeLayout.LayoutParams {
        return RelativeLayout.LayoutParams((pixels * width).toInt(), (pixels * height).toInt())
    }

    private val displayDensity: Float
        get() {
            return activity?.resources?.displayMetrics?.density ?: DisplayMetrics.DENSITY_DEFAULT.toFloat()
        }

    companion object {
        private val TAG = BannerAdWrapper::class.java.simpleName

        @JvmStatic
        fun wrap(ad: ChartboostMediationBannerAdView): BannerAdWrapper {
            return BannerAdWrapper(ad)
        }

        fun runTaskOnUiThread(runnable: Runnable) {
            UnityPlayer.currentActivity.runOnUiThread {
                try {
                    runnable.run()
                } catch (ex: Exception) {
                    UnityLoggingBridge.logException(TAG, "Exception found when running on UI Thread: $ex")
                }
            }
        }
    }

    init {
        this._position = PointF(0F, 0F)
        this._pivot = PointF(0F, 0F)
        createBannerLayout()
    }

}

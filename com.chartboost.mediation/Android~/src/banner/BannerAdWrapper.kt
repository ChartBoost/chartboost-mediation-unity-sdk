@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.banner

import android.app.Activity
import android.graphics.Color
import android.graphics.PointF
import android.os.Build
import android.util.DisplayMetrics
import android.util.Size
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.view.ViewTreeObserver
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
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.cancel
import kotlinx.coroutines.launch
import org.json.JSONObject
import kotlin.math.roundToInt

class BannerAdWrapper(private val ad: ChartboostMediationBannerAdView) {
    private var _winningBidInfo: Map<String, String>? = null
    private var _loadId: String = ""
    private var _metrics: JSONObject? = null
    private var _position:PointF
    private var _pivot:PointF

    // Thread-safe backing fields for properties that may be accessed from multiple threads
    // These store the logical state immediately on the calling thread, while UI updates
    // are dispatched to the main thread asynchronously
    private var horizontalGravity = Gravity.CENTER_HORIZONTAL
    private var verticalGravity = Gravity.CENTER_VERTICAL
    private var _containerSize: Size = Size(0, 0)
    private var _isVisible: Boolean = true

    private var partnerAd: View? = null
    private var bannerLayout: BannerLayout? = null
    private var bannerAdListener: ChartboostMediationBannerAdListener? = null

    // Coroutine scope tied to wrapper lifecycle
    private val scope = CoroutineScope(Main + SupervisorJob())

    // Store the desired draggable state to apply when bannerLayout is created
    private var _canDrag: Boolean = false

    // Store ViewTreeObserver listener reference for proper cleanup
    private var layoutListener: ViewTreeObserver.OnGlobalLayoutListener? = null

    /**
     * Gets the current Unity activity. Does not retain a reference to avoid memory leaks.
     */
    private fun getActivity(): Activity? = UnityPlayer.currentActivity

    // Wrapper class to match the Keywords pattern for C# JNI bridge compatibility
    class PartnerSettingsWrapper(private val map: MutableMap<String, Any>) {
        @Suppress("unused")
        fun get(): MutableMap<String, Any> {
            return map
        }
    }

    //region Ad Load Result Properties

    /**
     * Gets the winning bid info from the last successful ad load.
     * @return Map of winning bid information, or null if no ad has been loaded
     */
    @Suppress("unused")
    fun getWinningBidInfo(): Map<String, String>? = _winningBidInfo

    /**
     * Gets the load ID from the last successful ad load.
     * @return Load ID string, empty if no ad has been loaded
     */
    @Suppress("unused")
    fun getLoadId(): String = _loadId

    /**
     * Gets the load metrics from the last successful ad load.
     * @return JSON object containing metrics, or null if no ad has been loaded
     */
    @Suppress("unused")
    fun getMetrics(): JSONObject? = _metrics

    //endregion

    //region Keywords and Partner Settings

    /**
     * Gets the current keywords associated with this banner ad.
     */
    @Suppress("unused")
    fun getKeywords(): Keywords {
        return ad.keywords
    }

    /**
     * Sets keywords for targeting this banner ad.
     */
    @Suppress("unused")
    fun setKeywords(keywords: Keywords) {
        ad.keywords = keywords
    }

    /**
     * Gets the current partner settings.
     */
    @Suppress("unused")
    fun getPartnerSettings(): PartnerSettingsWrapper {
        return PartnerSettingsWrapper(ad.partnerSettings)
    }

    /**
     * Sets partner-specific settings for this banner ad.
     */
    @Suppress("unused")
    fun setPartnerSettings(partnerSettings: HashMap<String, Any>) {
        ad.partnerSettings = partnerSettings
    }

    //endregion

    //region Position and Pivot

    /**
     * Sets the banner position in native coordinates.
     * Position represents the absolute screen coordinates where the banner should be placed.
     */
    @Suppress("unused")
    fun setPosition(x: Float, y: Float){
        _position = PointF(x,y)
        runTaskOnUiThread {
            ad.translationX = x * displayDensity
            ad.translationY = y * displayDensity
        }
    }

    /**
     * Gets the current banner position in native coordinates.
     */
    @Suppress("unused")
    fun getPosition():PointF{
        return _position
    }

    /**
     * Sets the banner pivot point (0-1 range for both x and y).
     * Pivot determines which point of the banner aligns with the position.
     * (0,0) = top-left, (0.5,0.5) = center, (1,1) = bottom-right
     */
    @Suppress("unused")
    fun setPivot(x: Float, y: Float){
        _pivot = PointF(x,y)
        updateMargins()
    }

    /**
     * Gets the current banner pivot point.
     */
    @Suppress("unused")
    fun getPivot(): PointF {
        return _pivot
    }

    //endregion

    //region Alignment

    /**
     * Sets the horizontal alignment of the partner ad within the banner container.
     * @param horizontalAlignment 0=Left, 1=Center, 2=Right
     */
    @Suppress("unused")
    fun setHorizontalAlignment(horizontalAlignment: Int) {
        this.horizontalGravity = when (horizontalAlignment) {
            HORIZONTAL_LEFT -> Gravity.LEFT
            HORIZONTAL_CENTER -> Gravity.CENTER_HORIZONTAL
            HORIZONTAL_RIGHT -> Gravity.RIGHT
            else -> Gravity.CENTER_HORIZONTAL
        }
        UnityLoggingBridge.log(TAG, "Setting horizontal alignment as ${this.horizontalGravity}", LogLevel.DEBUG)
        runTaskOnUiThread {
            partnerAd?.let {
                val layoutParams = it.layoutParams as FrameLayout.LayoutParams

                // apply both since we don't want to overwrite previously set verticalAlignment by
                // only setting horizontalAlignment
                layoutParams.gravity = this.horizontalGravity or this.verticalGravity
                partnerAd?.layoutParams = layoutParams
            }
        }
    }

    /**
     * Gets the current horizontal alignment.
     * @return 0=Left, 1=Center, 2=Right
     */
    @Suppress("unused")
    fun getHorizontalAlignment(): Int {
        return when (horizontalGravity) {
            Gravity.LEFT -> HORIZONTAL_LEFT
            Gravity.CENTER_HORIZONTAL -> HORIZONTAL_CENTER
            Gravity.RIGHT -> HORIZONTAL_RIGHT
            else -> HORIZONTAL_CENTER
        }
    }

    /**
     * Sets the vertical alignment of the partner ad within the banner container.
     * @param verticalAlignment 0=Top, 1=Center, 2=Bottom
     */
    @Suppress("unused")
    fun setVerticalAlignment(verticalAlignment: Int) {
        this.verticalGravity = when (verticalAlignment) {
            VERTICAL_TOP -> Gravity.TOP
            VERTICAL_CENTER -> Gravity.CENTER_VERTICAL
            VERTICAL_BOTTOM -> Gravity.BOTTOM
            else -> Gravity.CENTER_VERTICAL
        }
        UnityLoggingBridge.log(TAG, "Setting vertical alignment as ${this.verticalGravity}", LogLevel.DEBUG)
        runTaskOnUiThread {
            partnerAd?.let {
                val layoutParams = it.layoutParams as FrameLayout.LayoutParams

                // apply both since we don't want to overwrite previously set horizontalAlignment by
                // only setting verticalAlignment
                layoutParams.gravity = this.horizontalGravity or this.verticalGravity
                partnerAd?.layoutParams = layoutParams
            }
        }
    }

    /**
     * Gets the current vertical alignment.
     * @return 0=Top, 1=Center, 2=Bottom
     */
    @Suppress("unused")
    fun getVerticalAlignment(): Int {
        return when (verticalGravity) {
            Gravity.TOP -> VERTICAL_TOP
            Gravity.CENTER_VERTICAL -> VERTICAL_CENTER
            Gravity.BOTTOM -> VERTICAL_BOTTOM
            else -> VERTICAL_CENTER
        }
    }

    //endregion

    //region Size and Container

    /**
     * Gets the current banner size.
     * @return The ChartboostMediationBannerSize representing the loaded banner's dimensions
     */
    @Suppress("unused")
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

    /**
     * Sets the container size for the banner ad.
     * @param width Container width in native units. Use -1 for wrap content.
     * @param height Container height in native units. Use -1 for wrap content.
     */
    @Suppress("unused")
    fun setContainerSize(width: Int, height: Int){
        // Store the container size immediately on calling thread
        _containerSize = Size(width, height)
        UnityLoggingBridge.log(TAG, "Setting container size to ${width}x${height}", LogLevel.DEBUG)

        // Only UI manipulation on UI thread
        runTaskOnUiThread {

            // Fit Both
            if(width == FIT_CONTENT && height == FIT_CONTENT){
                ad.layoutParams = RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WRAP_CONTENT,
                    ViewGroup.LayoutParams.WRAP_CONTENT
                )
            }
            // Fit Horizontal

            else if(width == FIT_CONTENT){
                ad.layoutParams = RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WRAP_CONTENT,
                    (height * displayDensity).toInt()
                )
            }

            // Fit Vertical
            else if(height == FIT_CONTENT){
                ad.layoutParams = RelativeLayout.LayoutParams(
                    (width * displayDensity).toInt(),
                    ViewGroup.LayoutParams.WRAP_CONTENT
                )
            }

            // Fixed size
            else{
                ad.layoutParams = RelativeLayout.LayoutParams(
                    (width * displayDensity).toInt(),
                    (height * displayDensity).toInt()
                )
            }
        }
    }

    /**
     * Gets the current container size.
     */
    @Suppress("unused")
    fun getContainerSize(): Size = _containerSize

    //endregion

    //region Visibility and Draggability

    /**
     * Gets whether the banner is currently draggable.
     */
    @Suppress("unused")
    fun getDraggability(): Boolean {
        return _canDrag
    }

    /**
     * Sets whether the banner can be dragged by the user.
     */
    @Suppress("unused")
    fun setDraggability(canDrag: Boolean) {
        _canDrag = canDrag
        bannerLayout?.canDrag = canDrag
    }

    /**
     * Gets whether the banner is currently visible.
     */
    @Suppress("unused")
    fun getVisibility(): Boolean {
        return _isVisible
    }

    /**
     * Sets the visibility of the banner.
     */
    @Suppress("unused")
    fun setVisibility(isVisible: Boolean) {
        // Store the visibility state immediately on calling thread
        _isVisible = isVisible
        UnityLoggingBridge.log(TAG, "Setting visibility to $isVisible", LogLevel.DEBUG)

        // Only UI manipulation on UI thread
        runTaskOnUiThread {
            // Null check inside UI thread to avoid race condition
            val layout = bannerLayout
            if (layout != null) {
                val visibility = if (isVisible) View.VISIBLE else View.INVISIBLE
                layout.visibility = visibility
                ad.visibility = visibility
            }
        }
    }

    //endregion

    //region Ad Lifecycle

    /**
     * Loads a banner ad with the specified parameters.
     * @param placementName The placement name for this banner ad
     * @param sizeType Banner size type (0=Standard, 1=Medium, 2=Leaderboard, 3=Adaptive)
     * @param sizeWidth Width for adaptive banners
     * @param sizeHeight Height for adaptive banners
     * @param adLoadResultHandler Callback to receive load result
     */
    @Suppress("unused")
    fun load(placementName: String, sizeType: Int, sizeWidth: Float, sizeHeight: Float, adLoadResultHandler: ChartboostMediationBannerAdLoadListener) {
        scope.launch {
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

    /**
     * Resets the banner ad, clearing any loaded ad content.
     */
    @Suppress("unused")
    fun reset() {
        runTaskOnUiThread { ad.clearAd() }
    }

    /**
     * Destroys the banner ad and releases all resources.
     * Cancels any ongoing coroutines and cleans up the banner layout.
     */
    @Suppress("unused")
    fun destroy() {
        // Cancel all ongoing coroutines to prevent memory leaks
        scope.cancel()

        runTaskOnUiThread {
            destroyBannerLayout()
            ad.destroy()
        }
    }

    /**
     * Sets the listener for banner ad events.
     */
    @Suppress("unused")
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

    //endregion

    //region Background Colors

    /**
     * Sets the background color of the banner container.
     * Requires Android O (API 26) or higher.
     *
     * @param colorArray Float array of RGBA values [red, green, blue, alpha] in range [0.0, 1.0]
     */
    @Suppress("unused")
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

    /**
     * Sets the background color of the actual ad view within the container.
     * Creates a partner ad view if it doesn't exist.
     * Requires Android O (API 26) or higher.
     *
     * @param colorArray Float array of RGBA values [red, green, blue, alpha] in range [0.0, 1.0]
     */
    @Suppress("unused")
    @RequiresApi(Build.VERSION_CODES.O)
    fun setAdBackgroundColor(colorArray: FloatArray) {
        if (colorArray.size == 4) {
            val r = colorArray[0]
            val g = colorArray[1]
            val b = colorArray[2]
            val a = colorArray[3]

            UnityLoggingBridge.log(TAG, "Setting ad background color to - R: $r, G: $g, B: $b, A: $a", LogLevel.DEBUG)
            runTaskOnUiThread {
                // Only create partnerAd if it doesn't exist
                if (partnerAd == null) {
                    val activity = getActivity()
                    if (activity != null) {
                        partnerAd = View(activity).apply {
                            layoutParams = FrameLayout.LayoutParams(
                                ViewGroup.LayoutParams.MATCH_PARENT,
                                ViewGroup.LayoutParams.MATCH_PARENT
                            )
                        }
                        ad.addView(partnerAd)
                    }
                }
                partnerAd?.setBackgroundColor(Color.argb(a, r, g, b))
            }
        }
        else {
            UnityLoggingBridge.log(TAG, "Invalid Color Array Length: expected 4, got ${colorArray.size}", LogLevel.WARNING)
        }
    }

    //endregion

    //region Partner Ad Positioning

    /**
     * Sets the relative position of the partner ad within the container.
     * Position is specified in density-independent pixels (dp).
     *
     * @param x The X coordinate in dp
     * @param y The Y coordinate in dp
     */
    @Suppress("unused")
    fun setAdRelativePosition(x: Float, y: Float){
        runTaskOnUiThread {
            partnerAd?.let {
                it.x = x * displayDensity
                it.y = y * displayDensity
            }
        }
    }

    /**
     * Gets the relative position of the partner ad within the container.
     *
     * @return PointF containing the X and Y coordinates, or (0, 0) if partner ad doesn't exist
     */
    @Suppress("unused")
    fun getAdRelativePosition(): PointF {
        partnerAd?.let { return PointF(it.x, it.y) }
        return PointF(0F,0F)
    }

    //endregion

    //region Private Utilities

    /**
     * Creates and attaches the banner layout to the Unity activity.
     * Handles cleanup of existing layouts and sets up drag listeners and view observers.
     */
    private fun createBannerLayout() {
        if (getActivity() == null) {
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
            layout = BannerLayout(getActivity(), ad, dragListener)
            layout.setBackgroundColor(Color.TRANSPARENT)

            // Apply the stored draggable state
            layout.canDrag = _canDrag

            // Attach the banner layout to the activity.
            try {

                // Default is wrap content
                ad.layoutParams = RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WRAP_CONTENT,
                    ViewGroup.LayoutParams.WRAP_CONTENT
                )

                // Update layout based on pivot
                layoutListener = ViewTreeObserver.OnGlobalLayoutListener {
                    updateMargins()
                }
                ad.viewTreeObserver.addOnGlobalLayoutListener(layoutListener)

                // Attach the banner to the banner layout.
                layout.addView(ad)
                getActivity()?.addContentView(
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

    /**
     * Listener that forwards drag events from BannerLayout to the banner ad listener.
     * Bridges between native Android drag events and Unity callbacks.
     */
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

    /**
     * Updates the banner margins based on the pivot point.
     * Android doesn't have a native pivot concept, so we use negative margins to achieve
     * the same positioning effect. Only applies to BannerAd API; UnityBannerAd uses (0,0) pivot.
     */
    private fun updateMargins() {
        runTaskOnUiThread {
            // Since there is no concept of pivot in Android, we negate the pivot values
            // and assign them as margins. This adjustment sets the left and top margins
            // to move the view based on the pivot point.
            // Note: We only do this for BannerAd API. For UnityBannerAd the pivot is
            // always assumed to be at (0,0) since positioning and resizing is handled by the GameObject

            val layoutParams = ad.layoutParams as RelativeLayout.LayoutParams
            layoutParams.leftMargin = (ad.width * -(_pivot.x)).toInt()
            layoutParams.topMargin = (ad.height * -(_pivot.y)).toInt()

            ad.layoutParams = layoutParams
        }
    }

    /**
     * Updates the gravity of the partner ad view within its container.
     * Applies both horizontal and vertical gravity using bitwise OR.
     */
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

    /**
     * Converts a size type integer constant to the corresponding banner size.
     *
     * @param sizeType The size type constant (STANDARD, MEDIUM, LEADERBOARD, or ADAPTIVE)
     * @param sizeWidth The width for adaptive banners
     * @param sizeHeight The height for adaptive banners
     * @return The corresponding ChartboostMediationBannerSize
     */
    private fun getSizeFromSizeType(sizeType: Int, sizeWidth: Float, sizeHeight: Float): ChartboostMediationBannerAdView.ChartboostMediationBannerSize {
        return when (sizeType) {
            SIZE_TYPE_STANDARD -> ChartboostMediationBannerAdView.ChartboostMediationBannerSize.STANDARD
            SIZE_TYPE_MEDIUM -> ChartboostMediationBannerAdView.ChartboostMediationBannerSize.MEDIUM
            SIZE_TYPE_LEADERBOARD -> ChartboostMediationBannerAdView.ChartboostMediationBannerSize.LEADERBOARD
            SIZE_TYPE_ADAPTIVE -> ChartboostMediationBannerAdView.ChartboostMediationBannerSize.bannerSize(
                sizeWidth.roundToInt(),
                sizeHeight.roundToInt()
            )
            else -> {
                UnityLoggingBridge.log(TAG, "Unknown size type: $sizeType, defaulting to ADAPTIVE(0x0)", LogLevel.WARNING)
                ChartboostMediationBannerAdView.ChartboostMediationBannerSize.bannerSize(0, 0)
            }
        }
    }

    /**
     * Destroys the banner layout and cleans up all associated resources.
     * Removes ViewTreeObserver listeners, clears views, and removes from parent hierarchy.
     */
    private fun destroyBannerLayout() {
        // Remove ViewTreeObserver listener to prevent memory leak
        layoutListener?.let {
            if (ad.viewTreeObserver.isAlive) {
                ad.viewTreeObserver.removeOnGlobalLayoutListener(it)
            }
        }
        layoutListener = null

        bannerLayout?.let {
            it.removeAllViews()
            it.visibility = View.GONE
            it.cleanup() // Call cleanup to clear references

            // Remove from parent
            (it.parent as? ViewGroup)?.removeView(it)
        }
        bannerLayout = null // Clear reference
    }

    /**
     * Gets the current display density of the device.
     *
     * @return The density value, or DENSITY_DEFAULT if activity is unavailable
     */
    private val displayDensity: Float
        get() {
            return getActivity()?.resources?.displayMetrics?.density ?: DisplayMetrics.DENSITY_DEFAULT.toFloat()
        }

    //endregion

    //region Companion Object

    companion object {
        private val TAG = BannerAdWrapper::class.java.simpleName

        // Horizontal Alignment Constants (must match C# enum)
        private const val HORIZONTAL_LEFT = 0
        private const val HORIZONTAL_CENTER = 1
        private const val HORIZONTAL_RIGHT = 2

        // Vertical Alignment Constants (must match C# enum)
        private const val VERTICAL_TOP = 0
        private const val VERTICAL_CENTER = 1
        private const val VERTICAL_BOTTOM = 2

        // Banner Size Type Constants (must match C# enum)
        private const val SIZE_TYPE_STANDARD = 0
        private const val SIZE_TYPE_MEDIUM = 1
        private const val SIZE_TYPE_LEADERBOARD = 2
        private const val SIZE_TYPE_ADAPTIVE = 3

        // Container Size Special Value
        private const val FIT_CONTENT = -1

        /**
         * Factory method to wrap a ChartboostMediationBannerAdView.
         * Exposed to Java for JNI interoperability.
         *
         * @param ad The native Chartboost banner ad view to wrap
         * @return A new BannerAdWrapper instance
         */
        @Suppress("unused")
        @JvmStatic
        fun wrap(ad: ChartboostMediationBannerAdView): BannerAdWrapper {
            return BannerAdWrapper(ad)
        }

        /**
         * Executes a runnable on the UI thread with error handling.
         * Logs warnings if the activity is unavailable or if exceptions occur.
         *
         * @param runnable The task to execute on the main thread
         */
        fun runTaskOnUiThread(runnable: Runnable) {
            val activity = UnityPlayer.currentActivity
            if (activity == null) {
                UnityLoggingBridge.log(TAG, "Activity not available for UI thread operation", LogLevel.WARNING)
                return
            }
            activity.runOnUiThread {
                try {
                    runnable.run()
                } catch (ex: Exception) {
                    UnityLoggingBridge.logException(TAG, "Exception found when running on UI Thread: $ex")
                }
            }
        }
    }

    //endregion

    init {
        this._position = PointF(0F, 0F)
        this._pivot = PointF(0F, 0F)
        createBannerLayout()
    }

}

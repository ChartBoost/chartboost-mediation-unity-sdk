@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.banner

import android.graphics.Color
import android.graphics.PointF
import android.os.Build
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.widget.FrameLayout
import androidx.annotation.RequiresApi
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdLoadListener
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdLoadRequest
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdView
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdViewListener
import com.chartboost.chartboostmediationsdk.domain.Keywords
import com.chartboost.mediation.unity.logging.LogLevel
import com.chartboost.mediation.unity.logging.UnityLoggingBridge
import com.chartboost.unity.utilities.banner.BannerContainer
import com.chartboost.unity.utilities.view.UnityViewUtils
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers.Main
import kotlinx.coroutines.SupervisorJob
import kotlinx.coroutines.cancel
import kotlinx.coroutines.launch
import org.json.JSONObject
import java.util.concurrent.atomic.AtomicBoolean
import kotlin.math.roundToInt

/**
 * Wraps a [ChartboostMediationBannerAdView] for Unity. The ad view lives in a shared [BannerContainer]
 * (see [createContainer]) that owns placement, visibility, drag and hosting; this wrapper owns the ad
 * itself and how the partner creative sits inside the ad view.
 */
class BannerAdWrapper(private val ad: ChartboostMediationBannerAdView) {
    private var _winningBidInfo: Map<String, String>? = null
    private var _loadId: String = ""
    private var _metrics: JSONObject? = null

    // Stored on the calling thread; UI updates are dispatched to the main thread.
    private var horizontalGravity = Gravity.CENTER_HORIZONTAL
    private var verticalGravity = Gravity.CENTER_VERTICAL

    // Match Ad: a wrapping container axis. The ad view wraps the creative and may overflow.
    @Volatile
    private var _isMatchAdMode: Boolean = false

    // Set when AdRelativePosition places the creative (BannerVisualElement); cleared by an alignment change.
    @Volatile
    private var isCreativePinned: Boolean = false

    @Volatile
    private var partnerAd: View? = null
    private var bannerAdListener: ChartboostMediationBannerAdListener? = null

    private val scope = CoroutineScope(Main + SupervisorJob())

    @Volatile
    private var _isDestroyed: Boolean = false

    // Debounces partner positioning to one pass per frame.
    private val _hasPendingPartnerUpdate = AtomicBoolean(false)

    // Wrapper class to match the Keywords pattern for C# JNI bridge compatibility
    class PartnerSettingsWrapper(private val map: MutableMap<String, Any>) {
        @Suppress("unused")
        fun get(): MutableMap<String, Any> {
            return map
        }
    }

    init {
        // The container adopts these as the ad view's size; setContainerSize replaces them.
        ad.layoutParams = FrameLayout.LayoutParams(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT)
    }

    /** Creates the shared container holding the ad view; the C# side drives it. Any thread. */
    @Suppress("unused")
    fun createContainer(): BannerContainer =
        BannerContainer.create(ad.context).also { it.setContent(ad) }

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

    //region Alignment

    // An alignment change hands placement back from AdRelativePosition; re-sets of the same one are no-ops (callers sync per frame).
    private fun takeAlignment(changed: Boolean): Boolean {
        if (changed) isCreativePinned = false
        return changed
    }

    /**
     * Sets the horizontal alignment of the partner ad within the banner container.
     * @param horizontalAlignment 0=Left, 1=Center, 2=Right
     */
    @Suppress("unused")
    fun setHorizontalAlignment(horizontalAlignment: Int) {
        val gravity = when (horizontalAlignment) {
            HORIZONTAL_LEFT -> Gravity.LEFT
            HORIZONTAL_CENTER -> Gravity.CENTER_HORIZONTAL
            HORIZONTAL_RIGHT -> Gravity.RIGHT
            else -> Gravity.CENTER_HORIZONTAL
        }
        if (!takeAlignment(gravity != horizontalGravity)) return
        horizontalGravity = gravity
        schedulePartnerUpdate()
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
        val gravity = when (verticalAlignment) {
            VERTICAL_TOP -> Gravity.TOP
            VERTICAL_CENTER -> Gravity.CENTER_VERTICAL
            VERTICAL_BOTTOM -> Gravity.BOTTOM
            else -> Gravity.CENTER_VERTICAL
        }
        if (!takeAlignment(gravity != verticalGravity)) return
        verticalGravity = gravity
        schedulePartnerUpdate()
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

    //region Size

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
     * Sizes the ad view inside its container: a fixed axis matches the container size, a wrapping
     * axis (-1, Match Ad) wraps the creative and is not clipped. The container is sized from C#.
     * @param width Container width in native units, or -1 to wrap.
     * @param height Container height in native units, or -1 to wrap.
     */
    @Suppress("unused")
    fun setContainerSize(width: Int, height: Int) {
        _isMatchAdMode = width == FIT_CONTENT || height == FIT_CONTENT
        runTaskOnUiThread {
            val lp = ad.layoutParams ?: FrameLayout.LayoutParams(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT)
            lp.width = dimension(width)
            lp.height = dimension(height)
            ad.layoutParams = lp
            ad.clipChildren = !_isMatchAdMode
            ad.clipToPadding = !_isMatchAdMode
        }
        schedulePartnerUpdate()
    }

    private fun dimension(dp: Int): Int =
        if (dp == FIT_CONTENT) ViewGroup.LayoutParams.WRAP_CONTENT else (dp * UnityViewUtils.displayDensity()).roundToInt()

    //endregion

    //region Partner Ad Positioning

    /** Positions the partner creative inside the ad view on the next frame, once per frame. */
    private fun schedulePartnerUpdate() {
        if (_isDestroyed) return
        if (!_hasPendingPartnerUpdate.compareAndSet(false, true)) return
        runTaskOnUiThread {
            ad.post {
                _hasPendingPartnerUpdate.set(false)
                if (!_isDestroyed) applyPartnerPosition()
            }
        }
    }

    /** Match Ad: the ad view wraps the creative (gravity). Fixed size: the creative is aligned by translation. */
    private fun applyPartnerPosition() {
        if (isCreativePinned) return
        val partner = partnerAd ?: return
        if (_isMatchAdMode) {
            (partner.layoutParams as? FrameLayout.LayoutParams)?.let { lp ->
                lp.gravity = horizontalGravity or verticalGravity
                partner.layoutParams = lp
            }
            partner.translationX = 0f
            partner.translationY = 0f
        } else {
            positionPartnerAdInContainer(partner)
        }
    }

    /**
     * Aligns the creative inside a fixed-size ad view by translation: a larger creative shows its
     * aligned portion, a smaller one sits at the aligned edge.
     */
    private fun positionPartnerAdInContainer(partner: View) {
        (partner.layoutParams as? FrameLayout.LayoutParams)?.let { lp ->
            if (lp.gravity != (Gravity.TOP or Gravity.LEFT)) {
                lp.gravity = Gravity.TOP or Gravity.LEFT
                partner.layoutParams = lp
            }
        }

        val partnerWidth = partner.width
        val partnerHeight = partner.height
        val containerWidth = ad.width
        val containerHeight = ad.height

        // Not laid out yet: the creative wait (or the next size/alignment change) schedules another update.
        if (partnerWidth <= 0 || containerWidth <= 0) {
            UnityLoggingBridge.log(TAG, "Partner dimensions not ready, skipping positioning", LogLevel.DEBUG)
            return
        }

        partner.translationX = when (horizontalGravity) {
            Gravity.LEFT -> 0f
            Gravity.RIGHT -> (containerWidth - partnerWidth).toFloat()
            else -> (containerWidth - partnerWidth) / 2f
        }
        partner.translationY = when (verticalGravity) {
            Gravity.TOP -> 0f
            Gravity.BOTTOM -> (containerHeight - partnerHeight).toFloat()
            else -> (containerHeight - partnerHeight) / 2f
        }
    }

    /** Re-positions the partner once the creative size is known, retrying for up to 10 frames. */
    private fun waitForCreativeAndSchedulePartnerUpdate(attempts: Int = 0) {
        if (_isDestroyed) return
        if (attempts >= 10) {
            schedulePartnerUpdate()
            return
        }
        ad.post {
            if (_isDestroyed) return@post
            if (ad.getCreativeSizeDips().width > 0) schedulePartnerUpdate()
            else waitForCreativeAndSchedulePartnerUpdate(attempts + 1)
        }
    }

    /**
     * Sets the relative position of the partner ad within the container.
     * Position is specified in density-independent pixels (dp).
     *
     * @param x The X coordinate in dp
     * @param y The Y coordinate in dp
     */
    @Suppress("unused")
    fun setAdRelativePosition(x: Float, y: Float){
        isCreativePinned = true
        runTaskOnUiThread {
            partnerAd?.let {
                val density = UnityViewUtils.displayDensity()
                it.x = x * density
                it.y = y * density
            }
        }
    }

    /**
     * Gets the relative position of the partner ad within the container.
     *
     * @return PointF containing the X and Y coordinates in dp (the setter's unit), or (0, 0) if partner ad doesn't exist
     */
    @Suppress("unused")
    fun getAdRelativePosition(): PointF {
        partnerAd?.let {
            val density = UnityViewUtils.displayDensity()
            return PointF(it.x / density, it.y / density)
        }
        return PointF(0F,0F)
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

            // Kept on the wrapper for parity with the iOS BannerView, which stores them too.
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
     * Destroys the banner ad and releases all resources. The C# side disposes the container first.
     */
    @Suppress("unused")
    fun destroy() {
        _isDestroyed = true
        scope.cancel()
        runTaskOnUiThread {
            // Normally C# disposed the container already; after a finalizer it's still here, so release it.
            when (val parent = ad.parent) {
                is BannerContainer -> parent.dispose()
                is ViewGroup -> parent.removeView(ad)
            }
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
                isCreativePinned = false // a new creative starts from the alignment; a pinning caller re-pins it
                waitForCreativeAndSchedulePartnerUpdate()
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
                    partnerAd = View(ad.context).apply {
                        layoutParams = FrameLayout.LayoutParams(
                            ViewGroup.LayoutParams.MATCH_PARENT,
                            ViewGroup.LayoutParams.MATCH_PARENT
                        )
                    }
                    ad.addView(partnerAd)
                }
                partnerAd?.setBackgroundColor(Color.argb(a, r, g, b))
            }
        }
        else {
            UnityLoggingBridge.log(TAG, "Invalid Color Array Length: expected 4, got ${colorArray.size}", LogLevel.WARNING)
        }
    }

    //endregion

    //region Private Utilities

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

        /** Runs on the UI thread in call order with the container's own updates, logging any exception. */
        fun runTaskOnUiThread(runnable: Runnable) {
            UnityViewUtils.runOnUiThread {
                try {
                    runnable.run()
                } catch (ex: Exception) {
                    UnityLoggingBridge.logException(TAG, "Exception found when running on UI Thread: $ex")
                }
            }
        }
    }

    //endregion
}

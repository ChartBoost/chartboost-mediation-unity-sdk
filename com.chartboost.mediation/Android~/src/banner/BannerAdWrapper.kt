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
import java.util.concurrent.atomic.AtomicBoolean
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

    // Track if Match Ad mode was set - Unity sends -1,-1 first, then actual size
    // We need to remember the initial mode to handle clipping correctly
    @Volatile
    private var _isMatchAdMode: Boolean = false

    @Volatile
    private var partnerAd: View? = null
    private var bannerLayout: BannerLayout? = null
    private var bannerAdListener: ChartboostMediationBannerAdListener? = null

    // Automation-only host for the banner layout; null = default Unity activity.
    @Volatile
    private var _hostView: View? = null

    // Coroutine scope tied to wrapper lifecycle
    private val scope = CoroutineScope(Main + SupervisorJob())

    // Store the desired draggable state to apply when bannerLayout is created
    private var _canDrag: Boolean = false

    // Store ViewTreeObserver listener reference for proper cleanup
    private var layoutListener: ViewTreeObserver.OnGlobalLayoutListener? = null

    // Track destruction state to prevent async operations after cleanup
    @Volatile
    private var _isDestroyed: Boolean = false

    // Debouncing flag to prevent redundant position updates
    // Uses AtomicBoolean for proper atomic compare-and-set to avoid race conditions
    private val _hasPendingPositionUpdate = AtomicBoolean(false)

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
     * For banners wider than the screen, the position is adjusted to center the banner.
     */
    @Suppress("unused")
    fun setPosition(x: Float, y: Float){
        _position = PointF(x,y)
        schedulePositionUpdate()
    }

    // Hosts the banner view directly in the given container (automation slot), mirroring the
    // native canary. Pass null to return it to the wrapper's own positioning layout.
    @Suppress("unused")
    fun setHostView(hostView: View?) {
        _hostView = hostView
        runTaskOnUiThread {
            val host = hostView as? ViewGroup
            if (host != null) {
                attachAdToHost(host)
            } else {
                (ad.parent as? ViewGroup)?.removeView(ad)
                ad.layoutParams = RelativeLayout.LayoutParams(
                    RelativeLayout.LayoutParams.WRAP_CONTENT,
                    RelativeLayout.LayoutParams.WRAP_CONTENT
                )
                bannerLayout?.addView(ad)
                schedulePositionUpdate()
            }
        }
    }

    // Adds the banner into the container, letting the container generate compatible LayoutParams.
    // The ad carries the wrapper's RelativeLayout params, which a FrameLayout host cannot cast.
    private fun attachAdToHost(host: ViewGroup) {
        (ad.parent as? ViewGroup)?.removeView(ad)
        host.addView(ad, ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.WRAP_CONTENT)
        // The slot centers the ad via gravity, so clear any translation left over from positioning
        // before it was hosted (e.g. the Unity variant's default off-screen RectTransform sync),
        // otherwise the ad stays sized correctly but pushed off-screen.
        ad.translationX = 0f
        ad.translationY = 0f
        (ad.layoutParams as? FrameLayout.LayoutParams)?.let {
            it.gravity = Gravity.CENTER
            ad.layoutParams = it
        }
    }

    // Match params to the ad's actual parent, not the _hostView flag: setHostView() flips _hostView
    // before it re-parents the ad, so keying off the flag can put FrameLayout params on the
    // RelativeLayout wrapper and crash onMeasure with a ClassCastException.
    private fun buildAdLayoutParams(width: Int, height: Int): ViewGroup.LayoutParams =
        when (ad.parent) {
            is FrameLayout -> FrameLayout.LayoutParams(width, height).apply { gravity = Gravity.CENTER }
            is RelativeLayout -> RelativeLayout.LayoutParams(width, height)
            // Detached: fall back to the intended host type; the add/attach reconciles it.
            else -> if (_hostView != null)
                FrameLayout.LayoutParams(width, height).apply { gravity = Gravity.CENTER }
            else
                RelativeLayout.LayoutParams(width, height)
        }

    /**
     * Schedules a position update with debouncing to prevent redundant calls.
     * Multiple rapid calls will result in only one actual position update.
     * Uses atomic compare-and-set to ensure thread-safe scheduling.
     */
    private fun schedulePositionUpdate() {
        if (_isDestroyed) return

        // Atomic compare-and-set: only schedule if not already pending
        if (!_hasPendingPositionUpdate.compareAndSet(false, true)) return

        runTaskOnUiThread {
            ad.post {
                if (!_isDestroyed) {
                    _hasPendingPositionUpdate.set(false)
                    applyPosition()
                }
            }
        }
    }

    /**
     * Applies the current position to the ad view.
     * For Match Ad mode with wide banners, Unity already sends the centered position,
     * so we just apply it directly to the partnerAd.
     * For Fixed Size mode, centers the partnerAd within the container.
     */
    private fun applyPosition() {
        // When hosted in a container (automation slot), the container owns layout (native-canary parity).
        if (_hostView != null) return
        val isWideBanner = calculateCenteringOffset() != null
        val isContainerConstrained = isContainerConstrainedByScreen()
        val partner = partnerAd

        when {
            _isMatchAdMode && isWideBanner && partner != null -> {
                // Match Ad + wide banner: Unity sends the centered position, apply it to partnerAd
                (partner.layoutParams as? FrameLayout.LayoutParams)?.let { layoutParams ->
                    layoutParams.gravity = Gravity.TOP or Gravity.LEFT
                    partner.layoutParams = layoutParams
                }
                ad.translationX = 0f
                ad.translationY = _position.y * displayDensity
                partner.translationX = _position.x * displayDensity
                partner.translationY = 0f
            }
            _isMatchAdMode && isWideBanner -> {
                // Match Ad + wide banner but partnerAd not ready - use centering offset temporarily
                val centeringOffset = calculateCenteringOffset() ?: 0
                ad.translationX = centeringOffset.toFloat()
                ad.translationY = _position.y * displayDensity
            }
            _isMatchAdMode -> {
                // Match Ad mode with normal-sized banner - use gravity for centering
                partner?.let {
                    (it.layoutParams as? FrameLayout.LayoutParams)?.let { layoutParams ->
                        layoutParams.gravity = horizontalGravity or verticalGravity
                        it.layoutParams = layoutParams
                    }
                    it.translationX = 0f
                    it.translationY = 0f
                }
                ad.translationX = _position.x * displayDensity
                ad.translationY = _position.y * displayDensity
            }
            else -> {
                // Fixed Size mode - translation-based positioning with alignment.
                val posX = if (isContainerConstrained) 0f else _position.x * displayDensity
                ad.translationX = posX
                ad.translationY = _position.y * displayDensity
                positionPartnerAdInContainer()
            }
        }
    }

    /**
     * Checks if the container is being constrained by the screen width.
     * This happens when Unity requests a container larger than the screen.
     * @return true if the container width is being constrained
     */
    private fun isContainerConstrainedByScreen(): Boolean {
        val activity = getActivity() ?: return false
        val screenWidth = activity.resources.displayMetrics.widthPixels

        // Get the requested container width in pixels
        val requestedWidth = if (_containerSize.width == FIT_CONTENT) {
            // For wrap content, use the banner's creative size
            (ad.getCreativeSizeDips().width * displayDensity).toInt()
        } else {
            (_containerSize.width * displayDensity).toInt()
        }

        // Container is constrained if requested width exceeds screen width
        return requestedWidth > screenWidth
    }

    /**
     * Positions the partnerAd within the container for Fixed Size mode.
     * Uses translation-based positioning that respects alignment settings.
     * Works for both cases: banner larger than container (shows aligned portion)
     * and banner smaller than container (aligns within space).
     */
    private fun positionPartnerAdInContainer() {
        val partner = partnerAd ?: return

        // Reset gravity to TOP|LEFT since we use translation for positioning
        (partner.layoutParams as? FrameLayout.LayoutParams)?.let { layoutParams ->
            if (layoutParams.gravity != (Gravity.TOP or Gravity.LEFT)) {
                layoutParams.gravity = Gravity.TOP or Gravity.LEFT
                partner.layoutParams = layoutParams
            }
        }

        val partnerWidth = partner.width
        val partnerHeight = partner.height
        val containerWidth = ad.width
        val containerHeight = ad.height

        // Skip positioning if dimensions aren't ready yet - will be applied on next position update
        if (partnerWidth <= 0 || containerWidth <= 0) {
            UnityLoggingBridge.log(TAG, "Partner dimensions not ready, skipping positioning", LogLevel.DEBUG)
            return
        }

        // Calculate horizontal offset based on alignment setting
        // For LEFT: offset = 0 (show left portion if larger, align left if smaller)
        // For CENTER: offset = (container - partner) / 2 (center the content)
        // For RIGHT: offset = container - partner (show right portion if larger, align right if smaller)
        val offsetX = when (horizontalGravity) {
            Gravity.LEFT -> 0f
            Gravity.RIGHT -> (containerWidth - partnerWidth).toFloat()
            else -> (containerWidth - partnerWidth) / 2f // CENTER or default
        }

        // Calculate vertical offset based on alignment setting
        val offsetY = when (verticalGravity) {
            Gravity.TOP -> 0f
            Gravity.BOTTOM -> (containerHeight - partnerHeight).toFloat()
            else -> (containerHeight - partnerHeight) / 2f // CENTER or default
        }

        UnityLoggingBridge.log(TAG, "Positioning partner: offsetX=$offsetX, offsetY=$offsetY (hAlign=$horizontalGravity, vAlign=$verticalGravity)", LogLevel.DEBUG)

        // Use translationX/Y for reliable positioning independent of layout state
        partner.translationX = offsetX
        partner.translationY = offsetY
    }

    /**
     * Calculates the horizontal offset needed to center a wide banner.
     * Returns the offset in pixels, or null if no centering is needed.
     */
    private fun calculateCenteringOffset(): Int? {
        val creativeSizeDips = ad.getCreativeSizeDips()
        val bannerWidthPx = (creativeSizeDips.width * displayDensity).toInt()

        if (bannerWidthPx <= 0) return null

        val activity = getActivity() ?: return null
        val screenWidth = activity.resources.displayMetrics.widthPixels

        return if (bannerWidthPx > screenWidth) {
            (screenWidth - bannerWidthPx) / 2
        } else {
            null
        }
    }

    /**
     * Waits for the ad creative size to be available, then schedules position update.
     * Uses recursive posting to ensure we don't apply centering logic before dimensions are known.
     * Checks destruction state to prevent execution after cleanup.
     */
    private fun waitForLayoutAndSchedulePositionUpdate(attempts: Int = 0) {
        if (_isDestroyed) return

        if (attempts >= 10) {
            schedulePositionUpdate()
            return
        }

        ad.post {
            if (_isDestroyed) return@post

            val creativeSizeDips = ad.getCreativeSizeDips()
            if (creativeSizeDips.width > 0) {
                schedulePositionUpdate()
            } else {
                waitForLayoutAndSchedulePositionUpdate(attempts + 1)
            }
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

        // For Fixed Size mode, use translation-based positioning
        // For Match Ad mode, apply gravity via updateGravity()
        if (_isMatchAdMode) {
            updateGravity()
        } else {
            schedulePositionUpdate()
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

        // For Fixed Size mode, use translation-based positioning
        // For Match Ad mode, apply gravity via updateGravity()
        if (_isMatchAdMode) {
            updateGravity()
        } else {
            schedulePositionUpdate()
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

        // Update match ad mode based on whether this is wrap content
        val isWrapContent = width == FIT_CONTENT || height == FIT_CONTENT
        _isMatchAdMode = isWrapContent

        // Only UI manipulation on UI thread
        runTaskOnUiThread {

            // Fit Both
            if(width == FIT_CONTENT && height == FIT_CONTENT){
                ad.layoutParams = buildAdLayoutParams(
                    ViewGroup.LayoutParams.WRAP_CONTENT,
                    ViewGroup.LayoutParams.WRAP_CONTENT
                )
            }
            // Fit Horizontal

            else if(width == FIT_CONTENT){
                ad.layoutParams = buildAdLayoutParams(
                    ViewGroup.LayoutParams.WRAP_CONTENT,
                    (height * displayDensity).toInt()
                )
            }

            // Fit Vertical
            else if(height == FIT_CONTENT){
                ad.layoutParams = buildAdLayoutParams(
                    (width * displayDensity).toInt(),
                    ViewGroup.LayoutParams.WRAP_CONTENT
                )
            }

            // Fixed size
            else{
                ad.layoutParams = buildAdLayoutParams(
                    (width * displayDensity).toInt(),
                    (height * displayDensity).toInt()
                )
            }

            updateClipSettings(_isMatchAdMode)

            // Request layout to ensure dimensions are updated
            ad.requestLayout()
        }

        // Schedule position update after container size change
        // The debouncing mechanism ensures only one update happens even if called multiple times
        schedulePositionUpdate()
    }

    /**
     * Updates clip settings on the banner layout and ad view.
     * @param allowOverflow If true, disables clipping to allow banners to extend beyond bounds.
     *                      If false, enables clipping to constrain content within bounds.
     */
    private fun updateClipSettings(allowOverflow: Boolean) {
        bannerLayout?.let { layout ->
            layout.clipChildren = !allowOverflow
            layout.clipToPadding = !allowOverflow
        }
        ad.clipChildren = !allowOverflow
        ad.clipToPadding = !allowOverflow
    }

    /**
     * Gets the current container size.
     * Returns the actual calculated size, using banner dimensions for wrap content mode.
     */
    @Suppress("unused")
    fun getContainerSize(): Size {
        val width = if (_containerSize.width == FIT_CONTENT) {
            ad.getCreativeSizeDips().width.toInt()
        } else {
            _containerSize.width
        }
        val height = if (_containerSize.height == FIT_CONTENT) {
            ad.getCreativeSizeDips().height.toInt()
        } else {
            _containerSize.height
        }
        return Size(width, height)
    }

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
        _isMatchAdMode = false  // Reset mode for next load
        runTaskOnUiThread { ad.clearAd() }
    }

    /**
     * Destroys the banner ad and releases all resources.
     * Cancels any ongoing coroutines and cleans up the banner layout.
     */
    @Suppress("unused")
    fun destroy() {
        // Mark as destroyed to prevent async operations from executing
        _isDestroyed = true

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
                // Schedule position update after layout to handle centering with actual dimensions
                waitForLayoutAndSchedulePositionUpdate()
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

            layout = BannerLayout(getActivity(), ad, dragListener)
            layout.setBackgroundColor(Color.TRANSPARENT)

            layout.canDrag = _canDrag

            bannerLayout = layout

            updateClipSettings(_isMatchAdMode)

            try {

                // Default is wrap content
                ad.layoutParams = RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WRAP_CONTENT,
                    ViewGroup.LayoutParams.WRAP_CONTENT
                )

                layoutListener = ViewTreeObserver.OnGlobalLayoutListener {
                    updateMargins()
                }
                ad.viewTreeObserver.addOnGlobalLayoutListener(layoutListener)

                layout.addView(ad)
                getActivity()?.addContentView(
                    layout,
                    ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MATCH_PARENT,
                        ViewGroup.LayoutParams.MATCH_PARENT
                    )
                )
                // If hosted (automation), move the banner into the host so it survives ad refresh.
                (_hostView as? ViewGroup)?.let { host -> attachAdToHost(host) }

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
            // Persist the dragged position so it survives ad refreshes (onAdViewAdded -> applyPosition)
            _position = PointF(x / displayDensity, y / displayDensity)
            bannerAdListener?.onAdDragEnd(this@BannerAdWrapper, x, y)
        }
    }

    /**
     * Updates the banner margins based on the pivot point.
     * Android doesn't have a native pivot concept, so we use negative margins to achieve
     * the same positioning effect. Only applies to BannerAd API; UnityBannerAd uses (0,0) pivot.
     * Note: Centering for wide banners is handled separately via applyPosition().
     */
    private fun updateMargins() {
        runTaskOnUiThread {
            // Since there is no concept of pivot in Android, we negate the pivot values
            // and assign them as margins. This adjustment sets the left and top margins
            // to move the view based on the pivot point.
            // Note: We only do this for BannerAd API. For UnityBannerAd the pivot is
            // always assumed to be at (0,0) since positioning and resizing is handled by the GameObject

            // Pivot margins only apply while the wrapper's RelativeLayout owns the ad. When hosted
            // in the automation slot (FrameLayout), the container owns layout — skip.
            val layoutParams = ad.layoutParams as? RelativeLayout.LayoutParams ?: return@runTaskOnUiThread
            layoutParams.leftMargin = (ad.width * -(_pivot.x)).toInt()
            layoutParams.topMargin = (ad.height * -(_pivot.y)).toInt()
            ad.layoutParams = layoutParams
        }
    }

    /**
     * Updates the gravity of the partner ad view within its container.
     * Only applies for Match Ad mode - Fixed Size mode uses translation-based positioning.
     */
    private fun updateGravity(){
        // Only apply gravity for Match Ad mode
        // Fixed Size mode uses translation-based positioning in positionPartnerAdInContainer()
        if (!_isMatchAdMode) return

        runTaskOnUiThread {
            val partner = partnerAd ?: return@runTaskOnUiThread
            val layoutParams = partner.layoutParams as? FrameLayout.LayoutParams ?: return@runTaskOnUiThread
            layoutParams.gravity = horizontalGravity or verticalGravity
            partner.layoutParams = layoutParams
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

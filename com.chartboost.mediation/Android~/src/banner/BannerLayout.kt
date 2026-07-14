@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.banner

import android.annotation.SuppressLint
import android.content.Context
import android.content.res.Configuration
import android.os.Build
import android.util.DisplayMetrics
import android.view.DisplayCutout
import android.view.MotionEvent
import android.view.WindowInsets
import android.widget.RelativeLayout
import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdView
import com.unity3d.player.UnityPlayer
import kotlin.math.pow
import kotlin.math.sqrt

/**
 * Custom layout container for Chartboost Mediation banner ads in Unity applications.
 *
 * Provides drag-and-drop functionality for banner ads while respecting device safe areas
 * (notches, display cutouts). Handles touch events to enable smooth dragging within
 * valid screen boundaries.
 *
 * @param context The Android context, typically from UnityPlayer.currentActivity
 * @param bannerView The Chartboost banner ad view to be contained
 * @param dragListener Listener to receive drag event callbacks
 */
@SuppressLint("ViewConstructor")
class BannerLayout(context: Context?, private var bannerView: ChartboostMediationBannerAdView?, private var dragListener: IBannerDragListener?) : RelativeLayout(context) {

    companion object {
        /**
         * Minimum distance in pixels that the user must drag before the drag gesture is recognized.
         * This helps distinguish between taps and drags.
         */
        private const val DRAG_THRESHOLD_PIXELS = 10

        /**
         * Minimum interval in nanoseconds between drag event callbacks to Unity.
         * Android sends raw touch events at 120+ events/sec, but Unity processes events once per
         * frame (~60fps). Throttling to ~16ms matches iOS's UIPanGestureRecognizer rate and
         * prevents the MainThreadDispatcher queue from backing up with stale events.
         * The native banner view still moves immediately for smooth visual feedback.
         */
        private const val DRAG_CALLBACK_INTERVAL_NS = 16_000_000L // ~16ms ≈ 60fps
    }

    @Volatile
    var canDrag: Boolean = false
        set(value) {
            field = value
            if (!value && isDragging) {
                // Cancel any ongoing drag when dragging is disabled
                isDragging = false
                dragListener?.onDragEnd(bannerView?.x ?: 0f, bannerView?.y ?: 0f)
            }
        }
    private var safeAreaTop: Int = 0
    private var safeAreaLeft: Int = 0
    private var safeAreaRight: Int = 0
    private var safeAreaBottom: Int = 0
    private var screenWidth = 0
    private var screenHeight = 0

    private var isDragging = false
    private var lastDragCallbackTimeNs: Long = 0

    private var dragStartX: Int = 0
    private var dragStartY: Int = 0
    private var dragLastX: Int = 0
    private var dragLastY: Int = 0

    init {
        // making it clickable here allows onInterceptTouchEvent to intercept touch events on bannerView
        bannerView?.isClickable = true

        // Allow banners to extend beyond layout bounds (e.g., leaderboards wider than screen)
        clipChildren = false
        clipToPadding = false

        updateScreenDimensions()
    }

    /**
     * Updates screen dimensions based on current window metrics.
     * Handles both modern (API 30+) and legacy approaches.
     */
    private fun updateScreenDimensions() {
        val activity = UnityPlayer.currentActivity
        if (activity == null) {
            // Activity not available, use default values
            screenWidth = 0
            screenHeight = 0
            return
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
            // Use WindowMetrics API for Android R (API 30) and above
            val bounds = activity.windowManager.currentWindowMetrics.bounds
            screenWidth = bounds.width()
            screenHeight = bounds.height()
        } else {
            // Use deprecated Display API for older versions
            val outMetrics = DisplayMetrics()
            @Suppress("DEPRECATION")
            val display = activity.windowManager.defaultDisplay
            @Suppress("DEPRECATION")
            display.getRealMetrics(outMetrics)
            screenWidth = outMetrics.widthPixels
            screenHeight = outMetrics.heightPixels
        }
    }

    override fun onConfigurationChanged(newConfig: Configuration?) {
        super.onConfigurationChanged(newConfig)
        updateScreenDimensions()
    }

    override fun onDetachedFromWindow() {
        super.onDetachedFromWindow()
        cleanup()
    }

    /**
     * Cleans up references to prevent memory leaks.
     * Should be called when the banner is no longer needed.
     */
    fun cleanup() {
        bannerView = null
        dragListener = null
        removeAllViews()
    }

    override fun onApplyWindowInsets(insets: WindowInsets?): WindowInsets {
        // Get safe area insets
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.P) {
            val displayCutout: DisplayCutout? = insets?.displayCutout
            if (displayCutout != null) {
                // Get safe area insets and validate them to prevent negative values
                safeAreaTop = displayCutout.safeInsetTop.coerceAtLeast(0)
                safeAreaLeft = displayCutout.safeInsetLeft.coerceAtLeast(0)
                safeAreaRight = displayCutout.safeInsetRight.coerceAtLeast(0)
                safeAreaBottom = displayCutout.safeInsetBottom.coerceAtLeast(0)
            }
        }
        return super.onApplyWindowInsets(insets)
    }

    override fun onInterceptTouchEvent(event: MotionEvent?): Boolean {
        // Early return if event is null or dragging is disabled
        event ?: return super.onInterceptTouchEvent(event)

        if (!canDrag)
            return super.onInterceptTouchEvent(event)

        // Get local references to prevent null pointer issues
        val currentBanner = bannerView
        val currentDragListener = dragListener

        // If banner or listener is null, cannot handle drag
        if (currentBanner == null || currentDragListener == null)
            return super.onInterceptTouchEvent(event)

        when (event.action) {
            MotionEvent.ACTION_DOWN -> {
                dragStartX = event.rawX.toInt()
                dragStartY = event.rawY.toInt()

                dragLastX = dragStartX
                dragLastY = dragStartY

                isDragging = false
            }

            MotionEvent.ACTION_MOVE -> {
                val dx = (event.rawX - dragLastX).toInt()
                val dy = (event.rawY - dragLastY).toInt()

                dragLastX = event.rawX.toInt()
                dragLastY = event.rawY.toInt()

                if (!isDragging && hasDragged()) {
                    isDragging = true
                    lastDragCallbackTimeNs = System.nanoTime()
                    currentDragListener.onDragBegin(currentBanner.x, currentBanner.y)
                }

                if (isDragging) {
                    val newX = currentBanner.x + dx
                    val newY = currentBanner.y + dy

                    val bannerWidth = currentBanner.width.toFloat()
                    val bannerHeight = currentBanner.height.toFloat()
                    val safeWidth = (screenWidth - safeAreaLeft - safeAreaRight).toFloat()

                    val leftBound: Float
                    val rightBound: Float

                    if (bannerWidth > safeWidth) {
                        // Banner is wider than screen: allow dragging beyond borders but
                        // require at least 60% of the banner to remain visible horizontally.
                        val minVisibleH = minOf(bannerWidth * 0.6f, safeWidth)
                        leftBound = safeAreaLeft - (bannerWidth - minVisibleH)
                        rightBound = screenWidth - safeAreaRight - minVisibleH
                    } else {
                        // Banner fits within screen: keep fully within safe area.
                        leftBound = safeAreaLeft.toFloat()
                        rightBound = (screenWidth - safeAreaRight - bannerWidth)
                    }

                    // Vertical bounds: always keep fully within safe area
                    val topBound = safeAreaTop.toFloat()
                    val bottomBound = (screenHeight - safeAreaBottom - bannerHeight)

                    if (newX >= leftBound && newX <= rightBound &&
                        newY >= topBound && newY <= bottomBound) {
                        // Always update native view position immediately for smooth visual feedback
                        currentBanner.x = newX
                        currentBanner.y = newY

                        // Throttle drag callbacks to Unity to ~60fps to match iOS behavior.
                        // Android sends raw touch events at 120+ Hz which floods the
                        // MainThreadDispatcher queue, causing the layout container to lag behind.
                        val now = System.nanoTime()
                        if (now - lastDragCallbackTimeNs >= DRAG_CALLBACK_INTERVAL_NS) {
                            lastDragCallbackTimeNs = now
                            currentDragListener.onDrag(currentBanner.x, currentBanner.y)
                        }
                    }
                }
            }

            MotionEvent.ACTION_UP -> {
                val wasDragging = isDragging
                if (isDragging) {
                    currentDragListener.onDragEnd(currentBanner.x, currentBanner.y)
                    isDragging = false
                }
                return wasDragging // Return true if the event was a drag, indicating the touch was intercepted
            }
        }

        return super.onInterceptTouchEvent(event)
    }

    private fun hasDragged(): Boolean {
        val dx = (dragLastX - dragStartX).toDouble()
        val dy = (dragLastY - dragStartY).toDouble()
        val distance = sqrt(dx.pow(2.0) + dy.pow(2.0)).toFloat()

        return distance.isFinite() && distance > DRAG_THRESHOLD_PIXELS
    }
}

package com.chartboost.mediation.unity.banner

import android.content.Context
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

class BannerLayout
    (
    context: Context,
    private var bannerView: ChartboostMediationBannerAdView,
    private var dragListener: IBannerDragListener
) : RelativeLayout(context) {

    var canDrag: Boolean = false
    private var safeAreaTop:Int = 0
    private var safeAreaLeft:Int = 0 
    private var safeAreaRight:Int = 0
    private var safeAreaBottom:Int = 0
    private var screenWidth = 0;
    private var screenHeight = 0;

    private val dragThresholdDistance = 10 // in pixels

    private var startX: Int = 0
    private var startY: Int = 0
    private var lastX: Int = 0
    private var lastY: Int = 0

    init {
        // making it clickable here allows onInterceptTouchEvent to intercept touch events on bannerView
        bannerView.isClickable = true

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
            val bounds = UnityPlayer.currentActivity.windowManager.currentWindowMetrics.bounds
            screenWidth = bounds.width()
            screenHeight = bounds.height()
        }
        else {
            val metrics = DisplayMetrics();
            UnityPlayer.currentActivity.windowManager.defaultDisplay.getRealMetrics(metrics)
            screenWidth = metrics.widthPixels
            screenHeight = metrics.heightPixels
        }
    }

    override fun onApplyWindowInsets(insets: WindowInsets?): WindowInsets {
        // Get safe area insets
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.P) {
            val displayCutout: DisplayCutout? = insets?.displayCutout
            if (displayCutout != null) {
                // Get safe area insets
                safeAreaTop = displayCutout.safeInsetTop
                safeAreaLeft = displayCutout.safeInsetLeft
                safeAreaRight = displayCutout.safeInsetRight
                safeAreaBottom = displayCutout.safeInsetBottom
            }
        }
        return super.onApplyWindowInsets(insets)
    }

    override fun onInterceptTouchEvent(event: MotionEvent?): Boolean {

        if (!canDrag)
            return super.onInterceptTouchEvent(event)

        if (event?.action == MotionEvent.ACTION_DOWN) {
            startX = event.rawX.toInt()
            startY = event.rawY.toInt()

            lastX = startX
            lastY = startY
        }

        if (event?.action == MotionEvent.ACTION_MOVE) {

            val dx = (event.rawX - lastX).toInt()
            val dy = (event.rawY - lastY).toInt()

            lastX = event.rawX.toInt()
            lastY = event.rawY.toInt()

            if (hasDragged()) {
                val newX = bannerView.x + dx
                val newY = bannerView.y + dy
                val safeLeft = safeAreaLeft
                val safeRight = screenWidth - safeAreaRight
                val safeTop = safeAreaTop
                val safeBottom = screenHeight - safeAreaBottom

                // do not move any part of the banner out of the safe area
                if((newX >= safeLeft && newX + bannerView.width <= safeRight) &&
                    (newY >= safeTop && newY + bannerView.height <= safeBottom)) {
                    bannerView.x = newX
                    bannerView.y = newY
                    dragListener.onDrag(bannerView.x, bannerView.y)
                }
            }
        }

        if (event?.action == MotionEvent.ACTION_UP) {
            return hasDragged()
        }

        return super.onInterceptTouchEvent(event)
    }

    private fun hasDragged(): Boolean {
        val distance = sqrt(
            (lastX - startX).toDouble().pow(2.0) + (lastY - startY).toDouble().pow(2.0)
        ).toFloat()

        return distance > dragThresholdDistance
    }
}

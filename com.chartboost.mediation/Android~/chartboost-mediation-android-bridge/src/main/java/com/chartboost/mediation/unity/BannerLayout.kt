package com.chartboost.mediation.unity

import android.content.Context
import android.view.MotionEvent
import android.widget.RelativeLayout
import com.chartboost.heliumsdk.ad.HeliumBannerAd
import kotlin.math.pow
import kotlin.math.sqrt

class BannerLayout : RelativeLayout {
    private var bannerView: HeliumBannerAd
    private var dragListener: IBannerDragListener

    constructor(
        context: Context,
        bannerView: HeliumBannerAd,
        dragListener: IBannerDragListener
    ) : super(context) {
        this.bannerView = bannerView
        this.dragListener = dragListener

        // making it clickable here allows onInterceptTouchEvent to intercept touch events on bannerView
        bannerView.isClickable = true;
    }

    var canDrag:Boolean = true
    private val dragThresholdDistance = 10 // in pixels

    private var startX:Int = 0;
    private var startY:Int = 0;
    private var lastX:Int = 0;
    private var lastY:Int = 0;


    override fun onInterceptTouchEvent(event: MotionEvent?): Boolean {
        if(!canDrag)
            return super.onInterceptTouchEvent(event)

        if(event?.action == MotionEvent.ACTION_DOWN){
            startX = event.rawX.toInt()
            startY = event.rawY.toInt();

            lastX = startX
            lastY = startY
        }

        if(event?.action == MotionEvent.ACTION_MOVE) {

            val dx = (event.rawX - lastX).toInt();
            val dy = (event.rawY - lastY).toInt();

            lastX = event.rawX.toInt()
            lastY = event.rawY.toInt()

            if(hasDragged()){
                bannerView.x += dx;
                bannerView.y += dy;
                dragListener.onDrag(bannerView.x, bannerView.y);
            }
        }

        if(event?.action == MotionEvent.ACTION_UP){
            return hasDragged();
        }

        return super.onInterceptTouchEvent(event)
    }

    private fun hasDragged():Boolean {
        val distance = sqrt(
            (lastX - startX).toDouble().pow(2.0) + (lastY - startY).toDouble().pow(2.0)
        ).toFloat()

        return distance > dragThresholdDistance
    }
}
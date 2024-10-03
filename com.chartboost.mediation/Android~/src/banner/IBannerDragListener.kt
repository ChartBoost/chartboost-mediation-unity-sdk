@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.banner

interface IBannerDragListener {
    fun onDragBegin(x: Float, y: Float)
    fun onDrag(x: Float, y: Float)
    fun onDragEnd(x: Float, y: Float)
}

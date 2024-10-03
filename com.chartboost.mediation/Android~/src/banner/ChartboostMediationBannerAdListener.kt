@file:Suppress("PackageDirectoryMismatch")
package com.chartboost.mediation.unity.banner

interface ChartboostMediationBannerAdListener {
    fun onAdViewAdded(ad: BannerAdWrapper)

    fun onAdClicked(ad: BannerAdWrapper)

    fun onAdImpressionRecorded(ad: BannerAdWrapper)

    fun onAdDragBegin(ad: BannerAdWrapper, x: Float, y: Float)

    fun onAdDrag(ad: BannerAdWrapper, x: Float, y: Float)

    fun onAdDragEnd(ad: BannerAdWrapper, x: Float, y: Float)
}

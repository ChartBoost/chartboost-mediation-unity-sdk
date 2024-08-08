package com.chartboost.mediation.unity.banner

import com.chartboost.chartboostmediationsdk.ad.ChartboostMediationBannerAdView

interface ChartboostMediationBannerAdListener {
    fun onAdViewAdded(ad: BannerAdWrapper)

    fun onAdClicked(ad: BannerAdWrapper)

    fun onAdImpressionRecorded(ad: BannerAdWrapper)

    fun onAdDrag(ad: BannerAdWrapper, x: Float, y: Float)
}

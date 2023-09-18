package com.chartboost.mediation.unity

interface ChartboostMediationBannerViewListener {
    public abstract fun onAdCached(ad: BannerAdWrapper, error: String)

    public abstract fun onAdViewAdded(ad: BannerAdWrapper)

    public abstract fun onAdClicked(ad: BannerAdWrapper)

    public abstract fun onAdImpressionRecorded(ad: BannerAdWrapper)

    public abstract fun onAdDrag(ad: BannerAdWrapper, x: Float, y: Float)
}


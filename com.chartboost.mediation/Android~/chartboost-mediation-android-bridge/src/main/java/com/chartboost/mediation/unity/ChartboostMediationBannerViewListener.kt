package com.chartboost.mediation.unity

import android.util.Size
import com.chartboost.heliumsdk.ad.HeliumBannerAd

interface ChartboostMediationBannerViewListener {
    public abstract fun onAdCached(ad: BannerAdWrapper, size:HeliumBannerAd.HeliumBannerSize, error: String)

    public abstract fun onAdViewAdded(ad: BannerAdWrapper)

    public abstract fun onAdClicked(ad: BannerAdWrapper)

    public abstract fun onAdImpressionRecorded(ad: BannerAdWrapper)

    public abstract fun onAdDrag(ad: BannerAdWrapper, x: Float, y: Float)
}

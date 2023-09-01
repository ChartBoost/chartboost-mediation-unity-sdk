package com.chartboost.mediation.unity

import com.chartboost.heliumsdk.ad.HeliumBannerAd

interface ChartboostMediationBannerViewListener {
    public abstract fun onAdCached(ad: BannerAdWrapper, error:String)
    
    public abstract fun onAdRefreshed(ad: BannerAdWrapper)
    
    public abstract fun onAdClicked(ad: BannerAdWrapper): kotlin.Unit

    public abstract fun onAdImpressionRecorded(ad: BannerAdWrapper): kotlin.Unit
}
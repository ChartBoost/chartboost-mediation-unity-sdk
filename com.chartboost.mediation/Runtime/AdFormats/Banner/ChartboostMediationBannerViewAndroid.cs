#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Events;
using Chartboost.Platforms.Android;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using static Chartboost.Platforms.Android.ChartboostMediationAndroid;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Banner
{
    public class ChartboostMediationBannerViewAndroid : ChartboostMediationBannerViewBase
    {
        private readonly AndroidJavaObject _bannerAd;
        private BannerEventListener _bannerEventListener;
        internal bool IsPubTriggeredLoad;
        internal Later<ChartboostMediationBannerAdLoadResult> AdLoadResult;
        
        public ChartboostMediationBannerViewAndroid(AndroidJavaObject bannerAd) : base(new IntPtr(bannerAd.HashCode()))
        {
            LogTag = "ChartboostMediationBanner (Android)";
            _bannerAd = bannerAd;
        }

        public override Dictionary<string, string> Keywords
        {
            get => _bannerAd.Call<AndroidJavaObject>("getKeywords").ToKeywords();
            set => _bannerAd.Call("setKeywords", value.ToKeywords());
        }

        public override ChartboostMediationBannerAdLoadRequest Request { get; protected set; }

        public override BidInfo WinningBidInfo
        {
            get => _bannerAd.Get<AndroidJavaObject>("winningBidInfo").MapToWinningBidInfo();
            protected set { }
        }

        public override string LoadId
        {
            get => _bannerAd.Get<string>("loadId");
            protected set { }
        }

        public override Metrics? LoadMetrics
        {
            get => _bannerAd.Get<AndroidJavaObject>("loadMetrics").JsonObjectToMetrics();
            protected set { }
        }

        public override ChartboostMediationBannerAdSize AdSize
        {
            get
            {
                var sizeJson = _bannerAd.Call<string>("getAdSize");
                return JsonConvert.DeserializeObject<ChartboostMediationBannerAdSize>(sizeJson);
            }
            protected set { }
        }

        public override ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            get => (ChartboostMediationBannerHorizontalAlignment)_bannerAd.Get<int>("getHorizontalAlignment");
            set => _bannerAd.Call("setHorizontalAlignment", (int)value);
        }

        public override ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            get => (ChartboostMediationBannerVerticalAlignment)_bannerAd.Get<int>("getVerticalAlignment");
            set => _bannerAd.Call("setVerticalAlignment", (int)value);
        }

        public override void Reset()
        {
            _bannerAd.Call("reset"); ;
        }

        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            await base.Load(request, screenLocation);

            if (AdLoadResult != null)
            {
                Logger.LogWarning(LogTag, "A new load is triggered while the previous load is not yet complete");
            }
            else
            {
                AdLoadResult = new Later<ChartboostMediationBannerAdLoadResult>();
                _bannerAd.Call("load", request.PlacementName, request.AdSize.Name, request.AdSize.Width, request.AdSize.Height, screenLocation);
            }
            
            var result = await AdLoadResult;
            AdLoadResult = null;
            return result;
        }
    }
    
    internal class ChartboostMediationBannerAdListener : AndroidJavaProxy
    {
        public ChartboostMediationBannerAdListener() : base(GetQualifiedClassName("ChartboostMediationBannerViewListener")) {}
        
        private void onAdCached(AndroidJavaObject ad, string error)
        {
            var bannerView = CacheManager.GetBannerAd(ad.HashCode());
            if (!(bannerView is ChartboostMediationBannerViewAndroid androidBannerView)) 
                return;
            
            // auto refresh load
            if (androidBannerView.AdLoadResult == null)
            {
                EventProcessor.ProcessChartboostMediationBannerEvent(ad.HashCode(), (int)EventProcessor.BannerAdEvents.Show);
                return;
            }
                
            // Publisher triggered load 
            ChartboostMediationBannerAdLoadResult loadResult;
            if (!string.IsNullOrEmpty(error))
            {
                loadResult = new ChartboostMediationBannerAdLoadResult(new ChartboostMediationError(error));
            }
            else
            {
                loadResult = new ChartboostMediationBannerAdLoadResult(bannerView.LoadId, null, null);
                EventProcessor.ProcessChartboostMediationBannerEvent(ad.HashCode(), (int)EventProcessor.BannerAdEvents.Show);
            }

            androidBannerView.AdLoadResult.Complete(loadResult);
        }

        private void onAdClicked(AndroidJavaObject ad) =>
            EventProcessor.ProcessChartboostMediationBannerEvent(ad.HashCode(), (int)EventProcessor.BannerAdEvents.Click);

        private void onAdImpressionRecorded(AndroidJavaObject ad) =>
            EventProcessor.ProcessChartboostMediationBannerEvent(ad.HashCode(), (int)EventProcessor.BannerAdEvents.RecordImpression);

    }
}

#endif
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
using UnityEngine;
using static Chartboost.Platforms.Android.ChartboostMediationAndroid;

namespace Chartboost.AdFormats.Banner
{
    public class ChartboostMediationBannerViewAndroid : ChartboostMediationBannerViewBase
    {
        private readonly AndroidJavaObject _bannerAd;
        private BannerEventListener _bannerEventListener;
        
        public ChartboostMediationBannerViewAndroid(AndroidJavaObject bannerAd) : base(new IntPtr(bannerAd.HashCode()))
        {
            LogTag = "ChartboostMediationBanner (Android)";
            _bannerAd = bannerAd;
        }

        public override Dictionary<string, string> Keywords
        {
            get => _bannerAd.Get<AndroidJavaObject>("keywords").ToKeywords();
            set => _bannerAd.Set("keywords", value.ToKeywords());
        }

        public override ChartboostMediationBannerAdLoadRequest Request { get; internal set; }

        public override BidInfo WinningBidInfo
        {
            get => _bannerAd.Get<AndroidJavaObject>("winningBidInfo").MapToWinningBidInfo();
            protected set { }
        }

        public override Metrics? LoadMetrics
        {
            get => _bannerAd.Get<AndroidJavaObject>("loadMetrics").JsonObjectToMetrics();
            protected set { }
        }

        public override ChartboostMediationBannerSize Size
        {
            get => _bannerAd.Get<AndroidJavaObject>("size").ToChartboostMediationBannerSize();
            protected set { }
        }

        public override ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            // TODO : Enum in Native might need a bridge function to cast from int 
            get => (ChartboostMediationBannerHorizontalAlignment)_bannerAd.Get<int>("horizontalAlignment");
            set => _bannerAd.Set("horizontalAlignment", (int)value);
        }

        public override ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            // TODO : Enum in Native might need a bridge function to cast from int 
            get => (ChartboostMediationBannerVerticalAlignment)_bannerAd.Get<int>("verticalAlignment");
            set => _bannerAd.Set("verticalAlignment", (int)value);
        }

        public override void Reset()
        {
            _bannerAd.Call("reset"); ;
        }

        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            await base.Load(request, screenLocation);
            
            var adLoadListenerAwaitableProxy = new ChartboostMediationBannerAdLoadListener();
            

            try
            {
                // size
                var width = request.Size.Width;
                var height = request.Size.Height;
                var sizeClass = new AndroidJavaClass(GetQualifiedNativeClassName("HeliumBannerSize", true));
                var size = sizeClass.CallStatic<AndroidJavaObject>("bannerSize", width, height);
                
                _bannerAd.Call("load", request.PlacementName, size);
                
            }
            catch (NullReferenceException e)
            {
                EventProcessor.ReportUnexpectedSystemError(e.ToString());
            }

            return await adLoadListenerAwaitableProxy;
        }
    }
}

#endif
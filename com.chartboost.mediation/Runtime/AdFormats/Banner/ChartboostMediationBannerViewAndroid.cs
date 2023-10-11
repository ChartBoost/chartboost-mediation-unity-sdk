#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Events;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Banner
{
    internal class ChartboostMediationBannerViewAndroid : ChartboostMediationBannerViewBase
    {
        private readonly AndroidJavaObject _bannerAd;
        internal Later<ChartboostMediationBannerAdLoadResult> LoadRequest;

        private Dictionary<string, string> _keywords = new Dictionary<string, string>();

        public ChartboostMediationBannerViewAndroid(AndroidJavaObject bannerAd) : base(new IntPtr(bannerAd.HashCode()))
        {
            LogTag = "ChartboostMediationBanner (Android)";
            _bannerAd = bannerAd;
            
            // TODO: this can be also done on bridge?   
            AndroidAdStore.TrackBannerAd(bannerAd);
        }

        public override Dictionary<string, string> Keywords
        {
            get => _keywords;
            set
            {
                try
                {
                    _bannerAd.Call("setKeywords", value.ToKeywords());
                    _keywords = value;
                }
                catch (Exception e)
                {
                    EventProcessor.ReportUnexpectedSystemError($"Error setting keywords => {e.Message}");
                }
            }
        }

        public override ChartboostMediationBannerAdLoadRequest Request { get; protected set; }

        public override BidInfo WinningBidInfo
        {
            get
            {
                var winningBidInfo = _bannerAd.Get<AndroidJavaObject>("winningBidInfo");
                return winningBidInfo?.MapToWinningBidInfo() ?? new BidInfo();
            }
            protected set { }
        }

        public override string LoadId
        {
            get => _bannerAd.Get<string>("loadId");
            protected set { }
        }

        // Note: This is currently only available in iOS but not on Android   
        // Android will include this from 5.0, public API `IChartboostMediationBannerView` will then include this field as well
        public override Metrics? LoadMetrics
        {
            get => null;
            protected set { }
        }

        public override ChartboostMediationBannerSize? AdSize
        {
            get
            {
                var sizeJson = _bannerAd.Call<string>("getAdSize");
                if (string.IsNullOrEmpty(sizeJson))
                    return null;
                return JsonConvert.DeserializeObject<ChartboostMediationBannerSize>(sizeJson);
            }
            protected set { }
        }

        public override ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            get => (ChartboostMediationBannerHorizontalAlignment)_bannerAd.Call<int>("getHorizontalAlignment");
            set => _bannerAd.Call("setHorizontalAlignment", (int)value);
        }

        public override ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            get => (ChartboostMediationBannerVerticalAlignment)_bannerAd.Call<int>("getVerticalAlignment");
            set => _bannerAd.Call("setVerticalAlignment", (int)value);
        }

        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            await base.Load(request, screenLocation);
            Request = request;

            if (LoadRequest != null)
            {
                Logger.LogWarning(LogTag, "A new load is triggered while the previous load is not yet complete. Discarding previous load.");
                LoadRequest = null;
            }
            
            LoadRequest = new Later<ChartboostMediationBannerAdLoadResult>();
            _bannerAd.Call("load", request.PlacementName, (int)request.Size.SizeType, request.Size.Width, request.Size.Height, (int)screenLocation);
            
            var result = await LoadRequest;
            LoadRequest = null;
            return result;
        }
        
        // x,y is Native
        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, float x, float y)
        {
            await base.Load(request, x, y);
            Request = request;

            if (LoadRequest != null)
            {
                Logger.LogWarning(LogTag, "A new load is triggered while the previous load is not yet complete. Discarding previous load.");
                LoadRequest = null;
            }
            
            LoadRequest = new Later<ChartboostMediationBannerAdLoadResult>();
            // y is counted from top in Android whereas Unity counts it from bottom
            y = ChartboostMediationConverters.PixelsToNative(Screen.height) - y;
            _bannerAd.Call("load", request.PlacementName, (int)request.Size.SizeType, request.Size.Width, request.Size.Height, x, y);

            var result = await LoadRequest;
            LoadRequest = null;
            return result;
        }

        public override void ResizeToFit(ChartboostMediationBannerResizeAxis axis = ChartboostMediationBannerResizeAxis.Both,
            Vector2 pivot = default)
        {
            base.ResizeToFit(axis, pivot);
            _bannerAd.Call("resizeToFit", (int)axis, pivot.x, 1 - pivot.y);
        }

        public override void SetDraggability(bool canDrag)
        {
            base.SetDraggability(canDrag);
            _bannerAd.Call("setDraggability", canDrag);
        }

        public override void SetVisibility(bool visibility)
        {
            base.SetVisibility(visibility);
            _bannerAd.Call("setVisibility", visibility);
        }

        public override void Reset()
        {
            base.Reset();
            _bannerAd.Call("reset");
        }

        public override void Destroy()
        {
            base.Destroy();
            _bannerAd.Call("destroy");
            _bannerAd.Dispose();
            AndroidAdStore.ReleaseBannerAd(UniqueId.ToInt32());
        }

        internal override void MoveTo(float x, float y)
        {
            base.MoveTo(x,y);
            y = ChartboostMediationConverters.PixelsToNative(Screen.height) - y;
            _bannerAd.Call("moveTo", x, y);
        }
    }
}

#endif

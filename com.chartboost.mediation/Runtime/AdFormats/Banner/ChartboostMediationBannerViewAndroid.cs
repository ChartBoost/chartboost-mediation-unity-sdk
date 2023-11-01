#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Events;
using Chartboost.Requests;
using Chartboost.Utilities;
using UnityEngine;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Banner
{
    /// <summary>
    /// Android implementation of ChartboostMediationBannerViewBase
    /// </summary>
    internal sealed class ChartboostMediationBannerViewAndroid : ChartboostMediationBannerViewBase
    {
        private readonly AndroidJavaObject _bannerAd;
        internal Later<ChartboostMediationBannerAdLoadResult> LoadRequest;

        private Dictionary<string, string> _keywords = new Dictionary<string, string>();
        
        private const string WarningOverlapLoad = "A new load is triggered while the previous load is not yet complete. Discarding previous load.";

        public ChartboostMediationBannerViewAndroid(AndroidJavaObject bannerAd) : base(new IntPtr(bannerAd.HashCode()))
        {
            LogTag = "ChartboostMediationBanner (Android)";
            _bannerAd = bannerAd;
            
            // TODO: this can be also done on bridge?   
            AndroidAdStore.TrackBannerAd(bannerAd);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Keywords"/>
        public override Dictionary<string, string> Keywords
        {
            get => _keywords;
            set
            {
                try
                {
                    _bannerAd.Call(AndroidConstants.FunSetKeywords, value.ToKeywords());
                    _keywords = value;
                }
                catch (Exception e)
                {
                    EventProcessor.ReportUnexpectedSystemError($"Error setting keywords => {e.Message}");
                }
            }
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Request"/>
        public override ChartboostMediationBannerAdLoadRequest Request { get; protected set; }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.WinningBidInfo"/>
        public override BidInfo WinningBidInfo
        {
            get
            {
                var winningBidInfo = _bannerAd.Get<AndroidJavaObject>(AndroidConstants.PropertyWinningBidInfo);
                return winningBidInfo?.MapToWinningBidInfo() ?? new BidInfo();
            }
            protected set { }
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.LoadId"/>
        public override string LoadId
        {
            get => _bannerAd.Get<string>(AndroidConstants.PropertyLoadId);
            protected set { }
        }

        // Note: This is currently only available in iOS but not on Android   
        // Android will include this from 5.0, public API `IChartboostMediationBannerView` will then include this field as well
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.LoadMetrics"/>
        public override Metrics? LoadMetrics
        {
            get => null;
            protected set { }
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.AdSize"/>
        public override ChartboostMediationBannerSize? AdSize
        {
            get => _bannerAd.Call<AndroidJavaObject>(AndroidConstants.FunGetAdSize).ToChartboostMediationBannerSize();
            protected set { }
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.ContainerSize"/>
        public override ChartboostMediationBannerSize? ContainerSize
        {
            get => _bannerAd.Call<AndroidJavaObject>(AndroidConstants.FunGetContainerSize).ToChartboostMediationBannerSize();
            protected set { }
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.HorizontalAlignment"/>
        public override ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            get => (ChartboostMediationBannerHorizontalAlignment)_bannerAd.Call<int>(AndroidConstants.FunGetHorizontalAlignment);
            set => _bannerAd.Call(AndroidConstants.FunSetHorizontalAlignment, (int)value);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.VerticalAlignment"/>
        public override ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            get => (ChartboostMediationBannerVerticalAlignment)_bannerAd.Call<int>(AndroidConstants.FunGetVerticalAlignment);
            set => _bannerAd.Call(AndroidConstants.FunSetVerticalAlignment, (int)value);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Load(Chartboost.Requests.ChartboostMediationBannerAdLoadRequest,Chartboost.Banner.ChartboostMediationBannerAdScreenLocation)"/>
        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            await base.Load(request, screenLocation);
            Request = request;

            if (LoadRequest != null)
            {
                Logger.LogWarning(LogTag, WarningOverlapLoad);
                LoadRequest = null;
            }
            
            LoadRequest = new Later<ChartboostMediationBannerAdLoadResult>();
            _bannerAd.Call(AndroidConstants.FunLoad, request.PlacementName, (int)request.Size.SizeType, request.Size.Width, request.Size.Height, (int)screenLocation);
            
            var result = await LoadRequest;
            LoadRequest = null;
            return result;
        }
        
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Load(Chartboost.Requests.ChartboostMediationBannerAdLoadRequest,float, float)"/>
        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, float x, float y)
        {
            await base.Load(request, x, y);
            Request = request;

            if (LoadRequest != null)
            {
                Logger.LogWarning(LogTag, WarningOverlapLoad);
                LoadRequest = null;
            }
            
            LoadRequest = new Later<ChartboostMediationBannerAdLoadResult>();
            // y is counted from top in Android whereas Unity counts it from bottom
            y = ChartboostMediationConverters.PixelsToNative(Screen.height) - y;
            _bannerAd.Call(AndroidConstants.FunLoad, request.PlacementName, (int)request.Size.SizeType, request.Size.Width, request.Size.Height, x, y);

            var result = await LoadRequest;
            LoadRequest = null;
            return result;
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.ResizeToFit"/>
        public override void ResizeToFit(ChartboostMediationBannerResizeAxis axis = ChartboostMediationBannerResizeAxis.Both,
            Vector2 pivot = default)
        {
            base.ResizeToFit(axis, pivot);
            _bannerAd.Call(AndroidConstants.FunResizeToFit, (int)axis, pivot.x, 1 - pivot.y);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.SetDraggability"/>
        public override void SetDraggability(bool canDrag)
        {
            base.SetDraggability(canDrag);
            _bannerAd.Call(AndroidConstants.FunSetDraggability, canDrag);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.SetVisibility"/>
        public override void SetVisibility(bool visibility)
        {
            base.SetVisibility(visibility);
            _bannerAd.Call(AndroidConstants.FunSetVisibility, visibility);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Reset"/>
        public override void Reset()
        {
            base.Reset();
            _bannerAd.Call(AndroidConstants.FunReset);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Destroy"/>
        public override void Destroy()
        {
            base.Destroy();
            _bannerAd.Call(AndroidConstants.FunDestroy);
            _bannerAd.Dispose();
            AndroidAdStore.ReleaseBannerAd(UniqueId.ToInt32());
        }

        internal override void MoveTo(float x, float y)
        {
            base.MoveTo(x,y);
            y = ChartboostMediationConverters.PixelsToNative(Screen.height) - y;
            _bannerAd.Call(AndroidConstants.FunMoveTo, x, y);
        }
    }
}
#endif

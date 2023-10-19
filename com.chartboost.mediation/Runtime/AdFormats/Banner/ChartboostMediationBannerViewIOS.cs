#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using static Chartboost.Platforms.IOS.ChartboostMediationIOS;

namespace Chartboost.AdFormats.Banner
{
    /// <summary>
    /// IOS implementation of ChartboostMediationBannerViewBase
    /// </summary>
    internal class ChartboostMediationBannerViewIOS : ChartboostMediationBannerViewBase
    {
        private Dictionary<string, string> _keywords = new Dictionary<string, string>();

        public ChartboostMediationBannerViewIOS(IntPtr uniqueId) : base(uniqueId)
        {
            LogTag = "ChartboostMediationBanner (IOS)";
        }
        
        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Keywords"/>
        public override Dictionary<string, string> Keywords
        {
            get => _keywords;
            set
            {
                var keywordsJson = string.Empty;
                if (_keywords.Count > 0)
                    keywordsJson = JsonConvert.SerializeObject(_keywords);
                
                _chartboostMediationBannerViewSetKeywords(UniqueId, keywordsJson);
                _keywords = value;
            }
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Request"/>
        public override ChartboostMediationBannerAdLoadRequest Request { get; protected set; }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.WinningBidInfo"/>
        public override BidInfo WinningBidInfo
        {
            get
            {
                var winningBidJson = _chartboostMediationBannerViewGetWinningBidInfo(UniqueId);
                var winningBid = string.IsNullOrEmpty(winningBidJson) ? new BidInfo() : JsonConvert.DeserializeObject<BidInfo>(winningBidJson);
                return winningBid;
            }
            protected set { }
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.LoadId"/>
        public override string LoadId
        {
            // TODO: why metrics is a list ?
            get => LoadMetrics?.metrics != null ? LoadMetrics?.metrics.FirstOrDefault().loadId : "";
            protected set {}
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.LoadMetrics"/>
        public override Metrics? LoadMetrics
        {
            get
            {
                var metricsJson = _chartboostMediationBannerViewGetLoadMetrics(UniqueId);
                var metrics = string.IsNullOrEmpty(metricsJson) ? new Metrics() : JsonConvert.DeserializeObject<Metrics>(metricsJson);
                return metrics;
            }
            protected set { }
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.AdSize"/>
        public override ChartboostMediationBannerSize? AdSize
        {
            get
            {
                var sizeJson = _chartboostMediationBannerViewGetAdSize(UniqueId);
                var size = string.IsNullOrEmpty(sizeJson) ? new ChartboostMediationBannerSize(): JsonConvert.DeserializeObject<ChartboostMediationBannerSize>(sizeJson);
                return size;
            }
            protected set { }
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.ContainerSize"/>
        public override ChartboostMediationBannerSize? ContainerSize
        {
            get
            {
                var sizeJson = _chartboostMediationBannerViewGetContainerSize(UniqueId);
                var size = string.IsNullOrEmpty(sizeJson) ? new ChartboostMediationBannerSize(): JsonConvert.DeserializeObject<ChartboostMediationBannerSize>(sizeJson);
                return size;
            }
            protected set { }
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.HorizontalAlignment"/>
        public override ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            get =>  (ChartboostMediationBannerHorizontalAlignment)_chartboostMediationBannerViewGetHorizontalAlignment(UniqueId);
            set => _chartboostMediationBannerViewSetHorizontalAlignment(UniqueId, (int)value);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.VerticalAlignment"/>
        public override ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            get =>  (ChartboostMediationBannerVerticalAlignment)_chartboostMediationBannerViewGetVerticalAlignment(UniqueId);
            set => _chartboostMediationBannerViewSetVerticalAlignment(UniqueId, (int)value);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Load(Chartboost.Requests.ChartboostMediationBannerAdLoadRequest,Chartboost.Banner.ChartboostMediationBannerAdScreenLocation)"/>
        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            Request = request;
            await base.Load(request, screenLocation);
            
            var (proxy, hashCode) = _setupProxy<ChartboostMediationBannerAdLoadResult>();
            CacheManager.TrackBannerAdLoadRequest(hashCode, request);

            var placement = request.PlacementName;
            var sizeType = request.Size.SizeType;
            var sizeWidth = request.Size.Width;
            var sizeHeight = request.Size.Height;
            
            _chartboostMediationBannerViewLoadAdWithScreenPos(UniqueId, placement, (int)sizeType, sizeWidth, sizeHeight, (int)screenLocation, hashCode, BannerAdLoadResultCallbackProxy);
            
            var result = await proxy;
            return result;
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Load(Chartboost.Requests.ChartboostMediationBannerAdLoadRequest,float, float)"/>
        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, float x, float y)
        {
            Request = request;
            await base.Load(request, x, y);
            
            var (proxy, hashCode) = _setupProxy<ChartboostMediationBannerAdLoadResult>();
            CacheManager.TrackBannerAdLoadRequest(hashCode, request);

            var placement = request.PlacementName;
            var sizeType = request.Size.SizeType;
            var sizeWidth = request.Size.Width;
            var sizeHeight = request.Size.Height;
            // y is counted from top in iOS whereas Unity counts it from bottom
            y = ChartboostMediationConverters.PixelsToNative(Screen.height) - y;
            _chartboostMediationBannerViewLoadAdWithXY(UniqueId, placement, (int)sizeType, sizeWidth, sizeHeight, x, y, hashCode, BannerAdLoadResultCallbackProxy);
            
            var result = await proxy;
            return result;
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.ResizeToFit"/>
        public override void ResizeToFit(ChartboostMediationBannerResizeAxis axis = ChartboostMediationBannerResizeAxis.Both,
            Vector2 pivot = default)
        {
            base.ResizeToFit(axis, pivot);
            _chartboostMediationBannerViewResizeToFit(UniqueId, (int)axis, pivot.x, 1-pivot.y);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.SetDraggability"/>
        public override void SetDraggability(bool canDrag)
        {
            base.SetDraggability(canDrag);
            _chartboostMediationBannerViewSetDraggability(UniqueId, canDrag);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.SetVisibility"/>
        public override void SetVisibility(bool visibility)
        {
            base.SetVisibility(visibility);
            _chartboostMediationBannerViewSetVisibility(UniqueId, visibility);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Reset"/>
        public override void Reset()
        {
            base.Reset();
            _chartboostMediationBannerViewReset(UniqueId);
        }

        /// <inheritdoc cref="ChartboostMediationBannerViewBase.Destroy"/>
        public override void Destroy()
        {
            base.Destroy();
            _chartboostMediationBannerViewDestroy(UniqueId);   
        }

        internal override void MoveTo(float x, float y)
        {
            base.MoveTo(x,y);
            y = ChartboostMediationConverters.PixelsToNative(Screen.height) - y;
            _chartboostMediationBannerViewMoveTo(UniqueId, x, y);   
        }

        #region Native
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewLoadAdWithScreenPos(IntPtr uniqueId, string placementName, int sizeType, float width, float height, int screenLocation, int hashCode, ExternChartboostMediationBannerAdLoadResultEvent callback);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewLoadAdWithXY(IntPtr uniqueId, string placementName, int sizeType, float width, float height, float x, float y, int hashCode, ExternChartboostMediationBannerAdLoadResultEvent callback);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewSetKeywords(IntPtr uniqueId, string keywords);

        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerViewGetAdSize(IntPtr uniqueId);

        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerViewGetContainerSize(IntPtr uniqueId);

        [DllImport("__Internal")] [CanBeNull]
        private static extern string _chartboostMediationBannerViewGetWinningBidInfo(IntPtr uniqueId);

        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerViewGetLoadMetrics(IntPtr uniqueId);
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewSetHorizontalAlignment(IntPtr uniqueId, int horizontalAlignment );
        
        [DllImport("__Internal")]
        private static extern int _chartboostMediationBannerViewGetHorizontalAlignment(IntPtr uniqueId);
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewSetVerticalAlignment(IntPtr uniqueId, int verticalAlignment );
        
        [DllImport("__Internal")]
        private static extern int _chartboostMediationBannerViewGetVerticalAlignment(IntPtr uniqueId);
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewResizeToFit(IntPtr uniqueId, int axis, float pivotX, float pivotY );
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewSetDraggability(IntPtr uniqueId, bool canDrag );

        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewSetVisibility(IntPtr uniqueId, bool visible );
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewReset(IntPtr uniqueId);
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewDestroy(IntPtr uniqueId);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerViewMoveTo(IntPtr uniqueId, float x, float y);   
        
        #endregion
    }
}
#endif

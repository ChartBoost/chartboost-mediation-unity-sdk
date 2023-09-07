#if UNITY_IPHONE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Platforms;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using static Chartboost.Platforms.IOS.ChartboostMediationIOS;


namespace Chartboost.AdFormats.Banner
{
    internal class ChartboostMediationBannerViewIOS : ChartboostMediationBannerViewBase
    {
        private Dictionary<string, string> _keywords = new Dictionary<string, string>();

        public ChartboostMediationBannerViewIOS(IntPtr uniqueId) : base(uniqueId)
        {
            LogTag = "ChartboostMediationBanner (IOS)";
        }
        
        public override Dictionary<string, string> Keywords
        {
            get => _keywords;
            set
            {
                var keywordsJson = string.Empty;
                if (_keywords.Count > 0)
                    keywordsJson = JsonConvert.SerializeObject(_keywords);
                
                _chartboostMediationBannerSetKeywords(UniqueId, keywordsJson);
                _keywords = value;

            }
        }
        public override ChartboostMediationBannerAdLoadRequest Request { get; protected set; }

        public override BidInfo WinningBidInfo
        {
            get
            {
                var winningBidJson = _chartboostMediationBannerAdGetWinningBidInfo(UniqueId);
                var winningBid = string.IsNullOrEmpty(winningBidJson) ? new BidInfo() : JsonConvert.DeserializeObject<BidInfo>(winningBidJson);
                return winningBid;
            }
            protected set { }
        }
        public override string LoadId
        {
            // TODO: why metrics is a list ?
            get => LoadMetrics?.metrics.FirstOrDefault().loadId; 
            protected set {} 
        }

        public override Metrics? LoadMetrics
        {
            get
            {
                var metricsJson = _chartboostMediationBannerAdGetLoadMetrics(UniqueId);
                var metrics = string.IsNullOrEmpty(metricsJson) ? new Metrics() : JsonConvert.DeserializeObject<Metrics>(metricsJson);
                return metrics;
            }
            protected set { }
        }

        public override ChartboostMediationBannerAdSize AdSize
        {
            get
            {
                var sizeJson = _chartboostMediationBannerAdGetSize(UniqueId);
                var size = string.IsNullOrEmpty(sizeJson) ? new ChartboostMediationBannerAdSize(): JsonConvert.DeserializeObject<ChartboostMediationBannerAdSize>(sizeJson);
                return size;
            }
            protected set { }
        }

        public override ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            get =>  (ChartboostMediationBannerHorizontalAlignment)_chartboostMediationBannerGetHorizontalAlignment(UniqueId);
            set => _chartboostMediationBannerSetHorizontalAlignment(UniqueId, (int)value);
        }

        public override ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            get =>  (ChartboostMediationBannerVerticalAlignment)_chartboostMediationBannerGetVerticalAlignment(UniqueId);
            set => _chartboostMediationBannerSetVerticalAlignment(UniqueId, (int)value);
        }

        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            await base.Load(request, screenLocation);
            
            var (proxy, hashCode) = _setupProxy<ChartboostMediationBannerAdLoadResult>();
            CacheManager.TrackBannerAdLoadRequest(hashCode, request);

            var placement = request.PlacementName;
            var sizeName = request.Size.Name;
            var sizeWidth = request.Size.Width;
            var sizeHeight = request.Size.Height;
            
            _chartboostMediationBannerLoad(UniqueId, placement, sizeName, sizeWidth, sizeHeight, (int)screenLocation, hashCode, BannerAdLoadResultCallbackProxy);
            
            var result = await proxy;
            return result;
        }

        public override void Reset()
        {
            base.Reset();
            _chartboostMediationBannerReset(UniqueId);
        }

        public override void Destroy()
        {
            base.Destroy();
            _chartboostMediationBannerDestroy(UniqueId);   
        }

        #region Native
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerLoad(IntPtr UniqueId, string placementName, string sizeName, float width, float height, int screenLocation, int hashCode, ExternChartboostMediationBannerAdLoadResultEvent callback);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerSetKeywords(IntPtr UniqueId, string keywords);

        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerAdGetSize(IntPtr UniqueId);

        [DllImport("__Internal")] [CanBeNull]
        private static extern string _chartboostMediationBannerAdGetWinningBidInfo(IntPtr UniqueId);

        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerAdGetLoadMetrics(IntPtr UniqueId);
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerSetHorizontalAlignment(IntPtr UniqueId, int horizontalAlignment );
        
        [DllImport("__Internal")]
        private static extern int _chartboostMediationBannerGetHorizontalAlignment(IntPtr UniqueId);
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerSetVerticalAlignment(IntPtr UniqueId, int verticalAlignment );
        
        [DllImport("__Internal")]
        private static extern int _chartboostMediationBannerGetVerticalAlignment(IntPtr UniqueId);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerReset(IntPtr uniqueId);
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerDestroy(IntPtr uniqueId);
        
        #endregion
    }
}
#endif
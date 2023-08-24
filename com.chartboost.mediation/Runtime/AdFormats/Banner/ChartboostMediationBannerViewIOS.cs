#if UNITY_IPHONE
using System;
using System.Collections.Generic;
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
    public class ChartboostMediationBannerViewIOS : ChartboostMediationBannerViewBase
    {
        private Dictionary<string, string> _keywords;

        public ChartboostMediationBannerViewIOS(IntPtr uniqueId) : base(uniqueId)
        {
            LogTag = "ChartboostMediationBanner (IOS)";
        }
        
        public override Dictionary<string, string> Keywords
        {
            get => _keywords;
            set
            {
                _keywords = value;
                var keywordsJson = string.Empty;
                if (_keywords.Count > 0)
                    keywordsJson = JsonConvert.SerializeObject(_keywords);
                
                _chartboostMediationBannerSetKeywords(uniqueId, keywordsJson);
            }
        }
        public override ChartboostMediationBannerAdLoadRequest Request { get; internal set; }

        public override BidInfo WinningBidInfo
        {
            get
            {
                var winningBidJson = _chartboostMediationBannerAdGetWinningBidInfo(uniqueId);
                var winningBid = string.IsNullOrEmpty(winningBidJson) ? new BidInfo() : JsonConvert.DeserializeObject<BidInfo>(winningBidJson);
                return winningBid;
            }
            protected set { }
        }

        public override Metrics? LoadMetrics
        {
            get
            {
                var metricsJson = _chartboostMediationBannerAdGetLoadMetrics(uniqueId);
                var metrics = string.IsNullOrEmpty(metricsJson) ? new Metrics() : JsonConvert.DeserializeObject<Metrics>(metricsJson);
                return metrics;
            }
            protected set { }
        }

        public override ChartboostMediationBannerSize Size
        {
            get
            {
                var sizeJson = _chartboostMediationBannerAdGetSize(uniqueId);
                var size = string.IsNullOrEmpty(sizeJson) ? ChartboostMediationBannerSize.Adaptive(0,0): JsonConvert.DeserializeObject<ChartboostMediationBannerSize>(sizeJson);
                return size;
            }
            protected set { }
        }

        public override ChartboostMediationBannerHorizontalAlignment HorizontalAlignment
        {
            get =>  (ChartboostMediationBannerHorizontalAlignment)_chartboostMediationBannerGetHorizontalAlignment(uniqueId);
            set => _chartboostMediationBannerSetHorizontalAlignment(uniqueId, (int)value);
        }

        public override ChartboostMediationBannerVerticalAlignment VerticalAlignment
        {
            get =>  (ChartboostMediationBannerVerticalAlignment)_chartboostMediationBannerGetVerticalAlignment(uniqueId);
            set => _chartboostMediationBannerSetVerticalAlignment(uniqueId, (int)value);
        }

        public override async Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            await base.Load(request, screenLocation);
            
            var (proxy, hashCode) = _setupProxy<ChartboostMediationBannerAdLoadResult>();
            CacheManager.TrackBannerAdLoadRequest(hashCode, request);

            var placement = request.PlacementName;
            

            if(request.Size.BannerType == ChartboostMediationBannerType.Adaptive)
            {
                var width = request.Size.Width;
                var height = request.Size.Height;
                _chartboostMediationBannerLoadAdaptiveSize(uniqueId, placement, width, height, (int)screenLocation, hashCode, BannerAdLoadResultCallbackProxy);
            }
            else
            {
                var fixedSize = request.Size.AspectRatio switch
                {
                    320f / 50f => 0, // standard
                    300f / 250f => 1,// medium
                    728f / 90f => 2,// leaderboard
                    _ => 0
                };
                _chartboostMediationBannerLoadFixedSize(uniqueId, placement, fixedSize, (int)screenLocation, hashCode, BannerAdLoadResultCallbackProxy);
            }

            var result = await proxy;
            return result;
        }

        public override void Reset()
        {
            _chartboostMediationBannerReset(uniqueId);
        }

        #region Native
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerLoadAdaptiveSize(IntPtr uniqueId, string placementName, float width, float height, int screenLocation, int hashCode, ExternChartboostMediationBannerAdLoadResultEvent callback);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerLoadFixedSize(IntPtr uniqueId, string placementName, int fixedSize, int screenLocation, int hashCode, ExternChartboostMediationBannerAdLoadResultEvent callback);

        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerSetKeywords(IntPtr uniqueId, string keywords);

        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerAdGetSize(IntPtr uniqueId);

        [DllImport("__Internal")] [CanBeNull]
        private static extern string _chartboostMediationBannerAdGetWinningBidInfo(IntPtr uniqueId);

        [DllImport("__Internal")]
        private static extern string _chartboostMediationBannerAdGetLoadMetrics(IntPtr uniqueId);
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerSetHorizontalAlignment(IntPtr uniqueId, int horizontalAlignment );
        
        [DllImport("__Internal")]
        private static extern int _chartboostMediationBannerGetHorizontalAlignment(IntPtr uniqueId);
        
        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerSetVerticalAlignment(IntPtr uniqueId, int verticalAlignment );
        
        [DllImport("__Internal")]
        private static extern int _chartboostMediationBannerGetVerticalAlignment(IntPtr uniqueId);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationBannerReset(IntPtr uniqueI);
        
        #endregion
    }
}
#endif
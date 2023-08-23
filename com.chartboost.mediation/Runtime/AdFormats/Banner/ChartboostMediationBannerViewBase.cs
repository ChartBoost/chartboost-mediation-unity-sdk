using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Platforms;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Banner
{
    [Serializable]
    public abstract class ChartboostMediationBannerViewBase : IChartboostMediationBannerView
    {
        public event ChartboostMediationBannerEvent WillAppear;
        public event ChartboostMediationBannerEvent DidClick;
        public event ChartboostMediationBannerEvent DidRecordImpression;
        
        protected static string LogTag = "ChartboostMediationBanner (Base)";
        protected IntPtr uniqueId { get; set; }

        protected ChartboostMediationBannerViewBase() => Initialize();
        
        protected ChartboostMediationBannerViewBase(IntPtr uniqueId)
        {
            this.uniqueId = uniqueId;
            CacheManager.TrackBannerAd(uniqueId.ToInt64(), this);
            
            Initialize();
        }

        protected void Initialize()
        {
            Size = new ChartboostMediationBannerSize();
            Keywords = new Dictionary<string, string>();
            HorizontalAlignment = ChartboostMediationBannerHorizontalAlignment.Center;
            VerticalAlignment = ChartboostMediationBannerVerticalAlignment.Center;
        }

        public abstract Dictionary<string, string> Keywords { get; set; } 
        public abstract ChartboostMediationBannerAdLoadRequest Request { get; internal set; }
        public abstract BidInfo WinningBidInfo { get; protected set;  }
        public abstract Metrics? LoadMetrics { get; protected set;  }
        public abstract ChartboostMediationBannerSize Size { get; protected set; }
        public abstract ChartboostMediationBannerHorizontalAlignment HorizontalAlignment { get; set; }
        public abstract ChartboostMediationBannerVerticalAlignment VerticalAlignment { get; set; }
        public virtual Task<ChartboostMediationBannerAdLoadResult> Load(
            ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            Request = request;

            if (!CanFetchAd(request.PlacementName))
            {
                var error = new ChartboostMediationError("Chartboost Mediation is not ready or placement is invalid.");
                var adLoadResult = new ChartboostMediationBannerAdLoadResult(error);
                return Task.FromResult(adLoadResult);
            }
            
            return Task.FromResult<ChartboostMediationBannerAdLoadResult>(null);
        }

        public abstract void Reset();
        
        internal virtual void OnBannerWillAppear(IChartboostMediationBannerView bannerView)  => 
            WillAppear?.Invoke(bannerView);

        internal virtual void OnBannerClick(IChartboostMediationBannerView bannerView) =>
            DidClick?.Invoke(bannerView);
        
        internal virtual void OnBannerRecordImpression(IChartboostMediationBannerView bannerView) =>
            DidRecordImpression?.Invoke(bannerView);

        private static bool CanFetchAd(string placementName)
        {
            if (!ChartboostMediationExternal.IsInitialized)
            {
                Logger.LogError(LogTag, "The Chartboost Mediation SDK needs to be initialized before we can show any ads");
                return false;
            }
            if (!string.IsNullOrEmpty(placementName)) 
                return true;
            Logger.LogError(LogTag, "placementName passed is null cannot perform the operation requested");
            return false;
        }

        ~ChartboostMediationBannerViewBase()
        {
            if(uniqueId != IntPtr.Zero)
                CacheManager.ReleaseBannerAd(uniqueId.ToInt64());
        }
        
    }
}
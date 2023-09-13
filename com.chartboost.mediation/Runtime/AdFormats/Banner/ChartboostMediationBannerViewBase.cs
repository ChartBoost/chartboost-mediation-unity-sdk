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
    internal abstract class ChartboostMediationBannerViewBase : IChartboostMediationBannerView
    {
        public event ChartboostMediationBannerEvent WillAppear;
        public event ChartboostMediationBannerEvent DidClick;
        public event ChartboostMediationBannerEvent DidRecordImpression;
        public event ChartboostMediationBannerDragEvent DidDrag;

        protected static string LogTag = "ChartboostMediationBanner (Base)";
        protected IntPtr UniqueId { get; set; }

        protected ChartboostMediationBannerViewBase() { }
        protected ChartboostMediationBannerViewBase(IntPtr uniqueId)
        {
            this.UniqueId = uniqueId;
            CacheManager.TrackBannerAd(uniqueId.ToInt64(), this);
        }

        public abstract Dictionary<string, string> Keywords { get; set; } 
        public abstract ChartboostMediationBannerAdLoadRequest Request { get; protected set; }
        public abstract BidInfo WinningBidInfo { get; protected set;  }
        public abstract string LoadId { get; protected set;  }
        public abstract Metrics? LoadMetrics { get; protected set;  }
        public abstract ChartboostMediationBannerAdSize AdSize { get; protected set; }
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
            Logger.Log(LogTag, $"Loading banner ad for placement {request.PlacementName} and size {request.Size.Name} at {screenLocation}");
            return Task.FromResult<ChartboostMediationBannerAdLoadResult>(null);
        }

        public virtual Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, float x, float y)
        {
            if (!CanFetchAd(request.PlacementName))
            {
                var error = new ChartboostMediationError("Chartboost Mediation is not ready or placement is invalid.");
                var adLoadResult = new ChartboostMediationBannerAdLoadResult(error);
                return Task.FromResult(adLoadResult);
            }
            Logger.Log(LogTag, $"Loading banner ad for placement {request.PlacementName} and size {request.Size.Name} at ({x}, {y})");
            return Task.FromResult<ChartboostMediationBannerAdLoadResult>(null);
        }
        public virtual void ResizeToFit(ChartboostMediationBannerResizeAxis axis = ChartboostMediationBannerResizeAxis.Both, Vector2 pivot = default)=> Logger.Log(LogTag, $"Resizing at axis {axis} with pivot {pivot}");
        public virtual void SetDraggability(bool canDrag) => Logger.Log(LogTag, $"Setting Draggability to {canDrag}");
        public virtual void SetVisibility(bool visibility)=> Logger.Log(LogTag, $"Setting Visibility to {visibility}");
        public virtual void Reset() => Logger.Log(LogTag, $"Resetting banner ad");
        public virtual void Destroy()
        {
            Logger.Log(LogTag, $"Removing/Destroying banner ad");
            CacheManager.ReleaseBannerAd(UniqueId.ToInt64());
        }

        internal virtual void OnBannerWillAppear(IChartboostMediationBannerView bannerView)  => 
            WillAppear?.Invoke(bannerView);

        internal virtual void OnBannerClick(IChartboostMediationBannerView bannerView) =>
            DidClick?.Invoke(bannerView);
        
        internal virtual void OnBannerRecordImpression(IChartboostMediationBannerView bannerView) =>
            DidRecordImpression?.Invoke(bannerView);

        internal virtual void OnBannerDrag(IChartboostMediationBannerView bannerView, float x, float y) =>
            DidDrag?.Invoke(bannerView, x, y);

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
            if(UniqueId != IntPtr.Zero)
                CacheManager.ReleaseBannerAd(UniqueId.ToInt64());
        }
        
    }
}
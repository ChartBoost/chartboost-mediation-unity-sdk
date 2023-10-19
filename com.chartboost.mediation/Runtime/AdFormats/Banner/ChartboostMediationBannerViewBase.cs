using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Banner;
using Chartboost.Platforms;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using UnityEngine;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.AdFormats.Banner
{
    internal abstract class ChartboostMediationBannerViewBase : IChartboostMediationBannerView
    {
        /// <inheritdoc cref="IChartboostMediationBannerView.DidLoad"/>
        public event ChartboostMediationBannerEvent DidLoad;
        
        /// <inheritdoc cref="IChartboostMediationBannerView.DidClick"/>
        public event ChartboostMediationBannerEvent DidClick;
        
        /// <inheritdoc cref="IChartboostMediationBannerView.DidRecordImpression"/>
        public event ChartboostMediationBannerEvent DidRecordImpression;
        
        /// <inheritdoc cref="IChartboostMediationBannerView.DidDrag"/>
        public event ChartboostMediationBannerDragEvent DidDrag;

        protected static string LogTag = "ChartboostMediationBanner (Base)";
        protected IntPtr UniqueId { get; set; }

        protected ChartboostMediationBannerViewBase() { }
        protected ChartboostMediationBannerViewBase(IntPtr uniqueId)
        {
            UniqueId = uniqueId;
            CacheManager.TrackBannerAd(uniqueId.ToInt64(), this);
        }

        /// <inheritdoc cref="IChartboostMediationBannerView.Keywords"/>
        public abstract Dictionary<string, string> Keywords { get; set; }
        
        /// <inheritdoc cref="IChartboostMediationBannerView.Request"/>
        public abstract ChartboostMediationBannerAdLoadRequest Request { get; protected set; }
        
        /// <inheritdoc cref="IChartboostMediationBannerView.WinningBidInfo"/>
        public abstract BidInfo WinningBidInfo { get; protected set; }
        
        /// <inheritdoc cref="IChartboostMediationBannerView.LoadId"/>
        public abstract string LoadId { get; protected set; }
        
        // ReSharper disable once InvalidXmlDocComment
        /// <inheritdoc cref="IChartboostMediationBannerView.LoadMetrics"/>
        public abstract Metrics? LoadMetrics { get; protected set; }
        
        /// <inheritdoc cref="IChartboostMediationBannerView.AdSize"/>
        public abstract ChartboostMediationBannerSize? AdSize { get; protected set; }
        
        /// <inheritdoc cref="IChartboostMediationBannerView.ContainerSize"/>
        public abstract ChartboostMediationBannerSize? ContainerSize { get; protected set; }

        /// <inheritdoc cref="IChartboostMediationBannerView.HorizontalAlignment"/>
        public abstract ChartboostMediationBannerHorizontalAlignment HorizontalAlignment { get; set; }
        
        /// <inheritdoc cref="IChartboostMediationBannerView.VerticalAlignment"/>
        public abstract ChartboostMediationBannerVerticalAlignment VerticalAlignment { get; set; }
        
        /// <inheritdoc cref="IChartboostMediationBannerView.Load(Chartboost.Requests.ChartboostMediationBannerAdLoadRequest,Chartboost.Banner.ChartboostMediationBannerAdScreenLocation)"/>
        public virtual Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, ChartboostMediationBannerAdScreenLocation screenLocation)
        {
            Request = request;
            if (!CanFetchAd(request.PlacementName))
            {
                var error = new ChartboostMediationError("Chartboost Mediation is not ready or placement is invalid.");
                var adLoadResult = new ChartboostMediationBannerAdLoadResult(error);
                return Task.FromResult(adLoadResult);
            }
            Logger.Log(LogTag, $"Loading banner ad for placement {request.PlacementName} and size {request.Size.SizeType} at {screenLocation}");
            return Task.FromResult<ChartboostMediationBannerAdLoadResult>(null);
        }

        /// <inheritdoc cref="IChartboostMediationBannerView.Load(Chartboost.Requests.ChartboostMediationBannerAdLoadRequest,float, float)"/>
        public virtual Task<ChartboostMediationBannerAdLoadResult> Load(ChartboostMediationBannerAdLoadRequest request, float x, float y)
        {
            Request = request;
            if (!CanFetchAd(request.PlacementName))
            {
                var error = new ChartboostMediationError("Chartboost Mediation is not ready or placement is invalid.");
                var adLoadResult = new ChartboostMediationBannerAdLoadResult(error);
                return Task.FromResult(adLoadResult);
            }
            Logger.Log(LogTag, $"Loading banner ad for placement {request.PlacementName} and size {request.Size.SizeType} at ({x}, {y})");
            return Task.FromResult<ChartboostMediationBannerAdLoadResult>(null);
        }
        
        /// <inheritdoc cref="IChartboostMediationBannerView.ResizeToFit"/>
        public virtual void ResizeToFit(ChartboostMediationBannerResizeAxis axis = ChartboostMediationBannerResizeAxis.Both, Vector2 pivot = default)
            => Logger.Log(LogTag, $"Resizing at axis {axis} with pivot {pivot}");
        
        /// <inheritdoc cref="IChartboostMediationBannerView.SetDraggability"/>
        public virtual void SetDraggability(bool canDrag) 
            => Logger.Log(LogTag, $"Setting Draggability to {canDrag}");
        
        /// <inheritdoc cref="IChartboostMediationBannerView.SetVisibility"/>
        public virtual void SetVisibility(bool visibility) 
            => Logger.Log(LogTag, $"Setting Visibility to {visibility}");
        
        /// <inheritdoc cref="IChartboostMediationBannerView.Reset"/>
        public virtual void Reset() 
            => Logger.Log(LogTag, $"Resetting banner ad");
        
        /// <inheritdoc cref="IChartboostMediationBannerView.Destroy"/>
        public virtual void Destroy()
        {
            Logger.Log(LogTag, $"Removing/Destroying banner ad");
            CacheManager.ReleaseBannerAd(UniqueId.ToInt64());
        }

        internal virtual void MoveTo(float x, float y) {  }
        
        internal virtual void OnBannerDidLoad(IChartboostMediationBannerView bannerView) => DidLoad?.Invoke(bannerView);

        internal virtual void OnBannerClick(IChartboostMediationBannerView bannerView) => DidClick?.Invoke(bannerView);

        internal virtual void OnBannerRecordImpression(IChartboostMediationBannerView bannerView) => DidRecordImpression?.Invoke(bannerView);

        internal virtual void OnBannerDrag(IChartboostMediationBannerView bannerView, float x, float y) => DidDrag?.Invoke(bannerView, x, y);

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
            if (UniqueId != IntPtr.Zero)
                CacheManager.ReleaseBannerAd(UniqueId.ToInt64());
        }
        
    }
}

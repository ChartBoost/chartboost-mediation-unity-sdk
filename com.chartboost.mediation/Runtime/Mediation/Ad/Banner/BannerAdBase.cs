using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chartboost.Json;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using UnityEngine;

namespace Chartboost.Mediation.Ad.Banner
{
    /// <summary>
    /// Base class for all <see cref="IBannerAd"/> implementations for all platforms.
    /// </summary>
    public abstract class BannerAdBase : IBannerAd
    {
        protected const string BannerAd = "[BannerAd]";
        
        /// <summary>
        /// Tracks validity of placement
        /// </summary>
        protected bool IsDisposed;
        
        /// <summary>
        /// Unique identifier for this <see cref="IBannerAd"/>.
        /// </summary>
        protected readonly IntPtr UniqueId;
        
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Backing field for <see cref="BannerAdLoadRequest"/>.
        /// </summary>
        protected BannerAdLoadRequest _request;

        /// <summary>
        /// Base constructor for <see cref="IBannerAd"/> implementations.
        /// <see cref="IntPtr"/> must be present to associate native ad with C# instance.
        /// </summary>
        /// <param name="uniqueId">Identifier to associate and track native ads with C# layer.</param>
        protected BannerAdBase(IntPtr uniqueId)
        {
            UniqueId = uniqueId;
            LogController.Log($"{BannerAd} Creating with UniqueId:{UniqueId}", LogLevel.Debug);
            AdCache.TrackAd(uniqueId.ToInt64(), this);
        }
        
        /// <inheritdoc />
        public abstract IReadOnlyDictionary<string, string> Keywords { get; set; }
        
        /// <inheritdoc />
        public abstract IReadOnlyDictionary<string, string> PartnerSettings { get; set; }
        
        /// <inheritdoc />
        public abstract BannerSize? BannerSize { get; }
        
        /// <inheritdoc />
        public abstract BannerAdLoadRequest Request { get; }
        
        /// <inheritdoc cref="IBannerAd.WinningBidInfo"/>
        public abstract BidInfo WinningBidInfo { get; }
        
        /// <inheritdoc cref="IBannerAd.LoadId"/>
        public abstract string LoadId { get; }
        
        /// <inheritdoc cref="IBannerAd.LoadMetrics"/>
        public abstract Metrics? LoadMetrics { get; }
        
        /// <inheritdoc />
        public abstract Vector2 Position { get; set; }
        
        /// <inheritdoc />
        public abstract Vector2 Pivot { get; set; }
        
        /// <inheritdoc />
        public abstract bool Visible { get; set; }
        
        /// <inheritdoc />
        public abstract bool Draggable { get; set; }
        
        /// <inheritdoc />
        public abstract ContainerSize ContainerSize { get; set; }
        
        /// <inheritdoc cref="IBannerAd.HorizontalAlignment"/>
        public abstract BannerHorizontalAlignment HorizontalAlignment { get; set; }
        
        /// <inheritdoc cref="IBannerAd.VerticalAlignment"/>
        public abstract BannerVerticalAlignment VerticalAlignment { get; set; }
        
        /// <inheritdoc />
        public virtual Task<BannerAdLoadResult> Load(BannerAdLoadRequest request)
        {
            LogController.Log($"{BannerAd} Load with request: {JsonTools.SerializeObject(request)}", LogLevel.Debug);
            _request = request;
            var error = new ChartboostMediationError(Errors.ErrorNotReady);
            var adLoadResult = new BannerAdLoadResult(error);
            return Task.FromResult(adLoadResult);
        }

        /// <inheritdoc />
        public virtual void Reset()
            => LogController.Log($"{BannerAd} Reset", LogLevel.Debug);

        /// <inheritdoc cref="IBannerAd.Dispose"/>
        public void Dispose()
        {
            LogController.Log($"{BannerAd} Dispose", LogLevel.Debug);
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        /// <inheritdoc />
        public event BannerAdEvent WillAppear;
        
        /// <inheritdoc />
        public event BannerAdEvent DidClick;
        
        /// <inheritdoc />
        public event BannerAdEvent DidRecordImpression;
        
        /// <inheritdoc />
        public event BannerAdDragEvent DidBeginDrag;
        
        /// <inheritdoc />
        public event BannerAdDragEvent DidDrag;

        /// <inheritdoc />
        public event BannerAdDragEvent DidEndDrag;

        internal void OnWillAppear()
        {
            LogController.Log($"{BannerAd}: {_request?.PlacementName}/{UniqueId} WillAppear", LogLevel.Debug);
            WillAppear?.Invoke(this);
        }

        internal void OnClick()
        {
            LogController.Log($"{BannerAd}: {_request?.PlacementName}/{UniqueId} Click", LogLevel.Debug);
            DidClick?.Invoke(this);
        }

        internal void OnRecordImpression()
        {
            LogController.Log($"{BannerAd}: {_request?.PlacementName}/{UniqueId} RecordImpression", LogLevel.Debug);
            DidRecordImpression?.Invoke(this);
        }

        internal void OnDragBegin(float x, float y)
        {
            LogController.Log($"{BannerAd}: {_request?.PlacementName}/{UniqueId} Drag Begin at X:{x} Y:{y}", LogLevel.Debug);
            DidBeginDrag?.Invoke(this, x, y);
        }
        
        internal void OnDrag(float x, float y)
        {
            LogController.Log($"{BannerAd}: {_request?.PlacementName}/{UniqueId} Drag to X:{x} Y:{y}", LogLevel.Verbose);
            DidDrag?.Invoke(this, x, y);
        }
        
        internal void OnDragEnd(float x, float y)
        {
            LogController.Log($"{BannerAd}: {_request?.PlacementName}/{UniqueId} Drag End at X:{x} Y:{y}", LogLevel.Debug);
            DidEndDrag?.Invoke(this, x, y);
        }

        ~BannerAdBase()
        {
            if(!IsDisposed)
                LogController.Log($"Banner Ad with UniqueId: {UniqueId}, got GC. Make sure to properly dispose of ads utilizing Invalidate for the best integration experience.", LogLevel.Error);
            Dispose(false);
        }
    }
}

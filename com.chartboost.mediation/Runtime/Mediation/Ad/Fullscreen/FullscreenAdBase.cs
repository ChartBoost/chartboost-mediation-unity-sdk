using System;
using System.Threading.Tasks;
using Chartboost.Logging;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;

namespace Chartboost.Mediation.Ad.Fullscreen
{
    internal abstract class FullscreenAdBase : IFullscreenAd
    {
        /// <summary>
        /// Unique identifier for this <see cref="IFullscreenAd"/>.
        /// </summary>
        protected readonly IntPtr UniqueId;
        
        /// <summary>
        /// Tracks validity of placement
        /// </summary>
        protected bool IsDisposed = false;
        
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Backing field for <see cref="FullscreenAdLoadRequest"/>.
        /// </summary>
        protected FullscreenAdLoadRequest _request;
        
        /// <summary>
        /// Backing field for <see cref="IFullscreenAd.LoadId"/>.
        /// </summary>
        protected string _loadId;
        
        /// <summary>
        /// Backing field for <see cref="IFullscreenAd.WinningBidInfo"/>.
        /// </summary>
        protected BidInfo? _bidInfo;
        // ReSharper restore InconsistentNaming
        
        /// <summary>
        /// Generic error used when a <see cref="IFullscreenAd"/> becomes invalid.
        /// </summary>
        protected static readonly AdShowResult InvalidAdShowResult = new(new ChartboostMediationError(Errors.InvalidAdError));
        
        /// <summary>
        /// Base constructor for <see cref="IFullscreenAd"/> implementations.
        /// <see cref="IntPtr"/> must be present to associate native ad with C# instance.
        /// </summary>
        /// <param name="uniqueId">Identifier to associate and track native ads with C# layer.</param>
        protected FullscreenAdBase(long uniqueId)
        {
            UniqueId = new IntPtr(uniqueId);
            LogController.Log($"Creating FullscreenAd with UniqueId:{UniqueId}", LogLevel.Debug);
            AdCache.TrackAd(uniqueId, this);
        }

        /// <summary>
        /// Base constructor for <see cref="IFullscreenAd"/> implementations.
        /// <see cref="IntPtr"/> must be present to associate native ad with C# instance.
        /// </summary>
        /// <param name="uniqueId">Identifier to associate and track native ads with C# layer.</param>
        protected FullscreenAdBase(IntPtr uniqueId)
        {
            UniqueId = uniqueId;
            AdCache.TrackAd(uniqueId, this);
        }

        /// <inheritdoc cref="IFullscreenAd.Request"/>
        public abstract FullscreenAdLoadRequest Request { get; }
        
        /// <inheritdoc cref="IFullscreenAd.CustomData"/>
        public abstract string CustomData { get; set; }
        
        /// <inheritdoc cref="IFullscreenAd.LoadId"/>
        public abstract string LoadId { get; }
        
        /// <inheritdoc cref="IFullscreenAd.WinningBidInfo"/>
        public abstract BidInfo WinningBidInfo { get; }
        
        /// <inheritdoc cref="IFullscreenAd.Show"/>
        public abstract Task<AdShowResult> Show();

        /// <inheritdoc cref="IFullscreenAd.Dispose" />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected abstract void Dispose(bool disposing);
        
        /// <inheritdoc cref="IFullscreenAd.DidClick"/>
        public event FullscreenAdEvent DidClick;
        
        /// <inheritdoc cref="IFullscreenAd.DidClose"/>
        public event FullscreenAdEventWithError DidClose;
        
        /// <inheritdoc cref="IFullscreenAd.DidExpire"/>
        public event FullscreenAdEvent DidExpire;
        
        /// <inheritdoc cref="IFullscreenAd.DidRecordImpression"/>
        public event FullscreenAdEvent DidRecordImpression;
        
        /// <inheritdoc cref="IFullscreenAd.DidReward"/>
        public event FullscreenAdEvent DidReward;
        
        internal void OnClick()
        {
            LogController.Log($"FullscreenAd: {_request?.PlacementName}/{UniqueId} Click", LogLevel.Debug);
            DidClick?.Invoke(this);
        }

        internal void OnClose(ChartboostMediationError? error)
        {
            LogController.Log($"FullscreenAd: {_request?.PlacementName}/{UniqueId} Close", LogLevel.Debug);
            DidClose?.Invoke(this, error);
        }

        internal void OnRecordImpression()
        {
            LogController.Log($"FullscreenAd: {_request?.PlacementName}/{UniqueId} RecordImpression", LogLevel.Debug);
            DidRecordImpression?.Invoke(this);
        }

        internal void OnExpire()
        {
            LogController.Log($"FullscreenAd: {_request?.PlacementName}/{UniqueId} Expire", LogLevel.Debug);
            DidExpire?.Invoke(this);
        }

        internal void OnReward()
        {
            LogController.Log($"FullscreenAd: {_request?.PlacementName}/{UniqueId} Reward", LogLevel.Debug);
            DidReward?.Invoke(this);
        }

        ~FullscreenAdBase()
        {
            // only report error if the ad is being disposed by gc and it has not been manually invalidated.
            if(!IsDisposed)
                LogController.Log($"Fullscreen Ad with UniqueId: {UniqueId}, got GC. Make sure to properly dispose of ads utilizing Invalidate for the best integration experience.", LogLevel.Error);
            Dispose(false);
        }
    }
}

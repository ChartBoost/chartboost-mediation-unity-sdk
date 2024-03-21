using System;
using System.Collections.Generic;
using Chartboost.Requests;
using Chartboost.Utilities;

namespace Chartboost.AdFormats.Fullscreen.Queue
{
    public abstract class ChartboostMediationFullscreenAdQueueBase : ChartboostMediationFullscreenAdQueue
    {
        protected static string LogTag = "ChartboostMediationFullscreenAdQueue (Base)";
        protected IntPtr UniqueId;
        
        protected ChartboostMediationFullscreenAdQueueBase(IntPtr uniqueId)
        {
            UniqueId = uniqueId;
            CacheManager.TrackFullscreenAdQueue(UniqueId.ToInt64(), this);
        }
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.QueueCapacity"/>
        public abstract int QueueCapacity { get; }
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.NumberOfAdsReady"/>
        public abstract int NumberOfAdsReady { get; }
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.Keywords"/>
        public abstract Dictionary<string, string> Keywords { get; set; }
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.IsRunning"/>
        public abstract bool IsRunning { get; }
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.GetNextAd"/>
        public abstract IChartboostMediationFullscreenAd GetNextAd();
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.HasNextAd"/>
        public abstract bool HasNextAd();
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.Start"/>
        public abstract void Start();
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.Stop"/>
        public abstract void Stop();
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.SetCapacity"/>
        public abstract void SetCapacity(int capacity);
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.DidUpdate"/>
        public event ChartboostMediationFullscreenAdQueueEvent DidUpdate;
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.DidRemoveExpiredAd"/>
        public event ChartboostMediationFullscreenAdQueueRemoveExpiredAdEvent DidRemoveExpiredAd;

        internal void OnFullscreenAdQueueUpdated(ChartboostMediationFullscreenAdQueue adQueue, ChartboostMediationAdLoadResult result, int numberOfAdsReady) 
            => MainThreadDispatcher.Post(_ => DidUpdate?.Invoke(adQueue, result, numberOfAdsReady));

        internal void OnFullscreenAdQueueDidRemoveExpiredAd(ChartboostMediationFullscreenAdQueue adQueue, int numberOfAdsReady) 
            => MainThreadDispatcher.Post(_ => DidRemoveExpiredAd?.Invoke(adQueue, numberOfAdsReady));
    }

}

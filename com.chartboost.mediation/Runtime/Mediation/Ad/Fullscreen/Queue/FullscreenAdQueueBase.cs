using System;
using System.Collections.Generic;
using Chartboost.Logging;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;

namespace Chartboost.Mediation.Ad.Fullscreen.Queue
{
    internal abstract class FullscreenAdQueueBase : IFullscreenAdQueue
    {
        protected const string FullscreenAdQueue = "[FullscreenAdQueue]";
        protected IntPtr UniqueId;
        
        protected FullscreenAdQueueBase(IntPtr uniqueId)
        {
            UniqueId = uniqueId;
            AdCache.TrackAd(UniqueId.ToInt64(), this);
        }
        
        public abstract Dictionary<string, string> Keywords { get; set; }
        public abstract int QueueCapacity { get; set; }
        public abstract int NumberOfAdsReady { get; }
        public abstract bool IsRunning { get; }
        
        public virtual IFullscreenAd GetNextAd()
        {
            LogController.Log($"{FullscreenAdQueue} attempting to get next ad.", LogLevel.Info);
            return null;
        }

        public virtual bool HasNextAd()
        {
            LogController.Log($"{FullscreenAdQueue} checking for next ad.", LogLevel.Info);
            return false;
        }

        public virtual void Start() 
            => LogController.Log($"{FullscreenAdQueue} Starting.", LogLevel.Info);

        public virtual void Stop() 
            => LogController.Log($"{FullscreenAdQueue} Stopping.", LogLevel.Info);

        public event FullscreenAdQueueUpdateEvent DidUpdate;
        public event FullscreenAdQueueRemoveExpiredAdEvent DidRemoveExpiredAd;

        internal void OnDidUpdate(IAdLoadResult adLoadResult, int numberOfAdsReady) 
            => DidUpdate?.Invoke(this, adLoadResult, numberOfAdsReady);

        internal void OnDidRemoveExpiredAd(int numberOfAdsReady) => DidRemoveExpiredAd?.Invoke(this, numberOfAdsReady);
    }
}

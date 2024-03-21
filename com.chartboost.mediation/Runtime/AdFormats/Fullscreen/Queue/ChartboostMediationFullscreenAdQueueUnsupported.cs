using System;
using System.Collections.Generic;
using Chartboost.Utilities;

namespace Chartboost.AdFormats.Fullscreen.Queue
{
    public class ChartboostMediationFullscreenAdQueueUnsupported : ChartboostMediationFullscreenAdQueueBase
    {
        private ChartboostMediationFullscreenAdQueueUnsupported(IntPtr uniqueId) : base(IntPtr.Zero)
        {
            LogTag = "ChartboostMediationFullscreenAdQueue (Unsupported)";
            Logger.Log(LogTag, $"Creating FullscreenAdQueue for placement : {uniqueId}");
        }
        // TODO : Do we want to return an exception or default values ?
        public override int QueueCapacity => 0;
        public override int NumberOfAdsReady => 0;
        public override Dictionary<string, string> Keywords { get; set; } = null;
        public override bool IsRunning => false;

        public override IChartboostMediationFullscreenAd GetNextAd() => null;

        public override bool HasNextAd() => false;

        public override void Start() {}

        public override void Stop() {}

        public override void SetCapacity(int capacity) {}
        
        public static ChartboostMediationFullscreenAdQueueUnsupported Queue(string placementName)
        {
            var nativeQueue = IntPtr.Zero;
            var queue = (ChartboostMediationFullscreenAdQueueUnsupported)CacheManager.GetFullscreenAdQueue(nativeQueue.ToInt64());
            if (queue != null)
                return queue;

            queue = new ChartboostMediationFullscreenAdQueueUnsupported(nativeQueue);
            return queue;
        }
    }
}

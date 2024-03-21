#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Chartboost.Events;
using Chartboost.Requests;
using Chartboost.Utilities;
using Newtonsoft.Json;

namespace Chartboost.AdFormats.Fullscreen.Queue
{
    internal sealed class ChartboostMediationFullscreenAdQueueIOS : ChartboostMediationFullscreenAdQueueBase
    {
        private Dictionary<string, string> _keywords;

        private ChartboostMediationFullscreenAdQueueIOS(IntPtr uniqueId) : base(uniqueId)
        {
            LogTag = "ChartboostMediationFullscreenAdQueue (IOS)";
        }
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.Keywords"/>
        public override Dictionary<string, string> Keywords
        {
            get => _keywords;
            set
            {
                try
                {
                    var keywordsJson = string.Empty;
                    if (_keywords.Count > 0)
                        keywordsJson = JsonConvert.SerializeObject(_keywords);
                    _chartboostMediationFullscreenAdQueueSetKeywords(UniqueId, keywordsJson);
                    _keywords = value;
                }
                catch (Exception e)
                {
                    EventProcessor.ReportUnexpectedSystemError($"Error setting keywords => {e.Message}");
                }
            }
        }

        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.QueueCapacity"/>
        public override int QueueCapacity => _chartboostMediationFullscreenAdQueueQueueCapacity(UniqueId);
        
        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.NumberOfAdsReady"/>
        public override int NumberOfAdsReady => _chartboostMediationFullscreenAdQueueNumberOfAdsReady(UniqueId);

        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.IsRunning"/>
        public override bool IsRunning => _chartboostMediationFullscreenAdQueueIsRunning(UniqueId);

        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.GetNextAd"/>
        public override IChartboostMediationFullscreenAd GetNextAd()
        {
            try
            {
                var nativeAd = _chartboostMediationFullscreenAdQueueGetNextAd(UniqueId);
                // TODO: check if nativeAd is NULL
                return new ChartboostMediationFullscreenAdIOS(nativeAd);
            }
            catch (Exception)
            {
                Logger.Log(LogTag, "GetNextAd() :: No more ads left in queue returning NULL");
                return null;
            }
        }

        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.HasNextAd"/>
        public override bool HasNextAd()
            => _chartboostMediationFullscreenAdQueueHasNextAd(UniqueId);

        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.Start"/>
        public override void Start()
            => _chartboostMediationFullscreenAdQueueStart(UniqueId);

        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.Stop"/>
        public override void Stop()
            => _chartboostMediationFullscreenAdQueueStop(UniqueId);

        /// <inheritdoc cref="ChartboostMediationFullscreenAdQueue.SetCapacity"/>
        public override void SetCapacity(int capacity)
            => _chartboostMediationFullscreenAdQueueSetCapacity(UniqueId, capacity);

        internal static ChartboostMediationFullscreenAdQueueIOS Queue(string placementName)
        {
            // Queues are a "singleton per placement", meaning that if a publisher attempts to
            // create multiple queues with the same placement ID the same object will be returned each time.
            var nativeQueue = _chartboostMediationFullscreenAdQueueQueue(placementName);
            var queue = (ChartboostMediationFullscreenAdQueueIOS)CacheManager.GetFullscreenAdQueue(nativeQueue.ToInt64());
            if (queue != null)
                return queue;
            
            queue = new ChartboostMediationFullscreenAdQueueIOS(nativeQueue);
            return queue;
        }
        
        [DllImport(IOSConstants.Internal)]
        private static extern IntPtr _chartboostMediationFullscreenAdQueueQueue(string placementName);
        
        [DllImport(IOSConstants.Internal)]
        private static extern void _chartboostMediationFullscreenAdQueueSetKeywords(IntPtr uniqueId, string keywordsJson);
        
        [DllImport(IOSConstants.Internal)]
        private static extern int _chartboostMediationFullscreenAdQueueQueueCapacity(IntPtr uniqueId);

        [DllImport(IOSConstants.Internal)]
        private static extern int _chartboostMediationFullscreenAdQueueNumberOfAdsReady(IntPtr uniqueId);
        
        [DllImport(IOSConstants.Internal)]
        private static extern bool _chartboostMediationFullscreenAdQueueIsRunning(IntPtr uniqueId);
        
        [DllImport(IOSConstants.Internal)]
        private static extern IntPtr _chartboostMediationFullscreenAdQueueGetNextAd(IntPtr uniqueId);

        [DllImport(IOSConstants.Internal)]
        private static extern bool _chartboostMediationFullscreenAdQueueHasNextAd(IntPtr uniqueId);
        
        [DllImport(IOSConstants.Internal)]
        private static extern void _chartboostMediationFullscreenAdQueueStart(IntPtr uniqueId);
        
        [DllImport(IOSConstants.Internal)]
        private static extern void _chartboostMediationFullscreenAdQueueStop(IntPtr uniqueId);
        
        [DllImport(IOSConstants.Internal)]
        private static extern void _chartboostMediationFullscreenAdQueueSetCapacity(IntPtr uniqueId, int capacity);
    }
}
#endif

using System;
using System.Collections.Generic;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Ad.Fullscreen.Queue;

namespace Chartboost.Mediation.Default.Ad.Fullscreen.Queue
{
    /// <summary>
    /// Default implementation of <see cref="FullscreenAdQueueDefault"/> for any unsupported platforms.
    /// </summary>
    internal class FullscreenAdQueueDefault : FullscreenAdQueueBase 
    {
        public FullscreenAdQueueDefault(IntPtr uniqueId) : base(uniqueId) { }

        /// <inheritdoc cref="IFullscreenAdQueue.QueueCapacity"/>
        public override int QueueCapacity { get; set; }
        
        /// <inheritdoc cref="IFullscreenAdQueue.NumberOfAdsReady"/>
        public override int NumberOfAdsReady => 0;
        
        /// <inheritdoc cref="IFullscreenAdQueue.Keywords"/>
        public override Dictionary<string, string> Keywords { get; set; } = null;
        
        /// <inheritdoc cref="IFullscreenAdQueue.IsRunning"/>
        public override bool IsRunning => false;

        /// <inheritdoc cref="IFullscreenAdQueue.GetNextAd"/>
        public override IFullscreenAd GetNextAd() => null;
        
        /// <inheritdoc cref="IFullscreenAdQueue.HasNextAd"/>
        public override bool HasNextAd() => false;
    }
}

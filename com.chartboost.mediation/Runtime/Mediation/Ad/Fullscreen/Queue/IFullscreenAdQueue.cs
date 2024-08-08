using System.Collections.Generic;

namespace Chartboost.Mediation.Ad.Fullscreen.Queue
{
    /// <summary>
    /// Manages the pre-loading of fullscreen ads. Each <see cref="IFullscreenAdQueue"/> manages ads for one placement.
    /// </summary>
    public interface IFullscreenAdQueue : IAd
    {
        /// <summary>
        /// The keywords for the queue. Every load request made by this queue will include these keywords. 
        /// </summary>
        Dictionary<string, string> Keywords { get; set; }
        
        /// <summary>
        /// Maximum number of loaded ads the queue can hold at one time.
        ///
        /// <para/>
        /// Request a new queue size. If the number of ad slots requested is larger than the maximum
        /// configured by the dashboard this method will log an error and set the queue size to maximum
        /// allowed size as configured on dashboard.
        /// </summary>
        int QueueCapacity { get; set; }
        
        /// <summary>
        /// Number of ready-to-show ads that can currently be retrieved with <see cref="GetNextAd"/>
        /// </summary>
        int NumberOfAdsReady { get; }

        /// <summary>
        /// Whether or not the ad queue is currently running (active) and automatically queueing ads.
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// Gets the next ad from the queue.
        /// </summary>
        /// <returns> A <see cref="IFullscreenAd"/> ad</returns>
        IFullscreenAd GetNextAd();
        
        /// <summary>
        /// Checks if there's an ad available.
        /// </summary>
        /// <returns> Whether or not there exists an ad in the queue.</returns>
        bool HasNextAd();
        
        /// <summary>
        /// Starts loading ads and append them to the queue automatically until capacity has been reached.
        /// </summary>
        void Start();
        
        /// <summary>
        /// Stops loading ads
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Event triggered when a <see cref="IFullscreenAdQueue"/> completes a load request.
        /// </summary>
        event FullscreenAdQueueUpdateEvent DidUpdate;
        
        /// <summary>
        /// Event triggered when an ad expires and is removed from the queue.
        /// </summary>
        event FullscreenAdQueueRemoveExpiredAdEvent DidRemoveExpiredAd;
    }
}

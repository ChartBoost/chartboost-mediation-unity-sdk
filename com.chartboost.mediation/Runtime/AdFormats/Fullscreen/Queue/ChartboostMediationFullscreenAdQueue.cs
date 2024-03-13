using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Chartboost.Requests;

namespace Chartboost.AdFormats.Fullscreen.Queue
{
    public delegate void ChartboostMediationFullscreenAdQueueEvent(ChartboostMediationFullscreenAdQueue adQueue,
        ChartboostMediationAdLoadResult adLoadResult, int numberOfAdsReady);
    public delegate void ChartboostMediationFullscreenAdQueueRemoveExpiredAdEvent(ChartboostMediationFullscreenAdQueue adQueue, int numberOfAdsReady);
    
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Manages the pre-loading of fullscreen ads. Each <see cref="ChartboostMediationFullscreenAdQueue"/> manages ads for one placement.
    /// 
    /// </summary>
    public interface ChartboostMediationFullscreenAdQueue
    {
        /// <summary>
        /// Returns a <see cref="ChartboostMediationFullscreenAdQueue"/>. Queue will not begin loading ads until `Start()` is called.
        /// Calling <see cref="Queue"/> more than once with the same placement ID returns the same object each time.
        /// </summary>
        /// <param name="placementName">Identifier for the Chartboost placement this queue should load ads from.</param>
        /// <returns></returns>
        static ChartboostMediationFullscreenAdQueue Queue(string placementName)
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            ChartboostMediationFullscreenAdQueue queue;
            #if UNITY_EDITOR
            queue = ChartboostMediationFullscreenAdQueueUnsupported.Queue(placementName);
            #elif UNITY_ANDROID
            queue = ChartboostMediationFullscreenAdQueueAndroid.Queue(placementName);
            #elif UNITY_IOS
            queue = ChartboostMediationFullscreenAdQueueIOS.Queue(placementName);
            #else
            queue = ChartboostMediationFullscreenAdQueueUnsupported.Queue(placementName);
            #endif
            
            return queue;        
        }

        /// <summary>
        /// Maximum number of loaded ads the queue can hold at one time.
        /// </summary>
        int QueueCapacity { get; }
        
        /// <summary>
        /// Number of ready-to-show ads that can currently be retrieved with <see cref="GetNextAd"/>
        /// </summary>
        int NumberOfAdsReady { get; }
        
        /// <summary>
        /// The keywords for the queue. Every load request made by this queue will include these keywords. 
        /// </summary>
        Dictionary<string, string> Keywords { get; set; }
        
        /// <summary>
        /// Whether or not the ad queue is currently running (active) and automatically queueing ads.
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// Gets the next ad from the queue.
        /// </summary>
        /// <returns> A <see cref="IChartboostMediationFullscreenAd"/> ad</returns>
        IChartboostMediationFullscreenAd GetNextAd();
        
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
        /// Request a new queue size. If the number of ad slots requested is larger than the maximum
        /// configured by the dashboard this method will log an error and set the queue size to maximum
        /// allowed size as configured on dashboard.
        /// </summary>
        /// <param name="capacity"> The most loaded ads the queue should be able to hold at once. </param>
        void SetCapacity(int capacity);
        
        /// <summary>
        /// Event triggered when a <see cref="ChartboostMediationFullscreenAdQueue"/> completes a load request.
        /// </summary>
        event ChartboostMediationFullscreenAdQueueEvent DidUpdate;
        
        /// <summary>
        /// Event triggered when an ad expires and is removed from the queue.
        /// </summary>
        event ChartboostMediationFullscreenAdQueueRemoveExpiredAdEvent DidRemoveExpiredAd;
        
    }

}

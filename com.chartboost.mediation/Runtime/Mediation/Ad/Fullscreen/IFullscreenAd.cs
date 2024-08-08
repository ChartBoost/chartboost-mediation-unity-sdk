using System;
using System.Threading.Tasks;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;

namespace Chartboost.Mediation.Ad.Fullscreen
{
    /// <summary>
    /// Base public interface for Fullscreen API.
    /// </summary>
    public interface IFullscreenAd : IAd, IDisposable
    {
        /// <summary>
        /// The publisher-supplied ChartboostMediationFullscreenAdLoadRequest that was used to load the ad.
        /// </summary>
        public FullscreenAdLoadRequest Request { get; }

        /// <summary>
        /// The custom data for the ad.
        /// </summary>
        public string CustomData { get; set; }
        
        /// <summary>
        /// The identifier for this load call.
        /// </summary>
        public string LoadId { get; }

        /// <summary>
        /// The winning bid info for the ad.
        /// </summary>
        public BidInfo WinningBidInfo { get; }

        /// <summary>
        /// Show the fullscreen ad.
        /// </summary>
        /// <returns>Ad show result data.</returns>
        public Task<AdShowResult> Show();

        /// <summary>
        /// Disposes the ad. This should be called when the ad is no longer needed.
        /// </summary>
        public new void Dispose();
        
        /// <summary>
        /// Called when the ad executes its click-through. This may happen multiple times for the same ad.
        /// </summary>
        public event FullscreenAdEvent DidClick;

        /// <summary>
        /// Called when the ad is closed.
        /// </summary>
        public event FullscreenAdEventWithError DidClose;
        
        /// <summary>
        /// Called when the ad is expired by the partner SDK/adapter.
        /// </summary>
        public event FullscreenAdEvent DidExpire;
        
        /// <summary>
        /// Called when an ad impression occurs. This signal is when Chartboost Mediation fires an impression and is independent of any partner impression.
        /// </summary>
        public event FullscreenAdEvent DidRecordImpression;
        
        /// <summary>
        /// Called when the user should receive the reward associated with this rewarded ad.
        /// </summary>
        public event FullscreenAdEvent DidReward;
    }
}

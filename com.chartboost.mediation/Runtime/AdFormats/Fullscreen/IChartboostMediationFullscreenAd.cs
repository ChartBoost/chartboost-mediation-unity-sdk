using System.Threading.Tasks;
using Chartboost.Requests;

namespace Chartboost.AdFormats.Fullscreen
{
    /// <summary>
    /// Base public interface for Fullscreen API
    /// </summary>
    public interface IChartboostMediationFullscreenAd
    {
        /// <summary>
        /// The publisher-supplied ChartboostMediationFullscreenAdLoadRequest that was used to load the ad.
        /// </summary>
        public abstract ChartboostMediationFullscreenAdLoadRequest Request { get; }

        /// <summary>
        /// The custom data for the ad.
        /// </summary>
        public abstract string CustomData { get; set; }
        
        /// <summary>
        /// The identifier for this load call.
        /// </summary>
        public abstract string LoadId { get; }

        /// <summary>
        /// The winning bid info for the ad.
        /// </summary>
        public abstract BidInfo WinningBidInfo { get; }

        /// <summary>
        /// Show the fullscreen ad.
        /// </summary>
        /// <returns>Ad show result data.</returns>
        public abstract Task<ChartboostMediationAdShowResult> Show();

        /// <summary>
        /// Invalidate the ad. This should be called when the ad is no longer needed.
        /// </summary>
        public abstract void Invalidate();
        
        /// <summary>
        /// Called when the ad executes its click-through. This may happen multiple times for the same ad.
        /// </summary>
        public event ChartboostMediationFullscreenAdEvent DidClick;

        /// <summary>
        /// Called when the ad is closed.
        /// </summary>
        public event ChartboostMediationFullscreenAdEventWithError DidClose;
        
        /// <summary>
        /// Called when the ad is expired by the partner SDK/adapter.
        /// </summary>
        public event ChartboostMediationFullscreenAdEvent DidExpire;
        
        /// <summary>
        /// Called when an ad impression occurs. This signal is when Chartboost Mediation fires an impression and is independent of any partner impression.
        /// </summary>
        public event ChartboostMediationFullscreenAdEvent DidRecordImpression;
        
        /// <summary>
        /// Called when the user should receive the reward associated with this rewarded ad.
        /// </summary>
        public event ChartboostMediationFullscreenAdEvent DidReward;
    }
}

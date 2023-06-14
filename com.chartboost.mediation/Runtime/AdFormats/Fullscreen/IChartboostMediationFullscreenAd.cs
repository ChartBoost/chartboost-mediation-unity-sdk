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
    }
}

using Chartboost.AdFormats.Banner;

namespace Chartboost.Requests
{
    /// <summary>
    /// The Chartboost Mediation Banner ad load result.
    /// </summary>
    public struct ChartboostMediationBannerAdLoadResult 
    {
        /// <summary>
        /// Constructor for successful loads
        /// </summary>
        /// <param name="loadId"></param>
        /// <param name="metrics"></param>
        /// <param name="error"></param>
        /// <param name="size"></param>
        public ChartboostMediationBannerAdLoadResult(string loadId, Metrics? metrics, ChartboostMediationError? error, ChartboostMediationBannerSize? size = null)
        {
            LoadId = loadId;
            Metrics = metrics;
            Error = error;
            Size = size;
        }
        
        /// <summary>
        /// Constructor for failed loads
        /// </summary>
        public ChartboostMediationBannerAdLoadResult(ChartboostMediationError error)
        {
            LoadId = string.Empty;
            Metrics = null;
            Error = error;
            Size = null;
        }

        /// <summary>
        /// The identifier for this load call.
        /// </summary>
        public string LoadId { get; }

        /// <summary>
        /// Metrics data for the ad load event.
        /// </summary>
        public Metrics? Metrics { get; }

        /// <summary>
        /// The error that occurred during the ad load event, if any.
        /// </summary>
        public ChartboostMediationError? Error { get;  }
        
        /// <summary>
        /// The size of the loaded ad in device independent pixels or `null` in case of load error.
        /// The requested size will be used as a fallback when a size is not available from the partner.
        /// </summary>
        public ChartboostMediationBannerSize? Size { get;  }
    }
}

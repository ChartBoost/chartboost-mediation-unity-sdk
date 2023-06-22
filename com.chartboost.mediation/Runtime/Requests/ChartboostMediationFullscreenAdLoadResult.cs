using Chartboost.AdFormats.Fullscreen;

namespace Chartboost.Requests
{
    #nullable enable
    /// <summary>
    /// The Chartboost Mediation fullscreen ad load result.
    /// </summary>
    public struct ChartboostMediationFullscreenAdLoadResult
    {
        /// <summary>
        /// Constructor for successful loads
        /// </summary>
        /// <param name="ad"></param>
        /// <param name="loadId"></param>
        /// <param name="metrics"></param>
        /// <param name="error"></param>
        public ChartboostMediationFullscreenAdLoadResult(IChartboostMediationFullscreenAd ad, string loadId, Metrics? metrics, ChartboostMediationError? error = null)
        {
            Ad = ad;
            LoadId = loadId;
            Metrics = metrics;
            Error = error;
        }

        /// <summary>
        /// Constructor for failed loads
        /// </summary>
        public ChartboostMediationFullscreenAdLoadResult(ChartboostMediationError error)
        {
            Ad = null;
            LoadId = string.Empty;
            Metrics = null;
            Error = error;
        }

        /// <summary>
        /// The <see cref="IChartboostMediationFullscreenAd"/> that was loaded, if any.
        /// </summary>
        public IChartboostMediationFullscreenAd? Ad { get;  }

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
    }
    #nullable disable
}

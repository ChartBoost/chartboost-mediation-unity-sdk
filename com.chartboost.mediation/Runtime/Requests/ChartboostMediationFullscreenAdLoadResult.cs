using Chartboost.AdFormats.Fullscreen;
using Chartboost.Placements;

namespace Chartboost.Requests
{
    #nullable enable
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

        public IChartboostMediationFullscreenAd? Ad { get;  }

        public string LoadId { get; }

        public Metrics? Metrics { get; }

        public ChartboostMediationError? Error { get;  }
    }
    #nullable disable
}

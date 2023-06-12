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
        /// <param name="requestId"></param>
        /// <param name="metrics"></param>
        /// <param name="error"></param>
        public ChartboostMediationFullscreenAdLoadResult(IChartboostMediationFullscreenAd ad, string requestId, Metrics? metrics, ChartboostMediationError? error = null)
        {
            AD = ad;
            RequestId = requestId;
            Metrics = metrics;
            Error = error;
        }

        /// <summary>
        /// Constructor for failed loads
        /// </summary>
        public ChartboostMediationFullscreenAdLoadResult(ChartboostMediationError error)
        {
            AD = null;
            RequestId = string.Empty;
            Metrics = null;
            Error = error;
        }

        public IChartboostMediationFullscreenAd? AD { get;  }

        public string RequestId { get; }

        public Metrics? Metrics { get; }

        public ChartboostMediationError? Error { get;  }
    }
    #nullable disable
}

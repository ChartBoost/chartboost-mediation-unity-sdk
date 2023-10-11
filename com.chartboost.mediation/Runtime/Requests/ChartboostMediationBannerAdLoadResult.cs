namespace Chartboost.Requests
{
    /// <summary>
    /// The Chartboost Mediation Banner ad load result.
    /// </summary>
    public class ChartboostMediationBannerAdLoadResult 
    {
        /// <summary>
        /// Constructor for successful loads
        /// </summary>
        /// <param name="loadId"></param>
        /// <param name="metrics"></param>
        /// <param name="error"></param>
        public ChartboostMediationBannerAdLoadResult(string loadId, Metrics? metrics, ChartboostMediationError? error)
        {
            LoadId = loadId;
            Metrics = metrics;
            Error = error;
        }
        
        /// <summary>
        /// Constructor for failed loads
        /// </summary>
        public ChartboostMediationBannerAdLoadResult(ChartboostMediationError error)
        {
            LoadId = string.Empty;
            Metrics = null;
            Error = error;
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
    }
}

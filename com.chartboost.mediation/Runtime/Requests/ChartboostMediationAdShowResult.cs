using System;

namespace Chartboost.Requests
{
    #nullable enable
    /// <summary>
    /// The Chartboost Mediation ad show result.
    /// </summary>
    [Serializable]
    public struct ChartboostMediationAdShowResult
    {
        public ChartboostMediationAdShowResult(Metrics? metrics, ChartboostMediationError? error = null)
        {
            Metrics = metrics;
            Error = error;
        }
        
        public ChartboostMediationAdShowResult(ChartboostMediationError error)
        {
            Metrics = null;
            Error = error;
        }
        
        /// <summary>
        /// Metrics data for the ad show event.
        /// </summary>
        public Metrics? Metrics { get; }

        /// <summary>
        /// The error that occurred during the ad show event, if any.
        /// </summary>
        public ChartboostMediationError? Error { get;  }
    }
    #nullable disable
}

using System;
using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;

namespace Chartboost.Mediation.Requests
{
    #nullable enable
    /// <summary>
    /// The Chartboost Mediation <see cref="IAd"/> show result.
    /// </summary>
    [Serializable]
    public struct AdShowResult
    {
        public AdShowResult(Metrics? metrics, ChartboostMediationError? error = null)
        {
            Metrics = metrics;
            Error = error;
        }
        
        public AdShowResult(ChartboostMediationError error)
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

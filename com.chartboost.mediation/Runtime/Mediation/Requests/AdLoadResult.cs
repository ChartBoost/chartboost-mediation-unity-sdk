using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;

namespace Chartboost.Mediation.Requests
{
    /// <summary>
    /// The Chartboost Mediation <see cref="IAd"/> load result.
    /// </summary>
    public interface IAdLoadResult 
    {
        /// <summary>
        /// The identifier for this load call.
        /// </summary>
        public string LoadId { get; }

        /// <summary>
        /// Metrics data for the ad load event.
        /// </summary>
        public Metrics? Metrics { get; }

        /// Information about the winning bid.
        public BidInfo? WinningBidInfo { get; }

        /// <summary>
        /// The error that occurred during the ad load event, if any.
        /// </summary>
        public ChartboostMediationError? Error { get;  }
    }
}

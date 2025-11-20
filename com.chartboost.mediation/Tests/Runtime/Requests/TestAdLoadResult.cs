using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;

namespace Chartboost.Tests.Runtime.Requests
{
    /// <summary>
    /// Simple implementation of IAdLoadResult for testing request-related functionality.
    /// All properties return default/empty values unless explicitly set.
    /// This helper is used across multiple test files that need to verify ad loading behavior.
    /// </summary>
    public class TestAdLoadResult : IAdLoadResult
    {
        /// <summary>
        /// Gets or sets the winning bid information for the ad load.
        /// Default: null
        /// </summary>
        public BidInfo? WinningBidInfo => null;

        /// <summary>
        /// Gets or sets the error that occurred during ad loading, if any.
        /// Default: null (indicating a successful load)
        /// </summary>
        public ChartboostMediationError? Error => null;

        /// <summary>
        /// Gets or sets the unique identifier for this ad load operation.
        /// Default: string.Empty
        /// </summary>
        public string LoadId => string.Empty;

        /// <summary>
        /// Gets or sets the metrics associated with this ad load operation.
        /// Default: null
        /// </summary>
        public Metrics? Metrics => null;
    }
}

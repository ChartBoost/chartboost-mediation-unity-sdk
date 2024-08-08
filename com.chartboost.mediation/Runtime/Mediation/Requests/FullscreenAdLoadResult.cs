using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;

namespace Chartboost.Mediation.Requests
{
    #nullable enable
    /// <summary>
    /// Chartboost Mediation's <see cref="IFullscreenAd"/> load result.
    /// </summary>
    public struct FullscreenAdLoadResult : IAdLoadResult
    {
        /// <summary>
        /// Constructor for successful loads
        /// </summary>
        /// <param name="ad"></param>
        /// <param name="loadId"></param>
        /// <param name="metrics"></param>
        /// <param name="winningBidInfo"></param>
        /// <param name="error"></param>
        public FullscreenAdLoadResult(IFullscreenAd ad, string loadId, Metrics? metrics, BidInfo? winningBidInfo, ChartboostMediationError? error = null)
        {
            Ad = ad;
            LoadId = loadId;
            Metrics = metrics;
            WinningBidInfo = winningBidInfo;
            Error = error;
        }

        /// <summary>
        /// Constructor for failed loads
        /// </summary>
        public FullscreenAdLoadResult(ChartboostMediationError error)
        {
            Ad = null;
            LoadId = null!;
            Metrics = null;
            Error = error;
            WinningBidInfo = null;
        }

        /// <summary>
        /// The <see cref="IFullscreenAd"/> that was loaded, if any.
        /// </summary>
        public IFullscreenAd? Ad { get;  }

        /// <inheritdoc cref="LoadId"/>
        public string LoadId { get; }

        /// <inheritdoc cref="Metrics"/>
        public Metrics? Metrics { get; }
        
        /// <inheritdoc cref="WinningBidInfo"/>
        public BidInfo? WinningBidInfo { get; }
        
        /// <summary>
        /// The error that occurred during the ad load event, if any.
        /// </summary>
        public ChartboostMediationError? Error { get;  }
    }
}

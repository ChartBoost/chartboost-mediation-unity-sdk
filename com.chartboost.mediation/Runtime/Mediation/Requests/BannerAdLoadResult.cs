using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;

namespace Chartboost.Mediation.Requests
{
    /// <summary>
    /// Chartboost Mediation's <see cref="IBannerAd"/> load result.
    /// </summary>
    public struct BannerAdLoadResult : IAdLoadResult
    {
        /// <summary>
        /// Constructor for successful loads
        /// </summary>
        /// <param name="loadId"></param>
        /// <param name="metrics"></param>
        /// <param name="error"></param>
        /// <param name="winningBidInfo"></param>
        /// <param name="size"></param>
        public BannerAdLoadResult(string loadId, Metrics? metrics, BidInfo? winningBidInfo, ChartboostMediationError? error, BannerSize? size = null)
        {
            LoadId = loadId;
            Metrics = metrics;
            WinningBidInfo = winningBidInfo;
            Error = error;
            Size = size;
        }
        
        /// <summary>
        /// Constructor for failed loads
        /// </summary>
        public BannerAdLoadResult(ChartboostMediationError error)
        {
            LoadId = string.Empty;
            Metrics = null;
            WinningBidInfo = null;
            Size = null;
            Error = error;
        }

        /// <inheritdoc cref="LoadId"/>
        public string LoadId { get; }

        /// <inheritdoc cref="Metrics"/>
        public Metrics? Metrics { get; }

        /// <inheritdoc cref="WinningBidInfo"/>
        public BidInfo? WinningBidInfo { get; }

        /// <inheritdoc cref="Error"/>
        public ChartboostMediationError? Error { get;  }
        
        /// <summary>
        /// The size of the loaded ad in device independent pixels or `null` in case of load error.
        /// The requested size will be used as a fallback when a size is not available from the partner.
        /// </summary>
        public BannerSize? Size { get;  }
    }
}

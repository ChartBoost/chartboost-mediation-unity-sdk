using System;

namespace Chartboost.FullScreen.Interstitial
{
    /// <summary>
    /// All interstitial placement callbacks.
    /// </summary>
    [Obsolete("IChartboostMediationInterstitialEvents has been deprecated, use the new fullscreen API instead.")]
    public interface IChartboostMediationInterstitialEvents
    {
        /// <summary>
        /// Called after an interstitial has been loaded from the Chartboost Mediation API
        /// servers and cached locally.
        /// </summary>
        [Obsolete("DidLoadInterstitial has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementLoadEvent DidLoadInterstitial;

        /// <summary>
        /// Called after an interstitial has been displayed on screen.
        /// </summary>
        [Obsolete("DidShowInterstitial has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementEvent DidShowInterstitial;

        /// <summary>
        /// Called after an interstitial has been closed.
        /// Implement to be notified of when an interstitial has been closed for a given placement.
        /// </summary>
        [Obsolete("DidCloseInterstitial has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementEvent DidCloseInterstitial;

        /// <summary>
        /// Called after an interstitial has been clicked.
        /// Implement to be notified of when an interstitial has been clicked for a given placement.
        /// </summary>
        [Obsolete("DidClickInterstitial has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementEvent DidClickInterstitial;
        
        /// <summary>
        /// Determines an ad visibility on the screen.
        /// Implement to be notified of when a interstitial ad has been become visible on the screen.
        /// </summary>
        [Obsolete("DidRecordImpressionInterstitial has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial;
    }
}

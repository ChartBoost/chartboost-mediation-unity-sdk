namespace Chartboost.FullScreen.Interstitial
{
    /// <summary>
    /// All interstitial placement callbacks.
    /// </summary>
    public interface IChartboostMediationInterstitialEvents
    {
        /// <summary>
        /// Called after an interstitial has been loaded from the Chartboost Mediation API
        /// servers and cached locally.
        /// </summary>
        public event ChartboostMediationPlacementLoadEvent DidLoadInterstitial;

        /// <summary>
        /// Called after an interstitial has been displayed on screen.
        /// </summary>
        public event ChartboostMediationPlacementEvent DidShowInterstitial;

        /// <summary>
        /// Called after an interstitial has been closed.
        /// Implement to be notified of when an interstitial has been closed for a given placement.
        /// </summary>
        public event ChartboostMediationPlacementEvent DidCloseInterstitial;

        /// <summary>
        /// Called after an interstitial has been clicked.
        /// Implement to be notified of when an interstitial has been clicked for a given placement.
        /// </summary>
        public event ChartboostMediationPlacementEvent DidClickInterstitial;
        
        /// <summary>
        /// Determines an ad visibility on the screen.
        /// Implement to be notified of when a interstitial ad has been become visible on the screen.
        /// </summary>
        public event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial;
    }
}

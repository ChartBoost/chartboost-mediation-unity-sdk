// ReSharper disable InconsistentNaming
using UnityEngine.Scripting;

namespace Chartboost.Banner
{
    /// <summary>
    /// All Banner placement callbacks.
    /// </summary>
    public interface IChartboostMediationBannerEvents
    {
        /// <summary>
        /// Called after a banner has been loaded from the Chartboost Mediation API
        /// servers and cached locally.
        /// </summary>
        [Preserve]
        public event ChartboostMediationPlacementLoadEvent DidLoadBanner;

        /// <summary>
        /// Called after a banner ad has been clicked.
        /// Implement to be notified of when a banner ad has been clicked for a given placement
        /// </summary>
        [Preserve]
        public event ChartboostMediationPlacementEvent DidClickBanner;

        /// <summary>
        /// Determines an ad visibility on the screen.
        /// Implement to be notified of when a banner ad has been become visible on the screen.
        /// </summary>
        [Preserve]
        public event ChartboostMediationPlacementEvent DidRecordImpressionBanner;
    }
}

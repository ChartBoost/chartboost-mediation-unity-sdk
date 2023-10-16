using System;

namespace Chartboost.FullScreen.Rewarded
{
    /// <summary>
    /// Interface implemented by Chartboost Mediation rewarded ads. 
    /// </summary>
    public interface IChartboostMediationRewardedEvents
    {
        /// <summary>
        /// Called after a rewarded ad has been loaded from the Chartboost Mediation API
        /// servers and cached locally.
        /// </summary>
        [Obsolete("DidLoadRewarded has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementLoadEvent DidLoadRewarded;

        /// <summary>
        /// Called after a rewarded ad has been displayed on screen.
        /// </summary>
        [Obsolete("DidShowRewarded has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementEvent DidShowRewarded;

        /// <summary>
        /// Called after a rewarded ad has been closed.
        /// Implement to be notified of when a rewarded ad has been closed for a given placement
        /// </summary>
        [Obsolete("DidCloseRewarded has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementEvent DidCloseRewarded;

        /// <summary>
        /// Called after a rewarded ad has been clicked.
        /// Implement to be notified of when a rewarded ad has been clicked for a given placement
        /// </summary>
        [Obsolete("DidClickRewarded has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementEvent DidClickRewarded;
        
        /// <summary>
        /// Determines an ad visibility on the screen.
        /// Implement to be notified of when a rewarded ad has been become visible on the screen.
        /// </summary>
        [Obsolete("DidRecordImpressionRewarded has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementEvent DidRecordImpressionRewarded;

        /// <summary>
        /// Called after a rewarded has been received (after watching a rewarded video).
        /// Implement to be notified of when a user has earned a reward.
        /// This version could be called on a background thread, even if the Unity runtime is paused.
        /// </summary>
        [Obsolete("DidReceiveReward has been deprecated, use the new fullscreen API instead.")]
        public event ChartboostMediationPlacementEvent DidReceiveReward;
    }
}

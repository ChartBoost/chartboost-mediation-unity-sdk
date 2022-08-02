// ReSharper disable InconsistentNaming
using UnityEngine.Scripting;

namespace Helium.Interfaces
{
    /// <summary>
    /// All Rewarded placement callbacks.
    /// </summary>
    public interface IRewardedEvents
    {
        /// <summary>
        /// Called after a rewarded ad has been loaded from the Helium API
        /// servers and cached locally.
        /// </summary>
        [Preserve]
        public event HeliumPlacementEvent DidLoadRewarded;

        /// <summary>
        /// Called after a rewarded ad has been displayed on screen.
        /// </summary>
        [Preserve]
        public event HeliumPlacementEvent DidShowRewarded;

        /// <summary>
        /// Called after a rewarded ad has been closed.
        /// Implement to be notified of when a rewarded ad has been closed for a given placement
        /// </summary>
        [Preserve]
        public event HeliumPlacementEvent DidCloseRewarded;

        /// <summary>
        /// Called after a rewarded ad has been clicked.
        /// Implement to be notified of when a rewarded ad has been clicked for a given placement
        /// </summary>
        [Preserve]
        public event HeliumPlacementEvent DidClickRewarded;
        
        /// <summary>
        /// Called with bid information after an rewarded ad has been loaded from the Helium API
        /// servers and cached locally.
        /// </summary>
        [Preserve]
        public event HeliumBidEvent DidWinBidRewarded;
        
        /// <summary>
        /// Determines an ad visibility on the screen.
        /// Implement to be notified of when a rewarded ad has been become visible on the screen.
        /// </summary>
        [Preserve]
        public event HeliumPlacementEvent DidRecordImpressionRewarded;

        /// <summary>
        /// Called after a rewarded has been received (after watching a rewarded video).
        /// Implement to be notified of when a user has earned a reward.
        /// This version could be called on a background thread, even if the Unity runtime is paused.
        /// </summary>
        [Preserve]
        public event HeliumRewardEvent DidReceiveReward;
    }
}

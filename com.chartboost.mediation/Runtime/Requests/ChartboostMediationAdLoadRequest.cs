using Chartboost.Utilities;

namespace Chartboost.Requests
{
    /// <summary>
    /// Base class utilized by all load request.
    /// </summary>
    public class ChartboostMediationAdLoadRequest
    {
        /// <summary>
        /// Base constructor for all load requests.
        /// </summary>
        /// <param name="placementName"></param>
        internal ChartboostMediationAdLoadRequest(string placementName) => PlacementName = placementName;

        /// <summary>
        /// The placement name for the ad.
        /// </summary>
        public string PlacementName { get; }

        internal long AssociatedProxy { get; set; }
        
        ~ChartboostMediationAdLoadRequest()
        {
            // In case that the request never finishes, and no response ever happens, if disposed by GC remove from Cached requests.
            CacheManager.ReleaseFullscreenAdLoadRequest(AssociatedProxy);
        }
    }
}

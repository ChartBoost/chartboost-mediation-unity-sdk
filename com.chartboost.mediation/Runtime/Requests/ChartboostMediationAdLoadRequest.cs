using System.Collections.Generic;
using Chartboost.AdFormats.Fullscreen;
using Chartboost.Events;
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
        /// <param name="keywords"></param>
        internal ChartboostMediationAdLoadRequest(string placementName, Dictionary<string, string> keywords)
        {
            PlacementName = placementName;
            Keywords = keywords;
        }

        /// <summary>
        /// The placement name for the ad.
        /// </summary>
        public string PlacementName { get; }
        
        /// <summary>
        /// The keywords targeted for the ad.
        /// </summary>
        public Dictionary<string, string> Keywords { get;  }

        internal int AssociatedProxy { get; set; }
        
        ~ChartboostMediationAdLoadRequest()
        {
            // In case that the request never finishes, and no response ever happens, if disposed by GC remove from Cached requests.
            CacheManager.ReleaseFullscreenAdLoadRequest(AssociatedProxy);
        }
    }
}

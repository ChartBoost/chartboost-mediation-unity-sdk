using Chartboost.AdFormats.Banner;
using Chartboost.Banner;
using Chartboost.Utilities;

namespace Chartboost.Requests
{
    /// <summary>
    /// Load request for banner ad.
    /// </summary>
    public class ChartboostMediationBannerAdLoadRequest
    {
        public ChartboostMediationBannerAdLoadRequest(string placementName, ChartboostMediationBannerSize size)
        {
            PlacementName = placementName;
            Size = size;
        }
        
        /// <summary>
        /// The placement name for the ad.
        /// </summary>
        public string PlacementName { get; set; }
        
        /// <summary>
        /// The <see cref="ChartboostMediationBannerSize"/> size for the request 
        /// </summary>
        public ChartboostMediationBannerSize Size { get; set; }

        internal long AssociatedProxy { get; set; }
        
        ~ChartboostMediationBannerAdLoadRequest()
        {
            // In case that the request never finishes, and no response ever happens, if disposed by GC remove from Cached requests.
            CacheManager.ReleaseBannerAdLoadRequest(AssociatedProxy);
        }
    }
}

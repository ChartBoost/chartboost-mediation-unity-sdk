using Chartboost.AdFormats.Banner;
using Chartboost.Banner;
using Chartboost.Utilities;

namespace Chartboost.Requests
{
    public class ChartboostMediationBannerAdLoadRequest
    {
        public ChartboostMediationBannerAdLoadRequest(string placementName, ChartboostMediationBannerSize size)
        {
            PlacementName = placementName;
            Size = size;
        }

        public string PlacementName { get; set; }
        public ChartboostMediationBannerSize Size { get; set; }

        internal long AssociatedProxy { get; set; }
        
        ~ChartboostMediationBannerAdLoadRequest()
        {
            // In case that the request never finishes, and no response ever happens, if disposed by GC remove from Cached requests.
            CacheManager.ReleaseBannerAdLoadRequest(AssociatedProxy);
        }
    }
}

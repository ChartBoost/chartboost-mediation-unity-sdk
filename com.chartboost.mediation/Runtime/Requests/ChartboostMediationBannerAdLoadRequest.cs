using Chartboost.AdFormats.Banner;
using Chartboost.Banner;
using Chartboost.Utilities;

namespace Chartboost.Requests
{
    public class ChartboostMediationBannerAdLoadRequest
    {
        public ChartboostMediationBannerAdLoadRequest(string placementName, ChartboostMediationBannerAdSize adSize)
        {
            PlacementName = placementName;
            AdSize = adSize;
        }

        public string PlacementName { get; set; }
        public ChartboostMediationBannerAdSize AdSize { get; set; }

        internal long AssociatedProxy { get; set; }
        
        ~ChartboostMediationBannerAdLoadRequest()
        {
            // In case that the request never finishes, and no response ever happens, if disposed by GC remove from Cached requests.
            CacheManager.ReleaseBannerAdLoadRequest(AssociatedProxy);
        }
    }
}
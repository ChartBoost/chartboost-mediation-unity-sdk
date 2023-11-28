using Chartboost;
using Chartboost.AdFormats.Banner;
using JetBrains.Annotations;

namespace AdController
{
    public class AdLoadDetails
    {
        public string placementName;
        public string loadId;
        public BidInfo bidInfo;
        public Metrics? metrics;
        [CanBeNull] public string customData;
        [CanBeNull] public ChartboostMediationError? error;
        public AdLoadDetails() { }

        public AdLoadDetails(ChartboostMediationError? error) {
            this.error = error;
        }
    }

    public class BannerAdLoadDetails : AdLoadDetails
    {
        public ChartboostMediationBannerSize containerSize;
        public ChartboostMediationBannerSize adSize;

        public BannerAdLoadDetails()
        {
            
        }
        
        public BannerAdLoadDetails(AdLoadDetails adLoadDetails)
        {
            placementName = adLoadDetails.placementName;
            loadId = adLoadDetails.loadId;
            bidInfo = adLoadDetails.bidInfo;
            metrics = adLoadDetails.metrics;
            customData = adLoadDetails.customData;
            error = adLoadDetails.error;
        }
    }
}

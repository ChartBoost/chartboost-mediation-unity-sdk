using System.Collections.Generic;
using Chartboost.AdFormats.Fullscreen;
using Chartboost.Placements;

namespace Chartboost.Requests
{
    public class ChartboostMediationAdLoadRequest
    {
        internal ChartboostMediationAdLoadRequest(string placementName, Dictionary<string, string> keywords)
        {
            PlacementName = placementName;
            Keywords = keywords;
        }

        public string PlacementName { get; }
        public Dictionary<string, string> Keywords { get;  }

        internal int AssociatedProxy { get; set; }
        ~ChartboostMediationAdLoadRequest()
        {
            // In case that the request never finishes, and no response ever happens, if disposed by GC remove from Cached requests.
            CacheManager.ReleaseFullscreenAdLoadRequest(AssociatedProxy);
        }
    }

    public class ChartboostMediationFullscreenAdLoadRequest : ChartboostMediationAdLoadRequest
    {
        public ChartboostMediationFullscreenAdLoadRequest(string placementName, Dictionary<string, string> keywords) : base(placementName, keywords) { }

        public event ChartboostMediationFullscreenAdEvent DidClick;

        public event ChartboostMediationFullscreenAdEventWithError DidClose;
        
        public event ChartboostMediationFullscreenAdEvent DidExpire;
        
        public event ChartboostMediationFullscreenAdEvent DidRecordImpression;
        
        public event ChartboostMediationFullscreenAdEvent DidReward;

        internal void OnClick(IChartboostMediationFullscreenAd ad) => DidClick?.Invoke(ad);

        internal void OnClose(IChartboostMediationFullscreenAd ad, ChartboostMediationError? error) => DidClose?.Invoke(ad, error);

        internal void OnRecordImpression(IChartboostMediationFullscreenAd ad) => DidRecordImpression?.Invoke(ad);

        internal void OnExpire(IChartboostMediationFullscreenAd ad) => DidExpire?.Invoke(ad);

        internal void OnReward(IChartboostMediationFullscreenAd ad) => DidReward?.Invoke(ad);
    }
}

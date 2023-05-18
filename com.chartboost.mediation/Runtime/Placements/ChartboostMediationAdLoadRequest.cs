using System;
using System.Collections.Generic;

namespace Chartboost.Placements
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

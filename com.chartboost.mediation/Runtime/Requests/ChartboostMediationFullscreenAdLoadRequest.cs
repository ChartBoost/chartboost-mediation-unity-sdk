using System.Collections.Generic;
using Chartboost.AdFormats.Fullscreen;

namespace Chartboost.Requests
{
    /// <summary>
    /// The Chartboost Mediation fullscreen ad load request.
    /// </summary>
    public sealed class ChartboostMediationFullscreenAdLoadRequest : ChartboostMediationAdLoadRequest
    {
        public ChartboostMediationFullscreenAdLoadRequest(string placementName, Dictionary<string, string> keywords) : base(placementName) => Keywords = keywords;

        /// <summary>
        /// The keywords targeted for the ad.
        /// </summary>
        public Dictionary<string, string> Keywords { get;  }
        
        /// <summary>
        /// Called when the ad executes its click-through. This may happen multiple times for the same ad.
        /// </summary>
        public event ChartboostMediationFullscreenAdEvent DidClick;

        /// <summary>
        /// Called when the ad is closed.
        /// </summary>
        public event ChartboostMediationFullscreenAdEventWithError DidClose;
        
        /// <summary>
        /// Called when the ad is expired by the partner SDK/adapter.
        /// </summary>
        public event ChartboostMediationFullscreenAdEvent DidExpire;
        
        /// <summary>
        /// Called when an ad impression occurs. This signal is when Chartboost Mediation fires an impression and is independent of any partner impression.
        /// </summary>
        public event ChartboostMediationFullscreenAdEvent DidRecordImpression;
        
        /// <summary>
        /// Called when the user should receive the reward associated with this rewarded ad.
        /// </summary>
        public event ChartboostMediationFullscreenAdEvent DidReward;

        internal void OnClick(IChartboostMediationFullscreenAd ad) => DidClick?.Invoke(ad);

        internal void OnClose(IChartboostMediationFullscreenAd ad, ChartboostMediationError? error) => DidClose?.Invoke(ad, error);

        internal void OnRecordImpression(IChartboostMediationFullscreenAd ad) => DidRecordImpression?.Invoke(ad);

        internal void OnExpire(IChartboostMediationFullscreenAd ad) => DidExpire?.Invoke(ad);

        internal void OnReward(IChartboostMediationFullscreenAd ad) => DidReward?.Invoke(ad);
    }
}

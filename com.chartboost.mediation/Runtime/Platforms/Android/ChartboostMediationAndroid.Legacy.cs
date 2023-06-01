#if UNITY_ANDROID
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Chartboost.Platforms.Android
{
    public sealed partial class ChartboostMediationAndroid
    {
        #region Interstitial Callbacks
        internal class InterstitialEventListener : AndroidJavaProxy
        {
            private InterstitialEventListener() : base(GetQualifiedClassName("IInterstitialEventListener")) { }

            public static readonly InterstitialEventListener Instance = new InterstitialEventListener();

            private void DidLoadInterstitial(string placementName, string loadId, string auctionId, string partnerId, double price, string error)
                => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadInterstitial);

            private void DidShowInterstitial(string placementName, string error) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidShowInterstitial);

            private void DidCloseInterstitial(string placementName, string error) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidCloseInterstitial);
            
            private void DidClickInterstitial(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidClickInterstitial);

            private void DidRecordImpression(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidRecordImpressionInterstitial);
        }

        public override event ChartboostMediationPlacementLoadEvent DidLoadInterstitial;
        public override event ChartboostMediationPlacementEvent DidShowInterstitial;
        public override event ChartboostMediationPlacementEvent DidCloseInterstitial; 
        public override event ChartboostMediationPlacementEvent DidClickInterstitial;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial;
        #endregion

        #region Rewarded Callbacks
        internal class RewardedVideoEventListener : AndroidJavaProxy
        {
            private RewardedVideoEventListener() : base(GetQualifiedClassName("IRewardedEventListener")) { }

            public static readonly RewardedVideoEventListener Instance = new RewardedVideoEventListener();

            private void DidLoadRewarded(string placementName, string loadId, string auctionId, string partnerId, double price, string error) 
                => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadRewarded);

            private void DidShowRewarded(string placementName, string error) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidShowRewarded);

            private void DidCloseRewarded(string placementName, string error) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidCloseRewarded);

            private void DidClickRewarded(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidClickRewarded);

            private void DidRecordImpression(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidRecordImpressionRewarded);

            private void DidReceiveReward(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidReceiveReward);
        }

        public override event ChartboostMediationPlacementLoadEvent DidLoadRewarded;
        public override event ChartboostMediationPlacementEvent DidShowRewarded;
        public override event ChartboostMediationPlacementEvent DidCloseRewarded;
        public override event ChartboostMediationPlacementEvent DidClickRewarded;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionRewarded;
        public override event ChartboostMediationPlacementEvent DidReceiveReward;
        #endregion
    }
}
#endif

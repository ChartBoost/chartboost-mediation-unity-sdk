#if UNITY_ANDROID
using System;
using Chartboost.Events;
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Chartboost.Platforms.Android
{
    internal sealed partial class ChartboostMediationAndroid
    {
        #region Interstitial Callbacks
        internal class InterstitialEventListener : AndroidJavaProxy
        {
            private InterstitialEventListener() : base(GetQualifiedClassName("IInterstitialEventListener")) { }

            public static readonly InterstitialEventListener Instance = new InterstitialEventListener();

            private void DidLoadInterstitial(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error)
                => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price,  lineItemName, lineItemId, error, _instance.DidLoadInterstitial);

            private void DidShowInterstitial(string placementName, string error) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidShowInterstitial);

            private void DidCloseInterstitial(string placementName, string error) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidCloseInterstitial);
            
            private void DidClickInterstitial(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidClickInterstitial);

            private void DidRecordImpression(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidRecordImpressionInterstitial);
        }

        [Obsolete("DidLoadInterstitial has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementLoadEvent DidLoadInterstitial;
        [Obsolete("DidShowInterstitial has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidShowInterstitial;
        [Obsolete("DidCloseInterstitial has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidCloseInterstitial;
        [Obsolete("DidClickInterstitial has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidClickInterstitial;
        [Obsolete("DidRecordImpressionInterstitial has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial;
        #endregion

        #region Rewarded Callbacks
        internal class RewardedVideoEventListener : AndroidJavaProxy
        {
            private RewardedVideoEventListener() : base(GetQualifiedClassName("IRewardedEventListener")) { }

            public static readonly RewardedVideoEventListener Instance = new RewardedVideoEventListener();

            private void DidLoadRewarded(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error) 
                => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, lineItemName, lineItemId, error, _instance.DidLoadRewarded);

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

        [Obsolete("DidLoadRewarded has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementLoadEvent DidLoadRewarded;
        [Obsolete("DidShowRewarded has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidShowRewarded;
        [Obsolete("DidCloseRewarded has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidCloseRewarded;
        [Obsolete("DidClickRewarded has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidClickRewarded;
        [Obsolete("DidRecordImpressionRewarded has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidRecordImpressionRewarded;
        [Obsolete("DidReceiveReward has been deprecated, use the new fullscreen API instead.")]
        public override event ChartboostMediationPlacementEvent DidReceiveReward;
        #endregion
        
        #region Banner Callbacks
        internal class BannerEventListener : AndroidJavaProxy
        {
            private BannerEventListener() : base(GetQualifiedClassName("IBannerEventListener")) { }

            public static readonly BannerEventListener Instance = new BannerEventListener();

            private void DidLoadBanner(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error) 
                => EventProcessor.ProcessChartboostMediationLoadEvent(placementName,  loadId, auctionId, partnerId, price, lineItemName, lineItemId, error, _instance.DidLoadBanner);

            private void DidClickBanner(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidClickBanner);

            private void DidRecordImpression(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidRecordImpressionBanner);
        }

        [Obsolete("DidLoadBanner has been deprecated, use the new ChartboostMediationBannerView API instead.")]
        public override event ChartboostMediationPlacementLoadEvent DidLoadBanner;
        [Obsolete("DidClickBanner has been deprecated, use the new ChartboostMediationBannerView API instead.")]
        public override event ChartboostMediationPlacementEvent DidClickBanner;
        [Obsolete("DidRecordImpressionBanner has been deprecated, use the new ChartboostMediationBannerView API instead.")]
        public override event ChartboostMediationPlacementEvent DidRecordImpressionBanner;
        #endregion
    }
}
#endif

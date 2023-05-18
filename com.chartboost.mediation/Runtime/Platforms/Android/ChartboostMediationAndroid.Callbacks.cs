using System;
using System.Collections.Generic;
using Chartboost.Placements;
using Chartboost.Utilities;
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Chartboost.Platforms.Android
{
    public sealed partial class ChartboostMediationAndroid
    {
        #region LifeCycle Callbacks
        internal class LifeCycleEventListener : AndroidJavaProxy
        {
            private LifeCycleEventListener() : base(GetQualifiedClassName("ILifeCycleEventListener")) { }

            public static readonly LifeCycleEventListener Instance = new LifeCycleEventListener();

            private void DidStart(string error) 
                => EventProcessor.ProcessChartboostMediationEvent(error, _instance.DidStart);

            private void DidReceiveILRD(string impressionDataJson) 
                => EventProcessor.ProcessEventWithILRD(impressionDataJson, _instance.DidReceiveImpressionLevelRevenueData);

            private void DidReceivePartnerInitializationData(string partnerInitializationDataJson) 
                => EventProcessor.ProcessEventWithPartnerInitializationData(partnerInitializationDataJson, _instance.DidReceivePartnerInitializationData);
        }

        public override event ChartboostMediationEvent DidStart;
        public override event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData;
        public override event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData;
        #endregion
        
        internal class CMFullscreenAdLoadResultHandler : AwaitableAndroidJavaProxy<ChartboostMediationFullscreenAdLoadResult>
        {
            private ChartboostMediationFullscreenAdLoadRequest _request;
            public CMFullscreenAdLoadResultHandler(ChartboostMediationFullscreenAdLoadRequest request) : base(GetQualifiedClassName("CMFullscreenAdLoadResultHandler"))
            {
                _request = request;
            }

            private void onAdLoadResult(AndroidJavaObject adLoadResult)
            {
                var error = adLoadResult.ToChartboostMediationError();
                if (error.HasValue)
                {
                    _complete(new ChartboostMediationFullscreenAdLoadResult(error.Value));
                    return;
                }

                var ad = adLoadResult.FullscreenFromAdResult(_request);
                var requestIdentifier = adLoadResult.Get<string>("requestIdentifier");
                var metrics = adLoadResult.Get<AndroidJavaObject>("metrics").JsonObjectToMetrics();
                _complete(new ChartboostMediationFullscreenAdLoadResult(ad, requestIdentifier, metrics));
            }
        }
        
        internal class CMAdShowResultHandler : AwaitableAndroidJavaProxy<ChartboostMediationAdShowResult>
        {
            public CMAdShowResultHandler() : base(GetQualifiedClassName("CMAdShowResultHandler")) { } 
            
            private void onAdShowResult(AndroidJavaObject adShowResult)
            {
                var error = adShowResult.ToChartboostMediationError();
                if (error.HasValue)
                {
                    _complete(new ChartboostMediationAdShowResult(error.Value));
                    return;
                }
                
                var metrics = adShowResult.Get<AndroidJavaObject>("metrics").JsonObjectToMetrics();
                _complete(new ChartboostMediationAdShowResult(metrics));
            }
        }

        internal class CMFullscreenAdListener : AndroidJavaProxy
        {
            private readonly ChartboostMediationFullscreenAdLoadRequest _listeners;
            public CMFullscreenAdListener(ChartboostMediationFullscreenAdLoadRequest listeners) : base("com.chartboost.heliumsdk.ad.ChartboostMediationFullscreenAdListener")
            {
                _listeners = listeners;
            }

            private void onAdClicked(AndroidJavaObject ad)
            {
                _listeners.OnClick(new ChartboostMediationFullscreenAdAndroid(ad, _listeners));
            }
            
            private void onAdClosed(AndroidJavaObject ad, AndroidJavaObject error)
            {
                ChartboostMediationError? mediationError = null;
                if (error != null)
                    mediationError = error.ToChartboostMediationError("chartboostMediationError");
                _listeners.OnClose(new ChartboostMediationFullscreenAdAndroid(ad, _listeners), mediationError);
            }
            
            private void onAdExpired(AndroidJavaObject ad)
            {
                _listeners.OnExpire(new ChartboostMediationFullscreenAdAndroid(ad, _listeners));
            }
            
            private void onAdImpressionRecorded(AndroidJavaObject ad)
            {
                _listeners.OnRecordImpression(new ChartboostMediationFullscreenAdAndroid(ad, _listeners));
            }
            
            private void onAdRewarded(AndroidJavaObject ad)
            {
                _listeners.OnReward(new ChartboostMediationFullscreenAdAndroid(ad, _listeners)); 
            }
        }
        
        
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

        #region Banner Callbacks
        internal class BannerEventListener : AndroidJavaProxy
        {
            private BannerEventListener() : base(GetQualifiedClassName("IBannerEventListener")) { }

            public static readonly BannerEventListener Instance = new BannerEventListener();

            private void DidLoadBanner(string placementName, string loadId, string auctionId, string partnerId, double price, string error) 
                => EventProcessor.ProcessChartboostMediationLoadEvent(placementName,  loadId, auctionId, partnerId, price, error, _instance.DidLoadBanner);

            private void DidClickBanner(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidClickBanner);

            private void DidRecordImpression(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidRecordImpressionBanner);
        }

        public override event ChartboostMediationPlacementLoadEvent DidLoadBanner;
        public override event ChartboostMediationPlacementEvent DidClickBanner;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionBanner;
        #endregion
    }
}

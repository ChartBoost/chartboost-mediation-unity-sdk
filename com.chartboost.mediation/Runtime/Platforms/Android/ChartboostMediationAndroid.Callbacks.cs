#if UNITY_ANDROID
using Chartboost.AdFormats.Fullscreen;
using Chartboost.Events;
using Chartboost.Requests;
using Chartboost.Utilities;
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Chartboost.Platforms.Android
{
    public sealed partial class ChartboostMediationAndroid
    {
        #region LifeCycle Callbacks
        internal class ChartboostMediationSDKListener : AndroidJavaProxy
        {
            public ChartboostMediationSDKListener() : base(GetQualifiedNativeClassName("HeliumSdk$HeliumSdkListener")) { }

            private void didInitialize(AndroidJavaObject error)
            {
                EventProcessor.ProcessEvent(() => {
                    if (error != null)
                    {
                        var message = error.Call<string>("toString");
                        EventProcessor.ProcessChartboostMediationEvent(message, _instance.DidStart);
                        return;
                    }
                    
                    using var nativeSDK = GetNativeSDK();
                    nativeSDK.CallStatic("setGameEngine", "unity", Application.unityVersion);
                    nativeSDK.CallStatic("subscribeIlrd", ILRDObserver.Instance);
                    nativeSDK.CallStatic("subscribeInitializationResults", new PartnerInitializationResultsObserver());
                    EventProcessor.ProcessChartboostMediationEvent(null, _instance.DidStart);
                });
            }
        }

        internal class ILRDObserver : AndroidJavaProxy
        {
            private ILRDObserver() : base(GetQualifiedNativeClassName("HeliumIlrdObserver")) { }
            public static readonly ILRDObserver Instance = new ILRDObserver();

            private void onImpression(AndroidJavaObject impressionData) 
                => EventProcessor.ProcessEventWithILRD(impressionData.ImpressionDataToJsonString(), _instance.DidReceiveImpressionLevelRevenueData);
        }

        internal class PartnerInitializationResultsObserver : AndroidJavaProxy
        {
            public PartnerInitializationResultsObserver() : base(GetQualifiedNativeClassName("PartnerInitializationResultsObserver")) { }
            
            private void onPartnerInitializationResultsReady(AndroidJavaObject data) 
                => EventProcessor.ProcessEventWithPartnerInitializationData(data.PartnerInitializationDataToJsonString(), _instance.DidReceivePartnerInitializationData);
        }

        public override event ChartboostMediationEvent DidStart;
        public override event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData;
        public override event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData;
        #endregion

        #region Fullscreen Callbacks
        internal class ChartboostMediationFullscreenAdLoadListener : AwaitableAndroidJavaProxy<ChartboostMediationFullscreenAdLoadResult>
        {
            public ChartboostMediationFullscreenAdLoadListener() : base(GetQualifiedNativeClassName("ChartboostMediationFullscreenAdLoadListener", true)) { }

            private void onAdLoaded(AndroidJavaObject result)
            {
                EventProcessor.ProcessEvent(() =>
                {
                    var error = result.ToChartboostMediationError();
                    if (error.HasValue)
                    {
                        CacheManager.ReleaseFullscreenAdLoadRequest(hashCode());
                        _complete(new ChartboostMediationFullscreenAdLoadResult(error.Value));
                        return;
                    }

                    var nativeAd = result.Get<AndroidJavaObject>("ad");
                    var ad = new ChartboostMediationFullscreenAdAndroid(nativeAd, CacheManager.GetFullScreenAdLoadRequest(hashCode()));
                    var loadId = result.Get<string>("loadId");
                    var metrics = result.Get<AndroidJavaObject>("metrics").JsonObjectToMetrics();
                    _complete(new ChartboostMediationFullscreenAdLoadResult(ad, loadId, metrics));
                });
            }
        }
        
        internal class ChartboostMediationFullscreenAdShowListener : AwaitableAndroidJavaProxy<ChartboostMediationAdShowResult>
        {
            public ChartboostMediationFullscreenAdShowListener() : base(GetQualifiedNativeClassName("ChartboostMediationFullscreenAdShowListener", true)) { } 
            
            private void onAdShown(AndroidJavaObject result)
            {
                EventProcessor.ProcessEvent(() => { 
                    var error = result.ToChartboostMediationError();
                    if (error.HasValue)
                    {
                        _complete(new ChartboostMediationAdShowResult(error.Value));
                        return;
                    }
                    var metrics = result.Get<AndroidJavaObject>("metrics").JsonObjectToMetrics();
                    _complete(new ChartboostMediationAdShowResult(metrics));
                });
            }
        }

        internal class ChartboostMediationFullscreenAdListener : AndroidJavaProxy
        {
            internal static readonly ChartboostMediationFullscreenAdListener Instance = new ChartboostMediationFullscreenAdListener();

            private ChartboostMediationFullscreenAdListener() : base(GetQualifiedNativeClassName("ChartboostMediationFullscreenAdListener", true)) { }

            private void onAdClicked(AndroidJavaObject ad) 
                => EventProcessor.ProcessFullscreenEvent(ad.HashCode(), (int)EventProcessor.FullscreenAdEvents.Click, null, null);

            private void onAdClosed(AndroidJavaObject ad, AndroidJavaObject error)
            {
                string code = null;
                string message = null;

                var mediationError = error?.Get<AndroidJavaObject>("chartboostMediationError");
                if (mediationError != null)
                {
                    code = error.Get<string>("code");
                    message = error.Call<string>("toString");
                }
                
                EventProcessor.ProcessFullscreenEvent(ad.HashCode(), (int)EventProcessor.FullscreenAdEvents.Close, code, message);
            }
            
            private void onAdExpired(AndroidJavaObject ad)
                => EventProcessor.ProcessFullscreenEvent(ad.HashCode(), (int)EventProcessor.FullscreenAdEvents.Expire, null, null);

            private void onAdImpressionRecorded(AndroidJavaObject ad)
                => EventProcessor.ProcessFullscreenEvent(ad.HashCode(), (int)EventProcessor.FullscreenAdEvents.RecordImpression, null, null);

            private void onAdRewarded(AndroidJavaObject ad) 
                => EventProcessor.ProcessFullscreenEvent(ad.HashCode(), (int)EventProcessor.FullscreenAdEvents.Reward,null, null);
        }
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

        public override event ChartboostMediationPlacementLoadEvent DidLoadBanner;
        public override event ChartboostMediationPlacementEvent DidClickBanner;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionBanner;
        #endregion
    }
}
#endif

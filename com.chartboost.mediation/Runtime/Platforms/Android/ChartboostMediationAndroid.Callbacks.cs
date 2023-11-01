#if UNITY_ANDROID
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Fullscreen;
using Chartboost.Events;
using Chartboost.Requests;
using Chartboost.Utilities;
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Chartboost.Platforms.Android
{
    internal sealed partial class ChartboostMediationAndroid
    {
        #region LifeCycle Callbacks
        internal class ChartboostMediationSDKListener : AndroidJavaProxy
        {
            public ChartboostMediationSDKListener() : base(GetQualifiedNativeClassName(AndroidConstants.ClassHeliumSDKListener)) { }

            private void didInitialize(AndroidJavaObject error)
            {
                EventProcessor.ProcessEvent(() => {
                    if (error != null)
                    {
                        var message = error.Call<string>(AndroidConstants.FunToString);
                        EventProcessor.ProcessChartboostMediationEvent(message, _instance.DidStart);
                        return;
                    }
                    
                    using var nativeSDK = GetNativeSDK();
                    nativeSDK.CallStatic(AndroidConstants.FunSetGameEngine, AndroidConstants.GameEngine, Application.unityVersion);
                    nativeSDK.CallStatic(AndroidConstants.FunSubscribeIlrd, ILRDObserver.Instance);
                    nativeSDK.CallStatic(AndroidConstants.FunSubscribeInitializationResults, new PartnerInitializationResultsObserver());
                    EventProcessor.ProcessChartboostMediationEvent(null, _instance.DidStart);
                });
            }
        }

        internal class ILRDObserver : AndroidJavaProxy
        {
            private ILRDObserver() : base(GetQualifiedNativeClassName(AndroidConstants.ClassHeliumIlrdObserver)) { }
            public static readonly ILRDObserver Instance = new ILRDObserver();

            private void onImpression(AndroidJavaObject impressionData) 
                => EventProcessor.ProcessEventWithILRD(impressionData.ImpressionDataToJsonString(), _instance.DidReceiveImpressionLevelRevenueData);
        }
        
        internal class PartnerInitializationResultsObserver : AndroidJavaProxy
        {
            public PartnerInitializationResultsObserver() : base(GetQualifiedNativeClassName(AndroidConstants.ClassPartnerInitializationResultsObserver)) { }
            
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
            public ChartboostMediationFullscreenAdLoadListener() : base(GetQualifiedNativeClassName(AndroidConstants.ClassChartboostMediationFullscreenAdLoadListener, true)) { }

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

                    var nativeAd = result.Get<AndroidJavaObject>(AndroidConstants.PropertyAd);
                    var ad = new ChartboostMediationFullscreenAdAndroid(nativeAd, CacheManager.GetFullScreenAdLoadRequest(hashCode()));
                    var loadId = result.Get<string>(AndroidConstants.PropertyLoadId);
                    var metrics = result.Get<AndroidJavaObject>(AndroidConstants.PropertyMetrics).JsonObjectToMetrics();
                    _complete(new ChartboostMediationFullscreenAdLoadResult(ad, loadId, metrics));
                });
            }
        }
        
        internal class ChartboostMediationFullscreenAdShowListener : AwaitableAndroidJavaProxy<ChartboostMediationAdShowResult>
        {
            public ChartboostMediationFullscreenAdShowListener() : base(GetQualifiedNativeClassName(AndroidConstants.ClassChartboostMediationFullscreenAdShowListener, true)) { } 
            
            private void onAdShown(AndroidJavaObject result)
            {
                EventProcessor.ProcessEvent(() => { 
                    var error = result.ToChartboostMediationError();
                    if (error.HasValue)
                    {
                        _complete(new ChartboostMediationAdShowResult(error.Value));
                        return;
                    }
                    var metrics = result.Get<AndroidJavaObject>(AndroidConstants.PropertyMetrics).JsonObjectToMetrics();
                    _complete(new ChartboostMediationAdShowResult(metrics));
                });
            }
        }

        internal class ChartboostMediationFullscreenAdListener : AndroidJavaProxy
        {
            internal static readonly ChartboostMediationFullscreenAdListener Instance = new ChartboostMediationFullscreenAdListener();

            private ChartboostMediationFullscreenAdListener() : base(GetQualifiedNativeClassName(AndroidConstants.ClassChartboostMediationFullscreenAdListener, true)) { }

            private void onAdClicked(AndroidJavaObject ad) 
                => EventProcessor.ProcessFullscreenEvent(ad.HashCode(), (int)EventProcessor.FullscreenAdEvents.Click, null, null);

            private void onAdClosed(AndroidJavaObject ad, AndroidJavaObject error)
            {
                string code = null;
                string message = null;

                var mediationError = error?.Get<AndroidJavaObject>(AndroidConstants.PropertyChartboostMediationError);
                if (mediationError != null)
                {
                    code = error.Get<string>(AndroidConstants.PropertyCode);
                    message = error.Call<string>(AndroidConstants.FunToString);
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

        internal class ChartboostMediationBannerViewListener : AndroidJavaProxy
        {
            public ChartboostMediationBannerViewListener() : base(GetQualifiedClassName(AndroidConstants.ClassChartboostMediationBannerViewListener)) {}
            
            private void onAdCached(AndroidJavaObject ad, string error)
            {
                var bannerView = CacheManager.GetBannerAd(ad.HashCode());
                if (!(bannerView is ChartboostMediationBannerViewAndroid androidBannerView)) 
                    return;

                if (androidBannerView.LoadRequest == null)
                {
                    EventProcessor.ReportUnexpectedSystemError("Load result received for a null request");
                    return;
                }

                var loadResult = !string.IsNullOrEmpty(error) 
                    ? new ChartboostMediationBannerAdLoadResult(new ChartboostMediationError(error)) 
                    : new ChartboostMediationBannerAdLoadResult(bannerView.LoadId, null, null);

                androidBannerView.LoadRequest.Complete(loadResult);
            }

            private void onAdViewAdded(AndroidJavaObject ad) =>
                EventProcessor.ProcessChartboostMediationBannerEvent(ad.HashCode(), (int)EventProcessor.BannerAdEvents.Load);
            
            private void onAdClicked(AndroidJavaObject ad) =>
                EventProcessor.ProcessChartboostMediationBannerEvent(ad.HashCode(), (int)EventProcessor.BannerAdEvents.Click);

            private void onAdImpressionRecorded(AndroidJavaObject ad) =>
                EventProcessor.ProcessChartboostMediationBannerEvent(ad.HashCode(), (int)EventProcessor.BannerAdEvents.RecordImpression);

            private void onAdDrag(AndroidJavaObject ad, float x, float y)
                => EventProcessor.ProcessChartboostMediationBannerEvent(ad.HashCode(), (int)EventProcessor.BannerAdEvents.Drag, x, Screen.height - y);

        }

        #endregion
    }
}
#endif

#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Fullscreen;
using Chartboost.Events;
using Chartboost.Requests;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Chartboost.Platforms.IOS
{
    internal sealed partial class ChartboostMediationIOS
    {
        private delegate void ExternChartboostMediationFullscreenAdLoadResultEvent(int hashCode, IntPtr adHashCode, string loadId, string winningBidJson, string metricsJson, string code, string message);
        internal delegate void ExternChartboostMediationFullscreenAdShowResultEvent(int hashCode, string metricsJson, string code, string message);

        private delegate void ExternChartboostMediationFullscreenAdEvent(long adHashCode, int eventType, string code, string error);
        
        // callback definitions for objective-c layer
        private delegate void ExternChartboostMediationEvent(string error);
        private delegate void ExternChartboostMediationILRDEvent(string impressionDataJson);
        private delegate void ExternChartboostMediationPartnerInitializationDataEvent(string partnerInitializationData);
        private delegate void ExternChartboostMediationPlacementEvent(string placementName, string error);
        private delegate void ExternChartboostMediationPlacementLoadEvent(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error);
        
        [DllImport(IOSConstants.Internal)]
        private static extern void _setLifeCycleCallbacks(ExternChartboostMediationEvent DidStartCallback,
            ExternChartboostMediationILRDEvent DidReceiveILRDCallback, ExternChartboostMediationPartnerInitializationDataEvent DidReceivePartnerInitializationDataCallback);

        [DllImport(IOSConstants.Internal)]
        private static extern void _setFullscreenCallbacks(ExternChartboostMediationFullscreenAdEvent fullscreenAdEvents);

        [DllImport(IOSConstants.Internal)]
        private static extern void _setBannerAdCallbacks(ExternChartboostMediationBannerAdEvent bannerAdEvents);
        
        [DllImport(IOSConstants.Internal)]
        private static extern void _setFullscreenAdQueueCallbacks(ExternChartboostMediationFullscreenAdQueueUpdateEvent updateEvent, ExternChartboostMediationFullscreenAdQueueRemoveExpiredAdEvent removeExpiredAdEvent);

        #region LifeCycle Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationEvent))]
        private static void ExternDidStart(string error) 
            => EventProcessor.ProcessChartboostMediationEvent(error, _instance.DidStart);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationILRDEvent))]
        private static void ExternDidReceiveILRD(string impressionDataJson) 
            => EventProcessor.ProcessEventWithILRD(impressionDataJson, _instance.DidReceiveImpressionLevelRevenueData);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPartnerInitializationDataEvent))]
        private static void ExternDidReceivePartnerInitializationData(string partnerInitializationData) 
            => EventProcessor.ProcessEventWithPartnerInitializationData(partnerInitializationData, _instance.DidReceivePartnerInitializationData);

        public override event ChartboostMediationEvent DidStart;
        public override event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData;
        public override event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData;
        #endregion

        #region Fullscreen Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationFullscreenAdLoadResultEvent))]
        private static void FullscreenAdLoadResultCallbackProxy(int hashCode, IntPtr adHashCode, string loadId, string winningBidJson, string metricsJson, string code, string message)
        {
            EventProcessor.ProcessEvent(() => { 
                ChartboostMediationFullscreenAdLoadResult adLoadResult;
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(message))
                {
                    var error = new ChartboostMediationError(code, message);
                    adLoadResult = new ChartboostMediationFullscreenAdLoadResult(error);
                    CacheManager.ReleaseFullscreenAdLoadRequest(hashCode);
                    AwaitableProxies.ResolveCallbackProxy(hashCode, adLoadResult);
                    return;
                }

                var winningBid = string.IsNullOrEmpty(winningBidJson) ? new BidInfo() : JsonConvert.DeserializeObject<BidInfo>(winningBidJson);
                var metrics = string.IsNullOrEmpty(metricsJson)? new Metrics() : JsonConvert.DeserializeObject<Metrics>(metricsJson);
                var iosAd = new ChartboostMediationFullscreenAdIOS(adHashCode, CacheManager.GetFullScreenAdLoadRequest(hashCode));
                adLoadResult = new ChartboostMediationFullscreenAdLoadResult(iosAd, loadId, metrics);
                AwaitableProxies.ResolveCallbackProxy(hashCode, adLoadResult);
            });
        }

        [MonoPInvokeCallback(typeof(ExternChartboostMediationFullscreenAdShowResultEvent))]
        internal static void FullscreenAdShowResultCallbackProxy(int hashCode, string metricsJson, string code, string message)
        {
            EventProcessor.ProcessEvent(() =>
            {
                ChartboostMediationAdShowResult adShowResult;
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(message))
                {
                    var error = new ChartboostMediationError(code, message);
                    adShowResult = new ChartboostMediationAdShowResult(error);
                    AwaitableProxies.ResolveCallbackProxy(hashCode, adShowResult);
                    return;
                }
                
                var metrics = JsonConvert.DeserializeObject<Metrics>(metricsJson);
                adShowResult = new ChartboostMediationAdShowResult(metrics);
                AwaitableProxies.ResolveCallbackProxy(hashCode, adShowResult);
            });
        }

        [MonoPInvokeCallback(typeof(ExternChartboostMediationFullscreenAdEvent))]
        private static void FullscreenAdEvents(long adHashCode, int eventType, string code, string message) 
            => EventProcessor.ProcessFullscreenEvent(adHashCode, eventType, code, message);
        #endregion

        #region Banner Callbacks
        public delegate void ExternChartboostMediationBannerAdLoadResultEvent(int hashCode, IntPtr adHashCode, string loadId, string metricsJson, float sizeWidth, float sizeHeight, string code, string message);
        
        [MonoPInvokeCallback(typeof(ExternChartboostMediationBannerAdLoadResultEvent))]
        internal static void BannerAdLoadResultCallbackProxy(int hashCode, IntPtr adHashCode, string loadId, string metricsJson, float sizeWidth, float sizeHeight, string code, string message)
        {
            EventProcessor.ProcessEvent(() => { 
                ChartboostMediationBannerAdLoadResult adLoadResult;
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(message))
                {
                    var error = new ChartboostMediationError(code, message);
                    adLoadResult = new ChartboostMediationBannerAdLoadResult(error);
                    AwaitableProxies.ResolveCallbackProxy(hashCode, adLoadResult);
                    CacheManager.ReleaseBannerAdLoadRequest(hashCode);
                    return;
                }
                var metrics = string.IsNullOrEmpty(metricsJson)? new Metrics() : JsonConvert.DeserializeObject<Metrics>(metricsJson);
                var size = ChartboostMediationBannerSize.Adaptive(sizeWidth, sizeHeight);
                adLoadResult = new ChartboostMediationBannerAdLoadResult(loadId, metrics, null, size);
                AwaitableProxies.ResolveCallbackProxy(hashCode, adLoadResult);
                CacheManager.ReleaseBannerAdLoadRequest(hashCode);
            });
        }

        private delegate void ExternChartboostMediationBannerAdEvent(long adHashCode, int eventType);

        private delegate void ExternChartboostMediationBannerAdDragEvent(long adHasCode, float x, float y);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationBannerAdEvent))]
        internal static void BannerAdEvents(long adHashCode, int eventType)
        {
            EventProcessor.ProcessChartboostMediationBannerEvent(adHashCode, eventType);
        }

        [MonoPInvokeCallback(typeof(ExternChartboostMediationBannerAdDragEvent))]
        internal static void BannerAdDragEvent(long adHashCode, float x, float y)
        {
            EventProcessor.ProcessChartboostMediationBannerEvent(adHashCode,(int)EventProcessor.BannerAdEvents.Drag, x, Screen.height - y);
        }
        
        #endregion

        #region Fullscreen Ad Queue
        private delegate void ExternChartboostMediationFullscreenAdQueueUpdateEvent(long hashCode, string loadId, string metricsJson, string code, string message, int numberOfAdsReady);
        private delegate void ExternChartboostMediationFullscreenAdQueueRemoveExpiredAdEvent(long hashCode, int numberOfAdsReady);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationFullscreenAdQueueUpdateEvent))]
        internal static void FullscreenAdQueueUpdateEvent(long hashCode, string loadId, string metricsJson, string code, string message,
            int numberOfAdsReady)
        {
            ChartboostMediationAdLoadResult loadResult;

                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(message))
                {
                    var error = new ChartboostMediationError(code, message);
                    loadResult = new ChartboostMediationAdLoadResult(error);
                }
                else
                {
                    var metrics = string.IsNullOrEmpty(metricsJson) ? new Metrics() : JsonConvert.DeserializeObject<Metrics>(metricsJson);
                    loadResult = new ChartboostMediationAdLoadResult(loadId, metrics, null);
                }

                EventProcessor.ProcessFullscreenAdQueueEvent(hashCode, (int)EventProcessor.FullscreenAdQueueEvents.Update, loadResult,
                    numberOfAdsReady);
        }

        [MonoPInvokeCallback(typeof(ExternChartboostMediationFullscreenAdQueueRemoveExpiredAdEvent))]
        internal static void FullscreenAdQueueRemoveExpiredAdEvent(long hashCode, int numberOfAdsReady)
        {
            EventProcessor.ProcessFullscreenAdQueueEvent(hashCode, (int)EventProcessor.FullscreenAdQueueEvents.RemoveExpiredAd, null,
                numberOfAdsReady);
        }
        #endregion

    }
}
#endif

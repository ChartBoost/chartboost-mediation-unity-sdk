#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Chartboost.AdFormats.Fullscreen;
using Chartboost.Placements;
using Chartboost.Requests;
using Chartboost.Utilities;
using Newtonsoft.Json;
// ReSharper disable InconsistentNaming

namespace Chartboost.Platforms.IOS
{
    public sealed partial class ChartboostMediationIOS
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
        
        private static readonly Dictionary<int, ILater> WaitingProxies = new Dictionary<int, ILater>();
        
        internal static (Later<TResult> proxy, int hashCode) _setupProxy<TResult>() {
            var proxy    = new Later<TResult>();
            var hashCode = proxy.GetHashCode();
            WaitingProxies[hashCode] = proxy;
            return (proxy, hashCode);
        }
        private static void ResolveCallbackProxy<TResponse>(int hashCode, TResponse response) {
            if (!WaitingProxies.ContainsKey(hashCode))
                return;
            
            if (WaitingProxies[hashCode] is Later<TResponse> later)
                later.Complete(response);
            
            WaitingProxies.Remove(hashCode);
        }
        
        [DllImport("__Internal")]
        private static extern void _setLifeCycleCallbacks(ExternChartboostMediationEvent DidStartCallback,
            ExternChartboostMediationILRDEvent DidReceiveILRDCallback, ExternChartboostMediationPartnerInitializationDataEvent DidReceivePartnerInitializationDataCallback);

        [DllImport("__Internal")]
        private static extern void _setFullscreenCallbacks(ExternChartboostMediationFullscreenAdEvent fullscreenAdEvents);

        [DllImport("__Internal")]
        private static extern void _setBannerCallbacks(ExternChartboostMediationPlacementLoadEvent DidLoadCallback, ExternChartboostMediationPlacementEvent DidRecordImpression, ExternChartboostMediationPlacementEvent DidClickCallback);

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
                    ResolveCallbackProxy(hashCode, adLoadResult);
                    return;
                }

                var winningBid = string.IsNullOrEmpty(winningBidJson) ? new BidInfo() : JsonConvert.DeserializeObject<BidInfo>(winningBidJson);
                var metrics = string.IsNullOrEmpty(metricsJson)? new Metrics() : JsonConvert.DeserializeObject<Metrics>(metricsJson);
                var iosAd = new ChartboostMediationFullscreenAdIOS(adHashCode, loadId, CacheManager.GetFullScreenAdLoadRequest(hashCode), winningBid);
                adLoadResult = new ChartboostMediationFullscreenAdLoadResult(iosAd, loadId, metrics);
                ResolveCallbackProxy(hashCode, adLoadResult);
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
                    ResolveCallbackProxy(hashCode, adShowResult);
                    return;
                }
                
                var metrics = JsonConvert.DeserializeObject<Metrics>(metricsJson);
                adShowResult = new ChartboostMediationAdShowResult(metrics);
                ResolveCallbackProxy(hashCode, adShowResult);
            });
        }

        [MonoPInvokeCallback(typeof(ExternChartboostMediationFullscreenAdEvent))]
        private static void FullscreenAdEvents(long adHashCode, int eventType, string code, string message) 
            => EventProcessor.ProcessFullscreenEvent(adHashCode, eventType, code, message);
        #endregion
        
        #region Banner Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementLoadEvent))]
        private static void ExternDidLoadBanner(string placementName, string loadId, string auctionId, string partnerId, double price, string lineItemName, string lineItemId, string error) 
            => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, lineItemName, lineItemId, error, _instance.DidLoadBanner);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidClickBanner(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidClickBanner);
        
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidRecordImpressionBanner(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error,  _instance.DidRecordImpressionBanner);

        public override event ChartboostMediationPlacementLoadEvent DidLoadBanner;
        public override event ChartboostMediationPlacementEvent DidClickBanner;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionBanner;
        #endregion
    }
}
#endif

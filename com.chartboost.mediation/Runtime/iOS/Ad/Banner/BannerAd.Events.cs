using System;
using AOT;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Mediation.Utilities.Events;

namespace Chartboost.Mediation.iOS.Ad.Banner
{
    internal partial class BannerAd
    {
        [MonoPInvokeCallback(typeof(ExternBannerAdLoadResultEvent))]
        internal static void BannerAdLoadResultCallbackProxy(int hashCode, IntPtr adHashCode, string loadId, string metricsJson, string winningBidJson, float sizeWidth, float sizeHeight, string code, string message)
        {
            MainThreadDispatcher.Post(_ => { 
                BannerAdLoadResult loadResult;
                
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(message))
                {
                    var error = new ChartboostMediationError(code, message);
                    loadResult = new BannerAdLoadResult(error);
                    AwaitableProxies.ResolveCallbackProxy(hashCode, loadResult);
                    AdCache.ReleaseAdLoadRequest(hashCode);
                    return;
                }
                
                var size = Chartboost.Mediation.Ad.Banner.BannerSize.Adaptive(sizeWidth, sizeHeight);
                loadResult = new BannerAdLoadResult(loadId, metricsJson.ToMetrics(), winningBidJson.ToBidInfo(), null, size);
                AwaitableProxies.ResolveCallbackProxy(hashCode, loadResult);
                AdCache.ReleaseAdLoadRequest(hashCode);
            });
        }
        
        [MonoPInvokeCallback(typeof(ExternBannerAdEvent))]
        internal static void BannerAdEvent(long adHashCode, int eventType) 
            => AdEventHandler.ProcessBannerEvent(adHashCode, (BannerAdEvents)eventType);

        [MonoPInvokeCallback(typeof(ExternBannerAdDragEvent))]
        internal static void BannerAdDragEvent(long adHashCode, float x, float y) 
            => AdEventHandler.ProcessBannerEvent(adHashCode,BannerAdEvents.Drag, x, y);
    }
}

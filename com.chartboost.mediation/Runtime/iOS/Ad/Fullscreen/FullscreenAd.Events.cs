using System;
using AOT;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Mediation.Utilities.Events;

namespace Chartboost.Mediation.iOS.Ad.Fullscreen
{
    internal partial class FullscreenAd 
    {
        [MonoPInvokeCallback(typeof(ExternFullscreenAdLoadResultEvent))]
        internal static void FullscreenAdLoadResultCallbackProxy(int hashCode, IntPtr adHashCode, string loadId, string metricsJson, string bidInfoJson, string code, string message)
        {
            MainThreadDispatcher.Post(o =>
            {
                FullscreenAdLoadResult adLoadResult;
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(message))
                {
                    var error = new ChartboostMediationError(code, message);
                    adLoadResult = new FullscreenAdLoadResult(error);
                    AdCache.ReleaseAdLoadRequest(hashCode);
                    AwaitableProxies.ResolveCallbackProxy(hashCode, adLoadResult);
                    return;
                }

                var iosAd = new FullscreenAd(adHashCode, (FullscreenAdLoadRequest)AdCache.GetAdLoadRequest(hashCode));
                adLoadResult = new FullscreenAdLoadResult(iosAd, loadId, metricsJson.ToMetrics(), bidInfoJson.ToBidInfo());
                AwaitableProxies.ResolveCallbackProxy(hashCode, adLoadResult);
            });
        }

        [MonoPInvokeCallback(typeof(ExternFullscreenAdShowResultEvent))]
        internal static void FullscreenAdShowResultCallbackProxy(int hashCode, string metricsJson, string code, string message)
        {
            MainThreadDispatcher.Post(o =>
            {
                AdShowResult adShowResult;
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(message))
                {
                    var error = new ChartboostMediationError(code, message);
                    adShowResult = new AdShowResult(error);
                    AwaitableProxies.ResolveCallbackProxy(hashCode, adShowResult);
                    return;
                }
                
                adShowResult = new AdShowResult(metricsJson.ToMetrics());
                AwaitableProxies.ResolveCallbackProxy(hashCode, adShowResult);
            });
        }
        
        [MonoPInvokeCallback(typeof(ExternFullscreenAdEvent))]
        internal static void FullscreenAdEvent(long adHashCode, int eventType, string code, string message) 
            => AdEventHandler.ProcessFullscreenEvent(adHashCode, (FullscreenAdEvents)eventType, code, message);
    }
}

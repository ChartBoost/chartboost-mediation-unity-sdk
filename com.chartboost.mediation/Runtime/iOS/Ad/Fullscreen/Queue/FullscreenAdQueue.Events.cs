using AOT;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Mediation.Utilities.Events;

namespace Chartboost.Mediation.iOS.Ad.Fullscreen.Queue
{
    internal partial class FullscreenAdQueue
    {
        [MonoPInvokeCallback(typeof(ExternFullscreenAdQueueUpdateEvent))]
        internal static void FullscreenAdQueueUpdateEvent(long hashCode, string loadId, string metricsJson, string winningBidInfoJson, string code, string message, int numberOfAdsReady)
        {
            IAdLoadResult loadResult;

            if (NativeResults.ToError(code, message) is { } error)
                loadResult = new FullscreenAdLoadResult(error);
            else
                loadResult = new FullscreenAdLoadResult(null!, loadId, metricsJson.ToMetrics(), null);

            AdEventHandler.ProcessFullscreenAdQueueEvent(hashCode, FullscreenAdQueueEvents.Update, loadResult, numberOfAdsReady);
        }
        
        [MonoPInvokeCallback(typeof(ExternFullscreenAdQueueRemoveExpiredAdEvent))]
        internal static void FullscreenAdQueueRemoveExpiredAdEvent(long hashCode, int numberOfAdsReady) 
            => AdEventHandler.ProcessFullscreenAdQueueEvent(hashCode, FullscreenAdQueueEvents.RemoveExpiredAd, null, numberOfAdsReady);
    }
}

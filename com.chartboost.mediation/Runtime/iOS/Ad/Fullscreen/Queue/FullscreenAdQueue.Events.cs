using AOT;
using Chartboost.Mediation.Error;
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

            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(message))
            {
                var error = new ChartboostMediationError(code, message);
                loadResult = new FullscreenAdLoadResult(error);
            }
            else
                loadResult = new FullscreenAdLoadResult(null!, loadId, metricsJson.ToMetrics(), null);

            AdEventHandler.ProcessFullscreenAdQueueEvent(hashCode, FullscreenAdQueueEvents.Update, loadResult, numberOfAdsReady);
        }
        
        [MonoPInvokeCallback(typeof(ExternFullscreenAdQueueRemoveExpiredAdEvent))]
        internal static void FullscreenAdQueueRemoveExpiredAdEvent(long hashCode, int numberOfAdsReady) 
            => AdEventHandler.ProcessFullscreenAdQueueEvent(hashCode, FullscreenAdQueueEvents.RemoveExpiredAd, null, numberOfAdsReady);
    }
}

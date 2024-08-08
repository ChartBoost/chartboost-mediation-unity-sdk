using Chartboost.Mediation.Android.Utilities;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities.Events;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chartboost.Mediation.Android.Ad.Fullscreen.Queue
{
    internal partial class FullscreenAdQueue
    {
        internal class FullscreenAdQueueListener : AndroidJavaProxy
        {
            internal FullscreenAdQueueListener() : base(AndroidConstants.ClassFullscreenAdQueueListener) {}

            [Preserve]
            private void onFullScreenAdQueueUpdated(AndroidJavaObject adQueue, AndroidJavaObject adLoadResult, int numberOfAdsReady)
            {
                var error = adLoadResult.ToChartboostMediationError();
                var loadId = adLoadResult.Call<string>(AndroidConstants.FunctionGetLoadId);
                var metrics = adLoadResult.Call<AndroidJavaObject>(AndroidConstants.FunctionGetMetrics).JsonObjectToMetrics();
                var winningBidInfo = adLoadResult.Call<AndroidJavaObject>(AndroidConstants.FunctionGetWinningBidInfo).MapToWinningBidInfo();
                var loadResult = new FullscreenAdLoadResult(null!, loadId, metrics, winningBidInfo, error);
                
                AdEventHandler.ProcessFullscreenAdQueueEvent(adQueue.NativeHashCode(), FullscreenAdQueueEvents.Update, loadResult, numberOfAdsReady);
            }
            
            [Preserve]
            private void onFullscreenAdQueueExpiredAdRemoved(AndroidJavaObject adQueue, int numberOfAdsReady) 
                => AdEventHandler.ProcessFullscreenAdQueueEvent(adQueue.NativeHashCode(), FullscreenAdQueueEvents.RemoveExpiredAd, null!, numberOfAdsReady);
        }
    }
}

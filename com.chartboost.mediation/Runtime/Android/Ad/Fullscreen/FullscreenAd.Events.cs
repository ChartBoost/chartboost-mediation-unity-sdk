using Chartboost.Constants;
using Chartboost.Mediation.Android.Utilities;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Mediation.Utilities.Events;
using UnityEngine;
using UnityEngine.Scripting;
// ReSharper disable InconsistentNaming

namespace Chartboost.Mediation.Android.Ad.Fullscreen
{
    internal partial class FullscreenAd
    {
        internal class FullscreenAdLoadListener : AwaitableAndroidJavaProxy<FullscreenAdLoadResult>
        {
            public FullscreenAdLoadListener() : base(AndroidConstants.ClassFullscreenAdLoadListener) { }

            [Preserve]
            private void onAdLoaded(AndroidJavaObject result)
            {
                MainThreadDispatcher.Post(_ =>
                {
                    var error = result.ToChartboostMediationError();
                    if (error.HasValue)
                    {
                        AdCache.ReleaseAdLoadRequest(hashCode());
                        _complete(new FullscreenAdLoadResult(error.Value));
                        return;
                    }

                    var nativeAd = result.Call<AndroidJavaObject>(AndroidConstants.FunctionGetAd);
                    var unityAdReference = new FullscreenAd(nativeAd, (FullscreenAdLoadRequest)AdCache.GetAdLoadRequest(hashCode()));
                    var loadId = result.Call<string>(AndroidConstants.FunctionGetLoadId);
                    var metrics = result.Call<AndroidJavaObject>(AndroidConstants.FunctionGetMetrics).JsonObjectToMetrics();
                    var winningBidInfo = result.Call<AndroidJavaObject>(AndroidConstants.FunctionGetWinningBidInfo).MapToWinningBidInfo();
                    _complete(new FullscreenAdLoadResult(unityAdReference, loadId, metrics, winningBidInfo));
                });
            }
        }
        
        internal class FullscreenAdShowListener : AwaitableAndroidJavaProxy<AdShowResult>
        {
            public FullscreenAdShowListener() : base(AndroidConstants.ClassFullscreenAdShowListener) { } 
            
            [Preserve]
            private void onAdShown(AndroidJavaObject result)
            {
                MainThreadDispatcher.Post(_ => { 
                    var error = result.ToChartboostMediationError();
                    if (error.HasValue)
                    {
                        _complete(new AdShowResult(error.Value));
                        return;
                    }
                    var metrics = result.Call<AndroidJavaObject>(AndroidConstants.FunctionGetMetrics).JsonObjectToMetrics();
                    _complete(new AdShowResult(metrics));
                });
            }
        }
        
        internal class FullscreenAdListener : AndroidJavaProxy
        {
            internal FullscreenAdListener() : base(AndroidConstants.ClassFullscreenAdListener) { }

            [Preserve]
            private void onAdClicked(AndroidJavaObject ad) 
                => AdEventHandler.ProcessFullscreenEvent(ad.NativeHashCode(), FullscreenAdEvents.Click, null, null);

            [Preserve]
            private void onAdClosed(AndroidJavaObject ad, AndroidJavaObject error)
            {
                string code = null;
                string message = null;

                var mediationError = error?.Call<AndroidJavaObject>(AndroidConstants.FunctionGetChartboostMediationError);
                if (mediationError != null)
                {
                    code = error.Call<string>(AndroidConstants.FunctionGetCode);
                    message = error.Call<string>(SharedAndroidConstants.FunctionToString);
                }
                
                AdEventHandler.ProcessFullscreenEvent(ad.NativeHashCode(), FullscreenAdEvents.Close, code, message);
            }
            
            [Preserve]
            private void onAdExpired(AndroidJavaObject ad)
                => AdEventHandler.ProcessFullscreenEvent(ad.NativeHashCode(), FullscreenAdEvents.Expire, null, null);

            [Preserve]
            private void onAdImpressionRecorded(AndroidJavaObject ad)
                => AdEventHandler.ProcessFullscreenEvent(ad.NativeHashCode(), FullscreenAdEvents.RecordImpression, null, null);

            [Preserve]
            private void onAdRewarded(AndroidJavaObject ad) 
                => AdEventHandler.ProcessFullscreenEvent(ad.NativeHashCode(), FullscreenAdEvents.Reward, null, null);
        }
    }
}

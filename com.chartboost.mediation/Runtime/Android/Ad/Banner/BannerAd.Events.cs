using Chartboost.Mediation.Android.Utilities;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Mediation.Utilities.Events;
using UnityEngine;
using UnityEngine.Scripting;

namespace Chartboost.Mediation.Android.Ad.Banner
{
    internal partial class BannerAd
    {
        internal class BannerAdLoadListener : AwaitableAndroidJavaProxy<BannerAdLoadResult>
        {
            public BannerAdLoadListener() : base(AndroidConstants.ClassBannerAdLoadListener) { } 
            
            [Preserve]
            private void onAdLoaded(AndroidJavaObject result)
            {
                MainThreadDispatcher.Post(_ => { 
                    var error = result.ToChartboostMediationError();
                    if (error.HasValue)
                    {
                        AdCache.ReleaseAdLoadRequest(hashCode());
                        _complete(new BannerAdLoadResult(error.Value));
                        return;
                    }
                    var loadId = result.Call<string>(AndroidConstants.FunctionGetLoadId);
                    var metrics = result.Call<AndroidJavaObject>(AndroidConstants.FunctionGetMetrics).JsonObjectToMetrics();
                    var winningBidInfo = result.Call<AndroidJavaObject>(AndroidConstants.FunctionGetWinningBidInfo).MapToWinningBidInfo();

                    // Size received from native is android `size` and not `BannerSize` so we need to convert this to `BannerSize`
                    var size = result.Call<AndroidJavaObject>(AndroidConstants.FunctionGetBannerSize);
                    var width = size.Call<int>(AndroidConstants.FunctionGetWidth);
                    var height = size.Call<int>(AndroidConstants.FunctionGetHeight);
                    var bannerSize = Mediation.Ad.Banner.BannerSize.Adaptive(width, height);
                    _complete(new BannerAdLoadResult(loadId, metrics, winningBidInfo, null, bannerSize));
                });
            }
        }
        
        internal class BannerAdListener : AndroidJavaProxy
        {
            internal BannerAdListener() : base(AndroidConstants.ClassBannerAdListener) { }

            [Preserve]
            private void onAdViewAdded(AndroidJavaObject ad)
            {
                AdEventHandler.ProcessBannerEvent(ad.NativeHashCode(), BannerAdEvents.Load);
            }

            [Preserve]
            private void onAdClicked(AndroidJavaObject ad) =>
                AdEventHandler.ProcessBannerEvent(ad.NativeHashCode(), BannerAdEvents.Click);

            [Preserve]
            private void onAdImpressionRecorded(AndroidJavaObject ad) =>
                AdEventHandler.ProcessBannerEvent(ad.NativeHashCode(), BannerAdEvents.RecordImpression);

            [Preserve]
            private void onAdDragBegin(AndroidJavaObject ad, float x, float y) =>
                AdEventHandler.ProcessBannerEvent(ad.NativeHashCode(), BannerAdEvents.BeginDrag, x, y);
            
            [Preserve]
            private void onAdDrag(AndroidJavaObject ad, float x, float y) =>
                AdEventHandler.ProcessBannerEvent(ad.NativeHashCode(), BannerAdEvents.Drag, x, y);
            
            [Preserve]
            private void onAdDragEnd(AndroidJavaObject ad, float x, float y) =>
                AdEventHandler.ProcessBannerEvent(ad.NativeHashCode(), BannerAdEvents.EndDrag, x, y);
        }
    }
}

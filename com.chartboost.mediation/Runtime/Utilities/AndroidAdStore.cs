using Chartboost.Events;
using UnityEngine;

namespace Chartboost.Utilities
{
    /// <summary>
    /// C# equivalent of AdStore. Allows to release unmanaged references to ads from any thread.
    /// </summary>
    public static class AndroidAdStore 
    {
        private const string QualifiedAdStoreName = "com.chartboost.mediation.unity.AdStore";
        private const string FunReleaseLegacyAd = "releaseLegacyAd";
        private const string FunTrackBannerAd = "trackBannerAd";
        private const string FunReleaseFullscreenAd = "releaseFullscreenAd";
        private const string FunReleaseBannerAd = "releaseBannerAd";
        private const string FunAdStoreInfo = "storeInfo";

        public static void ReleaseLegacyAd(int uniqueId)
        {
            EventProcessor.ProcessEvent(() =>
            {
                using var adStore = new AndroidJavaClass(QualifiedAdStoreName);
                adStore.CallStatic(FunReleaseLegacyAd, uniqueId);
            });
        }

        public static void TrackBannerAd(AndroidJavaObject bannerAd)
        {
            EventProcessor.ProcessEvent(() =>
            {
                using var adStore = new AndroidJavaClass(QualifiedAdStoreName);
                adStore.CallStatic(FunTrackBannerAd, bannerAd);
            });
        }
        
        public static void ReleaseFullscreenAd(int uniqueId)
        {
            EventProcessor.ProcessEvent(() => { 
                using var adStore = new AndroidJavaClass(QualifiedAdStoreName);
                adStore.CallStatic(FunReleaseFullscreenAd, uniqueId);
            });
        }
        
        public static void ReleaseBannerAd(int uniqueId)
        {
            EventProcessor.ProcessEvent(() => { 
                using var adStore = new AndroidJavaClass(QualifiedAdStoreName);
                adStore.CallStatic(FunReleaseBannerAd, uniqueId);
            });
        }

        public static string AdStoreInfo()
        {
            using var adStore = new AndroidJavaClass(QualifiedAdStoreName);
            return adStore.CallStatic<string>(FunAdStoreInfo);
        }
    }
}

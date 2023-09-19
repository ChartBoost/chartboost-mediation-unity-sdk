using System;
using Chartboost.Events;
using UnityEngine;

namespace Chartboost.Utilities
{
    public static class AndroidAdStore 
    {
        private const string QualifiedAdStoreName = "com.chartboost.mediation.unity.AdStore";
        
        public static void ReleaseLegacyAd(int uniqueId)
        {
            EventProcessor.ProcessEvent(() =>
            {
                using var adStore = new AndroidJavaClass(QualifiedAdStoreName);
                adStore.CallStatic("releaseLegacyAd", uniqueId);
            });
        }

        public static void TrackBannerAd(AndroidJavaObject bannerAd)
        {
            EventProcessor.ProcessEvent(() =>
            {
                using var adStore = new AndroidJavaClass(QualifiedAdStoreName);
                adStore.CallStatic("trackBannerAd", bannerAd);
            });
        }
        
        public static void ReleaseFullscreenAd(int uniqueId)
        {
            EventProcessor.ProcessEvent(() => { 
                using var adStore = new AndroidJavaClass(QualifiedAdStoreName);
                adStore.CallStatic("releaseFullscreenAd", uniqueId);
            });
        }
        
        public static void ReleaseBannerAd(int uniqueId)
        {
            EventProcessor.ProcessEvent(() => { 
                using var adStore = new AndroidJavaClass(QualifiedAdStoreName);
                adStore.CallStatic("releaseBannerAd", uniqueId);
            });
        }

        public static string AdStoreInfo()
        {
            using var adStore = new AndroidJavaClass(QualifiedAdStoreName);
            return adStore.CallStatic<string>("storeInfo");
        }
    }
}

using Chartboost.Events;
using UnityEngine;

namespace Chartboost.Utilities
{
    /// <summary>
    /// C# equivalent of AdStore. Allows to release unmanaged references to ads from any thread.
    /// </summary>
    internal static class AndroidAdStore
    {
        private static AndroidJavaObject GetAdStore() => new AndroidJavaObject(AndroidConstants.ClassAdStore);
        
        public static void ReleaseLegacyAd(int uniqueId)
        {
            EventProcessor.ProcessEvent(() =>
            {
                using var adStore = GetAdStore();
                adStore.CallStatic(AndroidConstants.FunReleaseLegacyAd, uniqueId);
            });
        }

        public static void ReleaseFullscreenAd(int uniqueId)
        {
            EventProcessor.ProcessEvent(() => { 
                using var adStore = GetAdStore();
                adStore.CallStatic(AndroidConstants.FunReleaseFullscreenAd, uniqueId);
            });
        }
        
        public static void ReleaseBannerAd(int uniqueId)
        {
            EventProcessor.ProcessEvent(() => { 
                using var adStore = GetAdStore();
                adStore.CallStatic(AndroidConstants.FunReleaseBannerAd, uniqueId);
            });
        }

        public static string AdStoreInfo()
        {
            using var adStore = GetAdStore();
            return adStore.CallStatic<string>(AndroidConstants.FunAdStoreInfo);
        }
    }
}

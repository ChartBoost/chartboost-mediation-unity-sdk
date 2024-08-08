using System;
using UnityEngine;

namespace Chartboost.Mediation.Android.Utilities
{
    /// <summary>
    /// C# equivalent of AdStore. Allows to release unmanaged references to ads from any thread.
    /// </summary>
    internal static class AndroidAdStore
    {
        private static AndroidJavaObject GetAdStore() => new(AndroidConstants.ClassAdStore);
        
        public static void ReleaseFullscreenAd(IntPtr uniqueId)
        {
            MainThreadDispatcher.Post(_ => { 
                using var adStore = GetAdStore();
                adStore.CallStatic(AndroidConstants.FunctionReleaseFullscreenAd, uniqueId.ToInt32());
            });
        }
        
        public static void ReleaseBannerAd(IntPtr uniqueId)
        {
            MainThreadDispatcher.Post(_ => { 
                using var adStore = GetAdStore();
                adStore.CallStatic(AndroidConstants.FunctionReleaseBannerAd, uniqueId.ToInt32());
            });
        }

        public static string AdStoreInfo()
        {
            using var adStore = GetAdStore();
            return adStore.CallStatic<string>(AndroidConstants.FunctionAdStoreInfo);
        }
    }
}

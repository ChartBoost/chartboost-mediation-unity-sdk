#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Utilities
{
    public class ToastManager
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass GetToastManager() => new AndroidJavaClass("com.chartboost.mediation.canary.unity.ToastManager");
        #elif UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void _showMessage(string message);
        #endif

        public static void ShowMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogWarning("Message for Toast messages cannot be null or empty");
                return;
            }

            #if UNITY_ANDROID && !UNITY_EDITOR
            using var toastManager = GetToastManager();
            toastManager.CallStatic("showToast", message);
            #elif UNITY_IOS && !UNITY_EDITOR
            _showMessage(message);
            #endif
        }
    }
}

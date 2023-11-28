using System;
using System.Runtime.InteropServices;
using Chartboost.Events;
using AOT;

namespace Chartboost.AppTrackingTransparency
{
    public enum AuthorizationStatus
    {
        NotDetermined = 0,
        Restricted = 1,
        Denied = 2,
        Authorized = 3,
    }
    
    public sealed class AppTrackingTransparency 
    {
        public static Action<AuthorizationStatus, string> OnAuthResponse; 

        public static AuthorizationStatus CurrentAuthorizationStatus
        {
            get
            {
                #if UNITY_IOS && !UNITY_EDITOR
                return (AuthorizationStatus)_getATTStatus();
                #else
                return AuthorizationStatus.Denied;
                #endif
            }
        }

        public static bool IsSupported
        {
            get
            {
                #if UNITY_IOS && !UNITY_EDITOR
                return _ATTSupported();
                #else
                return false;
                #endif
            }
        }

        public static void RequestAuthorization()
        {
            #if UNITY_IOS && !UNITY_EDITOR
            _requestATT(OnATTResponse);
            #else
            OnAuthResponse?.Invoke(AuthorizationStatus.Denied, null);
            #endif
        }
    
        #if UNITY_IOS
        private delegate void AuthorizationResponse(long code);
    
        [DllImport("__Internal")]
        private static extern bool _ATTSupported();
    
        [DllImport("__Internal")]
        private static extern long _getATTStatus();
    
        [DllImport("__Internal")]
        private static extern void _requestATT(AuthorizationResponse responseCode);
    
        [MonoPInvokeCallback(typeof(AuthorizationResponse))]
        private static void OnATTResponse(long responseCode)
        {
            EventProcessor.ProcessEvent(() =>
            {
                var status = (AuthorizationStatus)responseCode;
                var idfa = UnityEngine.iOS.Device.advertisingIdentifier;
                OnAuthResponse.Invoke(status, idfa);
            });
        }
        #endif
    }
}

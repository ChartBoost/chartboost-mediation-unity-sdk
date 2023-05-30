#if UNITY_ANDROID
using System.Threading.Tasks;
using Chartboost.Placements;
using Chartboost.Utilities;
using UnityEngine;
// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming

namespace Chartboost.Platforms.Android
{
    public sealed partial class ChartboostMediationAndroid : ChartboostMediationExternal
    {
        #region Chartboost Mediation
        private const string ChartboostMediationSDK = "com.chartboost.mediation";
        private const string NativeChartboostMediationSDK = "com.chartboost.heliumsdk";
        private static string GetQualifiedClassName(string nativeClass)
            => $"{ChartboostMediationSDK}.unity.{nativeClass}";
        private static string GetQualifiedNativeClassName(string nativeClass, bool isAdClass = false)
            => isAdClass? $"{NativeChartboostMediationSDK}.ad.{nativeClass}" : $"{NativeChartboostMediationSDK}.{nativeClass}";

        private static ChartboostMediationAndroid _instance;
        private static AndroidJavaObject _unityBridge;

        public ChartboostMediationAndroid()
        {
            _instance = this;
            LogTag = "ChartboostMediation (Android)";
            UnityBridge.Call("setupEventListeners",
                InterstitialEventListener.Instance,
                RewardedVideoEventListener.Instance,
                BannerEventListener.Instance);
        }

        // Initialize the android bridge
        internal static AndroidJavaObject UnityBridge 
        {
            get
            {
                if (_unityBridge != null)
                    return _unityBridge;
                // find the plugin instance
                using var pluginClass = new AndroidJavaClass(GetQualifiedClassName("UnityBridge"));
                _unityBridge = pluginClass.CallStatic<AndroidJavaObject>("instance");
                return _unityBridge;
            }
        }

        public override void Init()
        {
            base.Init();
            InitWithAppIdAndSignature(ChartboostMediationSettings.AndroidAppId, ChartboostMediationSettings.AndroidAppSignature);
        }

        public override void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            base.InitWithAppIdAndSignature(appId, appSignature);
            ChartboostMediationSettings.AndroidAppId = appId;
            ChartboostMediationSettings.AndroidAppSignature = appSignature;
            UnityBridge.Call("start", appId, appSignature, GetInitializationOptions(), new ChartboostMediationSDKListener());
            IsInitialized = true;
        }

        public override void SetSubjectToCoppa(bool isSubject)
        {
            base.SetSubjectToCoppa(isSubject);
            UnityBridge.CallStatic("setSubjectToCoppa", isSubject);
        }

        public override void SetSubjectToGDPR(bool isSubject)
        {
            base.SetSubjectToGDPR(isSubject);
            UnityBridge.Call("setSubjectToGDPR", isSubject);
        }

        public override void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            base.SetUserHasGivenConsent(hasGivenConsent);
            UnityBridge.Call("setUserHasGivenConsent", hasGivenConsent);
        }

        public override void SetCCPAConsent(bool hasGivenConsent)
        {
            base.SetCCPAConsent(hasGivenConsent);
            UnityBridge.Call("setCCPAConsent", hasGivenConsent);
        }

        public override void SetUserIdentifier(string userIdentifier)
        {
            base.SetUserIdentifier(userIdentifier);
            UnityBridge.Call("setUserIdentifier", userIdentifier);
        }

        public override string GetUserIdentifier()
        {
            base.GetUserIdentifier();
            return UnityBridge.Call<string>("getUserIdentifier");
        }
        
        public override void SetTestMode(bool testModeEnabled)
        {
            base.SetTestMode(testModeEnabled);
            UnityBridge.Call("setTestMode", testModeEnabled);
        }

        public override void Destroy()
        {
            if (!CheckInitialized())
                return;
            base.Destroy();
            UnityBridge.Call("unsubscribeILRDObserver", ILRDObserver.Instance);
            IsInitialized = false;
        }

        public override async Task<ChartboostMediationFullscreenAdLoadResult> GetFullscreenAd(ChartboostMediationFullscreenAdLoadRequest request)
        {
            var adLoadListenerAwaitableProxy = new ChartboostMediationFullscreenAdLoadListener();
            CacheManager.TrackFullscreenAdLoadRequest(adLoadListenerAwaitableProxy.hashCode(), request);
            var nativeAdRequest = new AndroidJavaObject(GetQualifiedNativeClassName("ChartboostMediationAdLoadRequest", true), request.PlacementName, request.Keywords.ToKeywords());
            UnityBridge.Call("getFullscreenAd", nativeAdRequest, adLoadListenerAwaitableProxy, ChartboostMediationFullscreenAdListener.Instance);
            return await adLoadListenerAwaitableProxy;
        }
        #endregion
    }
}
#endif

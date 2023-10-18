#if UNITY_ANDROID
using System;
using System.Linq;
using System.Threading.Tasks;
using Chartboost.AdFormats.Banner;
using Chartboost.Events;
using Chartboost.Requests;
using Chartboost.Results;
using Chartboost.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming

namespace Chartboost.Platforms.Android
{
    internal sealed partial class ChartboostMediationAndroid : ChartboostMediationExternal
    {
        #region Chartboost Mediation
        private const string ChartboostMediationSDK = "com.chartboost.mediation";
        private const string NativeChartboostMediationSDK = "com.chartboost.heliumsdk";
        internal static string GetQualifiedClassName(string nativeClass)
            => $"{ChartboostMediationSDK}.unity.{nativeClass}";
        internal static string GetQualifiedNativeClassName(string nativeClass, bool isAdClass = false)
            => isAdClass? $"{NativeChartboostMediationSDK}.ad.{nativeClass}" : $"{NativeChartboostMediationSDK}.{nativeClass}";

        private static ChartboostMediationAndroid _instance;
        private static AndroidJavaObject _unityBridge;

        public ChartboostMediationAndroid()
        {
            _instance = this;
            LogTag = "ChartboostMediation (Android)";
            using var unityBridge = GetUnityBridge();
            unityBridge.CallStatic("setupEventListeners",
                InterstitialEventListener.Instance,
                RewardedVideoEventListener.Instance,
                BannerEventListener.Instance);
        }

        // Initialize the android bridge
        internal static AndroidJavaObject GetUnityBridge()
            => new AndroidJavaClass(GetQualifiedClassName("UnityBridge"));

        internal static AndroidJavaClass GetNativeSDK() => new AndroidJavaClass(GetQualifiedNativeClassName("HeliumSdk"));

        public override void Init()
        {
            base.Init();
            #pragma warning disable CS0618
            InitWithAppIdAndSignature(ChartboostMediationSettings.AndroidAppId, ChartboostMediationSettings.AndroidAppSignature);
            #pragma warning restore CS0618
        }

        [Obsolete("InitWithAppIdAndSignature has been deprecated, please use StartWithOptions instead")]
        public override void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            base.InitWithAppIdAndSignature(appId, appSignature);
            ChartboostMediationSettings.AndroidAppId = appId;
            ChartboostMediationSettings.AndroidAppSignature = appSignature;
            using var nativeSDK = GetNativeSDK();
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var initializationOptions = GetInitializationOptions().ArrayToInitializationOptions();
            nativeSDK.CallStatic("start", activity, appId, appSignature, initializationOptions,  new ChartboostMediationSDKListener());
            IsInitialized = true;
        }

        public override void StartWithOptions(string appId, string appSignature, string[] initializationOptions = null)
        {
            base.StartWithOptions(appId, appSignature, initializationOptions);
            ChartboostMediationSettings.AndroidAppId = appId;
            ChartboostMediationSettings.AndroidAppSignature = appSignature;
            initializationOptions ??= Array.Empty<string>();
            var nativeOptions = initializationOptions.ArrayToInitializationOptions();
            using var nativeSDK = GetNativeSDK();
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            nativeSDK.CallStatic("start", activity, appId, appSignature, nativeOptions,  new ChartboostMediationSDKListener());
            IsInitialized = true;
        }

        public override void SetSubjectToCoppa(bool isSubject)
        {
            base.SetSubjectToCoppa(isSubject);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic("setSubjectToCoppa", isSubject);
        }

        public override void SetSubjectToGDPR(bool isSubject)
        {
            base.SetSubjectToGDPR(isSubject);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic("setSubjectToGDPR", isSubject);
        }

        public override void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            base.SetUserHasGivenConsent(hasGivenConsent);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic("setUserHasGivenConsent", hasGivenConsent);
        }

        public override void SetCCPAConsent(bool hasGivenConsent)
        {
            base.SetCCPAConsent(hasGivenConsent);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic("setCCPAConsent", hasGivenConsent);
        }

        public override void SetUserIdentifier(string userIdentifier)
        {
            base.SetUserIdentifier(userIdentifier);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic("setUserIdentifier", userIdentifier);
        }

        public override string GetUserIdentifier()
        {
            base.GetUserIdentifier();
            using var nativeSDK = GetNativeSDK();
            return nativeSDK.CallStatic<string>("getUserIdentifier");
        }
        
        public override void SetTestMode(bool testModeEnabled)
        {
            base.SetTestMode(testModeEnabled);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic("setTestMode", testModeEnabled);
        }

        public override void DiscardOversizedAds(bool shouldDiscard)
        {
            base.DiscardOversizedAds(shouldDiscard);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic("setShouldDiscardOversizedAds", shouldDiscard);
        }

        public override ChartboostMediationAdapterInfo[] InitializedAdaptersInfo()
        {
            base.InitializedAdaptersInfo();
            using var unityBridge = GetUnityBridge();
            var json = unityBridge.CallStatic<string>("adapterInfo");
            var adapterInfo = JsonConvert.DeserializeObject<ChartboostMediationAdapterInfo[]>(json);
            return adapterInfo;
        }

        public override void Destroy()
        {
            if (!CheckInitialized())
                return;
            base.Destroy();
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic("unsubscribeILRDObserver", ILRDObserver.Instance);
            IsInitialized = false;
        }

        public override async Task<ChartboostMediationFullscreenAdLoadResult> LoadFullscreenAd(ChartboostMediationFullscreenAdLoadRequest request)
        {
            if (!CanFetchAd(request.PlacementName))
            {
                var error = new ChartboostMediationError("Chartboost Mediation is not ready or placement is invalid.");
                var adLoadResult = new ChartboostMediationFullscreenAdLoadResult(error);
                return await Task.FromResult(adLoadResult);
            }

            var adLoadListenerAwaitableProxy = new ChartboostMediationFullscreenAdLoadListener();
            try
            {
                CacheManager.TrackFullscreenAdLoadRequest(adLoadListenerAwaitableProxy.hashCode(), request);
                using var nativeAdRequest = new AndroidJavaObject(GetQualifiedNativeClassName("ChartboostMediationAdLoadRequest", true), request.PlacementName, request.Keywords.ToKeywords());
                using var bridge = GetUnityBridge();
                bridge.CallStatic("loadFullscreenAd", nativeAdRequest, adLoadListenerAwaitableProxy, ChartboostMediationFullscreenAdListener.Instance);
            }
            catch (NullReferenceException exception)
            {
                EventProcessor.ReportUnexpectedSystemError(exception.ToString());
            }
            return await adLoadListenerAwaitableProxy;
        }

        public override IChartboostMediationBannerView GetBannerView()
        {
            using var unityBridge = GetUnityBridge();
            var bannerAd = unityBridge.CallStatic<AndroidJavaObject>("loadBannerAd", new ChartboostMediationBannerViewListener());
            return new ChartboostMediationBannerViewAndroid(bannerAd);
        }

        public static float GetUIScaleFactor()
        {
            using var unityBridge = GetUnityBridge();
            return unityBridge.CallStatic<float>("getUIScaleFactor");
        }
        
        #endregion
    }
}
#endif

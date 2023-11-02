#if UNITY_ANDROID
using System;
using System.Threading.Tasks;
using Chartboost.AdFormats.Banner;
using Chartboost.Events;
using Chartboost.Requests;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEngine;
// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming

namespace Chartboost.Platforms.Android
{
    internal sealed partial class ChartboostMediationAndroid : ChartboostMediationExternal
    {
        #region Chartboost Mediation
        private const string ErrorNotReady = "Chartboost Mediation is not ready or placement is invalid.";

        private static string GetQualifiedClassName(string nativeClass)
            => $"{AndroidConstants.NamespaceChartboostMediationBridge}.unity.{nativeClass}";

        private static string GetQualifiedNativeClassName(string nativeClass, bool isAdClass = false)
            => isAdClass? $"{AndroidConstants.NamespaceNativeChartboostMediationSDK}.ad.{nativeClass}" : $"{AndroidConstants.NamespaceNativeChartboostMediationSDK}.{nativeClass}";

        private static ChartboostMediationAndroid _instance;
        private static AndroidJavaObject _unityBridge;

        public ChartboostMediationAndroid()
        {
            _instance = this;
            LogTag = "ChartboostMediation (Android)";
            using var unityBridge = GetUnityBridge();
            unityBridge.CallStatic(AndroidConstants.FunSetupEventListeners,
                InterstitialEventListener.Instance,
                RewardedVideoEventListener.Instance,
                BannerEventListener.Instance);
        }

        // Initialize the android bridge
        internal static AndroidJavaObject GetUnityBridge()
            => new AndroidJavaClass(GetQualifiedClassName(AndroidConstants.ClassUnityBridge));

        private static AndroidJavaClass GetNativeSDK() => new AndroidJavaClass(GetQualifiedNativeClassName(AndroidConstants.ClassHeliumSdk));

        [Obsolete("Init has been deprecated and will be removed in future versions of the SDK.")]
        public override void Init()
        {
            base.Init();
            InitWithAppIdAndSignature(ChartboostMediationSettings.AndroidAppId, ChartboostMediationSettings.AndroidAppSignature);
        }

        [Obsolete("InitWithAppIdAndSignature has been deprecated, please use StartWithOptions instead")]
        public override void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            base.InitWithAppIdAndSignature(appId, appSignature);
            ChartboostMediationSettings.AndroidAppId = appId;
            ChartboostMediationSettings.AndroidAppSignature = appSignature;
            using var nativeSDK = GetNativeSDK();
            using var unityPlayer = new AndroidJavaClass(AndroidConstants.ClassUnityPlayer);
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>(AndroidConstants.PropertyCurrentActivity);
            using var initializationOptions = GetInitializationOptions().ToInitializationOptions();
            nativeSDK.CallStatic(AndroidConstants.FunStart, activity, appId, appSignature, initializationOptions,  new ChartboostMediationSDKListener());
            IsInitialized = true;
        }

        public override void StartWithOptions(string appId, string appSignature, string[] initializationOptions = null)
        {
            base.StartWithOptions(appId, appSignature, initializationOptions);
            ChartboostMediationSettings.AndroidAppId = appId;
            ChartboostMediationSettings.AndroidAppSignature = appSignature;
            initializationOptions ??= Array.Empty<string>();
            using var nativeOptions = initializationOptions.ToInitializationOptions();
            using var nativeSDK = GetNativeSDK();
            using var unityPlayer = new AndroidJavaClass(AndroidConstants.ClassUnityPlayer);
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>(AndroidConstants.PropertyCurrentActivity);
            nativeSDK.CallStatic(AndroidConstants.FunStart, activity, appId, appSignature, nativeOptions,  new ChartboostMediationSDKListener());
            IsInitialized = true;
        }

        public override void SetSubjectToCoppa(bool isSubject)
        {
            base.SetSubjectToCoppa(isSubject);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic(AndroidConstants.FunSetSubjectToCoppa, isSubject);
        }

        public override void SetSubjectToGDPR(bool isSubject)
        {
            base.SetSubjectToGDPR(isSubject);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic(AndroidConstants.FunSetSubjectToGDPR, isSubject);
        }

        public override void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            base.SetUserHasGivenConsent(hasGivenConsent);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic(AndroidConstants.FunSetUserHasGivenConsent, hasGivenConsent);
        }

        public override void SetCCPAConsent(bool hasGivenConsent)
        {
            base.SetCCPAConsent(hasGivenConsent);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic(AndroidConstants.FunSetCCPAConsent, hasGivenConsent);
        }

        public override void SetUserIdentifier(string userIdentifier)
        {
            base.SetUserIdentifier(userIdentifier);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic(AndroidConstants.FunSetUserIdentifier, userIdentifier);
        }

        public override string GetUserIdentifier()
        {
            base.GetUserIdentifier();
            using var nativeSDK = GetNativeSDK();
            return nativeSDK.CallStatic<string>(AndroidConstants.FunGetUserIdentifier);
        }
        
        public override void SetTestMode(bool testModeEnabled)
        {
            base.SetTestMode(testModeEnabled);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic(AndroidConstants.FunSetTestMode, testModeEnabled);
        }

        public override void DiscardOversizedAds(bool shouldDiscard)
        {
            base.DiscardOversizedAds(shouldDiscard);
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic(AndroidConstants.FunSetShouldDiscardOversizedAds, shouldDiscard);
        }

        public override ChartboostMediationAdapterInfo[] InitializedAdaptersInfo()
        {
            using var native = GetNativeSDK();
            using var adaptersInfo = native.CallStatic<AndroidJavaObject>(AndroidConstants.FunAdapterInfo);
            return adaptersInfo.ToAdapterInfo();
        }

        public override void Destroy()
        {
            if (!CheckInitialized())
                return;
            base.Destroy();
            using var nativeSDK = GetNativeSDK();
            nativeSDK.CallStatic(AndroidConstants.FunUnsubscribeILRDObserver, ILRDObserver.Instance);
            IsInitialized = false;
        }

        public override async Task<ChartboostMediationFullscreenAdLoadResult> LoadFullscreenAd(ChartboostMediationFullscreenAdLoadRequest request)
        {
            if (!CanFetchAd(request.PlacementName))
            {
                var error = new ChartboostMediationError(ErrorNotReady);
                var adLoadResult = new ChartboostMediationFullscreenAdLoadResult(error);
                return await Task.FromResult(adLoadResult);
            }

            var adLoadListenerAwaitableProxy = new ChartboostMediationFullscreenAdLoadListener();
            try
            {
                CacheManager.TrackFullscreenAdLoadRequest(adLoadListenerAwaitableProxy.hashCode(), request);
                using var nativeAdRequest = new AndroidJavaObject(GetQualifiedNativeClassName(AndroidConstants.ClassChartboostMediationAdLoadRequest, true), request.PlacementName, request.Keywords.ToKeywords());
                using var bridge = GetUnityBridge();
                bridge.CallStatic(AndroidConstants.FunLoadFullscreenAd, nativeAdRequest, adLoadListenerAwaitableProxy, ChartboostMediationFullscreenAdListener.Instance);
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
            var bannerAd = unityBridge.CallStatic<AndroidJavaObject>(AndroidConstants.FunLoadBannerAd, new ChartboostMediationBannerViewListener());
            return new ChartboostMediationBannerViewAndroid(bannerAd);
        }

        public static float GetUIScaleFactor()
        {
            using var unityBridge = GetUnityBridge();
            return unityBridge.CallStatic<float>(AndroidConstants.FunGetUIScaleFactor);
        }
        
        #endregion
    }
}
#endif

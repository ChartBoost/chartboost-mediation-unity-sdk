#if UNITY_ANDROID
using System.Collections.Generic;
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
        private static string GetQualifiedClassName(string nativeClass)=> $"{ChartboostMediationSDK}.unity.{nativeClass}";

        private static ChartboostMediationAndroid _instance;
        private static AndroidJavaObject _plugin;

        public ChartboostMediationAndroid()
        {
            _instance = this;
            LogTag = "ChartboostMediation (Android)";
            plugin().Call("setupEventListeners",
                LifeCycleEventListener.Instance,
                InterstitialEventListener.Instance,
                RewardedVideoEventListener.Instance,
                BannerEventListener.Instance);
        }

        // Initialize the android bridge
        internal static AndroidJavaObject plugin()
        {
            if (_plugin != null)
                return _plugin;
            // find the plugin instance
            using var pluginClass = new AndroidJavaClass(GetQualifiedClassName("UnityBridge"));
            _plugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
            return _plugin;
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
            plugin().Call("start", appId, appSignature, Application.unityVersion, GetInitializationOptions());
            IsInitialized = true;
        }

        public override void SetSubjectToCoppa(bool isSubject)
        {
            base.SetSubjectToCoppa(isSubject);
            plugin().Call("setSubjectToCoppa", isSubject);
        }

        public override void SetSubjectToGDPR(bool isSubject)
        {
            base.SetSubjectToGDPR(isSubject);
            plugin().Call("setSubjectToGDPR", isSubject);
        }

        public override void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            base.SetUserHasGivenConsent(hasGivenConsent);
            plugin().Call("setUserHasGivenConsent", hasGivenConsent);
        }

        public override void SetCCPAConsent(bool hasGivenConsent)
        {
            base.SetCCPAConsent(hasGivenConsent);
            plugin().Call("setCCPAConsent", hasGivenConsent);
        }

        public override void SetUserIdentifier(string userIdentifier)
        {
            base.SetUserIdentifier(userIdentifier);
            plugin().Call("setUserIdentifier", userIdentifier);
        }

        public override string GetUserIdentifier()
        {
            base.GetUserIdentifier();
            return plugin().Call<string>("getUserIdentifier");
        }
        
        public override void SetTestMode(bool testModeEnabled)
        {
            base.SetTestMode(testModeEnabled);
            plugin().Call("setTestMode", testModeEnabled);
        }

        public override void Destroy()
        {
            if (!CheckInitialized())
                return;
            base.Destroy();
            _plugin.Call("destroy");
            IsInitialized = false;
        }

        public override async Task<ChartboostMediationFullscreenAdLoadResult> GetFullscreenAd(ChartboostMediationFullscreenAdLoadRequest request)
        {
            var awaitableProxy = new CMFullscreenAdLoadResultHandler(request);
            var nativeAdListener = new CMFullscreenAdListener(request);
            var nativeAdRequest = new AndroidJavaObject("com.chartboost.heliumsdk.ad.ChartboostMediationAdLoadRequest", request.PlacementName, request.Keywords.ToKeywords());
            _plugin.Call("getFullscreenAd", nativeAdRequest, nativeAdListener, awaitableProxy);
            return await awaitableProxy;
        }
        #endregion
    }
}
#endif

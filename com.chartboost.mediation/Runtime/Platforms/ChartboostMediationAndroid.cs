#if UNITY_ANDROID
using UnityEngine;
using UnityEngine.Scripting;
// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming

namespace Chartboost.Platforms
{
    public sealed class ChartboostMediationAndroid : ChartboostMediationExternal
    {
        #region Chartboost Mediation
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
            using var pluginClass = new AndroidJavaClass("com.chartboost.mediation.unity.UnityBridge");
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
        #endregion

        #region LifeCycle Callbacks
        [Preserve]
        internal class LifeCycleEventListener : AndroidJavaProxy
        {
            private LifeCycleEventListener() : base("com.chartboost.mediation.unity.ILifeCycleEventListener") { }

            public static readonly LifeCycleEventListener Instance = new LifeCycleEventListener();

            [Preserve]
            private void DidStart(string error) 
                => EventProcessor.ProcessChartboostMediationEvent(error, _instance.DidStart);

            [Preserve]
            private void DidReceiveILRD(string impressionDataJson) 
                => EventProcessor.ProcessEventWithILRD(impressionDataJson, _instance.DidReceiveImpressionLevelRevenueData);

            [Preserve]
            private void DidReceivePartnerInitializationData(string partnerInitializationDataJson) 
                => EventProcessor.ProcessEventWithPartnerInitializationData(partnerInitializationDataJson, _instance.DidReceivePartnerInitializationData);
        }

        public override event ChartboostMediationEvent DidStart;
        public override event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData;
        public override event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData;
        #endregion

        #region Interstitial Callbacks
        [Preserve]
        internal class InterstitialEventListener : AndroidJavaProxy
        {
            private InterstitialEventListener() : base("com.chartboost.mediation.unity.IInterstitialEventListener") { }

            public static readonly InterstitialEventListener Instance = new InterstitialEventListener();

            [Preserve]
            private void DidLoadInterstitial(string placementName, string loadId, string auctionId, string partnerId, double price, string error)
                => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadInterstitial);

            [Preserve]
            private void DidShowInterstitial(string placementName, string error) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidShowInterstitial);

            [Preserve]
            private void DidCloseInterstitial(string placementName, string error) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidCloseInterstitial);
            
            [Preserve]
            private void DidClickInterstitial(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidClickInterstitial);

            [Preserve]
            private void DidRecordImpression(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidRecordImpressionInterstitial);
        }

        public override event ChartboostMediationPlacementLoadEvent DidLoadInterstitial;
        public override event ChartboostMediationPlacementEvent DidShowInterstitial;
        public override event ChartboostMediationPlacementEvent DidCloseInterstitial; 
        public override event ChartboostMediationPlacementEvent DidClickInterstitial;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial;
        #endregion

        #region Rewarded Callbacks
        internal class RewardedVideoEventListener : AndroidJavaProxy
        {
            private RewardedVideoEventListener() : base("com.chartboost.mediation.unity.IRewardedEventListener") { }

            public static readonly RewardedVideoEventListener Instance = new RewardedVideoEventListener();

            [Preserve]
            private void DidLoadRewarded(string placementName, string loadId, string auctionId, string partnerId, double price, string error) 
                => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadRewarded);

            [Preserve]
            private void DidShowRewarded(string placementName, string error) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidShowRewarded);

            [Preserve]
            private void DidCloseRewarded(string placementName, string error) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidCloseRewarded);

            [Preserve]
            private void DidClickRewarded(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidClickRewarded);

            [Preserve]
            private void DidRecordImpression(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidRecordImpressionRewarded);

            [Preserve]
            private void DidReceiveReward(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidReceiveReward);
        }

        public override event ChartboostMediationPlacementLoadEvent DidLoadRewarded;
        public override event ChartboostMediationPlacementEvent DidShowRewarded;
        public override event ChartboostMediationPlacementEvent DidCloseRewarded;
        public override event ChartboostMediationPlacementEvent DidClickRewarded;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionRewarded;
        public override event ChartboostMediationPlacementEvent DidReceiveReward;
        #endregion

        #region Banner Callbacks
        internal class BannerEventListener : AndroidJavaProxy
        {
            private BannerEventListener() : base("com.chartboost.mediation.unity.IBannerEventListener") { }

            public static readonly BannerEventListener Instance = new BannerEventListener();

            [Preserve]
            private void DidLoadBanner(string placementName, string loadId, string auctionId, string partnerId, double price, string error) 
                => EventProcessor.ProcessChartboostMediationLoadEvent(placementName,  loadId, auctionId, partnerId, price, error, _instance.DidLoadBanner);

            [Preserve]
            private void DidClickBanner(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidClickBanner);

            [Preserve]
            private void DidRecordImpression(string placementName) 
                => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, null, _instance.DidRecordImpressionBanner);
        }

        public override event ChartboostMediationPlacementLoadEvent DidLoadBanner;
        public override event ChartboostMediationPlacementEvent DidClickBanner;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionBanner;
        #endregion
    }
}
#endif

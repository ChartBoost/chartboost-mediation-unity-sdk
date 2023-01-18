#if UNITY_ANDROID
using UnityEngine;
using UnityEngine.Scripting;
// ReSharper disable StringLiteralTypo
// ReSharper disable InconsistentNaming

namespace Helium.Platforms
{
    public sealed class HeliumAndroid : HeliumExternal
    {
        #region Helium
        private static HeliumAndroid _instance;
        private static AndroidJavaObject _plugin;

        public HeliumAndroid()
        {
            _instance = this;
            LogTag = "Helium(Android)";
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
            using var pluginClass = new AndroidJavaClass("com.chartboost.heliumsdk.unity.HeliumUnityBridge");
            _plugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
            return _plugin;
        }

        public override void Init()
        {
            base.Init();
            InitWithAppIdAndSignature(HeliumSettings.AndroidAppId, HeliumSettings.AndroidAppSignature);
        }

        public override void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            base.InitWithAppIdAndSignature(appId, appSignature);
            HeliumSettings.AndroidAppId = appId;
            HeliumSettings.AndroidAppSignature = appSignature;
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
            return _plugin.Call<string>("getUserIdentifier");
        }

        public override void Pause(bool paused)
        {
            if (!CheckInitialized())
                return;
            base.Pause(paused);
            _plugin.Call("pause", paused);
        }

        public override void Destroy()
        {
            if (!CheckInitialized())
                return;
            base.Destroy();
            _plugin.Call("destroy");
            IsInitialized = false;
        }

        public override bool OnBackPressed()
        {
            var handled = base.OnBackPressed() && _plugin.Call<bool>("onBackPressed");
            return handled;
        }
        #endregion

        #region LifeCycle Callbacks
        [Preserve]
        internal class LifeCycleEventListener : AndroidJavaProxy
        {
            private LifeCycleEventListener() : base("com.chartboost.heliumsdk.unity.ILifeCycleEventListener") { }

            public static readonly LifeCycleEventListener Instance = new LifeCycleEventListener();

            [Preserve]
            private void DidStart(string error) 
                => HeliumEventProcessor.ProcessHeliumEvent(error, _instance.DidStart);

            [Preserve]
            private void DidReceiveILRD(string impressionDataJson) 
                => HeliumEventProcessor.ProcessEventWithILRD(impressionDataJson, _instance.DidReceiveImpressionLevelRevenueData);

            [Preserve]
            private void DidReceivePartnerInitializationData(string partnerInitializationDataJson) 
                => HeliumEventProcessor.ProcessEventWithPartnerInitializationData(partnerInitializationDataJson, _instance.DidReceivePartnerInitializationData);
        }

        public override event HeliumEvent DidStart;
        public override event HeliumILRDEvent DidReceiveImpressionLevelRevenueData;
        public override event HeliumPartnerInitializationEvent DidReceivePartnerInitializationData;
        #endregion

        #region Interstitial Callbacks
        [Preserve]
        internal class InterstitialEventListener : AndroidJavaProxy
        {
            private InterstitialEventListener() : base("com.chartboost.heliumsdk.unity.IInterstitialEventListener") { }

            public static readonly InterstitialEventListener Instance = new InterstitialEventListener();

            [Preserve]
            private void DidLoadInterstitial(string placementName, string auctionId, string partnerId, double price, string error)
                => HeliumEventProcessor.ProcessHeliumLoadEvent(placementName,  auctionId, partnerId, price, error, _instance.DidLoadInterstitial);

            [Preserve]
            private void DidShowInterstitial(string placementName, string error) => 
                HeliumEventProcessor.ProcessHeliumPlacementEventWithError(placementName, error, _instance.DidShowInterstitial);

            [Preserve]
            private void DidCloseInterstitial(string placementName, string error) 
                => HeliumEventProcessor.ProcessHeliumPlacementEventWithError(placementName, error, _instance.DidCloseInterstitial);
            
            [Preserve]
            private void DidClickInterstitial(string placementName) 
                => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidClickInterstitial);

            [Preserve]
            private void DidRecordImpression(string placementName) 
                => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName,_instance.DidRecordImpressionInterstitial);
        }

        public override event HeliumPlacementLoadEvent DidLoadInterstitial;
        public override event HeliumPlacementEventWithError DidShowInterstitial;
        public override event HeliumPlacementEventWithError DidCloseInterstitial; 
        public override event HeliumPlacementEvent DidClickInterstitial;
        public override event HeliumPlacementEvent DidRecordImpressionInterstitial;
        #endregion

        #region Rewarded Callbacks
        internal class RewardedVideoEventListener : AndroidJavaProxy
        {
            private RewardedVideoEventListener() : base("com.chartboost.heliumsdk.unity.IRewardedEventListener") { }

            public static readonly RewardedVideoEventListener Instance = new RewardedVideoEventListener();

            [Preserve]
            private void DidLoadRewarded(string placementName, string auctionId, string partnerId, double price, string error) 
                => HeliumEventProcessor.ProcessHeliumLoadEvent(placementName, auctionId, partnerId, price, error, _instance.DidLoadRewarded);

            [Preserve]
            private void DidShowRewarded(string placementName, string error) 
                => HeliumEventProcessor.ProcessHeliumPlacementEventWithError(placementName, error, _instance.DidShowRewarded);

            [Preserve]
            private void DidCloseRewarded(string placementName, string error) 
                => HeliumEventProcessor.ProcessHeliumPlacementEventWithError(placementName, error, _instance.DidCloseRewarded);

            [Preserve]
            private void DidClickRewarded(string placementName) 
                => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidClickRewarded);

            [Preserve]
            private void DidRecordImpression(string placementName) 
                => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidRecordImpressionRewarded);

            [Preserve]
            private void DidReceiveReward(string placementName) 
                => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidReceiveReward);
        }

        public override event HeliumPlacementLoadEvent DidLoadRewarded;
        public override event HeliumPlacementEventWithError DidShowRewarded;
        public override event HeliumPlacementEventWithError DidCloseRewarded;
        public override event HeliumPlacementEvent DidClickRewarded;
        public override event HeliumPlacementEvent DidRecordImpressionRewarded;
        public override event HeliumPlacementEvent DidReceiveReward;
        #endregion

        #region Banner Callbacks
        internal class BannerEventListener : AndroidJavaProxy
        {
            private BannerEventListener() : base("com.chartboost.heliumsdk.unity.IBannerEventListener") { }

            public static readonly BannerEventListener Instance = new BannerEventListener();

            [Preserve]
            private void DidLoadBanner(string placementName, string auctionId, string partnerId, double price, string error) 
                => HeliumEventProcessor.ProcessHeliumLoadEvent(placementName,  auctionId, partnerId, price, error, _instance.DidLoadBanner);

            [Preserve]
            private void DidClickBanner(string placementName) 
                => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidClickBanner);

            [Preserve]
            private void DidRecordImpression(string placementName) 
                => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidRecordImpressionBanner);
        }

        public override event HeliumPlacementLoadEvent DidLoadBanner;
        public override event HeliumPlacementEvent DidClickBanner;
        public override event HeliumPlacementEvent DidRecordImpressionBanner;
        #endregion
    }
}
#endif

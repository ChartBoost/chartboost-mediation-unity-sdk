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
            LOGTag = "Helium(Android)";
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
            private void DidStart(int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumEvent(errorCode, errorDescription, _instance.DidStart);
            }

            [Preserve]
            private void DidReceiveILRD(string impressionDataJson)
            {
                HeliumEventProcessor.ProcessEventWithILRD(impressionDataJson,
                    _instance.DidReceiveImpressionLevelRevenueData);
            }

            [Preserve]
            private void DidReceivePartnerInitializationData(string partnerInitializationDataJson)
            {
                HeliumEventProcessor.ProcessEventWithPartnerInitializationData(partnerInitializationDataJson, 
                    _instance.DidReceivePartnerInitializationData);
            }
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
            private void DidLoadInterstitial(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidLoadInterstitial);
            }

            [Preserve]
            private void DidShowInterstitial(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidShowInterstitial);
            }

            [Preserve]
            private void DidClickInterstitial(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidClickInterstitial);
            }

            [Preserve]
            private void DidCloseInterstitial(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidCloseInterstitial);
            }
            
            [Preserve]
            private void DidRecordImpression(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidRecordImpressionInterstitial);
            }

            [Preserve]
            private void DidWinBidInterstitial(string placementName, string auctionId, string partnerId, double price)
            {
                HeliumEventProcessor.ProcessHeliumBidEvent(placementName, auctionId, partnerId, price,
                    _instance.DidWinBidInterstitial);
            }
        }

        public override event HeliumPlacementEvent DidLoadInterstitial;
        public override event HeliumPlacementEvent DidShowInterstitial;
        public override event HeliumPlacementEvent DidClickInterstitial;
        public override event HeliumPlacementEvent DidCloseInterstitial; 
        public override event HeliumPlacementEvent DidRecordImpressionInterstitial;
        public override event HeliumBidEvent DidWinBidInterstitial;
        #endregion

        #region Rewarded Callbacks
        internal class RewardedVideoEventListener : AndroidJavaProxy
        {
            private RewardedVideoEventListener() : base("com.chartboost.heliumsdk.unity.IRewardedEventListener") { }

            public static readonly RewardedVideoEventListener Instance = new RewardedVideoEventListener();

            [Preserve]
            private void DidLoadRewarded(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidLoadRewarded);
            }

            [Preserve]
            private void DidShowRewarded(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidShowRewarded);
            }

            [Preserve]
            private void DidCloseRewarded(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidCloseRewarded);
            }

            [Preserve]
            private void DidClickRewarded(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidClickRewarded);
            }
            
            [Preserve]
            private void DidRecordImpression(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidRecordImpressionRewarded);
            }

            [Preserve]
            private void DidWinBidRewarded(string placementName, string auctionId, string partnerId,
                double price)
            {
                HeliumEventProcessor.ProcessHeliumBidEvent(placementName, auctionId, partnerId, price,
                    _instance.DidWinBidRewarded);
            }

            [Preserve]
            private void DidReceiveReward(string placementName, int reward)
            {
                HeliumEventProcessor.ProcessHeliumRewardEvent(placementName, reward, _instance.DidReceiveReward);
            }
        }

        public override event HeliumPlacementEvent DidLoadRewarded;
        public override event HeliumPlacementEvent DidShowRewarded;
        public override event HeliumPlacementEvent DidCloseRewarded;
        public override event HeliumPlacementEvent DidClickRewarded;
        public override event HeliumPlacementEvent DidRecordImpressionRewarded;
        public override event HeliumBidEvent DidWinBidRewarded;
        public override event HeliumRewardEvent DidReceiveReward;
        #endregion

        #region Banner Callbacks
        internal class BannerEventListener : AndroidJavaProxy
        {
            private BannerEventListener() : base("com.chartboost.heliumsdk.unity.IBannerEventListener") { }

            public static readonly BannerEventListener Instance = new BannerEventListener();

            [Preserve]
            private void DidLoadBanner(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidLoadBanner);
            }

            [Preserve]
            private void DidShowBanner(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidShowBanner);
            }

            [Preserve]
            private void DidClickBanner(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidClickBanner);
            }
            
            [Preserve]
            private void DidRecordImpression(string placementName, int errorCode, string errorDescription)
            {
                HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                    _instance.DidRecordImpressionBanner);
            }

            [Preserve]
            private void DidWinBidBanner(string placementName, string auctionId, string partnerId,
                double price)
            {
                HeliumEventProcessor.ProcessHeliumBidEvent(placementName, auctionId, partnerId, price,
                    _instance.DidWinBidBanner);
            }
        }

        public override event HeliumPlacementEvent DidLoadBanner;
        public override event HeliumPlacementEvent DidShowBanner;
        public override event HeliumPlacementEvent DidClickBanner;
        public override event HeliumPlacementEvent DidRecordImpressionBanner;
        public override event HeliumBidEvent DidWinBidBanner;
        #endregion
    }
}
#endif

#if UNITY_IPHONE
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Helium.Platforms
{
    public sealed class HeliumIOS : HeliumExternal
    {
        #region Objective-C Extern Members
        // callback definitions for objective-c layer
        private delegate void ExternHeliumEvent(string error);
        private delegate void ExternHeliumILRDEvent(string impressionDataJson);
        private delegate void ExternHeliumPartnerInitializationDataEvent(string partnerInitializationData);
        private delegate void ExternHeliumPlacementLoadEvent(string placementName, string auctionId, string partnerId, double price, string error);
        private delegate void ExternHeliumPlacementEventWithError(string placementName, string error);
        private delegate void ExternHeliumPlacementEvent(string placementName);

        [DllImport("__Internal")]
        private static extern void _setLifeCycleCallbacks(ExternHeliumEvent DidStartCallback,
            ExternHeliumILRDEvent DidReceiveILRDCallback, ExternHeliumPartnerInitializationDataEvent DidReceivePartnerInitializationDataCallback);

        [DllImport("__Internal")]
        private static extern void _setInterstitialCallbacks(ExternHeliumPlacementLoadEvent DidLoadCallback,
            ExternHeliumPlacementEventWithError DidShowCallback, ExternHeliumPlacementEventWithError DidCloseCallback,
            ExternHeliumPlacementEvent DidClickCallback, ExternHeliumPlacementEvent DidRecordImpression);

        [DllImport("__Internal")]
        private static extern void _setRewardedCallbacks(ExternHeliumPlacementLoadEvent DidLoadCallback,
            ExternHeliumPlacementEventWithError DidShowCallback, ExternHeliumPlacementEventWithError DidCloseCallback, ExternHeliumPlacementEvent DidClickCallback,
            ExternHeliumPlacementEvent DidRecordImpression, ExternHeliumPlacementEvent DidReceiveReward);

        [DllImport("__Internal")]
        private static extern void _setBannerCallbacks(ExternHeliumPlacementLoadEvent DidLoadCallback, ExternHeliumPlacementEvent DidRecordImpression,
            ExternHeliumPlacementEvent DidClickCallback);

        [DllImport("__Internal")]
        private static extern void _heliumSdkInit(string appId, string appSignature, string unityVersion, string[] initializationOptions, int initializationOptionsSize);

        [DllImport("__Internal")]
        private static extern void _heliumSdkSetSubjectToCoppa(bool isSubject);

        [DllImport("__Internal")]
        private static extern void _heliumSdkSetSubjectToGDPR(bool isSubject);

        [DllImport("__Internal")]
        private static extern void _heliumSdkSetUserHasGivenConsent(bool hasGivenConsent);

        [DllImport("__Internal")]
        private static extern void _heliumSetCCPAConsent(bool hasGivenConsent);

        [DllImport("__Internal")]
        private static extern void _heliumSetUserIdentifier(string userIdentifier);

        [DllImport("__Internal")]
        private static extern string _heliumGetUserIdentifier();
        #endregion

        #region Helium
        private static HeliumIOS _instance;

        public HeliumIOS()
        {
            _instance = this;
            LogTag = "Helium(iOS)";
            _setLifeCycleCallbacks(ExternDidStart, ExternDidReceiveILRD, ExternDidReceivePartnerInitializationData);
            _setInterstitialCallbacks(ExternDidLoadInterstitial, ExternDidShowInterstitial,
                ExternDidCloseInterstitial, ExternDidClickInterstitial,ExternDidRecordImpressionInterstitial);
            _setRewardedCallbacks(ExternDidLoadRewarded, ExternDidShowRewarded,
                ExternDidCloseRewarded, ExternDidClickRewarded, ExternDidRecordImpressionRewarded, ExternDidReceiveReward);
            _setBannerCallbacks(ExternDidLoadBanner,ExternDidRecordImpressionBanner, ExternDidClickBanner);
        }

        public override void Init()
        {
            base.Init();
            InitWithAppIdAndSignature(HeliumSettings.IOSAppId, HeliumSettings.IOSAppSignature);
        }

        public override void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            base.InitWithAppIdAndSignature(appId, appSignature);
            HeliumSettings.IOSAppId = appId;
            HeliumSettings.IOSAppSignature = appSignature;
            var initializationOptions = GetInitializationOptions();
            _heliumSdkInit(appId, appSignature, Application.unityVersion, initializationOptions, initializationOptions.Length);
            IsInitialized = true;
        }

        public override void SetSubjectToCoppa(bool isSubject)
        {
            base.SetSubjectToCoppa(isSubject);
            _heliumSdkSetSubjectToCoppa(isSubject);
        }

        public override void SetSubjectToGDPR(bool isSubject)
        {
            base.SetSubjectToGDPR(isSubject);
            _heliumSdkSetSubjectToGDPR(isSubject);
        }

        public override void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            base.SetUserHasGivenConsent(hasGivenConsent);
            _heliumSdkSetUserHasGivenConsent(hasGivenConsent);
        }

        public override void SetCCPAConsent(bool hasGivenConsent)
        {
            base.SetCCPAConsent(hasGivenConsent);
            _heliumSetCCPAConsent(hasGivenConsent);
        }

        public override void SetUserIdentifier(string userIdentifier)
        {
            base.SetUserIdentifier(userIdentifier);
            _heliumSetUserIdentifier(userIdentifier);
        }

        public override string GetUserIdentifier()
        {
            base.GetUserIdentifier();
            return _heliumGetUserIdentifier();
        }
        #endregion

        #region LifeCycle Callbacks
        [MonoPInvokeCallback(typeof(ExternHeliumEvent))]
        private static void ExternDidStart(string error) 
            => HeliumEventProcessor.ProcessHeliumEvent(error, _instance.DidStart);

        [MonoPInvokeCallback(typeof(ExternHeliumILRDEvent))]
        private static void ExternDidReceiveILRD(string impressionDataJson) 
            => HeliumEventProcessor.ProcessEventWithILRD(impressionDataJson, _instance.DidReceiveImpressionLevelRevenueData);

        [MonoPInvokeCallback(typeof(ExternHeliumPartnerInitializationDataEvent))]
        private static void ExternDidReceivePartnerInitializationData(string partnerInitializationData) 
            => HeliumEventProcessor.ProcessEventWithPartnerInitializationData(partnerInitializationData, _instance.DidReceivePartnerInitializationData);

        public override event HeliumEvent DidStart;
        public override event HeliumILRDEvent DidReceiveImpressionLevelRevenueData;
        public override event HeliumPartnerInitializationEvent DidReceivePartnerInitializationData;
        #endregion

        #region Interstitial Callbacks
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementLoadEvent))]
        private static void ExternDidLoadInterstitial(string placementName, string auctionId, string partnerId, double price, string error)
            => HeliumEventProcessor.ProcessHeliumLoadEvent(placementName, auctionId, partnerId, price, error, _instance.DidLoadInterstitial);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEventWithError))]
        private static void ExternDidShowInterstitial(string placementName, string error) 
            => HeliumEventProcessor.ProcessHeliumPlacementEventWithError(placementName, error, _instance.DidShowInterstitial);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEventWithError))]
        private static void ExternDidCloseInterstitial(string placementName, string error) 
            => HeliumEventProcessor.ProcessHeliumPlacementEventWithError(placementName, error, _instance.DidCloseInterstitial);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidClickInterstitial(string placementName) 
            => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidClickInterstitial);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidRecordImpressionInterstitial(string placementName)
            => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidRecordImpressionInterstitial);

        public override event HeliumPlacementLoadEvent DidLoadInterstitial;
        public override event HeliumPlacementEventWithError DidShowInterstitial;
        public override event HeliumPlacementEventWithError DidCloseInterstitial;
        public override event HeliumPlacementEvent DidClickInterstitial;
        public override event HeliumPlacementEvent DidRecordImpressionInterstitial;
        #endregion

        #region Rewarded Callbacks
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementLoadEvent))]
        private static void ExternDidLoadRewarded(string placementName, string auctionId, string partnerId, double price, string error) 
            => HeliumEventProcessor.ProcessHeliumLoadEvent(placementName, auctionId, partnerId, price, error, _instance.DidLoadRewarded);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEventWithError))]
        private static void ExternDidShowRewarded(string placementName, string error) 
            => HeliumEventProcessor.ProcessHeliumPlacementEventWithError(placementName, error, _instance.DidShowRewarded);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEventWithError))]
        private static void ExternDidCloseRewarded(string placementName, string error) 
            => HeliumEventProcessor.ProcessHeliumPlacementEventWithError(placementName, error, _instance.DidCloseRewarded);
        
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidClickRewarded(string placementName) 
            => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidClickRewarded);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidRecordImpressionRewarded(string placementName)
            => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidRecordImpressionRewarded);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidReceiveReward(string placementName) 
            => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidReceiveReward);

        public override event HeliumPlacementLoadEvent DidLoadRewarded;
        public override event HeliumPlacementEventWithError DidShowRewarded;
        public override event HeliumPlacementEventWithError DidCloseRewarded;
        public override event HeliumPlacementEvent DidClickRewarded;
        public override event HeliumPlacementEvent DidRecordImpressionRewarded;
        public override event HeliumPlacementEvent DidReceiveReward;
        #endregion

        #region Banner Callbacks
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementLoadEvent))]
        private static void ExternDidLoadBanner(string placementName, string auctionId, string partnerId, double price, string error) 
            => HeliumEventProcessor.ProcessHeliumLoadEvent(placementName, auctionId, partnerId, price, error, _instance.DidLoadBanner);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidRecordImpressionBanner(string placementName) 
            => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidRecordImpressionBanner);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidClickBanner(string placementName) 
            => HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, _instance.DidClickBanner);

        public override event HeliumPlacementLoadEvent DidLoadBanner;
        public override event HeliumPlacementEvent DidRecordImpressionBanner;
        public override event HeliumPlacementEvent DidClickBanner;
        #endregion
    }
}
#endif

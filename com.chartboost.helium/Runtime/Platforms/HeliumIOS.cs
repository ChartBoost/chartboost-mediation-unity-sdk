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
        private delegate void ExternHeliumEvent(int errorCode, string errorDescription);
        private delegate void ExternHeliumILRDEvent(string impressionDataJson);
        private delegate void ExternHeliumPartnerInitializationDataEvent(string partnerInitializationData);
        private delegate void ExternHeliumPlacementEvent(string placementName, int errorCode, string errorDescription);
        private delegate void ExternHeliumWinBidEvent(string placementName, string auctionId, string partnerId, double price);
        private delegate void ExternHeliumRewardEvent(string placementName, int reward);

        [DllImport("__Internal")]
        private static extern void _setLifeCycleCallbacks(ExternHeliumEvent DidStartCallback,
            ExternHeliumILRDEvent DidReceiveILRDCallback, ExternHeliumPartnerInitializationDataEvent DidReceivePartnerInitializationDataCallback);

        [DllImport("__Internal")]
        private static extern void _setInterstitialCallbacks(ExternHeliumPlacementEvent DidLoadCallback,
            ExternHeliumPlacementEvent DidShowCallback, ExternHeliumPlacementEvent DidClickCallback,
            ExternHeliumPlacementEvent DidCloseCallback, ExternHeliumPlacementEvent DidRecordImpression, ExternHeliumWinBidEvent DidWinBidCallback);

        [DllImport("__Internal")]
        private static extern void _setRewardedCallbacks(ExternHeliumPlacementEvent DidLoadCallback,
            ExternHeliumPlacementEvent DidShowCallback, ExternHeliumPlacementEvent DidClickCallback,
            ExternHeliumPlacementEvent DidCloseCallback, ExternHeliumPlacementEvent DidRecordImpression, ExternHeliumWinBidEvent DidWinBidCallback,
            ExternHeliumRewardEvent DidReceiveReward);

        [DllImport("__Internal")]
        private static extern void _setBannerCallbacks(ExternHeliumPlacementEvent DidLoadCallback, ExternHeliumPlacementEvent DidRecordImpression,
            ExternHeliumPlacementEvent DidClickCallback, ExternHeliumWinBidEvent DidWinBidCallback);

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
            _setInterstitialCallbacks(ExternDidLoadInterstitial, ExternDidShowInterstitial, ExternDidClickInterstitial,
                ExternDidCloseInterstitial, ExternDidRecordImpressionInterstitial, ExternDidWinBidInterstitial);
            _setRewardedCallbacks(ExternDidLoadRewarded, ExternDidShowRewarded, ExternDidClickRewarded,
                ExternDidCloseRewarded, ExternDidRecordImpressionRewarded, ExternDidWinBidRewarded, ExternDidReceiveReward);
            _setBannerCallbacks(ExternDidLoadBanner,ExternDidRecordImpressionBanner, ExternDidClickBanner, ExternDidWinBidBanner);
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
        private static void ExternDidStart(int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumEvent(errorCode, errorDescription, _instance.DidStart);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumILRDEvent))]
        private static void ExternDidReceiveILRD(string impressionDataJson)
        {
            HeliumEventProcessor.ProcessEventWithILRD(impressionDataJson,
                _instance.DidReceiveImpressionLevelRevenueData);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumPartnerInitializationDataEvent))]
        private static void ExternDidReceivePartnerInitializationData(string partnerInitializationData)
        {
            HeliumEventProcessor.ProcessEventWithPartnerInitializationData(partnerInitializationData, 
                _instance.DidReceivePartnerInitializationData);
        }

        public override event HeliumEvent DidStart;
        public override event HeliumILRDEvent DidReceiveImpressionLevelRevenueData;
        public override event HeliumPartnerInitializationEvent DidReceivePartnerInitializationData;
        #endregion

        #region Interstitial Callbacks
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidLoadInterstitial(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidLoadInterstitial);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidShowInterstitial(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidShowInterstitial);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidClickInterstitial(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidClickInterstitial);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidCloseInterstitial(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidCloseInterstitial);
        }
        
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidRecordImpressionInterstitial(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidRecordImpressionInterstitial);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumWinBidEvent))]
        private static void ExternDidWinBidInterstitial(string placementName, string auctionId, string partnerId,
            double price)
        {
            HeliumEventProcessor.ProcessHeliumBidEvent(placementName, auctionId, partnerId, price,
                _instance.DidWinBidInterstitial);
        }

        public override event HeliumPlacementEvent DidLoadInterstitial;
        public override event HeliumPlacementEvent DidShowInterstitial;
        public override event HeliumPlacementEvent DidClickInterstitial;
        public override event HeliumPlacementEvent DidCloseInterstitial;
        public override event HeliumPlacementEvent DidRecordImpressionInterstitial;
        public override event HeliumBidEvent DidWinBidInterstitial;
        #endregion

        #region Rewarded Callbacks
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidLoadRewarded(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidLoadRewarded);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidShowRewarded(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidShowRewarded);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidClickRewarded(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidClickRewarded);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidCloseRewarded(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidCloseRewarded);
        }
        
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidRecordImpressionRewarded(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidRecordImpressionRewarded);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumWinBidEvent))]
        private static void ExternDidWinBidRewarded(string placementName, string auctionId,
            string partnerId, double price)
        {
            HeliumEventProcessor.ProcessHeliumBidEvent(placementName,  auctionId, partnerId, price,
                _instance.DidWinBidRewarded);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumRewardEvent))]
        private static void ExternDidReceiveReward(string placementName, int reward)
        {
            HeliumEventProcessor.ProcessHeliumRewardEvent(placementName, reward, _instance.DidReceiveReward);
        }

        public override event HeliumPlacementEvent DidLoadRewarded;
        public override event HeliumPlacementEvent DidShowRewarded;
        public override event HeliumPlacementEvent DidClickRewarded;
        public override event HeliumPlacementEvent DidCloseRewarded;
        public override event HeliumPlacementEvent DidRecordImpressionRewarded;
        public override event HeliumBidEvent DidWinBidRewarded;
        public override event HeliumRewardEvent DidReceiveReward;
        #endregion

        #region Banner Callbacks
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidLoadBanner(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidLoadBanner);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidRecordImpressionBanner(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidRecordImpressionBanner);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidClickBanner(string placementName, int errorCode, string errorDescription)
        {
            HeliumEventProcessor.ProcessHeliumPlacementEvent(placementName, errorCode, errorDescription,
                _instance.DidClickBanner);
        }

        [MonoPInvokeCallback(typeof(ExternHeliumWinBidEvent))]
        private static void ExternDidWinBidBanner(string placementName, string auctionId,
            string partnerId, double price)
        {
            HeliumEventProcessor.ProcessHeliumBidEvent(placementName, auctionId, partnerId, price,
                _instance.DidWinBidBanner);
        }

        public override event HeliumPlacementEvent DidLoadBanner;
        public override event HeliumPlacementEvent DidRecordImpressionBanner;
        public override event HeliumPlacementEvent DidClickBanner;
        public override event HeliumBidEvent DidWinBidBanner;
        #endregion
    }
}
#endif

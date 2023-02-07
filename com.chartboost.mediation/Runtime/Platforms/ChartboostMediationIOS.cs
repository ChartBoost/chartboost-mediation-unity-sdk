#if UNITY_IPHONE
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Chartboost.Platforms
{
    public sealed class ChartboostMediationIOS : ChartboostMediationExternal
    {
        #region Objective-C Extern Members
        // callback definitions for objective-c layer
        private delegate void ExternHeliumEvent(string error);
        private delegate void ExternHeliumILRDEvent(string impressionDataJson);
        private delegate void ExternHeliumPartnerInitializationDataEvent(string partnerInitializationData);
        private delegate void ExternHeliumPlacementEvent(string placementName, string error);
        private delegate void ExternHeliumPlacementLoadEvent(string placementName, string loadId, string auctionId, string partnerId, double price, string error);
        
        [DllImport("__Internal")]
        private static extern void _setLifeCycleCallbacks(ExternHeliumEvent DidStartCallback,
            ExternHeliumILRDEvent DidReceiveILRDCallback, ExternHeliumPartnerInitializationDataEvent DidReceivePartnerInitializationDataCallback);

        [DllImport("__Internal")]
        private static extern void _setInterstitialCallbacks(ExternHeliumPlacementLoadEvent DidLoadCallback,
            ExternHeliumPlacementEvent DidShowCallback, ExternHeliumPlacementEvent DidCloseCallback,
            ExternHeliumPlacementEvent DidClickCallback, ExternHeliumPlacementEvent DidRecordImpression);

        [DllImport("__Internal")]
        private static extern void _setRewardedCallbacks(ExternHeliumPlacementLoadEvent DidLoadCallback,
            ExternHeliumPlacementEvent DidShowCallback, ExternHeliumPlacementEvent DidCloseCallback, ExternHeliumPlacementEvent DidClickCallback,
            ExternHeliumPlacementEvent DidRecordImpression, ExternHeliumPlacementEvent DidReceiveReward);

        [DllImport("__Internal")]
        private static extern void _setBannerCallbacks(ExternHeliumPlacementLoadEvent DidLoadCallback, ExternHeliumPlacementEvent DidRecordImpression, ExternHeliumPlacementEvent DidClickCallback);

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
        private static ChartboostMediationIOS _instance;

        public ChartboostMediationIOS()
        {
            _instance = this;
            LogTag = "Helium(iOS)";
            _setLifeCycleCallbacks(ExternDidStart, 
                ExternDidReceiveILRD, 
                ExternDidReceivePartnerInitializationData);
            
            _setInterstitialCallbacks(ExternDidLoadInterstitial, 
                ExternDidShowInterstitial,
                ExternDidCloseInterstitial, 
                ExternDidClickInterstitial, 
                ExternDidRecordImpressionInterstitial);
            
            _setRewardedCallbacks(ExternDidLoadRewarded, 
                ExternDidShowRewarded,
                ExternDidCloseRewarded, 
                ExternDidClickRewarded,
                ExternDidRecordImpressionRewarded, 
                ExternDidReceiveReward);
            
            _setBannerCallbacks(ExternDidLoadBanner,
                ExternDidRecordImpressionBanner, 
                ExternDidClickBanner);
        }

        public override void Init()
        {
            base.Init();
            InitWithAppIdAndSignature(ChartboostMediationSettings.IOSAppId, ChartboostMediationSettings.IOSAppSignature);
        }

        public override void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            base.InitWithAppIdAndSignature(appId, appSignature);
            ChartboostMediationSettings.IOSAppId = appId;
            ChartboostMediationSettings.IOSAppSignature = appSignature;
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
            => EventProcessor.ProcessHeliumEvent(error, _instance.DidStart);

        [MonoPInvokeCallback(typeof(ExternHeliumILRDEvent))]
        private static void ExternDidReceiveILRD(string impressionDataJson) 
            => EventProcessor.ProcessEventWithILRD(impressionDataJson, _instance.DidReceiveImpressionLevelRevenueData);

        [MonoPInvokeCallback(typeof(ExternHeliumPartnerInitializationDataEvent))]
        private static void ExternDidReceivePartnerInitializationData(string partnerInitializationData) 
            => EventProcessor.ProcessEventWithPartnerInitializationData(partnerInitializationData, _instance.DidReceivePartnerInitializationData);

        public override event ChartboostMediationEvent DidStart;
        public override event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData;
        public override event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData;
        #endregion

        #region Interstitial Callbacks
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementLoadEvent))]
        private static void ExternDidLoadInterstitial(string placementName, string loadId, string auctionId, string partnerId, double price, string error)
            => EventProcessor.ProcessHeliumLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadInterstitial);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidShowInterstitial(string placementName, string error) 
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error, _instance.DidShowInterstitial);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidCloseInterstitial(string placementName, string error) 
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error, _instance.DidCloseInterstitial);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidClickInterstitial(string placementName, string error) 
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error, _instance.DidClickInterstitial);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidRecordImpressionInterstitial(string placementName, string error)
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error, _instance.DidRecordImpressionInterstitial);
        
        public override event ChartboostMediationPlacementLoadEvent DidLoadInterstitial;
        public override event ChartboostMediationPlacementEvent DidShowInterstitial;
        public override event ChartboostMediationPlacementEvent DidCloseInterstitial;
        public override event ChartboostMediationPlacementEvent DidClickInterstitial;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial;
        #endregion

        #region Rewarded Callbacks
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementLoadEvent))]
        private static void ExternDidLoadRewarded(string placementName, string loadId, string auctionId, string partnerId, double price, string error) 
            => EventProcessor.ProcessHeliumLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadRewarded);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidShowRewarded(string placementName, string error) 
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error, _instance.DidShowRewarded);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidCloseRewarded(string placementName, string error) 
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error, _instance.DidCloseRewarded);
        
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidClickRewarded(string placementName, string error) 
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error, _instance.DidClickRewarded);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidRecordImpressionRewarded(string placementName, string error)
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error, _instance.DidRecordImpressionRewarded);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidReceiveReward(string placementName, string error) 
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error, _instance.DidReceiveReward);
        public override event ChartboostMediationPlacementLoadEvent DidLoadRewarded;
        public override event ChartboostMediationPlacementEvent DidShowRewarded;
        public override event ChartboostMediationPlacementEvent DidCloseRewarded;
        public override event ChartboostMediationPlacementEvent DidClickRewarded;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionRewarded;
        public override event ChartboostMediationPlacementEvent DidReceiveReward;
        #endregion

        #region Banner Callbacks
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementLoadEvent))]
        private static void ExternDidLoadBanner(string placementName, string loadId, string auctionId, string partnerId, double price, string error) 
            => EventProcessor.ProcessHeliumLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadBanner);

        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidClickBanner(string placementName, string error) 
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error, _instance.DidClickBanner);
        
        [MonoPInvokeCallback(typeof(ExternHeliumPlacementEvent))]
        private static void ExternDidRecordImpressionBanner(string placementName, string error) 
            => EventProcessor.ProcessHeliumPlacementEvent(placementName, error,  _instance.DidRecordImpressionBanner);

        public override event ChartboostMediationPlacementLoadEvent DidLoadBanner;
        public override event ChartboostMediationPlacementEvent DidClickBanner;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionBanner;
        #endregion
    }
}
#endif

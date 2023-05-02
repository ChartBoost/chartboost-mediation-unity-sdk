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
        private delegate void ExternChartboostMediationEvent(string error);
        private delegate void ExternChartboostMediationILRDEvent(string impressionDataJson);
        private delegate void ExternChartboostMediationPartnerInitializationDataEvent(string partnerInitializationData);
        private delegate void ExternChartboostMediationPlacementEvent(string placementName, string error);
        private delegate void ExternChartboostMediationPlacementLoadEvent(string placementName, string loadId, string auctionId, string partnerId, double price, string error);
        
        [DllImport("__Internal")]
        private static extern void _setLifeCycleCallbacks(ExternChartboostMediationEvent DidStartCallback,
            ExternChartboostMediationILRDEvent DidReceiveILRDCallback, ExternChartboostMediationPartnerInitializationDataEvent DidReceivePartnerInitializationDataCallback);

        [DllImport("__Internal")]
        private static extern void _setInterstitialCallbacks(ExternChartboostMediationPlacementLoadEvent DidLoadCallback,
            ExternChartboostMediationPlacementEvent DidShowCallback, ExternChartboostMediationPlacementEvent DidCloseCallback,
            ExternChartboostMediationPlacementEvent DidClickCallback, ExternChartboostMediationPlacementEvent DidRecordImpression);

        [DllImport("__Internal")]
        private static extern void _setRewardedCallbacks(ExternChartboostMediationPlacementLoadEvent DidLoadCallback,
            ExternChartboostMediationPlacementEvent DidShowCallback, ExternChartboostMediationPlacementEvent DidCloseCallback, ExternChartboostMediationPlacementEvent DidClickCallback,
            ExternChartboostMediationPlacementEvent DidRecordImpression, ExternChartboostMediationPlacementEvent DidReceiveReward);

        [DllImport("__Internal")]
        private static extern void _setBannerCallbacks(ExternChartboostMediationPlacementLoadEvent DidLoadCallback, ExternChartboostMediationPlacementEvent DidRecordImpression, ExternChartboostMediationPlacementEvent DidClickCallback);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationInit(string appId, string appSignature, string unityVersion, string[] initializationOptions, int initializationOptionsSize);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationSetSubjectToCoppa(bool isSubject);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationSetSubjectToGDPR(bool isSubject);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationSetUserHasGivenConsent(bool hasGivenConsent);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationSetCCPAConsent(bool hasGivenConsent);

        [DllImport("__Internal")]
        private static extern void _chartboostMediationSetUserIdentifier(string userIdentifier);

        [DllImport("__Internal")]
        private static extern string _chartboostMediationGetUserIdentifier();

        [DllImport("__Internal")]
        private static extern void _chartboostMediationSetTestMode(bool isTestMode);

        [DllImport("__Internal")]
        private static extern float _chartboostMediationGetRetinaScaleFactor();
        #endregion

        #region Chartboost Mediation
        private static ChartboostMediationIOS _instance;

        public ChartboostMediationIOS()
        {
            _instance = this;
            LogTag = "ChartboostMediation(iOS)";
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
            _chartboostMediationInit(appId, appSignature, Application.unityVersion, initializationOptions, initializationOptions.Length);
            IsInitialized = true;
        }

        public override void SetSubjectToCoppa(bool isSubject)
        {
            base.SetSubjectToCoppa(isSubject);
            _chartboostMediationSetSubjectToCoppa(isSubject);
        }

        public override void SetSubjectToGDPR(bool isSubject)
        {
            base.SetSubjectToGDPR(isSubject);
            _chartboostMediationSetSubjectToGDPR(isSubject);
        }

        public override void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            base.SetUserHasGivenConsent(hasGivenConsent);
            _chartboostMediationSetUserHasGivenConsent(hasGivenConsent);
        }

        public override void SetCCPAConsent(bool hasGivenConsent)
        {
            base.SetCCPAConsent(hasGivenConsent);
            _chartboostMediationSetCCPAConsent(hasGivenConsent);
        }

        public override void SetUserIdentifier(string userIdentifier)
        {
            base.SetUserIdentifier(userIdentifier);
            _chartboostMediationSetUserIdentifier(userIdentifier);
        }

        public override string GetUserIdentifier()
        {
            base.GetUserIdentifier();
            return _chartboostMediationGetUserIdentifier();
        }

        public override void SetTestMode(bool testModeEnabled)
        {
            base.SetTestMode(testModeEnabled);
            _chartboostMediationSetTestMode(testModeEnabled);
        }

        public override float GetUIScaleFactor()
        {
            base.GetUIScaleFactor();
            return _chartboostMediationGetRetinaScaleFactor();
        }
        #endregion

        #region LifeCycle Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationEvent))]
        private static void ExternDidStart(string error) 
            => EventProcessor.ProcessChartboostMediationEvent(error, _instance.DidStart);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationILRDEvent))]
        private static void ExternDidReceiveILRD(string impressionDataJson) 
            => EventProcessor.ProcessEventWithILRD(impressionDataJson, _instance.DidReceiveImpressionLevelRevenueData);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPartnerInitializationDataEvent))]
        private static void ExternDidReceivePartnerInitializationData(string partnerInitializationData) 
            => EventProcessor.ProcessEventWithPartnerInitializationData(partnerInitializationData, _instance.DidReceivePartnerInitializationData);

        public override event ChartboostMediationEvent DidStart;
        public override event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData;
        public override event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData;
        #endregion

        #region Interstitial Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementLoadEvent))]
        private static void ExternDidLoadInterstitial(string placementName, string loadId, string auctionId, string partnerId, double price, string error)
            => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadInterstitial);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidShowInterstitial(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidShowInterstitial);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidCloseInterstitial(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidCloseInterstitial);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidClickInterstitial(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidClickInterstitial);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidRecordImpressionInterstitial(string placementName, string error)
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidRecordImpressionInterstitial);
        
        public override event ChartboostMediationPlacementLoadEvent DidLoadInterstitial;
        public override event ChartboostMediationPlacementEvent DidShowInterstitial;
        public override event ChartboostMediationPlacementEvent DidCloseInterstitial;
        public override event ChartboostMediationPlacementEvent DidClickInterstitial;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial;
        #endregion

        #region Rewarded Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementLoadEvent))]
        private static void ExternDidLoadRewarded(string placementName, string loadId, string auctionId, string partnerId, double price, string error) 
            => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadRewarded);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidShowRewarded(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidShowRewarded);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidCloseRewarded(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidCloseRewarded);
        
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidClickRewarded(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidClickRewarded);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidRecordImpressionRewarded(string placementName, string error)
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidRecordImpressionRewarded);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidReceiveReward(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidReceiveReward);
        public override event ChartboostMediationPlacementLoadEvent DidLoadRewarded;
        public override event ChartboostMediationPlacementEvent DidShowRewarded;
        public override event ChartboostMediationPlacementEvent DidCloseRewarded;
        public override event ChartboostMediationPlacementEvent DidClickRewarded;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionRewarded;
        public override event ChartboostMediationPlacementEvent DidReceiveReward;
        #endregion

        #region Banner Callbacks
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementLoadEvent))]
        private static void ExternDidLoadBanner(string placementName, string loadId, string auctionId, string partnerId, double price, string error) 
            => EventProcessor.ProcessChartboostMediationLoadEvent(placementName, loadId, auctionId, partnerId, price, error, _instance.DidLoadBanner);

        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidClickBanner(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error, _instance.DidClickBanner);
        
        [MonoPInvokeCallback(typeof(ExternChartboostMediationPlacementEvent))]
        private static void ExternDidRecordImpressionBanner(string placementName, string error) 
            => EventProcessor.ProcessChartboostMediationPlacementEvent(placementName, error,  _instance.DidRecordImpressionBanner);

        public override event ChartboostMediationPlacementLoadEvent DidLoadBanner;
        public override event ChartboostMediationPlacementEvent DidClickBanner;
        public override event ChartboostMediationPlacementEvent DidRecordImpressionBanner;
        #endregion
    }
}
#endif

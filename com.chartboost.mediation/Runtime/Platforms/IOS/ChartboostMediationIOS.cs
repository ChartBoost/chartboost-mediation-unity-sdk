#if UNITY_IPHONE
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Chartboost.Requests;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Chartboost.Platforms.IOS
{
    public sealed partial class ChartboostMediationIOS : ChartboostMediationExternal
    {
        #region Objective-C Extern Members
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
            
            _setFullscreenCallbacks(FullscreenAdEvents);
            
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

        public override async Task<ChartboostMediationFullscreenAdLoadResult> LoadFullscreenAd(ChartboostMediationFullscreenAdLoadRequest request)
        {
            if (!CanFetchAd(request.PlacementName))
            {
                var error = new ChartboostMediationError("Chartboost Mediation is not ready or placement is invalid.");
                var adLoadResult = new ChartboostMediationFullscreenAdLoadResult(error);
                return await Task.FromResult(adLoadResult);
            }

            var (proxy, hashCode) = _setupProxy<ChartboostMediationFullscreenAdLoadResult>();
            CacheManager.TrackFullscreenAdLoadRequest(hashCode, request);
            var keywordsJson = string.Empty;
            if (request.Keywords.Count > 0)
                keywordsJson = JsonConvert.SerializeObject(request.Keywords);
            _chartboostMediationLoadFullscreenAd(request.PlacementName, keywordsJson, hashCode, FullscreenAdLoadResultCallbackProxy);
            return await proxy;
        }

        [DllImport("__Internal")]
        private static extern void _chartboostMediationLoadFullscreenAd(string placementName, string keywords, int hashCode, ExternChartboostMediationFullscreenAdLoadResultEvent callback);
        #endregion
    }
}
#endif

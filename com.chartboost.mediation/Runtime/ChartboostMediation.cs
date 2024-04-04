using System;
using System.Threading.Tasks;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Banner.Unity;
using Chartboost.Consent;
using Chartboost.Events;
using Chartboost.Platforms;
using Chartboost.Requests;
using Newtonsoft.Json.Utilities;
#if UNITY_ANDROID && !UNITY_EDITOR
using Chartboost.Platforms.Android;
#elif UNITY_IOS && !UNITY_EDITOR
using Chartboost.Platforms.IOS;
#endif
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace Chartboost
{
    /// <summary>
    ///  Provide methods to display and control Chartboost Mediation native advertising types.
    ///  For more information on integrating and using Chartboost Mediation
    ///  please visit our help site documentation at https://help.chartboost.com
    /// </summary>
    public sealed class ChartboostMediation
    {
        
        #pragma warning disable CS0618
        private static readonly ChartboostMediationExternal _chartboostMediationExternal;
        #pragma warning restore CS0618

        static ChartboostMediation() 
        {
            AotHelper.EnsureList<ChartboostMediationAdapterInfo>();
            #pragma warning disable CS0618
            #if UNITY_EDITOR
            _chartboostMediationExternal = new ChartboostMediationUnsupported();
            #elif UNITY_ANDROID
            _chartboostMediationExternal = new ChartboostMediationAndroid();
            #elif UNITY_IPHONE
            _chartboostMediationExternal = new ChartboostMediationIOS();
            #else
            _chartboostMediationExternal = new ChartboostMediationUnsupported();
            #endif
            #pragma warning restore CS0618
        }
        
        ~ChartboostMediation()
        {
            // Shut down the Chartboost Mediation plugin
            #if UNITY_ANDROID
            _chartboostMediationExternal.Destroy();
            #endif
        }

        #region LifeCycle Callbacks
        /// <inheritdoc cref="IChartboostMediationLifeCycle.DidStart"/>>
        public static event ChartboostMediationEvent DidStart
        {
            add => _chartboostMediationExternal.DidStart += value;
            remove => _chartboostMediationExternal.DidStart -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationLifeCycle.DidReceiveImpressionLevelRevenueData"/>>
        public static event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData
        {
            add => _chartboostMediationExternal.DidReceiveImpressionLevelRevenueData += value;
            remove => _chartboostMediationExternal.DidReceiveImpressionLevelRevenueData -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationLifeCycle.DidReceivePartnerInitializationData"/>>
        public static event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData
        {
            add => _chartboostMediationExternal.DidReceivePartnerInitializationData += value;
            remove => _chartboostMediationExternal.DidReceivePartnerInitializationData -= value;
        }

        /// <inheritdoc cref="EventProcessor.UnexpectedSystemErrorDidOccur"/>>
        public static event ChartboostMediationEvent UnexpectedSystemErrorDidOccur
        {
            add => EventProcessor.UnexpectedSystemErrorDidOccur += value;
            remove => EventProcessor.UnexpectedSystemErrorDidOccur -= value;
        }
        #endregion
        
        //////////////////////////////////////////////////////
        // Functions for showing ads
        //////////////////////////////////////////////////////

        public static string Version => "4.9.0";
        
        /// <summary>
        /// Load a fullscreen ad (interstitial, rewarded video, rewarded interstitial).
        /// </summary>
        public static async Task<ChartboostMediationFullscreenAdLoadResult> LoadFullscreenAd(ChartboostMediationFullscreenAdLoadRequest loadRequest) 
            => await _chartboostMediationExternal.LoadFullscreenAd(loadRequest);
        
        /// <summary>
        /// Returns a new ad unit that can be used to load and display banner ads.
        /// </summary>
        public static IChartboostMediationBannerView GetBannerView() => _chartboostMediationExternal.GetBannerView();

        /// <summary>
        /// Returns a new gameobject that can be used to load and display banner ads.
        /// </summary>
        /// <param name="placementName">The placement name for this banner ad</param>
        /// <param name="parent">The parent transform under which this gameobject will be created</param>
        /// <param name="size">size of the gameobject</param>
        /// <param name="screenLocation">pre-defined location on screen where this gameobject will be created</param>
        /// <param name="conformToSafeArea"> If true, this gameobject will be created within the safe area of screen</param>
        /// <returns></returns>
        public static ChartboostMediationUnityBannerAd GetUnityBannerAd(string placementName, Transform parent,
            ChartboostMediationBannerSize? size = null,
            ChartboostMediationBannerAdScreenLocation screenLocation = ChartboostMediationBannerAdScreenLocation.Center,
            bool conformToSafeArea = false)
        {
            var unityBannerAd = ChartboostMediationUnityBannerAd.Instantiate(parent, size, screenLocation, conformToSafeArea);
            unityBannerAd.PlacementName = placementName;
            return unityBannerAd; 
        }
        
        [Obsolete("Init has been deprecated and will be removed in future versions of the SDK.")]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (ChartboostMediationSettings.IsAutomaticInitializationEnabled && !ChartboostMediationExternal.IsInitialized)
                _chartboostMediationExternal.Init();
        }

        [Obsolete("StartWithAppIdAndAppSignature is obsolete and will be removed in future versions, please use StartWithOptions instead.")]
        public static void StartWithAppIdAndAppSignature(string appId, string appSignature)
        {
            if (!ChartboostMediationExternal.IsInitialized)
                _chartboostMediationExternal.InitWithAppIdAndSignature(appId, appSignature);
        }
        
        public static void StartWithOptions(string appId, string[] options = null)
        {
            if (!ChartboostMediationExternal.IsInitialized)
                _chartboostMediationExternal.StartWithOptions(appId, options);
        }

        public static void SetSubjectToCoppa(bool isSubject) => _chartboostMediationExternal.SetSubjectToCoppa(isSubject);
        public static void SetSubjectToGDPR(bool isSubject) => _chartboostMediationExternal.SetSubjectToGDPR(isSubject);
        public static void SetUserHasGivenConsent(bool hasGivenConsent) => _chartboostMediationExternal.SetUserHasGivenConsent(hasGivenConsent);
        // ReSharper disable once IdentifierTypo
        public static void SetCCPAConsent(bool hasGivenConsent) => _chartboostMediationExternal.SetCCPAConsent(hasGivenConsent);
        public static void SetUserIdentifier(string userIdentifier) => _chartboostMediationExternal.SetUserIdentifier(userIdentifier);
        public static string GetUserIdentifier() => _chartboostMediationExternal.GetUserIdentifier();
        public static void SetTestMode(bool testModeEnabled) => _chartboostMediationExternal.SetTestMode(testModeEnabled);
        public static void DiscardOversizedAds(bool shouldDiscard) => _chartboostMediationExternal.DiscardOversizedAds(shouldDiscard);
        
        /// <summary>
        /// Returns an array of all initialized adapters, or an empty array if the SDK is not initialized.
        /// </summary>
        /// <returns></returns>
        public static ChartboostMediationAdapterInfo[] AdaptersInfo => _chartboostMediationExternal.AdaptersInfo;
        
        /// <summary>
        /// 
        /// </summary>
        public static IPartnerConsent PartnerConsents => _chartboostMediationExternal.PartnerConsents;
    }
}

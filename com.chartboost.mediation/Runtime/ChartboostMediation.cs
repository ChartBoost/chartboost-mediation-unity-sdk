using System;
using System.Threading.Tasks;
using Chartboost.AdFormats.Banner;
using Chartboost.AdFormats.Banner.Unity;
using Chartboost.Banner;
using Chartboost.Events;
using Chartboost.FullScreen.Interstitial;
using Chartboost.FullScreen.Rewarded;
using Chartboost.Platforms;
using Chartboost.Requests;
using Chartboost.Results;
#if UNITY_ANDROID
using Chartboost.Platforms.Android;
#elif UNITY_IOS
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
        private static readonly ChartboostMediationExternal _chartboostMediationExternal;

        static ChartboostMediation() 
        {
            #if UNITY_EDITOR
            _chartboostMediationExternal = new ChartboostMediationUnsupported();
            #elif UNITY_ANDROID
            _chartboostMediationExternal = new ChartboostMediationAndroid();
            #elif UNITY_IPHONE
            _chartboostMediationExternal = new ChartboostMediationIOS();
            #else
            _chartboostMediationExternal = new ChartboostMediationUnsupported();
            #endif
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

        #region Interstitial Callbacks
        /// <inheritdoc cref="IChartboostMediationInterstitialEvents.DidLoadInterstitial"/>>
        public static event ChartboostMediationPlacementLoadEvent DidLoadInterstitial
        {
            add => _chartboostMediationExternal.DidLoadInterstitial += value;
            remove => _chartboostMediationExternal.DidLoadInterstitial -= value;
        }

        /// <inheritdoc cref="IChartboostMediationInterstitialEvents.DidShowInterstitial"/>>
        public static event ChartboostMediationPlacementEvent DidShowInterstitial
        {
            add => _chartboostMediationExternal.DidShowInterstitial += value;
            remove => _chartboostMediationExternal.DidShowInterstitial -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationInterstitialEvents.DidCloseInterstitial"/>>
        public static event ChartboostMediationPlacementEvent DidCloseInterstitial
        {
            add => _chartboostMediationExternal.DidCloseInterstitial += value;
            remove => _chartboostMediationExternal.DidCloseInterstitial -= value;
        }

        /// <inheritdoc cref="IChartboostMediationInterstitialEvents.DidClickInterstitial"/>>
        public static event ChartboostMediationPlacementEvent DidClickInterstitial
        {
            add => _chartboostMediationExternal.DidClickInterstitial += value;
            remove => _chartboostMediationExternal.DidClickInterstitial -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationInterstitialEvents.DidRecordImpressionInterstitial"/>>
        public static event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial
        {
            add => _chartboostMediationExternal.DidRecordImpressionInterstitial += value;
            remove => _chartboostMediationExternal.DidRecordImpressionInterstitial -= value;
        }
        #endregion

        #region Rewarded Callbacks
        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidLoadRewarded"/>>
        public static event ChartboostMediationPlacementLoadEvent DidLoadRewarded
        {
            add => _chartboostMediationExternal.DidLoadRewarded += value;
            remove => _chartboostMediationExternal.DidLoadRewarded -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidShowRewarded"/>>
        public static event ChartboostMediationPlacementEvent DidShowRewarded
        {
            add => _chartboostMediationExternal.DidShowRewarded += value;
            remove => _chartboostMediationExternal.DidShowRewarded -= value;
        }

        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidCloseRewarded"/>>
        public static event ChartboostMediationPlacementEvent DidCloseRewarded
        {
            add => _chartboostMediationExternal.DidCloseRewarded += value;
            remove => _chartboostMediationExternal.DidCloseRewarded -= value;
        }

        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidClickRewarded"/>>
        public static event ChartboostMediationPlacementEvent DidClickRewarded
        {
            add => _chartboostMediationExternal.DidClickRewarded += value;
            remove => _chartboostMediationExternal.DidClickRewarded -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidRecordImpressionRewarded"/>>
        public static event ChartboostMediationPlacementEvent DidRecordImpressionRewarded
        {
            add => _chartboostMediationExternal.DidRecordImpressionRewarded += value;
            remove => _chartboostMediationExternal.DidRecordImpressionRewarded -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidReceiveReward"/>>
        public static event ChartboostMediationPlacementEvent DidReceiveReward
        {
            add => _chartboostMediationExternal.DidReceiveReward += value;
            remove => _chartboostMediationExternal.DidReceiveReward -= value;
        }
        #endregion

        #region Banner Callbacks
        /// <inheritdoc cref="IChartboostMediationBannerEvents.DidLoadBanner"/>>
        public static event ChartboostMediationPlacementLoadEvent DidLoadBanner
        {
            add => _chartboostMediationExternal.DidLoadBanner += value;
            remove => _chartboostMediationExternal.DidLoadBanner -= value;
        }

        /// <inheritdoc cref="IChartboostMediationBannerEvents.DidClickBanner"/>>
        public static event ChartboostMediationPlacementEvent DidClickBanner
        {
            add => _chartboostMediationExternal.DidClickBanner += value;
            remove => _chartboostMediationExternal.DidClickBanner -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationBannerEvents.DidRecordImpressionBanner"/>>
        public static event ChartboostMediationPlacementEvent DidRecordImpressionBanner
        {
            add => _chartboostMediationExternal.DidRecordImpressionBanner += value;
            remove => _chartboostMediationExternal.DidRecordImpressionBanner -= value;
        }
        #endregion

        //////////////////////////////////////////////////////
        // Functions for showing ads
        //////////////////////////////////////////////////////

        /// <summary>
        /// Load a fullscreen ad (interstitial, rewarded video, rewarded interstitial).
        /// </summary>
        public static async Task<ChartboostMediationFullscreenAdLoadResult> LoadFullscreenAd(ChartboostMediationFullscreenAdLoadRequest loadRequest) 
            => await _chartboostMediationExternal.LoadFullscreenAd(loadRequest);

        /// <summary>
        /// Returns a new ad unit that can be used to load and display interstitial ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the Chartboost Mediation impression type.</param>
        [Obsolete("GetInterstitialAd has been deprecated and will be removed in future versions, use LoadFullscreenAd instead.")]
        public static ChartboostMediationInterstitialAd GetInterstitialAd(string placementName) 
            => _chartboostMediationExternal.GetInterstitialAd(placementName);

        /// <summary>
        /// Returns a new ad unit that can be used to load and display rewarded video ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the Chartboost Mediation impression type.</param>
        [Obsolete("GetRewardedAd has been deprecated and will be removed in future versions, use LoadFullscreenAd instead.")]
        public static ChartboostMediationRewardedAd GetRewardedAd(string placementName)
            => _chartboostMediationExternal.GetRewardedAd(placementName);

        /// <summary>
        /// Returns a new ad unit that can be used to load and display banner ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the Chartboost Mediation impression type.</param>
        /// <param name="size">The banner size</param>
        [Obsolete("GetBannerAd has been deprecated and will be removed in future versions, use GetBannerView instead.")]
        public static ChartboostMediationBannerAd GetBannerAd(string placementName, ChartboostMediationBannerAdSize size)
            => _chartboostMediationExternal.GetBannerAd(placementName, size);
        
        /// <summary>
        /// Returns a new ad unit that can be used to load and display banner ads.
        /// </summary>
        public static IChartboostMediationBannerView GetBannerView() => _chartboostMediationExternal.GetBannerView();

        /// <summary>
        /// Returns a new gameobject that can be used to load and display banner ads.
        /// </summary>
        /// <param name="placementName">The placement name for this banner ad</param>
        /// <param name="canvas">Canvas under which this gameobject will be created</param>
        /// <param name="size">size of the gameobject</param>
        /// <param name="screenLocation">pre-defined location on screen where this gameobject will be created</param>
        /// <returns></returns>
        public static ChartboostMediationUnityBannerAd GetUnityBannerAd(string placementName, Canvas canvas, ChartboostMediationBannerAdSize? size = null, ChartboostMediationBannerAdScreenLocation screenLocation = ChartboostMediationBannerAdScreenLocation.Center)
        {
            var unityBannerAd = ChartboostMediationUnityBannerAd.Instantiate(canvas, size, screenLocation);
            unityBannerAd.PlacementName = placementName;
            return unityBannerAd;
        }

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
        
        public static void StartWithOptions(string appId, string appSignature, string[] options = null)
        {
            if (!ChartboostMediationExternal.IsInitialized)
                _chartboostMediationExternal.StartWithOptions(appId, appSignature, options);
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
    }
}

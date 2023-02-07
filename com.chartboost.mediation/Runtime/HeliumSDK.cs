using System;
using Chartboost;
using Chartboost.Banner;
using Chartboost.FullScreen.Interstitial;
using Chartboost.FullScreen.Rewarded;
using Chartboost.Platforms;
using UnityEngine;

// ReSharper disable InconsistentNaming
namespace Helium
{
    /// <summary>
    ///  Provide methods to display and control Chartboost Mediation native advertising types.
    ///  For more information on integrating and using the Chartboost Mediation
    ///  please visit our help site documentation at https://help.chartboost.com
    /// </summary>
    [Obsolete("HeliumSDK has been replaced by ChartboostMediation, and will be removed in future releases.")]
    public class HeliumSDK
    {
        #region LifeCycle Callbacks
        /// <inheritdoc cref="IChartboostMediationLifeCycle.DidStart"/>>
        public static event ChartboostMediationEvent DidStart
        {
            add => ChartboostMediation._chartboostMediationExternal.DidStart += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidStart -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationLifeCycle.DidReceiveImpressionLevelRevenueData"/>>
        public static event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData
        {
            add => ChartboostMediation._chartboostMediationExternal.DidReceiveImpressionLevelRevenueData += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidReceiveImpressionLevelRevenueData -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationLifeCycle.DidReceivePartnerInitializationData"/>>
        public static event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData
        {
            add => ChartboostMediation._chartboostMediationExternal.DidReceivePartnerInitializationData += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidReceivePartnerInitializationData -= value;
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
            add => ChartboostMediation._chartboostMediationExternal.DidLoadInterstitial += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidLoadInterstitial -= value;
        }

        /// <inheritdoc cref="IChartboostMediationInterstitialEvents.DidShowInterstitial"/>>
        public static event ChartboostMediationPlacementEvent DidShowInterstitial
        {
            add => ChartboostMediation._chartboostMediationExternal.DidShowInterstitial += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidShowInterstitial -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationInterstitialEvents.DidCloseInterstitial"/>>
        public static event ChartboostMediationPlacementEvent DidCloseInterstitial
        {
            add => ChartboostMediation._chartboostMediationExternal.DidCloseInterstitial += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidCloseInterstitial -= value;
        }

        /// <inheritdoc cref="IChartboostMediationInterstitialEvents.DidClickInterstitial"/>>
        public static event ChartboostMediationPlacementEvent DidClickInterstitial
        {
            add => ChartboostMediation._chartboostMediationExternal.DidClickInterstitial += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidClickInterstitial -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationInterstitialEvents.DidRecordImpressionInterstitial"/>>
        public static event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial
        {
            add => ChartboostMediation._chartboostMediationExternal.DidRecordImpressionInterstitial += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidRecordImpressionInterstitial -= value;
        }
        #endregion

        #region Rewarded Callbacks
        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidLoadRewarded"/>>
        public static event ChartboostMediationPlacementLoadEvent DidLoadRewarded
        {
            add => ChartboostMediation._chartboostMediationExternal.DidLoadRewarded += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidLoadRewarded -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidShowRewarded"/>>
        public static event ChartboostMediationPlacementEvent DidShowRewarded
        {
            add => ChartboostMediation._chartboostMediationExternal.DidShowRewarded += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidShowRewarded -= value;
        }

        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidCloseRewarded"/>>
        public static event ChartboostMediationPlacementEvent DidCloseRewarded
        {
            add => ChartboostMediation._chartboostMediationExternal.DidCloseRewarded += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidCloseRewarded -= value;
        }

        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidClickRewarded"/>>
        public static event ChartboostMediationPlacementEvent DidClickRewarded
        {
            add => ChartboostMediation._chartboostMediationExternal.DidClickRewarded += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidClickRewarded -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidRecordImpressionRewarded"/>>
        public static event ChartboostMediationPlacementEvent DidRecordImpressionRewarded
        {
            add => ChartboostMediation._chartboostMediationExternal.DidRecordImpressionRewarded += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidRecordImpressionRewarded -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationRewardedEvents.DidReceiveReward"/>>
        public static event ChartboostMediationPlacementEvent DidReceiveReward
        {
            add => ChartboostMediation._chartboostMediationExternal.DidReceiveReward += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidReceiveReward -= value;
        }
        #endregion

        #region Banner Callbacks
        /// <inheritdoc cref="IChartboostMediationBannerEvents.DidLoadBanner"/>>
        public static event ChartboostMediationPlacementLoadEvent DidLoadBanner
        {
            add => ChartboostMediation._chartboostMediationExternal.DidLoadBanner += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidLoadBanner -= value;
        }

        /// <inheritdoc cref="IChartboostMediationBannerEvents.DidClickBanner"/>>
        public static event ChartboostMediationPlacementEvent DidClickBanner
        {
            add => ChartboostMediation._chartboostMediationExternal.DidClickBanner += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidClickBanner -= value;
        }
        
        /// <inheritdoc cref="IChartboostMediationBannerEvents.DidRecordImpressionBanner"/>>
        public static event ChartboostMediationPlacementEvent DidRecordImpressionBanner
        {
            add => ChartboostMediation._chartboostMediationExternal.DidRecordImpressionBanner += value;
            remove => ChartboostMediation._chartboostMediationExternal.DidRecordImpressionBanner -= value;
        }
        #endregion

        //////////////////////////////////////////////////////
        // Functions for showing ads
        //////////////////////////////////////////////////////

        /// <summary>
        /// Returns a new ad unit that can be used to load and display interstitial ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the Chartboost Mediation impression type.</param>
        public static ChartboostMediationInterstitialAd GetInterstitialAd(string placementName)
        {
            return ChartboostMediation._chartboostMediationExternal.GetInterstitialAd(placementName);
        }

        /// <summary>
        /// Returns a new ad unit that can be used to load and display rewarded video ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the Chartboost Mediation impression type.</param>
        public static ChartboostMediationRewardedAd GetRewardedAd(string placementName)
        {
            return ChartboostMediation._chartboostMediationExternal.GetRewardedAd(placementName);
        }

        /// <summary>
        /// Returns a new ad unit that can be used to load and display banner ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the Chartboost Mediation impression type.</param>
        /// <param name="size">The banner size</param>
        public static ChartboostMediationBannerAd GetBannerAd(string placementName, ChartboostMediationBannerAdSize size)
        {
            return ChartboostMediation._chartboostMediationExternal.GetBannerAd(placementName, size);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (ChartboostMediationSettings.IsAutomaticInitializationEnabled && !ChartboostMediationExternal.IsInitialized)
                ChartboostMediation._chartboostMediationExternal.Init();
        }

        public static void StartWithAppIdAndAppSignature(string appId, string appSignature)
        {
            if (!ChartboostMediationExternal.IsInitialized)
                ChartboostMediation._chartboostMediationExternal.InitWithAppIdAndSignature(appId, appSignature);
        }

        public static void SetSubjectToCoppa(bool isSubject) => ChartboostMediation._chartboostMediationExternal.SetSubjectToCoppa(isSubject);
        public static void SetSubjectToGDPR(bool isSubject) => ChartboostMediation._chartboostMediationExternal.SetSubjectToGDPR(isSubject);
        public static void SetUserHasGivenConsent(bool hasGivenConsent) => ChartboostMediation._chartboostMediationExternal.SetUserHasGivenConsent(hasGivenConsent);
        // ReSharper disable once IdentifierTypo
        public static void SetCCPAConsent(bool hasGivenConsent) => ChartboostMediation._chartboostMediationExternal.SetCCPAConsent(hasGivenConsent);
        public static void SetUserIdentifier(string userIdentifier) => ChartboostMediation._chartboostMediationExternal.SetUserIdentifier(userIdentifier);
        public static string GetUserIdentifier() => ChartboostMediation._chartboostMediationExternal.GetUserIdentifier();
    }
}

using Helium.Banner;
using UnityEngine;
using Helium.Interfaces;
using Helium.Platforms;

// ReSharper disable InconsistentNaming
namespace Helium
{
    /// <summary>
    ///  Provide methods to display and control HeliumSDK native advertising types.
    ///  For more information on integrating and using the HeliumSDK
    ///  please visit our help site documentation at https://help.chartboost.com
    /// </summary>
    public class HeliumSDK
    {
        private static readonly HeliumExternal _heliumExternal;

        static HeliumSDK() 
        {
            #if UNITY_EDITOR
            _heliumExternal = new HeliumUnsupported();
            #elif UNITY_ANDROID
            _heliumExternal = new HeliumAndroid();
            #elif UNITY_IPHONE
            _heliumExternal = new HeliumIOS();
            #else
            _heliumExternal = new HeliumUnsupported();
            #endif
        }
        
        ~HeliumSDK()
        {
            // Shut down the HeliumSdk plugin
            #if UNITY_ANDROID
            _heliumExternal.Destroy();
            #endif
        }

        #region LifeCycle Callbacks
        /// <inheritdoc cref="IHeliumLifeCycle.DidStart"/>>
        public static event HeliumEvent DidStart
        {
            add => _heliumExternal.DidStart += value;
            remove => _heliumExternal.DidStart -= value;
        }
        
        /// <inheritdoc cref="IHeliumLifeCycle.DidReceiveImpressionLevelRevenueData"/>>
        public static event HeliumILRDEvent DidReceiveImpressionLevelRevenueData
        {
            add => _heliumExternal.DidReceiveImpressionLevelRevenueData += value;
            remove => _heliumExternal.DidReceiveImpressionLevelRevenueData -= value;
        }
        
        /// <inheritdoc cref="IHeliumLifeCycle.DidReceivePartnerInitializationData"/>>
        public static event HeliumPartnerInitializationEvent DidReceivePartnerInitializationData
        {
            add => _heliumExternal.DidReceivePartnerInitializationData += value;
            remove => _heliumExternal.DidReceivePartnerInitializationData -= value;
        }

        /// <inheritdoc cref="HeliumEventProcessor.UnexpectedSystemErrorDidOccur"/>>
        public static event HeliumEvent UnexpectedSystemErrorDidOccur
        {
            add => HeliumEventProcessor.UnexpectedSystemErrorDidOccur += value;
            remove => HeliumEventProcessor.UnexpectedSystemErrorDidOccur -= value;
        }
        #endregion

        #region Interstitial Callbacks
        /// <inheritdoc cref="IInterstitialEvents.DidLoadInterstitial"/>>
        public static event HeliumPlacementEvent DidLoadInterstitial
        {
            add => _heliumExternal.DidLoadInterstitial += value;
            remove => _heliumExternal.DidLoadInterstitial -= value;
        }

        /// <inheritdoc cref="IInterstitialEvents.DidShowInterstitial"/>>
        public static event HeliumPlacementEvent DidShowInterstitial
        {
            add => _heliumExternal.DidShowInterstitial += value;
            remove => _heliumExternal.DidShowInterstitial -= value;
        }
        
        /// <inheritdoc cref="IInterstitialEvents.DidCloseInterstitial"/>>
        public static event HeliumPlacementEvent DidCloseInterstitial
        {
            add => _heliumExternal.DidCloseInterstitial += value;
            remove => _heliumExternal.DidCloseInterstitial -= value;
        }

        /// <inheritdoc cref="IInterstitialEvents.DidClickInterstitial"/>>
        public static event HeliumPlacementEvent DidClickInterstitial
        {
            add => _heliumExternal.DidClickInterstitial += value;
            remove => _heliumExternal.DidClickInterstitial -= value;
        }
        
        /// <inheritdoc cref="IInterstitialEvents.DidRecordImpressionInterstitial"/>>
        public static event HeliumPlacementEvent DidRecordImpressionInterstitial
        {
            add => _heliumExternal.DidRecordImpressionInterstitial += value;
            remove => _heliumExternal.DidRecordImpressionInterstitial -= value;
        }
        
        /// <inheritdoc cref="IInterstitialEvents.DidWinBidInterstitial"/>>
        public static event HeliumBidEvent DidWinBidInterstitial
        {
            add => _heliumExternal.DidWinBidInterstitial += value;
            remove => _heliumExternal.DidWinBidInterstitial -= value;
        }
        #endregion

        #region Rewarded Callbacks
        /// <inheritdoc cref="IRewardedEvents.DidLoadRewarded"/>>
        public static event HeliumPlacementEvent DidLoadRewarded
        {
            add => _heliumExternal.DidLoadRewarded += value;
            remove => _heliumExternal.DidLoadRewarded -= value;
        }
        
        /// <inheritdoc cref="IRewardedEvents.DidShowRewarded"/>>
        public static event HeliumPlacementEvent DidShowRewarded
        {
            add => _heliumExternal.DidShowRewarded += value;
            remove => _heliumExternal.DidShowRewarded -= value;
        }

        /// <inheritdoc cref="IRewardedEvents.DidCloseRewarded"/>>
        public static event HeliumPlacementEvent DidCloseRewarded
        {
            add => _heliumExternal.DidCloseRewarded += value;
            remove => _heliumExternal.DidCloseRewarded -= value;
        }

        /// <inheritdoc cref="IRewardedEvents.DidClickRewarded"/>>
        public static event HeliumPlacementEvent DidClickRewarded
        {
            add => _heliumExternal.DidClickRewarded += value;
            remove => _heliumExternal.DidClickRewarded -= value;
        }
        
        /// <inheritdoc cref="IRewardedEvents.DidRecordImpressionRewarded"/>>
        public static event HeliumPlacementEvent DidRecordImpressionRewarded
        {
            add => _heliumExternal.DidRecordImpressionRewarded += value;
            remove => _heliumExternal.DidRecordImpressionRewarded -= value;
        }
        
        /// <inheritdoc cref="IRewardedEvents.DidWinBidRewarded"/>>
        public static event HeliumBidEvent DidWinBidRewarded
        {
            add => _heliumExternal.DidWinBidRewarded += value;
            remove => _heliumExternal.DidWinBidRewarded -= value;
        }
        
        /// <inheritdoc cref="IRewardedEvents.DidReceiveReward"/>>
        public static event HeliumRewardEvent DidReceiveReward
        {
            add => _heliumExternal.DidReceiveReward += value;
            remove => _heliumExternal.DidReceiveReward -= value;
        }
        #endregion

        #region Banner Callbacks
        /// <inheritdoc cref="IBannerEvents.DidLoadBanner"/>>
        public static event HeliumPlacementEvent DidLoadBanner
        {
            add => _heliumExternal.DidLoadBanner += value;
            remove => _heliumExternal.DidLoadBanner -= value;
        }

        /// <inheritdoc cref="IBannerEvents.DidClickBanner"/>>
        public static event HeliumPlacementEvent DidClickBanner
        {
            add => _heliumExternal.DidClickBanner += value;
            remove => _heliumExternal.DidClickBanner -= value;
        }
        
        /// <inheritdoc cref="IBannerEvents.DidRecordImpressionBanner"/>>
        public static event HeliumPlacementEvent DidRecordImpressionBanner
        {
            add => _heliumExternal.DidRecordImpressionBanner += value;
            remove => _heliumExternal.DidRecordImpressionBanner -= value;
        }

        /// <inheritdoc cref="IBannerEvents.DidWinBidBanner"/>>
        public static event HeliumBidEvent DidWinBidBanner
        {
            add => _heliumExternal.DidWinBidBanner += value;
            remove => _heliumExternal.DidWinBidBanner -= value;
        }
        #endregion

        //////////////////////////////////////////////////////
        // Functions for showing ads
        //////////////////////////////////////////////////////

        /// <summary>
        /// Returns a new ad unit that can be used to load and display interstitial ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the HeliumSdk impression type.</param>
        public static HeliumInterstitialAd GetInterstitialAd(string placementName)
        {
            return _heliumExternal.GetInterstitialAd(placementName);
        }

        /// <summary>
        /// Returns a new ad unit that can be used to load and display rewarded video ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the HeliumSdk impression type.</param>
        public static HeliumRewardedAd GetRewardedAd(string placementName)
        {
            return _heliumExternal.GetRewardedAd(placementName);
        }

        /// <summary>
        /// Returns a new ad unit that can be used to load and display banner ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the HeliumSdk impression type.</param>
        /// <param name="size">The banner size</param>
        public static HeliumBannerAd GetBannerAd(string placementName, HeliumBannerAdSize size)
        {
            return _heliumExternal.GetBannerAd(placementName, size);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (HeliumSettings.IsAutomaticInitializationEnabled && !HeliumExternal.IsInitialized)
                _heliumExternal.Init();
        }

        public static void StartWithAppIdAndAppSignature(string appId, string appSignature)
        {
            if (!HeliumExternal.IsInitialized)
                _heliumExternal.InitWithAppIdAndSignature(appId, appSignature);
        }

        public static void SetSubjectToCoppa(bool isSubject)
        {
            _heliumExternal.SetSubjectToCoppa(isSubject);
        }

        public static void SetSubjectToGDPR(bool isSubject)
        {
            _heliumExternal.SetSubjectToGDPR(isSubject);
        }

        public static void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            _heliumExternal.SetUserHasGivenConsent(hasGivenConsent);
        }

        // ReSharper disable once IdentifierTypo
        public static void SetCCPAConsent(bool hasGivenConsent)
        {
            _heliumExternal.SetCCPAConsent(hasGivenConsent);
        }

        public static void SetUserIdentifier(string userIdentifier)
        {
            _heliumExternal.SetUserIdentifier(userIdentifier);
        }

        public static string GetUserIdentifier()
        {
            return _heliumExternal.GetUserIdentifier();
        }
    }
}

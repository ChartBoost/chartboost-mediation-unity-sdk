using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Chartboost.AdFormats.Banner;
using Chartboost.Banner;
using Chartboost.FullScreen.Interstitial;
using Chartboost.FullScreen.Rewarded;
using Chartboost.Requests;
using Chartboost.Results;
using Newtonsoft.Json;
using UnityEngine;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.Platforms
{
    #pragma warning disable CS0618
    internal abstract class ChartboostMediationExternal : IChartboostMediationLifeCycle, IChartboostMediationInterstitialEvents, IChartboostMediationRewardedEvents, IChartboostMediationBannerEvents
    #pragma warning restore CS0618
    {
        public static bool IsInitialized { get; protected set; }
        
        protected static string LogTag = "ChartboostMediation (External)";

        protected static bool CanFetchAd(string placementName)
        {
            if (!CheckInitialized())
                return false;
            if (!string.IsNullOrEmpty(placementName)) 
                return true;
            Logger.LogError(LogTag, "placementName passed is null cannot perform the operation requested");
            return false;
        }

        protected static bool CheckInitialized()
        {
            if (IsInitialized)
                return true;

            Logger.LogError(LogTag, "The Chartboost Mediation SDK needs to be initialized before we can show any ads");
            return false;
        }
        
        /// Initializes the Chartboost Mediation plugin.
        /// This must be called before using any other Chartboost Mediation features.
        public virtual void Init() 
            => Logger.Log(LogTag, "Init - Attempting to Initialize Chartboost Mediation SDK from ChartboostMediationSettings.");

        /// Initialize the Chartboost Mediation plugin with a specific appId
        /// Either one of the init() methods must be called before using any other Chartboost Mediation feature
        [Obsolete("InitWithAppIdAndSignature has been deprecated, please use StartWithOptions instead")]
        public virtual void InitWithAppIdAndSignature(string appId, string appSignature) 
            => Logger.Log(LogTag, $"InitWithAppIdAndSignature {appId}, {appSignature} and version {Application.unityVersion}");

        public virtual void StartWithOptions(string appId, string appSignature, string[] initializationOptions = null) 
            => Logger.Log(LogTag, $"StartWithOptions {appId}, {appSignature}, options {JsonConvert.SerializeObject(initializationOptions)} and version {Application.unityVersion}");

        public virtual void SetSubjectToCoppa(bool isSubject) 
            => Logger.Log(LogTag, $"SetSubjectToCoppa {isSubject}");

        // ReSharper disable once InconsistentNaming
        public virtual void SetSubjectToGDPR(bool isSubject) 
            => Logger.Log(LogTag, $"SetSubjectToGDPR {isSubject}");

        public virtual void SetUserHasGivenConsent(bool hasGivenConsent) 
            => Logger.Log(LogTag, $"SetUserHasGivenConsent {hasGivenConsent}");

        // ReSharper disable once InconsistentNaming
        public virtual void SetCCPAConsent(bool hasGivenConsent) 
            => Logger.Log(LogTag, $"SetCCPAConsent {hasGivenConsent}");

        public virtual void SetUserIdentifier(string userIdentifier) 
            => Logger.Log(LogTag, $"SetUserIdentifier {userIdentifier}");

        public virtual string GetUserIdentifier()
        {
            Logger.Log(LogTag, "GetUserIdentifier");
            return string.Empty;
        }
        
        public virtual void SetTestMode(bool testModeEnabled) 
            => Logger.Log(LogTag, $"SetTestMode {testModeEnabled}");

        public virtual void DiscardOversizedAds(bool shouldDiscard)
            => Logger.Log(LogTag, $"DiscardOversizedAds : {shouldDiscard}");
        
        public virtual void Destroy() 
            => Logger.Log(LogTag, "Destroy");

        public abstract Task<ChartboostMediationFullscreenAdLoadResult> LoadFullscreenAd(ChartboostMediationFullscreenAdLoadRequest request);

        public abstract IChartboostMediationBannerView GetBannerView();
      
        [Obsolete("GetInterstitialAd has been deprecated and will be removed in future versions, use LoadFullscreenAd instead.")]
        public ChartboostMediationInterstitialAd GetInterstitialAd(string placementName)
        {
            Logger.Log(LogTag, $"GetInterstitialAd at placement: {placementName}");
            if (!CanFetchAd(placementName))
                return null;
            try
            {
                return new ChartboostMediationInterstitialAd(placementName);
            }
            catch (Exception e)
            {
                Logger.LogError(LogTag, $"interstitial failed to be obtained {e}");
                return null;
            }
        }
        
        [Obsolete("GetRewardedAd has been deprecated and will be removed in future versions, use LoadFullscreenAd instead.")]
        public ChartboostMediationRewardedAd GetRewardedAd(string placementName)
        {
            Logger.Log(LogTag, $"GetRewardedAd at placement: {placementName}");
            if (!CanFetchAd(placementName))
                return null;
            try
            {
                return new ChartboostMediationRewardedAd(placementName);
            }
            catch (Exception e)
            {
                Logger.LogError(LogTag, $"rewarded ad failed to be obtained {e}");
                return null;
            }
        }
        
        public ChartboostMediationBannerAd GetBannerAd(string placementName, ChartboostMediationBannerAdSize size)
        {
            Logger.Log(LogTag, $"GetBannerAd at placement: {placementName}");
            if (!CanFetchAd(placementName))
                return null;
            try
            {
                return new ChartboostMediationBannerAd(placementName, size);
            }
            catch (Exception e)
            {
                Logger.LogError(LogTag, $"banner ad failed to be obtained {e}");
                return null;
            }
        }
        
        [Obsolete]
        protected static string[] GetInitializationOptions()
        {
            string GetEnumDescription(Enum value)
            {
                var fi = value.GetType().GetField(value.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return attributes.Length > 0 ? attributes[0].Description : value.ToString();
            }
            
            var killSwitch = ChartboostMediationSettings.PartnerKillSwitch;
            var initOptions  = Array.Empty<string>();

            if (killSwitch == ChartboostMediationPartners.None)
                return initOptions;
            
            var selectedPartners  = new HashSet<ChartboostMediationPartners>();
            foreach (ChartboostMediationPartners value in Enum.GetValues(killSwitch.GetType()))
                if (value != ChartboostMediationPartners.None && killSwitch.HasFlag(value))
                    selectedPartners.Add(value);

            var partnerIds = new HashSet<string>();
            
            foreach (var name in selectedPartners.Select(value => GetEnumDescription(value)))
                partnerIds.Add(name);

            initOptions = partnerIds.ToArray();

            return initOptions;
        }

#pragma warning disable 67
        // Life-cycle
        public virtual event ChartboostMediationEvent DidStart;
        public virtual event ChartboostMediationILRDEvent DidReceiveImpressionLevelRevenueData;
        public virtual event ChartboostMediationPartnerInitializationEvent DidReceivePartnerInitializationData;
        
        // Interstitials
        public virtual event ChartboostMediationPlacementLoadEvent DidLoadInterstitial;
        public virtual event ChartboostMediationPlacementEvent DidShowInterstitial;
        public virtual event ChartboostMediationPlacementEvent DidCloseInterstitial;
        public virtual event ChartboostMediationPlacementEvent DidClickInterstitial;
        public virtual event ChartboostMediationPlacementEvent DidRecordImpressionInterstitial;

        // Rewarded Videos
        public virtual event ChartboostMediationPlacementLoadEvent DidLoadRewarded;
        public virtual event ChartboostMediationPlacementEvent DidShowRewarded;
        public virtual event ChartboostMediationPlacementEvent DidCloseRewarded;
        public virtual event ChartboostMediationPlacementEvent DidClickRewarded;
        public virtual event ChartboostMediationPlacementEvent DidRecordImpressionRewarded;
        public virtual event ChartboostMediationPlacementEvent DidReceiveReward;

        // Banners
        public virtual event ChartboostMediationPlacementLoadEvent DidLoadBanner;
        public virtual event ChartboostMediationPlacementEvent DidClickBanner;
        public virtual event ChartboostMediationPlacementEvent DidRecordImpressionBanner;
#pragma warning restore 67
    }
}

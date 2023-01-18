using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Helium.Banner;
using Helium.FullScreen.Interstitial;
using Helium.FullScreen.Rewarded;
using Helium.Interfaces;
using UnityEngine;

namespace Helium.Platforms
{
    public abstract class HeliumExternal : IHeliumLifeCycle, IInterstitialEvents, IRewardedEvents, IBannerEvents
    {
        public static bool IsInitialized { get; protected set; }
        
        protected static string LogTag = "HeliumSDK";

        protected static bool CanFetchAd(string placementName)
        {
            if (!CheckInitialized())
                return false;
            if (placementName != null) 
                return true;
            HeliumLogger.LogError(LogTag, "placementName passed is null cannot perform the operation requested");
            return false;
        }

        protected static bool CheckInitialized()
        {
            if (IsInitialized)
                return true;

            HeliumLogger.LogError(LogTag, "The Helium SDK needs to be initialized before we can show any ads");
            return false;
        }
        
        /// Initializes the Helium plugin.
        /// This must be called before using any other Helium features.
        public virtual void Init()
        {
            HeliumLogger.Log(LogTag, "Init - Attempting to Initialize Helium SDK from HeliumSettings.");
        }

        /// Initialize the Helium plugin with a specific appId
        /// Either one of the init() methods must be called before using any other Helium feature
        public virtual void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            HeliumLogger.Log(LogTag, $"InitWithAppIdAndSignature {appId}, {appSignature} and version {Application.unityVersion}");
            HeliumEventProcessor.Initialize();
        }
        
        public virtual void SetSubjectToCoppa(bool isSubject)
        {
            HeliumLogger.Log(LogTag, $"SetSubjectToCoppa {isSubject}");
        }
        
        // ReSharper disable once InconsistentNaming
        public virtual void SetSubjectToGDPR(bool isSubject)
        {
            HeliumLogger.Log(LogTag, $"SetSubjectToGDPR {isSubject}");
        }

        public virtual void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            HeliumLogger.Log(LogTag, $"SetUserHasGivenConsent {hasGivenConsent}");
        }

        // ReSharper disable once InconsistentNaming
        public virtual void SetCCPAConsent(bool hasGivenConsent)
        {
            HeliumLogger.Log(LogTag, $"SetCCPAConsent {hasGivenConsent}");
        }

        public virtual void SetUserIdentifier(string userIdentifier)
        {
            HeliumLogger.Log(LogTag, $"SetUserIdentifier {userIdentifier}");
        }

        public virtual string GetUserIdentifier()
        {
            HeliumLogger.Log(LogTag, "GetUserIdentifier");
            return string.Empty;
        }

        public virtual void Destroy()
        {
            HeliumLogger.Log(LogTag, "Destroy");
        }

        public virtual void Pause(bool paused)
        {
            HeliumLogger.Log(LogTag, "pause");
        }

        public virtual bool OnBackPressed()
        {
            HeliumLogger.Log(LogTag, "OnBackPressed");
            return CheckInitialized();
        }

        public HeliumInterstitialAd GetInterstitialAd(string placementName)
        {
            HeliumLogger.Log(LogTag, $"GetInterstitialAd at placement: {placementName}");
            if (!CanFetchAd(placementName))
                return null;
            try
            {
                return new HeliumInterstitialAd(placementName);
            }
            catch (Exception e)
            {
                HeliumLogger.LogError(LogTag, $"interstitial failed to be obtained {e}");
                return null;
            }
        }
        
        public HeliumRewardedAd GetRewardedAd(string placementName)
        {
            HeliumLogger.Log(LogTag, $"GetRewardedAd at placement: {placementName}");
            if (!CanFetchAd(placementName))
                return null;
            try
            {
                return new HeliumRewardedAd(placementName);
            }
            catch (Exception e)
            {
                HeliumLogger.LogError(LogTag, $"rewarded ad failed to be obtained {e}");
                return null;
            }
        }
        
        public HeliumBannerAd GetBannerAd(string placementName, HeliumBannerAdSize size)
        {
            HeliumLogger.Log(LogTag, $"GetBannerAd at placement: {placementName}");
            if (!CanFetchAd(placementName))
                return null;
            try
            {
                return new HeliumBannerAd(placementName, size);
            }
            catch (Exception e)
            {
                HeliumLogger.LogError(LogTag, $"banner ad failed to be obtained {e}");
                return null;
            }
        }
        
        protected static string[] GetInitializationOptions()
        {
            string GetEnumDescription(Enum value)
            {
                var fi = value.GetType().GetField(value.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return attributes.Length > 0 ? attributes[0].Description : value.ToString();
            }
            
            var killSwitch = HeliumSettings.PartnerKillSwitch;
            var initOptions  = Array.Empty<string>();

            if (killSwitch == HeliumPartners.None)
                return initOptions;
            
            var selectedPartners  = new HashSet<HeliumPartners>();
            foreach (HeliumPartners value in Enum.GetValues(killSwitch.GetType()))
                if (value != HeliumPartners.None && killSwitch.HasFlag(value))
                    selectedPartners.Add(value);

            var partnerIds = new HashSet<string>();
            
            foreach (var name in selectedPartners.Select(value => GetEnumDescription(value)))
                partnerIds.Add(name);

            initOptions = partnerIds.ToArray();

            return initOptions;
        }

#pragma warning disable 67
        // Interstitials
        public virtual event HeliumEvent DidStart;
        public virtual event HeliumILRDEvent DidReceiveImpressionLevelRevenueData;
        public virtual event HeliumPartnerInitializationEvent DidReceivePartnerInitializationData;
        public virtual event HeliumPlacementLoadEvent DidLoadInterstitial;
        public virtual event HeliumPlacementEventWithError DidShowInterstitial;
        public virtual event HeliumPlacementEventWithError DidCloseInterstitial;
        public virtual event HeliumPlacementEvent DidClickInterstitial;
        public virtual event HeliumPlacementEvent DidRecordImpressionInterstitial;
        
        // Rewarded Videos
        public virtual event HeliumPlacementLoadEvent DidLoadRewarded;
        public virtual event HeliumPlacementEventWithError DidShowRewarded;
        public virtual event HeliumPlacementEventWithError DidCloseRewarded;
        public virtual event HeliumPlacementEvent DidClickRewarded;
        public virtual event HeliumPlacementEvent DidRecordImpressionRewarded;
        public virtual event HeliumPlacementEvent DidReceiveReward;
        
        // Banners
        public virtual event HeliumPlacementLoadEvent DidLoadBanner;
        public virtual event HeliumPlacementEvent DidClickBanner;
        public virtual event HeliumPlacementEvent DidRecordImpressionBanner;
#pragma warning restore 67
    }
}

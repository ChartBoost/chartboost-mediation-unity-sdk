using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Helium.Banner;
using Helium.Interfaces;
using UnityEngine;

namespace Helium.Platforms
{
    public abstract class HeliumExternal : IHeliumLifeCycle, IInterstitialEvents, IRewardedEvents, IBannerEvents
    {
        public static bool IsInitialized { get; protected set; }
        
        protected static string LOGTag = "HeliumSDK";

        protected static bool CanFetchAd(string placementName)
        {
            if (!CheckInitialized())
                return false;
            if (placementName != null) 
                return true;
            HeliumLogger.LogError(LOGTag, "placementName passed is null cannot perform the operation requested");
            return false;
        }

        protected static bool CheckInitialized()
        {
            if (IsInitialized)
                return true;

            HeliumLogger.LogError(LOGTag, "The Helium SDK needs to be initialized before we can show any ads");
            return false;
        }
        
        /// Initializes the Helium plugin.
        /// This must be called before using any other Helium features.
        public virtual void Init()
        {
            HeliumLogger.Log(LOGTag, "Init - Attempting to Initialize Helium SDK from HeliumSettings.");
        }

        /// Initialize the Helium plugin with a specific appId
        /// Either one of the init() methods must be called before using any other Helium feature
        public virtual void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            HeliumLogger.Log(LOGTag, $"InitWithAppIdAndSignature {appId}, {appSignature} and version {Application.unityVersion}");
            HeliumEventProcessor.Initialize();
        }
        
        public virtual void SetSubjectToCoppa(bool isSubject)
        {
            HeliumLogger.Log(LOGTag, $"SetSubjectToCoppa {isSubject}");
        }
        
        public virtual void SetSubjectToGDPR(bool isSubject)
        {
            HeliumLogger.Log(LOGTag, $"SetSubjectToGDPR {isSubject}");
        }

        public virtual void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            HeliumLogger.Log(LOGTag, $"SetUserHasGivenConsent {hasGivenConsent}");
        }

        public virtual void SetCCPAConsent(bool hasGivenConsent)
        {
            HeliumLogger.Log(LOGTag, $"SetCCPAConsent {hasGivenConsent}");
        }

        public virtual void SetUserIdentifier(string userIdentifier)
        {
            HeliumLogger.Log(LOGTag, $"SetUserIdentifier {userIdentifier}");
        }

        public virtual string GetUserIdentifier()
        {
            HeliumLogger.Log(LOGTag, "GetUserIdentifier");
            return string.Empty;
        }

        public virtual void Destroy()
        {
            HeliumLogger.Log(LOGTag, "Destroy");
        }

        public virtual void Pause(bool paused)
        {
            HeliumLogger.Log(LOGTag, "pause");
        }

        public virtual bool OnBackPressed()
        {
            HeliumLogger.Log(LOGTag, "OnBackPressed");
            return CheckInitialized();
        }

        public virtual HeliumInterstitialAd GetInterstitialAd(string placementName)
        {
            HeliumLogger.Log(LOGTag, $"GetInterstitialAd at placement: {placementName}");
            return null;
        }
        
        public virtual HeliumRewardedAd GetRewardedAd(string placementName)
        {
            HeliumLogger.Log(LOGTag, $"GetRewardedAd at placement: {placementName}");
            return null;
        }
        
        public HeliumBannerAd GetBannerAd(string placementName, HeliumBannerAdSize size)
        {
            HeliumLogger.Log(LOGTag, $"GetBannerAd at placement: {placementName}");
            if (!CanFetchAd(placementName))
                return null;
            try
            {
                return new HeliumBannerAd(placementName, size);
            }
            catch (Exception e)
            {
                HeliumLogger.LogError(LOGTag, $"banner ad failed to load {e}");
                return null;
            }
        }
        
        // todo - https://chartboost.atlassian.net/browse/HB-3868  remove this, change with MonoPInvokeCallback https://docs.unity3d.com/Manual/PluginsForIOS.html
        /// Sets the name of the game object to be used by the Helium iOS SDK
        public virtual void SetGameObjectName(string name)
        {
            HeliumLogger.Log(LOGTag, $"Setting GameObjectName: {name}");
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
            var initOptions  = Array.Empty<string>();;

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

        // provide the option to override callbacks
#pragma warning disable 67
        public virtual event HeliumEvent DidStart;
        public virtual event HeliumILRDEvent DidReceiveImpressionLevelRevenueData;
        public virtual event HeliumPartnerInitializationEvent DidReceivePartnerInitializationData;
        public virtual event HeliumPlacementEvent DidLoadInterstitial;
        public virtual event HeliumPlacementEvent DidShowInterstitial;
        public virtual event HeliumPlacementEvent DidCloseInterstitial;
        public virtual event HeliumPlacementEvent DidClickInterstitial;
        public virtual event HeliumPlacementEvent DidRecordImpressionInterstitial;
        public virtual event HeliumBidEvent DidWinBidInterstitial;
        public virtual event HeliumPlacementEvent DidLoadRewarded;
        public virtual event HeliumPlacementEvent DidShowRewarded;
        public virtual event HeliumPlacementEvent DidCloseRewarded;
        public virtual event HeliumPlacementEvent DidClickRewarded;
        public virtual event HeliumPlacementEvent DidRecordImpressionRewarded;
        public virtual event HeliumBidEvent DidWinBidRewarded;
        public virtual event HeliumRewardEvent DidReceiveReward;
        public virtual event HeliumPlacementEvent DidLoadBanner;
        public virtual event HeliumPlacementEvent DidShowBanner;
        public virtual event HeliumPlacementEvent DidClickBanner;
        public virtual event HeliumPlacementEvent DidRecordImpressionBanner;
        public virtual event HeliumBidEvent DidWinBidBanner;
#pragma warning restore 67
    }
}

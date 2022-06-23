using System;
using Helium.Interfaces;
using UnityEngine;

namespace Helium.Platforms
{
    public abstract class HeliumExternal : IHeliumLifeCycle, IInterstitialEvents, IRewardedEvents, IBannerEvents
    {
        protected static bool Initialized = false;
        protected static string LOGTag = "HeliumSDK";

        protected static void Log(string message)
        {
            if (HeliumSettings.IsLogging() && Debug.isDebugBuild)
                Debug.Log( $"{LOGTag}/{message}");
        }

        protected static void LogError(string error)
        {
            Debug.Log( $"{LOGTag}/{error}");
        }

        public static bool IsInitialized => Initialized;

        protected static bool CanFetchAd(string placementName)
        {
            if (!CheckInitialized())
                return false;
            if (placementName != null) 
                return true;
            Debug.LogError("placementName passed is null cannot perform the operation requested");
            return false;
        }

        protected static bool CheckInitialized()
        {
            if (Initialized)
                return true;

            Debug.LogError("The Helium SDK needs to be initialized before we can show any ads");
            return false;
        }
        
        /// Initializes the Helium plugin.
        /// This must be called before using any other Helium features.
        public virtual void Init()
        {
            Log("Init - Attempting to Initialize Helium SDK from HeliumSettings.");
        }

        /// Initialize the Helium plugin with a specific appId
        /// Either one of the init() methods must be called before using any other Helium feature
        public virtual void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            Log($"InitWithAppIdAndSignature {appId}, {appSignature} and version {Application.unityVersion}");
            HeliumSettings.SetAppId(appId);
            HeliumSettings.SetAppSignature(appSignature);
        }
        
        public virtual void SetSubjectToCoppa(bool isSubject)
        {
            Log($"SetSubjectToCoppa {isSubject}");
        }
        
        public virtual void SetSubjectToGDPR(bool isSubject)
        {
            Log($"SetSubjectToGDPR {isSubject}");
        }

        public virtual void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            Log($"SetUserHasGivenConsent {hasGivenConsent}");
        }

        public virtual void SetCCPAConsent(bool hasGivenConsent)
        {
            Log($"SetCCPAConsent {hasGivenConsent}");
        }

        public virtual void SetUserIdentifier(string userIdentifier)
        {
            Log($"SetUserIdentifier {userIdentifier}");
        }

        public virtual string GetUserIdentifier()
        {
            Log("GetUserIdentifier");
            return string.Empty;
        }

        public virtual void Destroy()
        {
            Log("Destroy");
        }

        public virtual void Pause(bool paused)
        {
            Log("pause");
        }

        public virtual bool OnBackPressed()
        {
            Log("OnBackPressed");
            return CheckInitialized();
        }

        public virtual HeliumInterstitialAd GetInterstitialAd(string placementName)
        {
            Log($"GetInterstitialAd at placement: {placementName}");
            return null;
        }
        
        public virtual HeliumRewardedAd GetRewardedAd(string placementName)
        {
            Log($"GetRewardedAd at placement: {placementName}");
            return null;
        }
        
        public virtual HeliumBannerAd GetBannerAd(string placementName, HeliumBannerAdSize size)
        {
            Log($"GetBannerAd at placement: {placementName}");
            return null;
        }
        
        // todo - https://chartboost.atlassian.net/browse/HB-3868  remove this, change with MonoPInvokeCallback https://docs.unity3d.com/Manual/PluginsForIOS.html
        /// Sets the name of the game object to be used by the Helium iOS SDK
        public virtual void SetGameObjectName(string name)
        {
            Log($"Setting GameObjectName: {name}");
        }

        // provide the option to override callbacks
        public virtual event HeliumEvent DidStart;
        public virtual event HeliumILRDEvent DidReceiveImpressionLevelRevenueData;
        public virtual event HeliumEvent UnexpectedSystemErrorDidOccur;
        public virtual event HeliumPlacementEvent DidLoadInterstitial;
        public virtual event HeliumPlacementEvent DidShowInterstitial;
        public virtual event HeliumPlacementEvent DidCloseInterstitial;
        public virtual event HeliumPlacementEvent DidClickInterstitial;
        public virtual event HeliumBidEvent DidWinBidInterstitial;
        public virtual event HeliumPlacementEvent DidLoadRewarded;
        public virtual event HeliumPlacementEvent DidShowRewarded;
        public virtual event HeliumPlacementEvent DidCloseRewarded;
        public virtual event HeliumPlacementEvent DidClickRewarded;
        public virtual event HeliumBidEvent DidWinBidRewarded;
        public virtual event HeliumRewardEvent DidReceiveReward;
        public virtual event HeliumPlacementEvent DidLoadBanner;
        public virtual event HeliumPlacementEvent DidShowBanner;
        public virtual event HeliumPlacementEvent DidClickBanner;
        public virtual event HeliumBidEvent DidWinBidBanner;
    }
}

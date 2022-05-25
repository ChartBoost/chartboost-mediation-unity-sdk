using UnityEngine;
using System;
using System.Collections;
using AOT;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#if UNITY_4_6 || UNITY_5 || UNITY_2017_1_OR_NEWER
using UnityEngine.EventSystems;
#endif

namespace Helium
{
    /// <summary>
    /// Returned to ChartboostDelegate methods to notify of HeliumSdk SDK errors.
    /// </summary>
    public enum HeliumErrorCode : int
    {
        /// No ad was available for the user from Helium
        NoAdFound = 0,
        /// No bid
        NoBid = 1,
        /// No internet connection was found
        NoNetwork = 2,
        /// An error occurred during network communication with the server
        ServerError = 3,
        /// An unknown or unexpected error
        Unknown = 4
    };

    public class HeliumError
    {
        public HeliumErrorCode errorCode;
        public string errorDescription;

        public HeliumError(HeliumErrorCode code, string description)
        {
            this.errorCode = code;
            this.errorDescription = description;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.errorCode, this.errorDescription);
        }
    }

    /// <summary>
    ///  Provide methods to display and controler HeliumSdk native advertising types.
    ///  For more information on integrating and using the HeliumSdk SDK
    ///  please visit our help site documentation at https://help.chartboost.com
    /// </summary>
    public class HeliumSdk : MonoBehaviour
    {
        //////////////////////////////////////////////////////
        /// Events to subscribe to for callbacks
        //////////////////////////////////////////////////////

        /// <summary>
        ///   Called after an the SDK has been initialized
        /// </summary>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<HeliumError> didStart;

        /// <summary>
        ///   Called after an interstitial has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didLoadInterstitial;

        /// <summary>
        ///   Called with bid information after an interstitial has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="info">A dictionary with information</param>
        public static event Action<string, HeliumBidInfo> didWinBidInterstitial;

        /// <summary>
        ///   Called after an interstitial has been displayed on screen.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didShowInterstitial;

        /// <summary>
        ///  Called after an interstitial has been closed.
        ///  Implement to be notified of when an interstitial has been closed for a given placement.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didCloseInterstitial;

        /// <summary>
        ///  Called after an interstitial has been clicked.
        ///  Implement to be notified of when an interstitial has been clicked for a given placement.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didClickInterstitial;

        /// <summary>
        ///   Called after a rewarded ad has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium rewarded ad.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didLoadRewarded;

        /// <summary>
        ///   Called with bid information after an rewarded has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="info">A dictionary with information</param>
        public static event Action<string, HeliumBidInfo> didWinBidRewarded;

        /// <summary>
        ///   Called after a rewarded ad has been displayed on screen.
        /// </summary>
        /// <param name="placement">The placement for the Helium rewarded ad.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didShowRewarded;

        /// <summary>
        ///  Called after a rewarded ad has been closed.
        ///  Implement to be notified of when a rewarded ad has been closed for a given placement
        /// </summary>
        /// <param name="placement">The placement for the Helium rewarded ad.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didCloseRewarded;

        /// <summary>
        ///  Called after a rewarded ad has been clicked.
        ///  Implement to be notified of when a rewarded ad has been clicked for a given placement
        /// </summary>
        /// <param name="placement">The placement for the Helium rewarded ad.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didClickRewarded;

        /// <summary>
        ///  Called after a rewarded has been received (after watching a rewarded video).
        ///  Implement to be notified of when a user has earned a reward.
        ///  This version could be called on a background thread, even if the Unity runtime is paused.
        /// </summary>
        /// <param name="reward">The reward earned.</param>
        public static event Action<string> didReceiveReward;

        /// <summary>
        ///   Called after an banner has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium banner.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didLoadBanner;

        /// <summary>
        ///   Called with bid information after an banner has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium banner.</param>
        /// <param name="info">A dictionary with information</param>
        public static event Action<string, HeliumBidInfo> didWinBidBanner;

        /// <summary>
        ///   Called after an banner has been displayed on screen.
        /// </summary>
        /// <param name="placement">The placement for the Helium banner.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didShowBanner;

        /// <summary>
        ///  Called after a banner ad has been clicked.
        ///  Implement to be notified of when a banner ad has been clicked for a given placement
        /// </summary>
        /// <param name="placement">The placement for the Helium banner ad.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> didClickBanner;

        /// <summary>
        ///   Called immediately when impression level revenue data has been received after an
        ///   ad was displayed on the screen. This may be called in a background thread. This event
        ///   is sent natively from iOS and Android.
        /// <param name="placement">The placement the Helium ad.</param>
        /// <param name="impressionData">The impression data delivered within a hashtable.</param>
        /// </summary>
        public static event Action<string, Hashtable> didReceiveImpressionLevelRevenueData;

        /// <summary>
        /// Called when an unexpected system error occurred.
        /// <param name="message">A message that describes the unexpected system error.</param>
        /// </summary>
        public static event Action<string> unexpectedSystemErrorDidOccur
        {
            add
            {
                privateUnexpectedSystemErrorDidOccur += value;
                HeliumEventProcessor.unexpectedSystemErrorDidOccur += value;
            }
            remove
            {
                privateUnexpectedSystemErrorDidOccur -= value;
                HeliumEventProcessor.unexpectedSystemErrorDidOccur -= value;
            }
        }
        private static event Action<string> privateUnexpectedSystemErrorDidOccur;


        //////////////////////////////////////////////////////
        /// Functions for showing ads
        //////////////////////////////////////////////////////

        /// <summary>
        /// Returns a new ad unit that can be used to load and display interstitial ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the HeliumSdk impression type.</param>
        public static HeliumInterstitialAd getInterstitialAd(string placementName)
        {
            return HeliumExternal.getInterstitialAd(placementName);
        }

        /// <summary>
        /// Returns a new ad unit that can be used to load and display rewarded video ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the HeliumSdk impression type.</param>
        public static HeliumRewardedAd getRewardedAd(string placementName)
        {
            return HeliumExternal.getRewardedAd(placementName);
        }

        /// <summary>
        /// Returns a new ad unit that can be used to load and display banner ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the HeliumSdk impression type.</param>
        public static HeliumBannerAd getBannerAd(string placementName, HeliumBannerAdSize size)
        {
            return HeliumExternal.getBannerAd(placementName, size);
        }

        //////////////////////////////////////////////////////
        /// Monobehaviour Lifecycle functionality
        //////////////////////////////////////////////////////

        /// <summary>
        /// The chartboost object is a singleton and only needs to be created once.
        /// If you don't include a HeliumSdk gameoject in your scene, calling this will create one
        /// If you have a HeliumSdk gameobject in your scene, this is not required.
        /// Usage HeliumSdk.Instance
        /// </summary>
        static private HeliumSdk instance = null;

        private HeliumEventProcessor eventProcessor = new HeliumEventProcessor();

        public static HeliumSdk Create()
        {
            if (instance == null)
            {
                GameObject singleton = new GameObject("HeliumSdk");
                instance = singleton.AddComponent<HeliumSdk>();
            }
            else
            {
                Debug.LogWarning("HELIUM: HeliumSdk instance already exists. Create() ignored");
            }
            return instance;
        }

        public static HeliumSdk StartWithAppIdAndAppSignature(string appId, string appSignature)
        {
            HeliumSettings.setAppId(appId);
            HeliumSettings.setAppSignature(appSignature);
            return Create();
        }

        public static void SetSubjectToCoppa(bool isSubject)
        {
            HeliumExternal.setSubjectToCoppa(isSubject);
        }

        public static void SetSubjectToGDPR(bool isSubject)
        {
            HeliumExternal.setSubjectToGDPR(isSubject);
        }

        public static void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            HeliumExternal.setUserHasGivenConsent(hasGivenConsent);
        }

        public static void SetCCPAConsent(bool hasGivenConsent)
        {
            HeliumExternal.setCCPAConsent(hasGivenConsent);
        }

        public static void SetUserIdentifier(string userIdentifier)
        {
            HeliumExternal.setUserIdentifier(userIdentifier);
        }

        public static string GetUserIdentifier()
        {
            return HeliumExternal.getUserIdentifer();
        }

        void Awake()
        {
            // Limit the number of instances to one
            if (instance == null)
            {
                instance = this;
                HeliumExternal.init();
                HeliumExternal.setGameObjectName(gameObject.name);

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // duplicate
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (this == instance)
            {
                instance = null;
                HeliumExternal.destroy();
            }
        }

        void OnDisable()
        {
            // Shut down the HeliumSdk plugin
            #if UNITY_ANDROID
			if (this == instance)
			{
				instance = null;
				HeliumExternal.destroy();
			}
            #endif
        }

        //////////////////////////////////////////////////////
        /// Managing the events and firing them
        //////////////////////////////////////////////////////

        private void didStartEvent(string dataString)
        {
            eventProcessor.ProcessEventWithError(dataString, didStart);
        }

        private void didCloseInterstitialEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didCloseInterstitial);
        }

        private void didLoadInterstitialEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didLoadInterstitial);
        }

        private void didWinBidInterstitialEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndBidInfo(dataString, didWinBidInterstitial);
        }

        private void didShowInterstitialEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didShowInterstitial);
        }

        private void didClickInterstitialEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didClickInterstitial);
        }

        private void didCloseRewardedEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didCloseRewarded);
        }

        private void didLoadRewardedEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didLoadRewarded);
        }

        private void didWinBidRewardedEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndBidInfo(dataString, didWinBidRewarded);
        }

        private void didShowRewardedEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didShowRewarded);
        }

        private void didClickRewardedEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didClickRewarded);
        }

        private void didLoadBannerEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didLoadBanner);
        }

        private void didWinBidBannerEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndBidInfo(dataString, didWinBidBanner);
        }

        private void didShowBannerEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didShowBanner);
        }

        private void didClickBannerEvent(string dataString)
        {
            eventProcessor.ProcessEventWithPlacementAndError(dataString, didClickBanner);
        }

        // ILRD

        private void didReceiveILRD(string dataString)
        {
            eventProcessor.ProcessEventWithILRD(dataString, didReceiveImpressionLevelRevenueData);
        }

        internal class BackgroundEventListener : AndroidJavaProxy
        {
            private BackgroundEventListener() : base("com.chartboost.heliumsdk.unity.HeliumUnityBridge$IBackgroundEventListener") { }

            public static readonly BackgroundEventListener Instance = new BackgroundEventListener();

            // Called from Android Wrapper (Java).
            public void onBackgroundEvent(string eventName, string eventArgsJson)
            {
                SendEvent(eventName, eventArgsJson);
            }

            // For use in DllImport declaration to specify the callback signature (iOS).
            public delegate void Delegate(string eventName, string eventArgsJson);

            // Called from iOS Wrapper (Objective-C) directly and from Android Wrapper (Java) via onBackgroundEvent.
            [MonoPInvokeCallback(typeof(Delegate))]
            public static void SendEvent(string eventName, string eventArgsJson)
            {
                HeliumEventProcessor eventProcessor = new HeliumEventProcessor();
                try
                {
                    // Handle events that are supported in the background.
                    switch (eventName)
                    {
                        case "didReceiveILRD":
                            eventProcessor.ProcessEventWithILRD(eventArgsJson, didReceiveImpressionLevelRevenueData);
                            break;
                        case "didReceiveRewardEvent":
                            eventProcessor.ProcessEventWithReward(eventArgsJson, didReceiveReward);
                            break;
                        default:
                            throw new ArgumentException("Unrecognized event callback name.");
                    }
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError(String.Format("Exception while handling background event {0}: {1}", eventName, e.Message));
                }
            }
        }

        // Utility methods

        private static void ReportUnexpectedSystemError(string message)
        {
            if (privateUnexpectedSystemErrorDidOccur == null)
                return;
            privateUnexpectedSystemErrorDidOccur(message);
        }

        [ObsoleteAttribute("isImpressionVisible() is obsolete and no longer provides value; usage must be deleted.", true)]
        public static bool isImpressionVisible()
        {
            return false;
        }
    }
}

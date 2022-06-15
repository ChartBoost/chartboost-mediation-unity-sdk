using UnityEngine;
using System;
using System.Collections;
using AOT;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;
#if UNITY_4_6 || UNITY_5 || UNITY_2017_1_OR_NEWER
using UnityEngine.EventSystems;
#endif

// ReSharper disable InconsistentNaming
namespace Helium
{
    /// <summary>
    /// Returned to ChartboostDelegate methods to notify of HeliumSdk SDK errors.
    /// </summary>
    public enum HeliumErrorCode
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
    }

    public class HeliumError
    {
        public HeliumErrorCode errorCode;
        public string errorDescription;

        public HeliumError(HeliumErrorCode code, string description)
        {
            errorCode = code;
            errorDescription = description;
        }

        public override string ToString()
        {
            return $"{errorCode} {errorDescription}";
        }
    }

    /// <summary>
    ///  Provide methods to display and controler HeliumSdk native advertising types.
    ///  For more information on integrating and using the HeliumSdk SDK
    ///  please visit our help site documentation at https://help.chartboost.com
    /// </summary>
    public class HeliumSDK : MonoBehaviour
    {
        //////////////////////////////////////////////////////
        /// Events to subscribe to for callbacks
        //////////////////////////////////////////////////////

        /// <summary>
        ///   Called after an the SDK has been initialized
        /// </summary>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<HeliumError> DidStart;

        /// <summary>
        ///   Called after an interstitial has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidLoadInterstitial;

        /// <summary>
        ///   Called with bid information after an interstitial has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="info">A dictionary with information</param>
        public static event Action<string, HeliumBidInfo> DidWinBidInterstitial;

        /// <summary>
        ///   Called after an interstitial has been displayed on screen.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidShowInterstitial;

        /// <summary>
        ///  Called after an interstitial has been closed.
        ///  Implement to be notified of when an interstitial has been closed for a given placement.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidCloseInterstitial;

        /// <summary>
        ///  Called after an interstitial has been clicked.
        ///  Implement to be notified of when an interstitial has been clicked for a given placement.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidClickInterstitial;

        /// <summary>
        ///   Called after a rewarded ad has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium rewarded ad.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidLoadRewarded;

        /// <summary>
        ///   Called with bid information after an rewarded has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium interstitial.</param>
        /// <param name="info">A dictionary with information</param>
        public static event Action<string, HeliumBidInfo> DidWinBidRewarded;

        /// <summary>
        ///   Called after a rewarded ad has been displayed on screen.
        /// </summary>
        /// <param name="placement">The placement for the Helium rewarded ad.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidShowRewarded;

        /// <summary>
        ///  Called after a rewarded ad has been closed.
        ///  Implement to be notified of when a rewarded ad has been closed for a given placement
        /// </summary>
        /// <param name="placement">The placement for the Helium rewarded ad.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidCloseRewarded;

        /// <summary>
        ///  Called after a rewarded ad has been clicked.
        ///  Implement to be notified of when a rewarded ad has been clicked for a given placement
        /// </summary>
        /// <param name="placement">The placement for the Helium rewarded ad.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidClickRewarded;

        /// <summary>
        ///  Called after a rewarded has been received (after watching a rewarded video).
        ///  Implement to be notified of when a user has earned a reward.
        ///  This version could be called on a background thread, even if the Unity runtime is paused.
        /// </summary>
        /// <param name="reward">The reward earned.</param>
        public static event Action<string> DidReceiveReward;

        /// <summary>
        ///   Called after an banner has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium banner.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidLoadBanner;

        /// <summary>
        ///   Called with bid information after an banner has been loaded from the Helium API
        ///   servers and cached locally.
        /// </summary>
        /// <param name="placement">The placement for the Helium banner.</param>
        /// <param name="info">A dictionary with information</param>
        public static event Action<string, HeliumBidInfo> DidWinBidBanner;

        /// <summary>
        ///   Called after an banner has been displayed on screen.
        /// </summary>
        /// <param name="placement">The placement for the Helium banner.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidShowBanner;

        /// <summary>
        ///  Called after a banner ad has been clicked.
        ///  Implement to be notified of when a banner ad has been clicked for a given placement
        /// </summary>
        /// <param name="placement">The placement for the Helium banner ad.</param>
        /// <param name="error">The error encountered, if any.</param>
        public static event Action<string, HeliumError> DidClickBanner;

        /// <summary>
        ///   Called immediately when impression level revenue data has been received after an
        ///   ad was displayed on the screen. This may be called in a background thread. This event
        ///   is sent natively from iOS and Android.
        /// <param name="placement">The placement the Helium ad.</param>
        /// <param name="impressionData">The impression data delivered within a hashtable.</param>
        /// </summary>
        public static event Action<string, Hashtable> DidReceiveImpressionLevelRevenueData;

        /// <summary>
        /// Called when an unexpected system error occurred.
        /// <param name="message">A message that describes the unexpected system error.</param>
        /// </summary>
        public static event Action<string> UnexpectedSystemErrorDidOccur
        {
            add
            {
                PrivateUnexpectedSystemErrorDidOccur += value;
                HeliumEventProcessor.UnexpectedSystemErrorDidOccur += value;
            }
            remove
            {
                PrivateUnexpectedSystemErrorDidOccur -= value;
                HeliumEventProcessor.UnexpectedSystemErrorDidOccur -= value;
            }
        }
        
        private static event Action<string> PrivateUnexpectedSystemErrorDidOccur;


        //////////////////////////////////////////////////////
        /// Functions for showing ads
        //////////////////////////////////////////////////////

        /// <summary>
        /// Returns a new ad unit that can be used to load and display interstitial ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the HeliumSdk impression type.</param>
        public static HeliumInterstitialAd GetInterstitialAd(string placementName)
        {
            return HeliumExternal.GetInterstitialAd(placementName);
        }

        /// <summary>
        /// Returns a new ad unit that can be used to load and display rewarded video ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the HeliumSdk impression type.</param>
        public static HeliumRewardedAd GetRewardedAd(string placementName)
        {
            return HeliumExternal.GetRewardedAd(placementName);
        }

        /// <summary>
        /// Returns a new ad unit that can be used to load and display banner ads.
        /// </summary>
        /// <param name="placementName">The placement ID for the HeliumSdk impression type.</param>
        /// <param name="size">The banner size</param>
        public static HeliumBannerAd GetBannerAd(string placementName, HeliumBannerAdSize size)
        {
            return HeliumExternal.GetBannerAd(placementName, size);
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
        private static HeliumSDK _instance = null;

        public static HeliumSDK Create()
        {
            if (_instance == null)
            {
                var singleton = new GameObject("HeliumSDK");
                _instance = singleton.AddComponent<HeliumSDK>();
            }
            else
            {
                Debug.LogWarning("HELIUM: HeliumSDK instance already exists. Create() ignored");
            }
            return _instance;
        }

        public static HeliumSDK StartWithAppIdAndAppSignature(string appId, string appSignature)
        {
            HeliumSettings.SetAppId(appId);
            HeliumSettings.SetAppSignature(appSignature);
            return Create();
        }

        public static void SetSubjectToCoppa(bool isSubject)
        {
            HeliumExternal.SetSubjectToCoppa(isSubject);
        }

        public static void SetSubjectToGDPR(bool isSubject)
        {
            HeliumExternal.SetSubjectToGDPR(isSubject);
        }

        public static void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            HeliumExternal.SetUserHasGivenConsent(hasGivenConsent);
        }

        public static void SetCCPAConsent(bool hasGivenConsent)
        {
            HeliumExternal.SetCCPAConsent(hasGivenConsent);
        }

        public static void SetUserIdentifier(string userIdentifier)
        {
            HeliumExternal.SetUserIdentifier(userIdentifier);
        }

        public static string GetUserIdentifier()
        {
            return HeliumExternal.GetUserIdentifier();
        }

        private void Awake()
        {
            // Limit the number of instances to one
            if (_instance == null)
            {
                _instance = this;
                HeliumExternal.Init();
                HeliumExternal.SetGameObjectName(gameObject.name);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // duplicate
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (this != _instance)
                return;
            _instance = null;
            HeliumExternal.Destroy();
        }

        private void OnDisable()
        {
            // Shut down the HeliumSdk plugin
            #if UNITY_ANDROID
            if (this != _instance) 
                return;
            _instance = null;
            HeliumExternal.Destroy();
            #endif
        }

        //////////////////////////////////////////////////////
        /// Managing the events and firing them
        //////////////////////////////////////////////////////

#pragma warning disable IDE0051

        [Preserve]
        private void didStartEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithError(dataString, DidStart);
        }

        [Preserve]
        private void didCloseInterstitialEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidCloseInterstitial);
        }

        [Preserve]
        private void didLoadInterstitialEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidLoadInterstitial);
        }

        [Preserve]
        private void didWinBidInterstitialEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(dataString, DidWinBidInterstitial);
        }

        [Preserve]
        private void didShowInterstitialEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidShowInterstitial);
        }

        [Preserve]
        private void didClickInterstitialEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidClickInterstitial);
        }

        [Preserve]
        private void didCloseRewardedEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidCloseRewarded);
        }

        [Preserve]
        private void didLoadRewardedEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidLoadRewarded);
        }

        [Preserve]
        private void didWinBidRewardedEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(dataString, DidWinBidRewarded);
        }
        
        [Preserve]
        private void didShowRewardedEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidShowRewarded);
        }

        [Preserve]
        private void didClickRewardedEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidClickRewarded);
        }

        [Preserve]
        private void didLoadBannerEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidLoadBanner);
        }

        [Preserve]
        private void didWinBidBannerEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndBidInfo(dataString, DidWinBidBanner);
        }

        [Preserve]
        private void didShowBannerEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidShowBanner);
        }

        [Preserve]
        private void didClickBannerEvent(string dataString)
        {
            HeliumEventProcessor.ProcessEventWithPlacementAndError(dataString, DidClickBanner);
        }
        
#pragma warning restore IDE0051

        internal class BackgroundEventListener : AndroidJavaProxy
        {
            private BackgroundEventListener() : base("com.chartboost.heliumsdk.unity.HeliumUnityBridge$IBackgroundEventListener") { }

            public static readonly BackgroundEventListener Instance = new BackgroundEventListener();

            [Preserve]
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
                try
                {
                    // Handle events that are supported in the background.
                    switch (eventName)
                    {
                        case "DidReceiveILRD":
                            HeliumEventProcessor.ProcessEventWithILRD(eventArgsJson, DidReceiveImpressionLevelRevenueData);
                            break;
                        case "DidReceiveRewardEvent":
                            HeliumEventProcessor.ProcessEventWithReward(eventArgsJson, DidReceiveReward);
                            break;
                        default:
                            throw new ArgumentException($"Unrecognized event callback name: {eventName}.");
                    }
                }
                catch (Exception e)
                {
                    ReportUnexpectedSystemError($"Exception while handling background event {eventName}: {e.Message}");
                }
            }
        }

        // Utility methods

        private static void ReportUnexpectedSystemError(string message)
        {
            PrivateUnexpectedSystemErrorDidOccur?.Invoke(message);
        }
    }
}

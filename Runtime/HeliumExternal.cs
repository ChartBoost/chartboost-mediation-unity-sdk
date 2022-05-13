using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Helium
{
    public class HeliumExternal
    {
        private static bool initialized = false;
        private static string _logTag = "HeliumSDK";

        public static void Log(string message)
        {
            if (HeliumSettings.isLogging() && Debug.isDebugBuild)
                Debug.Log(_logTag + "/" + message);
        }

        public static bool isInitialized()
        {
            return initialized;
        }

        private static bool checkInitialized()
        {
            if (initialized)
            {
                return true;
            }
            else
            {
                Debug.LogError("The Helium SDK needs to be initialized before we can show any ads");
                return false;
            }
        }

#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void _heliumSdkInit(string appId, string appSignature, string unityVersion, HeliumSdk.BackgroundEventListener.Delegate callback);
		// [DllImport("__Internal")]
		// private static extern bool _heliumSdkIsAnyViewVisible();
		[DllImport("__Internal")]
		private static extern void _heliumSdkSetGameObjectName(string name);
		[DllImport("__Internal")]
		private static extern IntPtr _heliumSdkGetInterstitialAd(string placementName);
		[DllImport("__Internal")]
		private static extern IntPtr _heliumSdkGetRewardedAd(string placementName);
		[DllImport("__Internal")]
		private static extern IntPtr _heliumSdkGetBannerAd(string placementName, int size);
		[DllImport("__Internal")]
		private static extern void _heliumSdkSetSubjectToCoppa(bool isSubject);
		[DllImport("__Internal")]
		private static extern void _heliumSdkSetSubjectToGDPR(bool isSubject);
		[DllImport("__Internal")]
		private static extern void _heliumSdkSetUserHasGivenConsent(bool hasGivenConsent);
		[DllImport("__Internal")]
		private static extern void _heliumSetCCPAConsent(bool hasGivenConsent);
		[DllImport("__Internal")]
		private static extern void _heliumSetUserIdentifer(string userIdentifier);
		[DllImport("__Internal")]
		private static extern string _heliumGetUserIdentifer();

		/// Initializes the Helium plugin.
		/// This must be called before using any other Helium features.
		public static void init()
		{
			// get the AppID and AppSecret from HeliumSettings
			string appID = HeliumSettings.getIOSAppId();
			string appSignature = HeliumSettings.getIOSAppSignature();
			initWithAppIdAndSignature(appID, appSignature);
		}

		/// Initialize the Helium plugin with a specific appId
		/// Either one of the init() methods must be called before using any other Helium feature
		public static void initWithAppIdAndSignature(string appId, string appSignature)
		{
			Log("Helium(iOS): initWithAppIdAndSignature " + appId + ", " + appSignature + " and version " + Application.unityVersion);
			if (!Application.isEditor)
				_heliumSdkInit(appId, appSignature, Application.unityVersion, HeliumSdk.BackgroundEventListener.SendEvent);
			initialized = true;
		}

		public static void setSubjectToCoppa(bool isSubject)
		{
			Log("Helium(iOS): setSubjectToCoppa " + isSubject);
			if (!Application.isEditor)
				_heliumSdkSetSubjectToCoppa(isSubject);
		}

		public static void setSubjectToGDPR(bool isSubject)
		{
			Log("Helium(iOS): setSubjectToGDPR " + isSubject);
			if (!Application.isEditor)
				_heliumSdkSetSubjectToGDPR(isSubject);
		}

		public static void setUserHasGivenConsent(bool hasGivenConsent)
		{
			Log("Helium(iOS): setUserHasGivenConsent " + hasGivenConsent);
			if (!Application.isEditor)
				_heliumSdkSetUserHasGivenConsent(hasGivenConsent);
		}

		public static void setCCPAConsent(bool hasGivenConsent)
		{
			Log("Helium(iOS): setCCPAConsent " + hasGivenConsent);
			if (!Application.isEditor)
				_heliumSetCCPAConsent(hasGivenConsent);
		}

		public static void setUserIdentifier(string userIdentifier)
		{
			Log("Helium(iOS): setUserIdentifier " + userIdentifier);
			if (!Application.isEditor)
				_heliumSetUserIdentifer(userIdentifier);
		}

		public static string getUserIdentifer()
		{
			Log("Helium(iOS): getUserIdentifer");
			if (!Application.isEditor)
				return _heliumGetUserIdentifer();
			else
				return null;
		}

		/// Shuts down the Helium plugin
		public static void destroy()
		{
			Log("Helium(iOS): destroy");
		}

		public static HeliumInterstitialAd getInterstitialAd(string placementName)
		{
			if (!checkInitialized())
			{
				return null;
			}
			else if (placementName == null)
			{
				Debug.LogError("Helium(iOS): placementName passed is null cannot perform the operation requested");
				return null;
			}

			Log("Helium(iOS): getInterstitialAd at placement = " + placementName);
			if (!Application.isEditor)
			{
				IntPtr adId = _heliumSdkGetInterstitialAd(placementName);
				if (adId == IntPtr.Zero)
				{
					return null;
				}

				HeliumInterstitialAd ad = new HeliumInterstitialAd(adId);
				return ad;
			}
			else
				return null;
		}

		public static HeliumRewardedAd getRewardedAd(string placementName)
		{
			if (!checkInitialized())
			{
				return null;
			}
			else if (placementName == null)
			{
				Debug.LogError("Helium(iOS): placementName passed is null cannot perform the operation requested");
				return null;
			}

			Log("Helium(iOS): getRewardedAd at placement = " + placementName);
			if (!Application.isEditor)
			{
				IntPtr adId = _heliumSdkGetRewardedAd(placementName);
				if (adId == IntPtr.Zero)
				{
					return null;
				}

				HeliumRewardedAd ad = new HeliumRewardedAd(adId);
				return ad;
			}
			else
				return null;
		}


		public static HeliumBannerAd getBannerAd(string placementName, HeliumBannerAdSize size)
		{
			if (!checkInitialized())
			{
				return null;
			}
			else if (placementName == null)
			{
				Debug.LogError("Helium(iOS): placementName passed is null cannot perform the operation requested");
				return null;
			}

			Log("Helium(iOS): getBannerAd at placement = " + placementName);
			if (!Application.isEditor)
			{
				IntPtr adId = _heliumSdkGetBannerAd(placementName, (int)size);
				if (adId == IntPtr.Zero)
				{
					return null;
				}

				HeliumBannerAd ad = new HeliumBannerAd(adId);
				return ad;
			}
			else
				return null;
		}

		/// Sets the name of the game object to be used by the Helium iOS SDK
		public static void setGameObjectName(string name)
		{
			Log("Helium(iOS): Set Game object name for callbacks to = " + name);
			if (!Application.isEditor)
				_heliumSdkSetGameObjectName(name);
		}

#elif UNITY_ANDROID
		private static AndroidJavaObject _plugin;

		/// Initialize the android sdk
		private static AndroidJavaObject plugin()
		{
			if (_plugin == null) {
				// find the plugin instance
				using (var pluginClass = new AndroidJavaClass("com.chartboost.heliumsdk.unity.HeliumUnityBridge"))
					_plugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
			}
			return _plugin;
		}

		/// Initialize the android sdk
		public static void init()
		{
			// get the AppID and AppSecret from HeliumSettings
			string appID = HeliumSettings.getAndroidAppId();
            string appSignature = HeliumSettings.getAndroidAppSignature();
            initWithAppIdAndSignature(appID, appSignature);
		}

        /// Initialize the Helium plugin with a specific appId
        /// Either one of the init() methods must be called before using any other Helium feature
        public static void initWithAppIdAndSignature(string appId, string appSignature)
		{ 
            string unityVersion = Application.unityVersion;
            Log("Helium(Android): initWithAppIdAndSignature " + appId + ", " + appSignature + " and version " + unityVersion);
			if (!Application.isEditor)
				plugin().Call("start", appId, appSignature, unityVersion, HeliumSdk.BackgroundEventListener.Instance);
			initialized = true;
		}

		public static void setSubjectToCoppa(bool isSubject)
		{
			Log("Helium(Android): setSubjectToCoppa " + isSubject);
			if (!Application.isEditor)
				plugin().Call("setSubjectToCoppa", isSubject);
		}

		public static void setSubjectToGDPR(bool isSubject)
		{
			Log("Helium(Android): setSubjectToGDPR " + isSubject);
			if (!Application.isEditor)
				plugin().Call("setSubjectToGDPR", isSubject);
		}

		public static void setUserHasGivenConsent(bool hasGivenConsent)
		{
			Log("Helium(Android): setUserHasGivenConsent " + hasGivenConsent);
			if (!Application.isEditor)
				plugin().Call("setUserHasGivenConsent", hasGivenConsent);
		}

		public static void setCCPAConsent(bool hasGivenConsent)
		{
			Log("Helium(Android): setCCPAConsent " + hasGivenConsent);
			if (!Application.isEditor)
				plugin().Call("setCCPAConsent", hasGivenConsent);
		}

		public static void setUserIdentifier(string userIdentifier)
        {
			Log("Helium(Android): setUserIdentifier " + userIdentifier);
			if (!Application.isEditor)
				plugin().Call("setUserIdentifier", userIdentifier);
		}

		public static string getUserIdentifer()
        {
			if (!Application.isEditor)
			{
				string userIdentifier = _plugin.Call<string>("getUserIdentifer");
				Log("Helium(Android): getUserIdentifer = " + userIdentifier);
				return userIdentifier;
			}
			else
				return null;
		}

		public static HeliumInterstitialAd getInterstitialAd(string placementName) {
			Log("Helium(Android): getInterstitialAd at placement = " + placementName);
			if (!checkInitialized()) {
				return null;
			} else if (placementName == null) {
				Debug.LogError("Helium(Android): placementName passed is null cannot perform the operation requested");
				return null;
			}

			if (!Application.isEditor)
			{
				try
				{
					AndroidJavaObject androidAd = _plugin.Call<AndroidJavaObject>("getInterstitialAd", placementName.ToString());
					HeliumInterstitialAd ad = new HeliumInterstitialAd(androidAd);
					return ad;
				}
				catch (Exception e)
				{
					Debug.LogError("Helium(Android): interstitial failed to load");
					Debug.LogError(e.ToString());
					return null;
				}
			}
			else
				return null;
		}

		public static HeliumRewardedAd getRewardedAd(string placementName) {
			Log("Helium(Android): getRewardedAd at placement = " + placementName);
			if (!checkInitialized()) {
				return null;
			} else if (placementName == null) {
				Debug.LogError("Helium(Android): placementName passed is null cannot perform the operation requested");
				return null;
			}

			if (!Application.isEditor)
			{
				try
				{
					AndroidJavaObject androidAd = _plugin.Call<AndroidJavaObject>("getRewardedAd", placementName.ToString());
					HeliumRewardedAd ad = new HeliumRewardedAd(androidAd);
					return ad;
				}
				catch (Exception e)
				{
					Debug.LogError("Helium(Android): rewarded ad failed to load");
					Debug.LogError(e.ToString());
					return null;
				}
			}
			else
				return null;
		}

		public static HeliumBannerAd getBannerAd(string placementName, HeliumBannerAdSize size) {
			Log("Helium(Android): getBannerAd at placement = " + placementName);
			if (!checkInitialized()) {
				return null;
			} else if (placementName == null) {
				Debug.LogError("Helium(Android): placementName passed is null cannot perform the operation requested");
				return null;
			}

			if (!Application.isEditor)
			{
				try
				{
					AndroidJavaObject androidAd = _plugin.Call<AndroidJavaObject>("getBannerAd", placementName.ToString(), (int)size);
					HeliumBannerAd ad = new HeliumBannerAd(androidAd);
					return ad;
				}
				catch (Exception e)
				{
					Debug.LogError("Helium(Android): banner ad failed to load");
					Debug.LogError(e.ToString());
					return null;
				}
			}
			else
				return null;
		}

		/// Sets the name of the game object to be used by the Helium Android SDK
		public static void setGameObjectName(string name) {
			if (!Application.isEditor)
				_plugin.Call("setGameObjectName", name);
		}

		/// Informs the Helium SDK about the lifecycle of your app
		public static void pause(bool paused) {
			if (!checkInitialized())
				return;

			Log("Helium(Android): pause");
			if (!Application.isEditor)
				_plugin.Call("pause", paused);
		}

		/// Shuts down the Helium plugin
		public static void destroy() {
			if (!checkInitialized())
				return;

			Log("Helium(Android): destroy");
			if (!Application.isEditor)
				_plugin.Call("destroy");
			initialized = false;
		}

		/// Used to notify Helium that the Android back button has been pressed
		/// Returns true to indicate that Helium has handled the event and it should not be further processed
		public static bool onBackPressed() {
			bool handled = false;
			if (!checkInitialized())
				return false;

			Log("Helium(Android): onBackPressed");
			if (!Application.isEditor)
				handled = _plugin.Call<bool>("onBackPressed");
			return handled;
		}

#else
		private static string userIdentifier;

        /// Initializes the Helium SDK plugin.
        /// This must be called before using any other Helium SDK features.
        public static void init()
        {
            Log("Helium(Unsupported): init with version " + Application.unityVersion);
        }

        /// Initialize the Helium SDK plugin with a specific appId
        /// Either one of the init() methods must be called before using any other Helium SDK feature
        public static void initWithAppIdAndSignature(string appId, string appSignature)
        {
            Log("Helium(Unsupported): initWithAppIdAndSignature " + appId + ", " + appSignature + " and version " + Application.unityVersion);
        }

        public static HeliumInterstitialAd getInterstitialAd(string placementName)
        {
            Log("Helium(Unsupported): getInterstitialAd at placement = " + placementName);
            return null;
        }

        public static HeliumRewardedAd getRewardedAd(string placementName)
        {
            Log("Helium(Unsupported): getRewardedAd at placement = " + placementName);
            return null;
        }

        public static HeliumBannerAd getBannerAd(string placementName, HeliumBannerAdSize size)
        {
            Log("Helium(Unsupported): getBannerAd at placement = " + placementName);
            return null;
        }

        /// Sets the name of the game object to be used by the Helium iOS SDK
        public static void setGameObjectName(string name)
        {
            Log("Helium(Unsupported): Set Game object name for callbacks to = " + name);
        }

        public static void pause(bool paused)
        {
            Log("Helium(Unsupported): pause");
        }

        /// Shuts down the Helium plugin
        public static void destroy()
        {
            Log("Helium(Unsupported): destroy");
        }

        /// Used to notify Helium that the Android back button has been pressed
        /// Returns true to indicate that Helium has handled the event and it should not be further processed
        public static bool onBackPressed()
        {
            Log("Helium(Unsupported): onBackPressed");
            return true;
        }

        public static bool isAnyViewVisible()
        {
            Log("Helium(Unsupported): isAnyViewVisible");
            return false;
        }

        public static void setSubjectToCoppa(bool isSubject)
        {
            Log("Helium(Unsupported): setSubjectToCoppa " + isSubject);
        }

        public static void setSubjectToGDPR(bool isSubject)
        {
            Log("Helium(Unsupported): setSubjectToGDPR " + isSubject);
        }

        public static void setUserHasGivenConsent(bool hasGivenConsent)
        {
            Log("Helium(Unsupported): setUserHasGivenConsent " + hasGivenConsent);
        }

		public static void setCCPAConsent(bool hasGivenConsent)
        {
            Log("Helium(Unsupported): setCCPAConsent " + hasGivenConsent);
        }

		public static void setUserIdentifier(string userIdentifier)
        {
			Log("Helium(Unsupported): setUserIdentifier " + userIdentifier);
			HeliumExternal.userIdentifier = userIdentifier;
		}

		public static string getUserIdentifer()
        {
			Log("Helium(Unsupported): getUserIdentifer");
			return HeliumExternal.userIdentifier;
		}
#endif
	}
}

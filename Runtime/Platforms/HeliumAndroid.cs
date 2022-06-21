#if UNITY_ANDROID
using System;
using Helium.Platforms;
using UnityEngine;

namespace Helium.Platforms
{
    public class HeliumAndroid : HeliumExternal
    {
        private static AndroidJavaObject _plugin;
        
        public HeliumAndroid()
        {
            LOGTag = "Helium(Android)";
        }
        
        // Initialize the android bridge
        private static AndroidJavaObject plugin()
        {
            if (_plugin != null) 
                return _plugin;
            // find the plugin instance
            using var pluginClass = new AndroidJavaClass("com.chartboost.heliumsdk.unity.HeliumUnityBridge");
            _plugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
            return _plugin;
        }

        public override void Init()
        {
            base.Init();
            var appID = HeliumSettings.GetAndroidAppId();
            var appSignature = HeliumSettings.GetAndroidAppSignature();
            InitWithAppIdAndSignature(appID, appSignature);
        }

        public override void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            base.InitWithAppIdAndSignature(appId, appSignature);
            plugin().Call("start", appId, appSignature, Application.unityVersion, HeliumSDK.BackgroundEventListener.Instance);
            Initialized = true;
        }

        public override void SetSubjectToCoppa(bool isSubject)
        {
            base.SetSubjectToCoppa(isSubject);
            plugin().Call("setSubjectToCoppa", isSubject);
        }

        public override void SetSubjectToGDPR(bool isSubject)
        {
            base.SetSubjectToGDPR(isSubject);
            plugin().Call("setSubjectToGDPR", isSubject);
        }

        public override void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            base.SetUserHasGivenConsent(hasGivenConsent);
            plugin().Call("setUserHasGivenConsent", hasGivenConsent);
        }

        public override void SetCCPAConsent(bool hasGivenConsent)
        {
            base.SetCCPAConsent(hasGivenConsent);
            plugin().Call("setCCPAConsent", hasGivenConsent);
        }

        public override void SetUserIdentifier(string userIdentifier)
        {
            base.SetUserIdentifier(userIdentifier);
            plugin().Call("setUserIdentifier", userIdentifier);
        }

        public override string GetUserIdentifier()
        {
            base.GetUserIdentifier();
            return _plugin.Call<string>("getUserIdentifier");
        }

        public override void Pause(bool paused)
        {
            if (!CheckInitialized())
                return;
            base.Pause(paused);
            _plugin.Call("pause", paused);
        }

        public override void Destroy()
        {
            if (!CheckInitialized())
                return;
            base.Destroy();
            _plugin.Call("destroy");
            Initialized = false;
        }

        public override bool OnBackPressed()
        {
            var handled = base.OnBackPressed() && _plugin.Call<bool>("onBackPressed");
            return handled;
        }

        public override HeliumInterstitialAd GetInterstitialAd(string placementName)
        {
            if (!CanFetchAd(placementName))
                return null;
            
            base.GetInterstitialAd(placementName);
            
            try
            {
                var androidAd = _plugin.Call<AndroidJavaObject>("getInterstitialAd", placementName);
                var ad = new HeliumInterstitialAd(androidAd);
                return ad;
            }
            catch (Exception e)
            {
                LogError("interstitial failed to load");
                LogError(e.ToString());
                return null;
            }
        }

        public override HeliumRewardedAd GetRewardedAd(string placementName)
        {
            if (!CanFetchAd(placementName))
                return null;
            
            base.GetRewardedAd(placementName);
            
            try
            {
                var androidAd = _plugin.Call<AndroidJavaObject>("getRewardedAd", placementName);
                var ad = new HeliumRewardedAd(androidAd);
                return ad;
            }
            catch (Exception e)
            {
                LogError("rewarded ad failed to load");
                LogError(e.ToString());
                return null;
            }
        }

        public override HeliumBannerAd GetBannerAd(string placementName, HeliumBannerAdSize size)
        {
            if (!CanFetchAd(placementName))
                return null;
            
            base.GetBannerAd(placementName, size);
            
            try
            {
                var androidAd = _plugin.Call<AndroidJavaObject>("getBannerAd", placementName, (int)size);
                var ad = new HeliumBannerAd(androidAd);
                return ad;
            }
            catch (Exception e)
            {
                LogError("Helium(Android): banner ad failed to load");
                LogError(e.ToString());
                return null;
            }
        }

        public override void SetGameObjectName(string name)
        {
            base.SetGameObjectName(name);
            _plugin.Call("setGameObjectName", name);
        }
    }
}
#endif

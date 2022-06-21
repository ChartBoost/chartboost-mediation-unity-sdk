#if UNITY_IPHONE
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Helium.Platforms
{
    public class HeliumIOS : HeliumExternal
    {
        [DllImport("__Internal")]
        private static extern void _heliumSdkInit(string appId, string appSignature, string unityVersion,
            HeliumSDK.BackgroundEventListener.Delegate callback);

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
        private static extern void _heliumSetUserIdentifier(string userIdentifier);

        [DllImport("__Internal")]
        private static extern string _heliumGetUserIdentifier();


        public HeliumIOS()
        {
            LOGTag = "Helium(iOS)";
        }

        public override void Init()
        {
            base.Init();
            var appID = HeliumSettings.GetIOSAppId();
            var appSignature = HeliumSettings.GetIOSAppSignature();
            InitWithAppIdAndSignature(appID, appSignature);
        }

        public override void InitWithAppIdAndSignature(string appId, string appSignature)
        {
            base.InitWithAppIdAndSignature(appId, appSignature);
            _heliumSdkInit(appId, appSignature, Application.unityVersion, HeliumSDK.BackgroundEventListener.SendEvent);
            Initialized = true;
        }

        public override void SetSubjectToCoppa(bool isSubject)
        {
            base.SetSubjectToCoppa(isSubject);
            _heliumSdkSetSubjectToCoppa(isSubject);
        }

        public override void SetSubjectToGDPR(bool isSubject)
        {
            base.SetSubjectToGDPR(isSubject);
            _heliumSdkSetSubjectToGDPR(isSubject);
        }

        public override void SetUserHasGivenConsent(bool hasGivenConsent)
        {
            base.SetUserHasGivenConsent(hasGivenConsent);
            _heliumSdkSetUserHasGivenConsent(hasGivenConsent);
        }

        public override void SetCCPAConsent(bool hasGivenConsent)
        {
            base.SetCCPAConsent(hasGivenConsent);
            _heliumSetCCPAConsent(hasGivenConsent);
        }

        public override void SetUserIdentifier(string userIdentifier)
        {
            base.SetUserIdentifier(userIdentifier);
            _heliumSetUserIdentifier(userIdentifier);
        }

        public override string GetUserIdentifier()
        {
            base.GetUserIdentifier();
            return _heliumGetUserIdentifier();
        }

        public override HeliumInterstitialAd GetInterstitialAd(string placementName)
        {
            if (!CanFetchAd(placementName))
                return null;

            base.GetInterstitialAd(placementName);

            var adId = _heliumSdkGetInterstitialAd(placementName);
            return adId == IntPtr.Zero ? null : new HeliumInterstitialAd(adId);
        }

        public override HeliumRewardedAd GetRewardedAd(string placementName)
        {
            if (!CanFetchAd(placementName))
                return null;

            base.GetRewardedAd(placementName);

            var adId = _heliumSdkGetRewardedAd(placementName);
            if (adId == IntPtr.Zero)
                return null;
            
            return adId == IntPtr.Zero ? null : new HeliumRewardedAd(adId);
        }
        
        public override HeliumBannerAd GetBannerAd(string placementName,  HeliumBannerAdSize size)
        {
            if (!CanFetchAd(placementName))
                return null;

            base.GetBannerAd(placementName, size);

            var adId = _heliumSdkGetBannerAd(placementName, (int)size);
            if (adId == IntPtr.Zero)
                return null;
            
            return adId == IntPtr.Zero ? null : new HeliumBannerAd(adId);
        }

        public override void SetGameObjectName(string name)
        {
            base.SetGameObjectName(name);
            _heliumSdkSetGameObjectName(name);
        }
    }
}
#endif

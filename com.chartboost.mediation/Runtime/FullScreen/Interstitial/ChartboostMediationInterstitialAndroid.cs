#if UNITY_ANDROID
using Chartboost.Platforms;
using Chartboost.Platforms.Android;
using UnityEngine;

namespace Chartboost.FullScreen.Interstitial
{
    /// <summary>
    /// ChartboostMediation interstitial object for Android.
    /// </summary>
    public class ChartboostMediationInterstitialAndroid : ChartboostMediationFullScreenBase
    {
        private readonly AndroidJavaObject _androidAd;

        public ChartboostMediationInterstitialAndroid(string placementName) : base(placementName)
        {
            logTag = "ChartboostMediationInterstitial (Android)";
            using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
            _androidAd = unityBridge.CallStatic<AndroidJavaObject>("getInterstitialAd", placementName);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _androidAd.Call<bool>("setKeyword", keyword, value);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _androidAd.Call<string>("removeKeyword", keyword);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.Destroy"/>>
        public override void Destroy()
        {
            base.Destroy();
            _androidAd.Call("destroy");
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.Load"/>>
        public override void Load()
        {
            base.Load();
            _androidAd.Call("load");
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.Show"/>>
        public override void Show()
        {
            base.Show();
            _androidAd.Call("show");
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.ReadyToShow"/>>
        public override bool ReadyToShow()
        {
            base.ReadyToShow();
            return _androidAd.Call<bool>("readyToShow");
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _androidAd.Call("clearLoaded");
        }
    }
}
#endif

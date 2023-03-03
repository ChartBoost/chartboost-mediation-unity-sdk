#if UNITY_ANDROID
using Chartboost.Platforms;
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
            LogTag = "ChartboostMediationInterstitial (Android)";
            _androidAd = ChartboostMediationAndroid.plugin().Call<AndroidJavaObject>("getInterstitialAd", placementName);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _androidAd.Call<bool>("setKeyword", keyword, value);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _androidAd.Call<string>("removeKeyword", keyword);
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Destroy"/>>
        public override void Destroy()
        {
            base.Destroy();
            _androidAd.Call("destroy");
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Load"/>>
        public override void Load()
        {
            base.Load();
            _androidAd.Call("load");
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Show"/>>
        public override void Show()
        {
            base.Show();
            _androidAd.Call("show");
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.ReadyToShow"/>>
        public override bool ReadyToShow()
        {
            base.ReadyToShow();
            return _androidAd.Call<bool>("readyToShow");
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _androidAd.Call<bool>("clearLoaded");
        }
    }
}
#endif
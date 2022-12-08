#if UNITY_ANDROID
using Helium.Platforms;
using UnityEngine;

namespace Helium.FullScreen.Interstitial
{
    /// <summary>
    /// Helium interstitial object for Android.
    /// </summary>
    public class HeliumInterstitialAndroid : HeliumFullScreenBase
    {
        private readonly AndroidJavaObject _androidAd;

        public HeliumInterstitialAndroid(string placementName) : base(placementName)
        {
            LogTag = "HeliumInterstitial (Android)";
            _androidAd = HeliumAndroid.plugin().Call<AndroidJavaObject>("getInterstitialAd", placementName);
        }

        /// <inheritdoc cref="HeliumFullScreenBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _androidAd.Call<bool>("setKeyword", keyword, value);
        }

        /// <inheritdoc cref="HeliumFullScreenBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _androidAd.Call<string>("removeKeyword", keyword);
        }

        /// <inheritdoc cref="HeliumFullScreenBase.Destroy"/>>
        public override void Destroy()
        {
            base.Destroy();
            _androidAd.Call("destroy");
        }

        /// <inheritdoc cref="HeliumFullScreenBase.Load"/>>
        public override void Load()
        {
            base.Load();
            _androidAd.Call("load");
        }

        /// <inheritdoc cref="HeliumFullScreenBase.Show"/>>
        public override void Show()
        {
            base.Show();
            _androidAd.Call("show");
        }

        /// <inheritdoc cref="HeliumFullScreenBase.ReadyToShow"/>>
        public override bool ReadyToShow()
        {
            base.ReadyToShow();
            return _androidAd.Call<bool>("readyToShow");
        }

        /// <inheritdoc cref="HeliumFullScreenBase.ClearLoaded"/>>
        public override bool ClearLoaded()
        {
            base.ClearLoaded();
            return _androidAd.Call<bool>("clearLoaded");
        }
    }
}
#endif

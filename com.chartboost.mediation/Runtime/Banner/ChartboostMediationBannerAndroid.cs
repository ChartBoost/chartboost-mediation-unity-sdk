#if UNITY_ANDROID
using Chartboost.Interfaces;
using Chartboost.Platforms.Android;
using UnityEngine;

namespace Chartboost.Banner
{
    /// <summary>
    /// Chartboost Mediation banner object for Android.
    /// </summary>
    public sealed class ChartboostMediationBannerAndroid : ChartboostMediationBannerBase
    {
        private readonly AndroidJavaObject _androidAd;

        public ChartboostMediationBannerAndroid(string placementName, ChartboostMediationBannerAdSize size) : base(placementName, size)
        {
            LogTag = "ChartboostMediationBanner (Android)";
            using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
            _androidAd = unityBridge.CallStatic<AndroidJavaObject>("getBannerAd", placementName, (int)size);
        }

        internal override bool IsValid { get; set; } = true;

        /// <inheritdoc cref="IChartboostMediationAd.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
        {
            base.SetKeyword(keyword, value);
            return _androidAd.Call<bool>("setKeyword", keyword, value);
        }

        /// <inheritdoc cref="IChartboostMediationAd.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
        {
            base.RemoveKeyword(keyword);
            return _androidAd.Call<string>("removeKeyword", keyword);
        }

        /// <inheritdoc cref="IChartboostMediationAd.Destroy"/>>
        public override void Destroy()
        {
            base.Destroy();
            _androidAd.Call("destroy");
            IsValid = false;
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.Load"/>>
        public override void Load(ChartboostMediationBannerAdScreenLocation location)
        {
            base.Load(location);
            _androidAd.Call("load", (int)location);
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.SetVisibility"/>>
        public override void SetVisibility(bool isVisible)
        {
            base.SetVisibility(isVisible);
            _androidAd.Call("setBannerVisibility", isVisible);
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            base.ClearLoaded();
            _androidAd.Call("clearLoaded");
        }

        /// <inheritdoc cref="IChartboostMediationBannerAd.Remove"/>>
        public override void Remove()
        {
            //android doesn't have a remove method. Instead, calling destroy
            Destroy();
        }
    }
}
#endif

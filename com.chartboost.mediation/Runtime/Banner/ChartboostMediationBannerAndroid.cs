#if UNITY_ANDROID
using System;
using Chartboost.Interfaces;
using Chartboost.Platforms.Android;
using Chartboost.Utilities;
using UnityEngine;
using Logger = Chartboost.Utilities.Logger;

namespace Chartboost.Banner
{
    /// <summary>
    /// Chartboost Mediation banner object for Android.
    /// </summary>
    public sealed class ChartboostMediationBannerAndroid : ChartboostMediationBannerBase
    {
        private readonly AndroidJavaObject _androidAd;
        private readonly int _uniqueId;

        public ChartboostMediationBannerAndroid(string placementName, ChartboostMediationBannerAdSize size) : base(placementName, size)
        {
            LogTag = "ChartboostMediationBanner (Android)";
            
            if (size.Name == "ADAPTIVE")
            {
                Logger.LogError(LogTag,$"Adaptive sizes are not supported for `ChartboostMediationBannerAd`. Use `ChartboostMediationBannerView` instead");
                return;
            }
            
            var fixedSize = size.Name switch
            {
                "STANDARD" => 0,
                "MEDIUM" => 1,
                "LEADERBOARD" => 2,
                _ => 0
            };
            
            using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
            _androidAd = unityBridge.CallStatic<AndroidJavaObject>("getBannerAd", placementName, fixedSize);
            _uniqueId = _androidAd.HashCode();
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
            IsValid = false;
            _androidAd.Dispose();
            AndroidAdStore.ReleaseLegacyAd(_uniqueId);
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
        [Obsolete("Remove has been deprecated, please use Destroy instead.")]
        public override void Remove()
        {
            //android doesn't have a remove method. Instead, calling destroy
            base.Remove();
            Destroy();
        }
    }
}
#endif

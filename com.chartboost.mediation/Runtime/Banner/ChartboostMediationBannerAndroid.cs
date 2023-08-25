#if UNITY_ANDROID
using System;
using Chartboost.Interfaces;
using Chartboost.Platforms.Android;
using Chartboost.Utilities;
using UnityEngine;

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
            using var unityBridge = ChartboostMediationAndroid.GetUnityBridge();
            {
                AndroidJavaObject nativeSize = null;
                var sizeClass = new AndroidJavaClass(ChartboostMediationAndroid.GetQualifiedNativeClassName("HeliumBannerSize"));

                if (size.BannerType == ChartboostMediationBannerType.Adaptive)
                {
                    nativeSize = sizeClass.CallStatic<AndroidJavaObject>("bannerSize", size.Width, size.Height);
                }
                else
                {
                    nativeSize = size.Width switch
                    {
                        320 => sizeClass.CallStatic<AndroidJavaObject>("STANDARD"),
                        300 => sizeClass.CallStatic<AndroidJavaObject>("MEDIUM"),
                        728 => sizeClass.CallStatic<AndroidJavaObject>("LEADERBOARD"),
                        _ => null
                    };
                }
                
                _androidAd = unityBridge.CallStatic<AndroidJavaObject>("getBannerAd", placementName, nativeSize);
                _uniqueId = _androidAd.HashCode();
            }
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

        public override void SetHorizontalAlignment(ChartboostMediationBannerHorizontalAlignment horizontalAlignment)
        {
            // TODO
            base.SetHorizontalAlignment(horizontalAlignment);
            _androidAd.Call("SetHorizontalAlignment", (int)horizontalAlignment);

        }

        public override void SetVerticalAlignment(ChartboostMediationBannerVerticalAlignment verticalAlignment)
        {
            // TODO
            base.SetVerticalAlignment(verticalAlignment);
            _androidAd.Call("setVerticalAlignment", (int)verticalAlignment);
        }

        public override ChartboostMediationBannerAdSize GetAdSize()
        {
            base.GetAdSize();
            var nativeSize = _androidAd.Call<AndroidJavaObject>("getSize");

            var width = nativeSize.Call<int>("width");
            var height = nativeSize.Call<int>("height");
            var isAdaptive =  nativeSize.Call<bool>("isAdaptive");
            if (isAdaptive)
            {
                return ChartboostMediationBannerAdSize.Adaptive(width, height);
            }

            var name = nativeSize.Call<string>("name");
            return name switch
            {
                "STANDARD" => ChartboostMediationBannerAdSize.Standard,
                "MEDIUM" => ChartboostMediationBannerAdSize.MediumRect,
                "LEADERBOARD" => ChartboostMediationBannerAdSize.Leaderboard,
                _ => null
            };
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

using System;

namespace Chartboost.FullScreen.Interstitial
{
    /// <summary>
    /// Chartboost Mediation interstitial ad object.
    /// </summary>
    [Obsolete("ChartboostMediationInterstitialAd has been deprecated, use the new fullscreen API instead.")]
    public class ChartboostMediationInterstitialAd : ChartboostMediationFullScreenBase
    {
        private readonly ChartboostMediationFullScreenBase _platformInterstitial;

        public ChartboostMediationInterstitialAd(string placementName) : base(placementName)
        {
            #if UNITY_EDITOR
            _platformInterstitial = new ChartboostMediationInterstitialUnsupported(placementName);
            #elif UNITY_ANDROID
            _platformInterstitial = new ChartboostMediationInterstitialAndroid(placementName);
            #elif UNITY_IOS
            _platformInterstitial = new ChartboostMediationInterstitialIOS(placementName);
            #else
            _platformInterstitial = new ChartboostMediationInterstitialUnsupported(placementName);
            #endif
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
            => _platformInterstitial.SetKeyword(keyword, value);

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
            => _platformInterstitial.RemoveKeyword(keyword);

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Destroy"/>>
        public override void Destroy()
            => _platformInterstitial.Destroy();

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Load"/>>
        public override void Load()
            => _platformInterstitial.Load();

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Show"/>>
        public override void Show()
            => _platformInterstitial.Show();

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.ReadyToShow"/>>
        public override bool ReadyToShow()
            => _platformInterstitial.ReadyToShow();

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.ClearLoaded"/>>
        public override void ClearLoaded()
            => _platformInterstitial.ClearLoaded();
    }
}

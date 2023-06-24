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

        internal ChartboostMediationInterstitialAd(string placementName) : base(placementName)
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

        internal override bool IsValid { get => _platformInterstitial.IsValid; set => _platformInterstitial.IsValid = value; }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value) 
            => IsValid && _platformInterstitial.SetKeyword(keyword, value);

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
            => IsValid ? _platformInterstitial.RemoveKeyword(keyword) : null; 

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Destroy"/>>
        public override void Destroy()
        {
            if (IsValid)
               _platformInterstitial.Destroy();
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Load"/>>
        public override void Load()
        {
            if (IsValid)
                _platformInterstitial.Load();
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.Show"/>>
        public override void Show()
        {
            if (IsValid)
                _platformInterstitial.Show();
        }

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.ReadyToShow"/>>
        public override bool ReadyToShow() 
            =>  IsValid && _platformInterstitial.ReadyToShow();

        /// <inheritdoc cref="ChartboostMediationFullScreenBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            if (IsValid)
                _platformInterstitial.ClearLoaded();
        }

        ~ChartboostMediationInterstitialAd() => Destroy();
    }
}

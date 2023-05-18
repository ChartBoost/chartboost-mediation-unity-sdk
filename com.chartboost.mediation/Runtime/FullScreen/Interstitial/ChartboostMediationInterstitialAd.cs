namespace Chartboost.FullScreen.Interstitial
{
    /// <summary>
    /// Chartboost Mediation interstitial ad object.
    /// </summary>
    public class ChartboostMediationInterstitialAd : ChartboostMediationFullScreenBaseOLD
    {
        private readonly ChartboostMediationFullScreenBaseOLD _platformInterstitial;

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

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
            => _platformInterstitial.SetKeyword(keyword, value);

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
            => _platformInterstitial.RemoveKeyword(keyword);

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.Destroy"/>>
        public override void Destroy()
            => _platformInterstitial.Destroy();

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.Load"/>>
        public override void Load()
            => _platformInterstitial.Load();

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.Show"/>>
        public override void Show()
            => _platformInterstitial.Show();

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.ReadyToShow"/>>
        public override bool ReadyToShow()
            => _platformInterstitial.ReadyToShow();

        /// <inheritdoc cref="ChartboostMediationFullScreenBaseOLD.ClearLoaded"/>>
        public override void ClearLoaded()
            => _platformInterstitial.ClearLoaded();
    }
}

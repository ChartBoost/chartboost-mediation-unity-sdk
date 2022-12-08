namespace Helium.FullScreen.Interstitial
{
    /// <summary>
    /// Helium interstitial ad object.
    /// </summary>
    public class HeliumInterstitialAd : HeliumFullScreenBase
    {
        private readonly HeliumFullScreenBase _platformInterstitial;

        public HeliumInterstitialAd(string placementName) : base(placementName)
        {
            #if UNITY_ANDROID
            _platformInterstitial = new HeliumInterstitialAndroid(placementName);
            #elif UNITY_IOS
			_platformInterstitial = new HeliumInterstitialIOS(placementName);
            #else
            _platformInterstitial = new HeliumInterstitialUnsupported(placementName);
            #endif
        }

        /// <inheritdoc cref="HeliumFullScreenBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value)
            => _platformInterstitial.SetKeyword(keyword, value);

        /// <inheritdoc cref="HeliumFullScreenBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword)
            => _platformInterstitial.RemoveKeyword(keyword);

        /// <inheritdoc cref="HeliumFullScreenBase.Destroy"/>>
        public override void Destroy()
            => _platformInterstitial.Destroy();

        /// <inheritdoc cref="HeliumFullScreenBase.Load"/>>
        public override void Load()
            => _platformInterstitial.Load();

        /// <inheritdoc cref="HeliumFullScreenBase.Show"/>>
        public override void Show()
            => _platformInterstitial.Show();

        /// <inheritdoc cref="HeliumFullScreenBase.ReadyToShow"/>>
        public override bool ReadyToShow()
            => _platformInterstitial.ReadyToShow();

        /// <inheritdoc cref="HeliumFullScreenBase.ClearLoaded"/>>
        public override bool ClearLoaded()
            => _platformInterstitial.ClearLoaded();
    }
}

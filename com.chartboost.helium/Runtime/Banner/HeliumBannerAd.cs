namespace Helium.Banner
{
    /// <summary>
    /// Helium defined banner sizes.
    /// </summary>
    public enum HeliumBannerAdSize
    {
        /// 320 x 50
        Standard = 0,
        /// 300 x 250
        MediumRect = 1,
        /// 728 x 90
        Leaderboard = 2
    }

    /// <summary>
    /// Helium defined screen locations.
    /// </summary>
    public enum HeliumBannerAdScreenLocation
    {
        TopLeft = 0,
        TopCenter = 1,
        TopRight = 2,
        Center = 3,
        BottomLeft = 4,
        BottomCenter = 5,
        BottomRight = 6
    }

    /// <summary>
    /// Helium banner ad object.
    /// </summary>
    public class HeliumBannerAd : HeliumBannerBase
    {
        private readonly HeliumBannerBase _platformBanner;

        public HeliumBannerAd(string placementName, HeliumBannerAdSize size) : base(placementName, size)
        {
            #if UNITY_ANDROID
            _platformBanner = new HeliumBannerAndroid(placementName, size);
            #elif UNITY_IOS
            _platformBanner = new HeliumBannerIOS(placementName, size);
            #else
            _platformBanner = new HeliumBannerUnsupported(placementName, size);
            #endif
        }

        /// <inheritdoc cref="HeliumBannerBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value) 
            => _platformBanner.SetKeyword(keyword, value);
        
        /// <inheritdoc cref="HeliumBannerBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword) 
            => _platformBanner.RemoveKeyword(keyword);
        
        /// <inheritdoc cref="HeliumBannerBase.Destroy"/>>
        public override void Destroy() 
            => _platformBanner.Destroy();
        
        /// <inheritdoc cref="HeliumBannerBase.Load"/>>
        public override void Load(HeliumBannerAdScreenLocation location) 
            => _platformBanner.Load(location);
        
        /// <inheritdoc cref="HeliumBannerBase.SetVisibility"/>>
        public override void SetVisibility(bool isVisible) 
            =>_platformBanner.SetVisibility(isVisible);

        /// <inheritdoc cref="HeliumBannerBase.ClearLoaded"/>>
        public override void ClearLoaded() 
            => _platformBanner.ClearLoaded();

        /// <inheritdoc cref="HeliumBannerBase.Remove"/>>
        public override void Remove()
            =>_platformBanner.Remove();
    }
}

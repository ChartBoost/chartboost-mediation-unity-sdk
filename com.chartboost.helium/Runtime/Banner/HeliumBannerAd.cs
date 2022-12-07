namespace Helium.Banner
{
    public enum HeliumBannerAdSize
    {
        /// 320 x 50
        Standard = 0,
        /// 300 x 250
        MediumRect = 1,
        /// 720 x 90
        Leaderboard = 2
    }

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

    public class HeliumBannerAd
    {
        private readonly HeliumBannerBase _banner;

        public HeliumBannerAd(string placementName, HeliumBannerAdSize size)
        {
            #if UNITY_ANDROID
            _banner = new HeliumBannerAndroid(placementName, size);
            #elif UNITY_IOS
            _banner = new HeliumBannerIOS(placementName, size);
            #else
            _banner = new HeliumBannerUnsupported(placementName, size);
            #endif
        }

        /// <inheritdoc cref="IHeliumAd.SetKeyword"/>>
        public bool SetKeyword(string keyword, string value) => _banner.SetKeyword(keyword, value);
        
        /// <inheritdoc cref="IHeliumAd.RemoveKeyword"/>>
        public string RemoveKeyword(string keyword) => _banner.RemoveKeyword(keyword);
        
        /// <inheritdoc cref="IHeliumAd.Destroy"/>>
        public void Destroy() => _banner.Destroy();
        
        /// <inheritdoc cref="IHeliumBannerAd.Load"/>>
        public void Load(HeliumBannerAdScreenLocation location) => _banner.Load(location);
        
        /// <inheritdoc cref="IHeliumBannerAd.SetVisibility"/>>
        public void SetVisibility(bool isVisible) =>_banner.SetVisibility(isVisible);

        /// <inheritdoc cref="IHeliumBannerAd.ClearLoaded"/>>
        public void ClearLoaded() => _banner.ClearLoaded();

        /// <inheritdoc cref="IHeliumBannerAd.Remove"/>>
        public void Remove() =>_banner.Remove();
    }
}

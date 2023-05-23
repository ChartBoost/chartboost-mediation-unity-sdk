using System;

namespace Chartboost.Banner
{
    /// <summary>
    /// Chartboost Mediation defined banner sizes.
    /// </summary>
    public enum ChartboostMediationBannerAdSize
    {
        /// 320 x 50
        Standard = 0,
        /// 300 x 250
        MediumRect = 1,
        /// 728 x 90
        Leaderboard = 2
    }

    /// <summary>
    /// Chartboost Mediation defined screen locations.
    /// </summary>
    public enum ChartboostMediationBannerAdScreenLocation
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
    /// Chartboost Mediation banner ad object.
    /// </summary>
    public class ChartboostMediationBannerAd : ChartboostMediationBannerBase
    {
        private readonly ChartboostMediationBannerBase _platformBanner;

        public ChartboostMediationBannerAd(string placementName, ChartboostMediationBannerAdSize size) : base(placementName, size)
        {
            #if UNITY_EDITOR
            _platformBanner = new ChartboostMediationBannerUnsupported(placementName, size);
            #elif UNITY_ANDROID
            _platformBanner = new ChartboostMediationBannerAndroid(placementName, size);
            #elif UNITY_IOS
            _platformBanner = new ChartboostMediationBannerIOS(placementName, size);
            #else
            _platformBanner = new ChartboostMediationBannerUnsupported(placementName, size);
            #endif
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value) 
            => _platformBanner.SetKeyword(keyword, value);
        
        /// <inheritdoc cref="ChartboostMediationBannerBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword) 
            => _platformBanner.RemoveKeyword(keyword);
        
        /// <inheritdoc cref="ChartboostMediationBannerBase.Destroy"/>>
        public override void Destroy() 
            => _platformBanner.Destroy();
        
        /// <inheritdoc cref="ChartboostMediationBannerBase.Load"/>>
        public override void Load(ChartboostMediationBannerAdScreenLocation location) 
            => _platformBanner.Load(location);

        /// <inheritdoc cref="ChartboostMediationBannerBase.Load(float, float, int, int)"/>>
        public override void Load(float x, float y, int width, int height)
            => _platformBanner.Load(x, y, width, height);

        /// <inheritdoc cref="ChartboostMediationBannerBase.SetVisibility"/>>
        public override void SetVisibility(bool isVisible) 
            =>_platformBanner.SetVisibility(isVisible);

        /// <inheritdoc cref="ChartboostMediationBannerBase.ClearLoaded"/>>
        public override void ClearLoaded() 
            => _platformBanner.ClearLoaded();

        /// <inheritdoc cref="ChartboostMediationBannerBase.Remove"/>>
        public override void Remove()
            =>_platformBanner.Remove();

        public override void EnableDrag(Action<float, float> onDrag = null)
            => _platformBanner.EnableDrag(onDrag);

        public override void DisableDrag()
            => _platformBanner.DisableDrag();
    }
}

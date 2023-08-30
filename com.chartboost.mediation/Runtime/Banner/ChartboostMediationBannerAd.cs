using System;
using Chartboost.Events;

namespace Chartboost.Banner
{
    public enum ChartboostMediationBannerHorizontalAlignment
    {
        Left,
        Center,
        Right
    }
    
    public enum ChartboostMediationBannerVerticalAlignment
    {
        Top,
        Center,
        Bottom
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
    public sealed class ChartboostMediationBannerAd : ChartboostMediationBannerBase
    {
        private readonly ChartboostMediationBannerBase _platformBanner;

        internal ChartboostMediationBannerAd(string placementName, ChartboostMediationBannerAdSize size) : base(placementName, size)
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

        internal override bool IsValid { get => _platformBanner.IsValid; set => _platformBanner.IsValid = value; }

        /// <inheritdoc cref="ChartboostMediationBannerBase.SetKeyword"/>>
        public override bool SetKeyword(string keyword, string value) 
            => IsValid && _platformBanner.SetKeyword(keyword, value);
        
        /// <inheritdoc cref="ChartboostMediationBannerBase.RemoveKeyword"/>>
        public override string RemoveKeyword(string keyword) 
            => IsValid ? _platformBanner.RemoveKeyword(keyword) : null;

        /// <inheritdoc cref="ChartboostMediationBannerBase.SetHorizontalAlignment"/>
        public override void SetHorizontalAlignment(ChartboostMediationBannerHorizontalAlignment horizontalAlignment)
        {
            if(IsValid) _platformBanner.SetHorizontalAlignment(horizontalAlignment);
        }
        
        /// <inheritdoc cref="ChartboostMediationBannerBase.SetVerticalAlignment"/>
        public override void SetVerticalAlignment(ChartboostMediationBannerVerticalAlignment verticalAlignment)
        {
            if(IsValid) _platformBanner.SetVerticalAlignment(verticalAlignment);
        }
        
        /// <inheritdoc cref="ChartboostMediationBannerBase.GetAdSize"/>
        public override ChartboostMediationBannerAdSize GetAdSize()
            => IsValid ? _platformBanner.GetAdSize() : null;

        /// <inheritdoc cref="ChartboostMediationBannerBase.Destroy"/>>
        public override void Destroy()
        {
            Destroy(false);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.Load"/>>
        public override void Load(ChartboostMediationBannerAdScreenLocation location)
        {
            if (IsValid)
                _platformBanner.Load(location);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.SetVisibility"/>>
        public override void SetVisibility(bool isVisible)
        {
            if (IsValid)
                _platformBanner.SetVisibility(isVisible);
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.ClearLoaded"/>>
        public override void ClearLoaded()
        {
            if (IsValid)
                _platformBanner.ClearLoaded();
        }

        /// <inheritdoc cref="ChartboostMediationBannerBase.Remove"/>>
        [Obsolete("Remove has been deprecated, please use Destroy instead.")]
        public override void Remove()
        {
            if (IsValid)
                _platformBanner.Remove();
        }

        private void Destroy(bool isCollected)
        {
            if (!IsValid)
                return;
            _platformBanner.Destroy();
            base.Destroy();
            
            if (isCollected) 
                EventProcessor.ReportUnexpectedSystemError($"Banner Ad with placement: {placementName}, got GC. Make sure to properly dispose of ads utilizing Destroy for the best integration experience.");
        }

        ~ChartboostMediationBannerAd() => Destroy(true);
    }
}

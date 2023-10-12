using System;

namespace Chartboost.Banner
{
    /// <summary>
    /// Interface implemented by all Chartboost Mediation banners. 
    /// </summary>
    [Obsolete("IChartboostMediationBannerAd has been deprecated, use the new ChartboostMediationBannerView API instead.")]
    public interface IChartboostMediationBannerAd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        void Load(ChartboostMediationBannerAdScreenLocation location);

        /// <summary>This method changes the visibility of the banner ad.</summary>
        /// <param name="isVisible">Specify if the banner should be visible.</param>
        void SetVisibility(bool isVisible);
        
        /// <summary>
        /// If an advertisement has been loaded, clear it. Once cleared, a new
        /// load can be performed.
        /// </summary>
        void ClearLoaded();
        
        /// <summary>
        /// Remove the banner.
        /// </summary>
        [Obsolete("Remove has been deprecated, please use Destroy instead.")] 
        void Remove();
    }
}

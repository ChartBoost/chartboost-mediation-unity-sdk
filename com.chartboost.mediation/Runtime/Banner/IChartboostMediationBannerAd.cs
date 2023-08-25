using System;

namespace Chartboost.Banner
{
    /// <summary>
    /// Interface implemented by all Chartboost Mediation banners. 
    /// </summary>
    public interface IChartboostMediationBannerAd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="placementName"></param>
        /// <param name="size"></param>
        /// <param name="location"></param>
        void Load(ChartboostMediationBannerAdScreenLocation location);

        /// <summary>This method changes the visibility of the banner ad.</summary>
        /// <param name="isVisible">Specify if the banner should be visible.</param>
        void SetVisibility(bool isVisible);
        
        /// <summary>
        /// This method sets the horizontal alignment of the banner ad within its container  
        /// </summary>
        /// <param name="horizontalAlignment"></param>
        void SetHorizontalAlignment(ChartboostMediationBannerHorizontalAlignment horizontalAlignment);
        
        /// <summary>
        /// This method sets the vertical alignment of the banner ad within its container
        /// </summary>
        /// <param name="verticalAlignment"></param>
        void SetVerticalAlignment(ChartboostMediationBannerVerticalAlignment verticalAlignment);

        /// <summary>
        /// Gets the size of banner ad loaded inside its container
        /// </summary>
        /// <returns></returns>
        ChartboostMediationBannerAdSize GetAdSize();
        
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

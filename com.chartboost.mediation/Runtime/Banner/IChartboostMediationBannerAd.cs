using UnityEngine.Scripting;

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
        /// <param name="location"></param>
        [Preserve]
        void Load(ChartboostMediationBannerAdScreenLocation location);

        /// <summary>This method changes the visibility of the banner ad.</summary>
        /// <param name="isVisible">Specify if the banner should be visible.</param>
        [Preserve]
        void SetVisibility(bool isVisible);
        
        /// <summary>
        /// If an advertisement has been loaded, clear it. Once cleared, a new
        /// load can be performed.
        /// </summary>
        [Preserve]
        void ClearLoaded();
        
        /// <summary>
        /// Remove the banner.
        /// </summary>
        [Preserve]
        void Remove();
    }
}

using UnityEngine.Scripting;

namespace Helium.Banner
{
    /// <summary>
    /// Interface implemented by all Helium banners. 
    /// </summary>
    public interface IHeliumBannerAd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        [Preserve]
        void Load(HeliumBannerAdScreenLocation location);

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
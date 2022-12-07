namespace Helium.Banner
{
    public interface IHeliumBannerAd
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        void Load(HeliumBannerAdScreenLocation location);

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
        void Remove();
    }
}

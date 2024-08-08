namespace Chartboost.Mediation.Ad.Banner
{
    /// <summary>
    /// Generic <see cref="IBannerAd"/> event.
    /// <param name="bannerAd"> The <see cref="IBannerAd"/> that triggers the event.</param>
    /// </summary>
    public delegate void BannerAdEvent(IBannerAd bannerAd);
    
    /// <summary>
    /// Reports a <see cref="IBannerAd"/> dragging motion.
    /// <param name="bannerAd"> The <see cref="IBannerAd"/> that triggers the drag event.</param>
    /// </summary>
    public delegate void BannerAdDragEvent(IBannerAd bannerAd, float x, float y);
}

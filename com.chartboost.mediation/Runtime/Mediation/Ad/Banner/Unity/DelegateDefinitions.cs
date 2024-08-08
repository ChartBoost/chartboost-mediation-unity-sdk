namespace Chartboost.Mediation.Ad.Banner.Unity
{
    /// <summary>
    /// Generic <see cref="UnityBannerAd"/> event.
    /// <param name="unityBannerAd"> The <see cref="UnityBannerAd"/> that triggers the event.</param>
    /// </summary>
    public delegate void UnityBannerAdEvent(UnityBannerAd unityBannerAd);

    /// <summary>
    /// Reports a <see cref="UnityBannerAd"/> dragging motion.
    /// <param name="unityBannerAd"> The <see cref="UnityBannerAd"/> that triggers the drag event.</param>
    /// </summary>
    public delegate void UnityBannerAdDragEvent(UnityBannerAd unityBannerAd, float x, float y);
}

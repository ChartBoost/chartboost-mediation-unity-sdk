namespace Chartboost.Mediation.Ad.Banner.UIToolkit
{
    /// <summary>
    /// Generic <see cref="BannerVisualElement"/> event.
    /// <param name="bannerVisualElement"> The <see cref="BannerVisualElement"/> that triggers the event.</param>
    /// </summary>
    public delegate void BannerVisualElementAdEvent(BannerVisualElement bannerVisualElement);

    /// <summary>
    /// Reports a <see cref="BannerVisualElement"/> dragging motion.
    /// <param name="bannerVisualElement"> The <see cref="BannerVisualElement"/> that triggers the drag event.</param>
    /// </summary>
    public delegate void BannerVisualElementAdDragEvent(BannerVisualElement bannerVisualElement, float x, float y);
}

using Chartboost.Mediation.Error;

namespace Chartboost.Mediation.Ad.Fullscreen
{
    /// <summary>
    /// Generic <see cref="IFullscreenAd"/> event.
    /// <param name="ad">The <see cref="IFullscreenAd"/> triggering the event.</param>
    /// </summary>
    public delegate void FullscreenAdEvent(IFullscreenAd ad);

    /// <summary>
    /// Generic <see cref="IFullscreenAd"/> even with a possible error.
    /// <param name="ad">The <see cref="IFullscreenAd"/> triggering the event.</param>
    /// <param name="error">The <see cref="ChartboostMediationError"/> that occurred during the event, if any.</param>
    /// </summary>
    public delegate void FullscreenAdEventWithError(IFullscreenAd ad, ChartboostMediationError? error);
}

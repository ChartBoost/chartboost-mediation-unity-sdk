using Chartboost.Mediation.Ad.Fullscreen;

namespace Chartboost.Mediation.Utilities.Events
{
    /// <summary>
    /// Possible <see cref="IFullscreenAd"/> events.
    /// </summary>
    internal enum FullscreenAdEvents
    {
        RecordImpression = 0,
        Click = 1,
        Reward = 2,
        Close = 3,
        Expire = 4
    }
}

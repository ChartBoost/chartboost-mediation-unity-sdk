using Chartboost.Mediation.Ad.Fullscreen.Queue;

namespace Chartboost.Mediation.Utilities.Events
{
    /// <summary>
    /// Possible <see cref="IFullscreenAdQueue"/> events.
    /// </summary>
    internal enum FullscreenAdQueueEvents
    {
        Update = 0,
        RemoveExpiredAd = 1
    }
}

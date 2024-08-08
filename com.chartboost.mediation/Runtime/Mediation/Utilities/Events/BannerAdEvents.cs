using Chartboost.Mediation.Ad.Banner;

namespace Chartboost.Mediation.Utilities.Events
{
    /// <summary>
    /// Possible <see cref="IBannerAd"/> events.
    /// </summary>
    internal enum BannerAdEvents
    {
        Load = 0,
        Click = 1,
        RecordImpression = 2,
        Drag = 3
    }
}

using System;

namespace Chartboost.Mediation.Ad.Banner.Enums
{
    /// <summary>
    /// Used to specify a banner size type.
    /// </summary>
    [Serializable]
    public enum BannerSizeType
    {
        Unknown = -1,
        Standard = 0,
        Medium = 1,
        Leaderboard = 2,
        Adaptive = 3,
    }
}

using Newtonsoft.Json;
using static Chartboost.Utilities.Constants;

namespace Chartboost.Banner
{
    public enum ChartboostMediationBannerType
    {
        Fixed,
        Adaptive
    }
    
    public enum ChartboostMediationBannerName
    {
        Adaptive,
        Standard,
        Medium,
        Leaderboard
    }

    /// <summary>
    /// Chartboost Mediation defined banner sizes.
    /// </summary>
    public class ChartboostMediationBannerAdSize
    {
        [JsonProperty("name")]
        public ChartboostMediationBannerName Name { get; private set; }
        [JsonProperty("aspectRatio")]
        public float AspectRatio;
        [JsonProperty("width")]
        public float Width; // native
        [JsonProperty("height")]
        public float Height; // native
        [JsonProperty("type")]
        public ChartboostMediationBannerType BannerType;
        
        private ChartboostMediationBannerAdSize(ChartboostMediationBannerName name, float width, float height, ChartboostMediationBannerType bannerType = ChartboostMediationBannerType.Adaptive)
        {
            Name = name;
            Width = width;
            Height = height;
            AspectRatio = (width <= 0 || height <= 0) ? 0 : width / height;
            BannerType = bannerType;
        }

        public static ChartboostMediationBannerAdSize Adaptive(float width)
        {
            return new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, 0);
        }

        public static ChartboostMediationBannerAdSize Adaptive(float width, float height)
        {
            return new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, height);
        }

        #region static conveniences
        public static readonly ChartboostMediationBannerAdSize Standard = new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Standard, BannerSize.STANDARD.Item1, BannerSize.STANDARD.Item2, ChartboostMediationBannerType.Fixed);

        public static readonly ChartboostMediationBannerAdSize MediumRect = new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Medium, BannerSize.MEDIUM.Item1, BannerSize.MEDIUM.Item2, ChartboostMediationBannerType.Fixed);

        public static readonly ChartboostMediationBannerAdSize Leaderboard = new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Leaderboard, BannerSize.LEADERBOARD.Item1, BannerSize.LEADERBOARD.Item2, ChartboostMediationBannerType.Fixed);

        //Horizontal 
        public static ChartboostMediationBannerAdSize Adaptive2X1(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, width / 2.0f);

        public static ChartboostMediationBannerAdSize Adaptive4X1(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, width / 4.0f);

        public static ChartboostMediationBannerAdSize Adaptive6X1(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, width / 6.0f);

        public static ChartboostMediationBannerAdSize Adaptive8X1(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, width / 8.0f);

        public static ChartboostMediationBannerAdSize Adaptive10X1(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, width / 10.0f);

        //vertical
        public static ChartboostMediationBannerAdSize Adaptive1X2(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, width * 2.0f);

        public static ChartboostMediationBannerAdSize Adaptive1X3(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, width * 3.0f);

        public static ChartboostMediationBannerAdSize Adaptive1X4(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, width * 4.0f);

        public static ChartboostMediationBannerAdSize Adaptive9X16(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerName.Adaptive, width, (width * 16.0f) / 9.0f);
        #endregion

    }
}

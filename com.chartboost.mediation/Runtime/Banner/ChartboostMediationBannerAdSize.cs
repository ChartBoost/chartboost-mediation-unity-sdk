using System;
using Newtonsoft.Json;

namespace Chartboost.Banner
{
    public enum ChartboostMediationBannerType
    {
        Fixed,
        Adaptive
    }

    /// <summary>
    /// Chartboost Mediation defined banner sizes.
    /// </summary>
    public class ChartboostMediationBannerAdSize
    {
        [JsonProperty("aspectRatio")]
        public float AspectRatio;
        [JsonProperty("width")]
        public float Width;         // native
        [JsonProperty("height")]
        public float Height;        // native
        [JsonProperty("type")]
        public ChartboostMediationBannerType BannerType = ChartboostMediationBannerType.Adaptive;
        
        public ChartboostMediationBannerAdSize() {}

        private ChartboostMediationBannerAdSize(float width, float height, ChartboostMediationBannerType bannerType = ChartboostMediationBannerType.Adaptive)
        {
            Width = width;
            Height = height;
            AspectRatio = (width <= 0 || height <= 0) ? 0 : width / height;
            BannerType = bannerType;
        }

        public static ChartboostMediationBannerAdSize Adaptive(float width)
        {
            return new ChartboostMediationBannerAdSize(width, 0);
        }
            
        public static ChartboostMediationBannerAdSize Adaptive(float width, float height)
        {
            return new ChartboostMediationBannerAdSize(width, height);
        }
        
        #region static conveniences

        public static ChartboostMediationBannerAdSize Standard => new ChartboostMediationBannerAdSize(320, 50, ChartboostMediationBannerType.Fixed);
        public static ChartboostMediationBannerAdSize MediumRect => new ChartboostMediationBannerAdSize(300, 250, ChartboostMediationBannerType.Fixed);
        public static ChartboostMediationBannerAdSize Leaderboard => new ChartboostMediationBannerAdSize(728, 90, ChartboostMediationBannerType.Fixed);

        //Horizontal 
        public static ChartboostMediationBannerAdSize Adaptive2X1(float width) => new ChartboostMediationBannerAdSize(width, width / 2.0f);
        public static ChartboostMediationBannerAdSize Adaptive4X1(float width) => new ChartboostMediationBannerAdSize(width, width / 4.0f);
        public static ChartboostMediationBannerAdSize Adaptive6X1(float width) => new ChartboostMediationBannerAdSize(width, width / 6.0f);
        public static ChartboostMediationBannerAdSize Adaptive8X1(float width) => new ChartboostMediationBannerAdSize(width, width / 8.0f);
        public static ChartboostMediationBannerAdSize Adaptive10X1(float width) => new ChartboostMediationBannerAdSize(width, width / 10.0f);
        //vertical
        public static ChartboostMediationBannerAdSize Adaptive1X2(float width) => new ChartboostMediationBannerAdSize(width, width * 2.0f);
        public static ChartboostMediationBannerAdSize Adaptive1X3(float width) => new ChartboostMediationBannerAdSize(width, width * 3.0f);
        public static ChartboostMediationBannerAdSize Adaptive1X4(float width) => new ChartboostMediationBannerAdSize(width, width * 4.0f);
        public static ChartboostMediationBannerAdSize Adaptive9X16(float width) => new ChartboostMediationBannerAdSize(width, (width *16.0f) / 9.0f);

        
        
        #endregion
    }
}
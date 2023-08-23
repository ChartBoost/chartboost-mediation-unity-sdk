using System;
using Chartboost.Banner;
using Newtonsoft.Json;

namespace Chartboost.AdFormats.Banner
{
    public enum ChartboostMediationBannerHorizontalAlignment
    {
        Left,
        Center,
        Right
    }
    
    public enum ChartboostMediationBannerVerticalAlignment
    {
        Top,
        Center,
        Bottom
    }
    
    public enum ChartboostMediationBannerType
    {
        Fixed,
        Adaptive
    }
    
    public class ChartboostMediationBannerSize
    {
        #region static conveniences
        public static ChartboostMediationBannerSize Standard => CreateFixedSizeBanner(ChartboostMediationBannerAdSize.Standard);
        public static ChartboostMediationBannerSize Medium => CreateFixedSizeBanner(ChartboostMediationBannerAdSize.MediumRect);
        public static ChartboostMediationBannerSize Leaderboard => CreateFixedSizeBanner(ChartboostMediationBannerAdSize.Leaderboard);

        //Horizontal 
        public static ChartboostMediationBannerSize Adaptive2X1(float width) => new ChartboostMediationBannerSize(width, width / 2.0f);
        public static ChartboostMediationBannerSize Adaptive4X1(float width) => new ChartboostMediationBannerSize(width, width / 4.0f);
        public static ChartboostMediationBannerSize Adaptive6X1(float width) => new ChartboostMediationBannerSize(width, width / 6.0f);
        public static ChartboostMediationBannerSize Adaptive8X1(float width) => new ChartboostMediationBannerSize(width, width / 8.0f);
        public static ChartboostMediationBannerSize Adaptive10X1(float width) => new ChartboostMediationBannerSize(width, width / 10.0f);
        //vertical
        public static ChartboostMediationBannerSize Adaptive1X2(float width) => new ChartboostMediationBannerSize(width, width * 2.0f);
        public static ChartboostMediationBannerSize Adaptive1X3(float width) => new ChartboostMediationBannerSize(width, width * 3.0f);
        public static ChartboostMediationBannerSize Adaptive1X4(float width) => new ChartboostMediationBannerSize(width, width * 4.0f);
        public static ChartboostMediationBannerSize Adaptive9X16(float width) => new ChartboostMediationBannerSize(width, (width *16.0f) / 9.0f);

        #endregion

        [JsonProperty("aspectRatio")]public float AspectRatio;
        [JsonProperty("width")]public float Width;         // native
        [JsonProperty("height")]public float Height;        // native
        [JsonProperty("type")] public ChartboostMediationBannerType BannerType = ChartboostMediationBannerType.Adaptive;

        public ChartboostMediationBannerSize() {}

        private ChartboostMediationBannerSize(float width, float height, ChartboostMediationBannerType bannerType = ChartboostMediationBannerType.Adaptive)
        {
            Width = width;
            Height = height;
            AspectRatio = (width <= 0 || height <= 0) ? 0 : width / height;
            BannerType = bannerType;
        }

        public static ChartboostMediationBannerSize Adaptive(float width)
        {
            return new ChartboostMediationBannerSize(width, 0);
        }
        
        public static ChartboostMediationBannerSize Adaptive(float width, float height)
        {
            return new ChartboostMediationBannerSize(width, height);
        }

        private static ChartboostMediationBannerSize CreateFixedSizeBanner(ChartboostMediationBannerAdSize size)
        {
            return size switch
            {
                ChartboostMediationBannerAdSize.Standard => new ChartboostMediationBannerSize(320, 50,
                    ChartboostMediationBannerType.Fixed),
                ChartboostMediationBannerAdSize.MediumRect => new ChartboostMediationBannerSize(300, 250,
                    ChartboostMediationBannerType.Fixed),
                ChartboostMediationBannerAdSize.Leaderboard => new ChartboostMediationBannerSize(728, 90,
                    ChartboostMediationBannerType.Fixed),
                _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
            };
        }
    }
}
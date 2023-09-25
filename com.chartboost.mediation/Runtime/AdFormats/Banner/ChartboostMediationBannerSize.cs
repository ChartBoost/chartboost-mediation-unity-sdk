using System;
using Chartboost.Utilities;
using Newtonsoft.Json;

namespace Chartboost.AdFormats.Banner
{
    public enum ChartboostMediationBannerType
    {
        Fixed,
        Adaptive
    }
    
    public enum ChartboostMediationBannerSizeType
    {
        Unknown = -1,
        Standard = 0,
        Medium = 1,
        Leaderboard = 2,
        Adaptive = 3,
    }

    /// <summary>
    /// Chartboost Mediation defined banner sizes.
    /// </summary>
    public struct ChartboostMediationBannerSize
    {
        [JsonProperty("sizeType")]
        public ChartboostMediationBannerSizeType SizeType { get; private set; }
        [JsonProperty("aspectRatio")]
        public float AspectRatio;
        [JsonProperty("width")]
        public float Width; // native
        [JsonProperty("height")]
        public float Height; // native
        [JsonProperty("type")]
        public ChartboostMediationBannerType BannerType;
        
        private ChartboostMediationBannerSize(ChartboostMediationBannerSizeType sizeType, float width, float height, ChartboostMediationBannerType bannerType = ChartboostMediationBannerType.Adaptive)
        {
            SizeType = sizeType;
            Width = width;
            Height = height;
            AspectRatio = (width <= 0 || height <= 0) ? 0 : width / height;
            BannerType = bannerType;
        }

        public static ChartboostMediationBannerSize Adaptive(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, 0);

        public static ChartboostMediationBannerSize Adaptive(float width, float height)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, height);

        #region static conveniences
        public static readonly ChartboostMediationBannerSize Standard = 
            GetFixedTypeAd(ChartboostMediationBannerSizeType.Standard);

        public static readonly ChartboostMediationBannerSize MediumRect =
            GetFixedTypeAd(ChartboostMediationBannerSizeType.Medium);

        public static readonly ChartboostMediationBannerSize Leaderboard = 
            GetFixedTypeAd(ChartboostMediationBannerSizeType.Leaderboard);
        
        //Horizontal 
        public static ChartboostMediationBannerSize Adaptive2X1(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 2.0f);

        public static ChartboostMediationBannerSize Adaptive4X1(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 4.0f);

        public static ChartboostMediationBannerSize Adaptive6X1(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 6.0f);

        public static ChartboostMediationBannerSize Adaptive8X1(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 8.0f);

        public static ChartboostMediationBannerSize Adaptive10X1(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 10.0f);

        //vertical
        public static ChartboostMediationBannerSize Adaptive1X2(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width * 2.0f);

        public static ChartboostMediationBannerSize Adaptive1X3(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width * 3.0f);

        public static ChartboostMediationBannerSize Adaptive1X4(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width * 4.0f);

        public static ChartboostMediationBannerSize Adaptive9X16(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, (width * 16.0f) / 9.0f);
        #endregion
        
        private static ChartboostMediationBannerSize GetFixedTypeAd(ChartboostMediationBannerSizeType fixedSizeType)
        {
            if (fixedSizeType == ChartboostMediationBannerSizeType.Adaptive)
                throw new Exception("Cannot create fixed size banner for size type Adaptive");

            var width = fixedSizeType.Size().x;
            var height = fixedSizeType.Size().y;
            return new ChartboostMediationBannerSize(fixedSizeType, width, height, ChartboostMediationBannerType.Fixed);
        }

    }
}

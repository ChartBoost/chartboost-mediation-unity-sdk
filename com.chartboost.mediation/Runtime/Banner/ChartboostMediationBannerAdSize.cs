using System;
using Chartboost.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using static Chartboost.Utilities.Constants;

namespace Chartboost.Banner
{
    public enum ChartboostMediationBannerType
    {
        Fixed,
        Adaptive
    }
    
    public enum ChartboostMediationBannerSizeType
    {
        Adaptive = -1,
        Standard = 0,
        Medium = 1,
        Leaderboard = 2
    }

    /// <summary>
    /// Chartboost Mediation defined banner sizes.
    /// </summary>
    public struct ChartboostMediationBannerAdSize
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
        
        private ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType sizeType, float width, float height, ChartboostMediationBannerType bannerType = ChartboostMediationBannerType.Adaptive)
        {
            SizeType = sizeType;
            Width = width;
            Height = height;
            AspectRatio = (width <= 0 || height <= 0) ? 0 : width / height;
            BannerType = bannerType;
        }

        public static ChartboostMediationBannerAdSize Adaptive(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, 0);

        public static ChartboostMediationBannerAdSize Adaptive(float width, float height)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, height);

        #region static conveniences
        public static readonly ChartboostMediationBannerAdSize Standard = 
            GetFixedTypeAd(ChartboostMediationBannerSizeType.Standard);

        public static readonly ChartboostMediationBannerAdSize MediumRect =
            GetFixedTypeAd(ChartboostMediationBannerSizeType.Medium);

        public static readonly ChartboostMediationBannerAdSize Leaderboard = 
            GetFixedTypeAd(ChartboostMediationBannerSizeType.Leaderboard);
        
        //Horizontal 
        public static ChartboostMediationBannerAdSize Adaptive2X1(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 2.0f);

        public static ChartboostMediationBannerAdSize Adaptive4X1(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 4.0f);

        public static ChartboostMediationBannerAdSize Adaptive6X1(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 6.0f);

        public static ChartboostMediationBannerAdSize Adaptive8X1(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 8.0f);

        public static ChartboostMediationBannerAdSize Adaptive10X1(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 10.0f);

        //vertical
        public static ChartboostMediationBannerAdSize Adaptive1X2(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, width * 2.0f);

        public static ChartboostMediationBannerAdSize Adaptive1X3(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, width * 3.0f);

        public static ChartboostMediationBannerAdSize Adaptive1X4(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, width * 4.0f);

        public static ChartboostMediationBannerAdSize Adaptive9X16(float width)
            => new ChartboostMediationBannerAdSize(ChartboostMediationBannerSizeType.Adaptive, width, (width * 16.0f) / 9.0f);
        #endregion
        
        private static ChartboostMediationBannerAdSize GetFixedTypeAd(ChartboostMediationBannerSizeType fixedSizeType)
        {
            if (fixedSizeType == ChartboostMediationBannerSizeType.Adaptive)
                throw new Exception("Cannot create fixed size banner for size type Adaptive");

            var width = fixedSizeType.Size().x;
            var height = fixedSizeType.Size().y;
            return new ChartboostMediationBannerAdSize(fixedSizeType, width, height, ChartboostMediationBannerType.Fixed);
        }

    }
}

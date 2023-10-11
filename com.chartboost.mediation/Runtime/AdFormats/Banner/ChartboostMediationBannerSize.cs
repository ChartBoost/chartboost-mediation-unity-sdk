using System;
using Chartboost.Utilities;
using Newtonsoft.Json;

namespace Chartboost.AdFormats.Banner
{
    /// <summary>
    /// The banner type enum
    /// </summary>
    public enum ChartboostMediationBannerType
    {
        Fixed,
        Adaptive
    }
    
    /// <summary>
    /// The banner size type enum
    /// </summary>
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

        /// <summary>
        /// Returns a size for an adaptive banner with the specified width.
        /// This will generally be used when requesting an inline ad that can be of any height. To request an adaptive size banner with
        /// a maximum height, use <see cref="Adaptive(float, float)"/> instead.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
        public static ChartboostMediationBannerSize Adaptive(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, 0);

        /// <summary>
        /// Returns a size for an adaptive banner with the specified width and maxHeight.
        /// This will generally be used when requesting an anchored adaptive size banner, or when requesting an inline ad where the maximum height can
        /// be constrained. To request an adaptive size banner without a maximum height, use <see cref="Adaptive(float)"/> instead.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <param name="height">The maximum height for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
        public static ChartboostMediationBannerSize Adaptive(float width, float height)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, height);

        #region static conveniences
        
        /// <summary>
        /// A <see cref="ChartboostMediationBannerSize"/> size object for a fixed size 320x50 standard banner.
        /// </summary>
        public static readonly ChartboostMediationBannerSize Standard = GetFixedTypeAd(ChartboostMediationBannerSizeType.Standard);

        /// <summary>
        /// A <see cref="ChartboostMediationBannerSize"/> size object for a fixed size 300x250 medium banner.
        /// </summary>
        public static readonly ChartboostMediationBannerSize MediumRect = GetFixedTypeAd(ChartboostMediationBannerSizeType.Medium);

        /// <summary>
        /// A <see cref="ChartboostMediationBannerSize"/> size object for a fixed size 728x90 leaderboard banner.
        /// </summary>
        public static readonly ChartboostMediationBannerSize Leaderboard = GetFixedTypeAd(ChartboostMediationBannerSizeType.Leaderboard);
        
        //Horizontal 
        
        /// <summary>
        /// Convenience that returns a 2:1 <see cref="ChartboostMediationBannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
        public static ChartboostMediationBannerSize Adaptive2X1(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 2.0f);

        
        /// <summary>
        /// Convenience that returns a 4:1 <see cref="ChartboostMediationBannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
        public static ChartboostMediationBannerSize Adaptive4X1(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 4.0f);

        
        /// <summary>
        /// Convenience that returns a 6:1 <see cref="ChartboostMediationBannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
        public static ChartboostMediationBannerSize Adaptive6X1(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 6.0f);

        /// <summary>
        /// Convenience that returns a 8:1 <see cref="ChartboostMediationBannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
        public static ChartboostMediationBannerSize Adaptive8X1(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 8.0f);

        
        /// <summary>
        /// Convenience that returns a 10:1 <see cref="ChartboostMediationBannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
        public static ChartboostMediationBannerSize Adaptive10X1(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width / 10.0f);

        //vertical
        
        /// <summary>
        /// Convenience that returns a 1:2 <see cref="ChartboostMediationBannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
        public static ChartboostMediationBannerSize Adaptive1X2(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width * 2.0f);
        
        /// <summary>
        /// Convenience that returns a 1:3 <see cref="ChartboostMediationBannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
        public static ChartboostMediationBannerSize Adaptive1X3(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width * 3.0f);
        
        /// <summary>
        /// Convenience that returns a 1:4 <see cref="ChartboostMediationBannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
        public static ChartboostMediationBannerSize Adaptive1X4(float width)
            => new ChartboostMediationBannerSize(ChartboostMediationBannerSizeType.Adaptive, width, width * 4.0f);
        
        /// <summary>
        /// Convenience that returns a 9:16 <see cref="ChartboostMediationBannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="ChartboostMediationBannerSize"/> that can be used to load a banner.</returns>
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

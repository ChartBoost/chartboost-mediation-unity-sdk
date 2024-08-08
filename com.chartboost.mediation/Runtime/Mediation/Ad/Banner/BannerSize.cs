using System;
using Chartboost.Mediation.Ad.Banner.Enums;
using Newtonsoft.Json;

namespace Chartboost.Mediation.Ad.Banner
{
    /// <summary>
    /// Chartboost Mediation defined banner sizes.
    /// </summary>
    public struct BannerSize
    {
        [JsonProperty("sizeType")]
        public BannerSizeType SizeType { get; internal set; }
        [JsonProperty("aspectRatio")]
        public float AspectRatio;
        [JsonProperty("width")]
        public float Width; // native
        [JsonProperty("height")]
        public float Height; // native
        [JsonProperty("type")]
        public BannerType BannerType;
        
        private BannerSize(BannerSizeType sizeType, float width, float height, BannerType bannerType = BannerType.Adaptive)
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
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive(float width)
            => new(BannerSizeType.Adaptive, width, 0);

        /// <summary>
        /// Returns a size for an adaptive banner with the specified width and maxHeight.
        /// This will generally be used when requesting an anchored adaptive size banner, or when requesting an inline ad where the maximum height can
        /// be constrained. To request an adaptive size banner without a maximum height, use <see cref="Adaptive(float)"/> instead.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <param name="height">The maximum height for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive(float width, float height)
            => new(BannerSizeType.Adaptive, width, height);

        #region static conveniences
        
        /// <summary>
        /// A <see cref="BannerSize"/> size object for a fixed size 320x50 standard banner.
        /// </summary>
        public static readonly BannerSize Standard = GetFixedTypeAd(BannerSizeType.Standard);

        /// <summary>
        /// A <see cref="BannerSize"/> size object for a fixed size 300x250 medium banner.
        /// </summary>
        public static readonly BannerSize MediumRect = GetFixedTypeAd(BannerSizeType.Medium);

        /// <summary>
        /// A <see cref="BannerSize"/> size object for a fixed size 728x90 leaderboard banner.
        /// </summary>
        public static readonly BannerSize Leaderboard = GetFixedTypeAd(BannerSizeType.Leaderboard);
        
        //Horizontal 
        
        /// <summary>
        /// Convenience that returns a 2:1 <see cref="BannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive2X1(float width)
            => new(BannerSizeType.Adaptive, width, width / 2.0f);

        
        /// <summary>
        /// Convenience that returns a 4:1 <see cref="BannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive4X1(float width)
            => new(BannerSizeType.Adaptive, width, width / 4.0f);

        
        /// <summary>
        /// Convenience that returns a 6:1 <see cref="BannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive6X1(float width)
            => new(BannerSizeType.Adaptive, width, width / 6.0f);

        /// <summary>
        /// Convenience that returns 8:1 <see cref="BannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive8X1(float width)
            => new(BannerSizeType.Adaptive, width, width / 8.0f);

        
        /// <summary>
        /// Convenience that returns a 10:1 <see cref="BannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive10X1(float width)
            => new(BannerSizeType.Adaptive, width, width / 10.0f);

        //vertical
        
        /// <summary>
        /// Convenience that returns a 1:2 <see cref="BannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive1X2(float width)
            => new(BannerSizeType.Adaptive, width, width * 2.0f);
        
        /// <summary>
        /// Convenience that returns a 1:3 <see cref="BannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive1X3(float width)
            => new(BannerSizeType.Adaptive, width, width * 3.0f);
        
        /// <summary>
        /// Convenience that returns a 1:4 <see cref="BannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive1X4(float width)
            => new(BannerSizeType.Adaptive, width, width * 4.0f);
        
        /// <summary>
        /// Convenience that returns a 9:16 <see cref="BannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive9X16(float width)
            => new(BannerSizeType.Adaptive, width, (width * 16.0f) / 9.0f);
        
        /// <summary>
        /// Convenience that returns a 1:1 <see cref="BannerSize"/> size for the specified width.
        /// Note: This is only a maximum banner size. Depending on how your waterfall is configured, smaller or different aspect ratio
        /// ads may be served.
        /// </summary>
        /// <param name="width">The maximum width for the banner.</param>
        /// <returns>A <see cref="BannerSize"/> that can be used to load a banner.</returns>
        public static BannerSize Adaptive1X1(float width)
            => new(BannerSizeType.Adaptive, width, width);
        #endregion
        
        private static BannerSize GetFixedTypeAd(BannerSizeType fixedBannerSizeType)
        {
            if (fixedBannerSizeType == BannerSizeType.Adaptive)
                throw new Exception("Cannot create fixed size banner for size type Adaptive");

            var width = fixedBannerSizeType.Size().x;
            var height = fixedBannerSizeType.Size().y;
            return new BannerSize(fixedBannerSizeType, width, height, BannerType.Fixed);
        }
    }
}

using System;
using Chartboost.Mediation.Ad.Banner.Enums;
using UnityEngine;

namespace Chartboost.Mediation.Ad.Banner
{
    /// <summary>
    /// Fixed banner sizes pre-defined data.
    /// </summary>
    internal static class BannerConstants
    {
        internal static readonly (float, float) Standard = (320, 50);
        internal static readonly (float, float) Medium = (300, 250);
        internal static readonly (float, float) Leaderboard = (728, 90);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static class BannerExtensions 
    {
        /// <summary>
        /// Returns <see cref="LayoutParams"/> information from a <see cref="RectTransform"/>.
        /// </summary>
        /// <param name="rectTransform">Target.</param>
        /// <returns>Formatted <see cref="LayoutParams"/>.</returns>
        public static LayoutParams LayoutParams(this RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
        
            // corners[0] -> bottom-left
            // corners[1] -> top-left
            // corners[2] -> top-right
            // corners[3] -> bottom-right
            //    1           2
            //     _ _ _ _ _ _ 
            //    |           |
            //    |           |
            //    |           |
            //     - - - - - -
            //    0           3
        
            return new LayoutParams
            {
                x = corners[1].x,  
                y = corners[1].y,
                width = (int)(corners[2].x - corners[0].x),
                height = (int)(corners[1].y - corners[0].y),
                bottomLeft = corners[0],
                topLeft = corners[1],
                topRight = corners[2],
                bottomRight =  corners[3]
            };
        }
        
        /// <summary>
        /// Compares two LayoutParams instances to determine if they are approximately equal.
        /// </summary>
        /// <param name="source">The first LayoutParams instance to compare.</param>
        /// <param name="other">The second LayoutParams instance to compare.</param>
        /// <returns>True if the x, y, width, and height properties of both instances are within 0.01 units of each other, otherwise false.</returns>
        public static bool IsEqual(this LayoutParams source, LayoutParams other)
        {
            return Math.Abs(source.x - other.x) < 0.01f && Math.Abs(source.y - other.y) < 0.01f && Math.Abs(source.width - other.width) < 0.01f &&
                   Math.Abs(source.height - other.height) < 0.01f;
        }

        /// <summary>
        /// Converts a <see cref="BannerSizeType"/> into a respective <see cref="Vector2"/> values.
        /// </summary>
        /// <param name="bannerSize">Size to convert.</param>
        /// <returns>Appropriate <see cref="Vector2"/> values.</returns>
        public static Vector2 Size(this BannerSizeType bannerSize)
        {
            return bannerSize switch
            {
                BannerSizeType.Standard => new Vector2(BannerConstants.Standard.Item1, BannerConstants.Standard.Item2),
                BannerSizeType.Medium => new Vector2(BannerConstants.Medium.Item1, BannerConstants.Medium.Item2),
                BannerSizeType.Leaderboard => new Vector2(BannerConstants.Leaderboard.Item1, BannerConstants.Leaderboard.Item2),
                _ => Vector2.zero
            };
        }
    }
}

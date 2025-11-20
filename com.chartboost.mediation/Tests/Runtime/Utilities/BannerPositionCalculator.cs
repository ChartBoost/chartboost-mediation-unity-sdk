using UnityEngine;

namespace Chartboost.Tests.Runtime.Utilities
{
    /// <summary>
    /// Utility methods for calculating banner positions in tests.
    /// Provides consistent position calculations with proper rounding to prevent fractional pixel issues.
    /// </summary>
    public static class BannerPositionCalculator
    {
        /// <summary>
        /// Calculates the right edge position for a banner of given width.
        /// </summary>
        /// <param name="bannerWidthInPixels">Banner width in screen pixels</param>
        /// <returns>X coordinate for right-aligned banner (rounded to nearest pixel)</returns>
        public static float CalculateRightPosition(float bannerWidthInPixels)
        {
            return Mathf.Round(Screen.width - bannerWidthInPixels);
        }

        /// <summary>
        /// Calculates the bottom-edge position for a banner of given height.
        /// </summary>
        /// <param name="bannerHeightInPixels">Banner height in screen pixels</param>
        /// <returns>Y coordinate for bottom-aligned banner (rounded to nearest pixel)</returns>
        public static float CalculateBottomPosition(float bannerHeightInPixels)
        {
            return Mathf.Round(Screen.height - bannerHeightInPixels);
        }

        /// <summary>
        /// Calculates the horizontal center position for a banner of given width.
        /// </summary>
        /// <param name="bannerWidthInPixels">Banner width in screen pixels</param>
        /// <returns>X coordinate for horizontally centered banner (rounded to nearest pixel)</returns>
        public static float CalculateCenterX(float bannerWidthInPixels)
        {
            return Mathf.Round((Screen.width - bannerWidthInPixels) / 2f);
        }
    }
}

using UnityEngine;

namespace Chartboost.Tests.Runtime.Utilities
{
    /// <summary>
    /// Banner position calculations for tests, rounded to whole pixels like the layout itself.
    /// </summary>
    public static class BannerPositionCalculator
    {
        /// <summary>
        /// Top-left corner, in screen pixels, of a banner aligned to <paramref name="alignment"/> (0..1 each axis, y down).
        /// </summary>
        public static Vector2 CalculateTopLeft(Vector2 alignment, float bannerWidthInPixels, float bannerHeightInPixels)
            => new(Mathf.Round((Screen.width - bannerWidthInPixels) * alignment.x), Mathf.Round((Screen.height - bannerHeightInPixels) * alignment.y));
    }
}

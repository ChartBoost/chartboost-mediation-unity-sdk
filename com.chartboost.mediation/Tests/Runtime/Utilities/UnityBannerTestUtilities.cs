using UnityEngine;

namespace Chartboost.Tests.Runtime.Utilities
{
    /// <summary>
    /// Utility methods for Unity UI (RectTransform-based) banner testing.
    /// </summary>
    public static class UnityBannerTestUtilities
    {
        /// <summary>
        /// Sets banner position by configuring anchors, pivot, and position for Unity UI banners.
        /// This method positions the banner using Unity UI's anchor system.
        /// </summary>
        /// <remarks>
        /// Unity UI uses a bottom-left origin coordinate system where:
        /// - Vertical anchor: 0 = bottom, 1 = top
        /// - Horizontal anchor: 0 = left, 1 = right
        /// This is inverted from UIToolkit's top-left origin system.
        /// </remarks>
        /// <param name="rectTransform">The RectTransform to configure</param>
        /// <param name="horizontalAnchor">Horizontal anchor position (0=left, 0.5=center, 1=right)</param>
        /// <param name="verticalAnchor">Vertical anchor position (0=bottom, 0.5=center, 1=top)</param>
        public static void SetBannerPosition(RectTransform rectTransform, float horizontalAnchor, float verticalAnchor)
        {
            rectTransform.anchorMin = new Vector2(horizontalAnchor, verticalAnchor);
            rectTransform.anchorMax = new Vector2(horizontalAnchor, verticalAnchor);
            rectTransform.pivot = new Vector2(horizontalAnchor, verticalAnchor);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}

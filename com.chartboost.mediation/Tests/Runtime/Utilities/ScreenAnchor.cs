using System.Collections.Generic;
using Chartboost.Mediation.Utilities;
using UnityEngine;

namespace Chartboost.Tests.Runtime.Utilities
{
    /// <summary>
    /// A screen anchor for banner position tests: the banner's pivot goes to the same fraction of the screen.
    /// </summary>
    public sealed class ScreenAnchor
    {
        private readonly string _name;

        /// <summary>Banner pivot, and the screen fraction it's placed at (0,0 is top-left).</summary>
        public readonly Vector2 Pivot;

        private ScreenAnchor(string name, float x, float y)
        {
            _name = name;
            Pivot = new Vector2(x, y);
        }

        /// <summary>The same anchor in Unity UI (UGUI) coordinates, where y runs bottom (0) to top (1).</summary>
        public Vector2 UnityUIAnchor => new(Pivot.x, 1f - Pivot.y);

        /// <summary>The anchor's point on the current screen, in native units.</summary>
        public Vector2 NativePosition => new(DensityConverters.PixelsToNative(Screen.width * Pivot.x), DensityConverters.PixelsToNative(Screen.height * Pivot.y));

        public static readonly ScreenAnchor Center = new("Center", 0.5f, 0.5f);

        public static IEnumerable<ScreenAnchor> All() => new[]
        {
            new ScreenAnchor("TopLeft", 0f, 0f),
            new ScreenAnchor("TopRight", 1f, 0f),
            new ScreenAnchor("BottomLeft", 0f, 1f),
            new ScreenAnchor("BottomRight", 1f, 1f),
            new ScreenAnchor("TopCenter", 0.5f, 0f),
            new ScreenAnchor("BottomCenter", 0.5f, 1f),
        };

        public override string ToString() => _name;
    }
}

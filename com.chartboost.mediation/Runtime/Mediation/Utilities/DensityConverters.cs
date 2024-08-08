using UnityEngine;

namespace Chartboost.Mediation.Utilities
{
    /// <summary>
    /// Display density converters for Unity - Native operations.
    /// </summary>
    public sealed class DensityConverters
    {
        public static float NativeToPixels(float native) 
            => native * PlatformScaleFactor;

        public static float PixelsToNative(float pixels) 
            => pixels / PlatformScaleFactor;

        public static Vector2 NativeToPixels(Vector2 native) 
            => new(NativeToPixels(native.x), NativeToPixels(native.y));

        public static Vector2 PixelsToNative(Vector2 pixels) 
            => new(PixelsToNative(pixels.x), NativeToPixels(pixels.y));
        
        private const float EditorUIScaleFactor = 2.5f;

        internal static float? ScaleFactor = EditorUIScaleFactor;
        
        private static float PlatformScaleFactor
        {
            get
            {
                return ScaleFactor ??= EditorUIScaleFactor;
            }
        }
    }
}

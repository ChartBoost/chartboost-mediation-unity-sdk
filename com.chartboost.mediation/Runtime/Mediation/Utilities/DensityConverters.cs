using UnityEngine;
using UnityEngine.UIElements;

namespace Chartboost.Mediation.Utilities
{
    /// <summary>
    /// Display density converters for Unity - Native operations.
    /// </summary>
    public sealed class DensityConverters
    {
        private static float _uiDocScaleFactor;
        
        public static float NativeToPixels(float native) 
            => native * PlatformScaleFactor;

        public static float PixelsToNative(float pixels) 
            => pixels / PlatformScaleFactor;

        public static Vector2 NativeToPixels(Vector2 native) 
            => new(NativeToPixels(native.x), NativeToPixels(native.y));

        public static Vector2 PixelsToNative(Vector2 pixels)
            => new(PixelsToNative(pixels.x), PixelsToNative(pixels.y));
        
        public static float UIDocToNative(float uiDoc) 
            => PixelsToNative(uiDoc * UIDocScaleFactor);
        
        public static float UIDocToPixels(float uiDoc) 
            => uiDoc * UIDocScaleFactor;
        
        public static float NativeToUIDoc(float native) 
            => NativeToPixels(native) / UIDocScaleFactor;
        
        public static float PixelsToUIDoc(float pixels) 
            => pixels / UIDocScaleFactor;
        
        private const float EditorUIScaleFactor = 2.5f;

        private static float? _scaleFactor = EditorUIScaleFactor;

        internal static float? ScaleFactor
        {
            get => _scaleFactor;
            set => _scaleFactor = value;
        }

        private static float PlatformScaleFactor
        {
            get
            {
                return _scaleFactor ??= EditorUIScaleFactor;
            }
        }

        private static float UIDocScaleFactor
        {
            get
            {
                if (_uiDocScaleFactor != 0)
                    return _uiDocScaleFactor;

                var uiDocument = Object.FindObjectOfType<UIDocument>();
                if (uiDocument == null)
                    return 1;

                var uiDoc = uiDocument.rootVisualElement;
                if (uiDoc == null || uiDoc.panel == null)
                    return 1;

                var uiWidth = uiDoc.panel.visualTree.worldBound.width;
                if (uiWidth <= 0 || Screen.width <= 0 || float.IsNaN(uiWidth) || float.IsInfinity(uiWidth))
                    return 1;

                _uiDocScaleFactor = Screen.width / uiWidth;
                return _uiDocScaleFactor;
            }
        }
    }
}

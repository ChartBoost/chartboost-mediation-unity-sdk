using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chartboost.Mediation.Utilities
{
    /// <summary>
    /// Display density converters for Unity - Native operations.
    /// </summary>
    public sealed class DensityConverters
    {
        /// <summary>Test override: when non-zero it replaces the panel's scale.</summary>
        internal static float _uiDocScaleFactor;

        private static UIDocument _uiDocument;
        // Per panel, held weakly: the frame's scale and the last good one for the rotation window.
        private static readonly ConditionalWeakTable<IPanel, PanelScale> PanelScales = new();

        private sealed class PanelScale
        {
            public float Scale;
            public int Frame = -1;
        }
        
        public static float NativeToPixels(float native) 
            => native * PlatformScaleFactor;

        public static float PixelsToNative(float pixels) 
            => pixels / PlatformScaleFactor;

        public static Vector2 NativeToPixels(Vector2 native) 
            => new(NativeToPixels(native.x), NativeToPixels(native.y));

        public static Vector2 PixelsToNative(Vector2 pixels)
            => new(PixelsToNative(pixels.x), PixelsToNative(pixels.y));
        
        public static float UIDocToNative(float uiDoc) => UIDocToNative(uiDoc, null);

        public static float UIDocToPixels(float uiDoc) => uiDoc * ScaleOf(null);

        public static float NativeToUIDoc(float native) => NativeToUIDoc(native, null);

        public static float PixelsToUIDoc(float pixels) => PixelsToUIDoc(pixels, null);

        // Element-scoped: a known panel (e.g. a BannerVisualElement's own) skips the UIDocument search.
        internal static float UIDocToNative(float uiDoc, IPanel panel) => PixelsToNative(uiDoc * ScaleOf(panel));

        internal static float NativeToUIDoc(float native, IPanel panel) => NativeToPixels(native) / ScaleOf(panel);

        internal static float PixelsToUIDoc(float pixels, IPanel panel) => pixels / ScaleOf(panel);
        
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

        // The cached document can be disabled (no panel) while another is live; take the first one with a panel.
        private static bool TryFindDocumentWithPanel(out IPanel panel)
        {
            foreach (var document in Object.FindObjectsByType<UIDocument>(FindObjectsSortMode.None))
            {
                panel = document.rootVisualElement?.panel;
                if (panel == null)
                    continue;
                _uiDocument = document;
                return true;
            }
            _uiDocument = null;
            panel = null;
            return false;
        }

        // Screen pixels per panel unit. No panel (global calls, or a detached element) uses the first live UIDocument.
        private static float ScaleOf(IPanel panel)
        {
            if (_uiDocScaleFactor != 0)
                return _uiDocScaleFactor;
            panel ??= _uiDocument != null ? _uiDocument.rootVisualElement?.panel : null;
            if (panel == null && !TryFindDocumentWithPanel(out panel))
                return 1;

            // GetValue with an explicit factory: GetOrCreateValue builds by reflection, which IL2CPP strips.
            var cached = PanelScales.GetValue(panel, _ => new PanelScale());
            // Many conversions run per frame; the scale only changes on rotation.
            if (cached.Frame == Time.frameCount && cached.Scale > 0)
                return cached.Scale;

            var panelBounds = panel.visualTree.worldBound;
            var panelWidth = panelBounds.width;
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            if (panelWidth <= 0 || screenWidth <= 0 || float.IsNaN(panelWidth) || float.IsInfinity(panelWidth))
                return cached.Scale > 0 ? cached.Scale : 1;

            // Right after a rotation the panel isn't laid out for the new orientation yet: keep the last scale.
            var stalePanel = (screenWidth > screenHeight) != (panelWidth > panelBounds.height);
            if (!stalePanel || cached.Scale <= 0)
                cached.Scale = screenWidth / panelWidth;
            cached.Frame = Time.frameCount;
            return cached.Scale;
        }
    }
}

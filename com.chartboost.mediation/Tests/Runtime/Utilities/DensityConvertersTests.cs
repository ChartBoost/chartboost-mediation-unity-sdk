using System;
using System.Collections;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Chartboost.Tests.Runtime.Utilities
{
    public class DensityConvertersTests
    {
        private const float Tolerance = TestConstants.Tolerance.Standard;
        private const float Platform = TestConstants.DensityConverters.DefaultScaleFactor;
        private const float UIDoc = TestConstants.DensityConverters.TestUIDocScaleFactor;
        private const float Custom = TestConstants.DensityConverters.CustomScaleFactor;
        private const float Native = TestConstants.DensityConverters.TestNativeValue;
        private const float Pixels = TestConstants.DensityConverters.TestPixelsValue;
        private const float UIDocValue = TestConstants.DensityConverters.TestUIDocValue;
        private const float X = TestConstants.Dimensions.VectorX;
        private const float Y = TestConstants.Dimensions.VectorY;
        private const float NegativeValue = -50f;
        private const float LargeValue = 10000f;

        private const string NativeToPixels = nameof(DensityConverters.NativeToPixels);
        private const string PixelsToNative = nameof(DensityConverters.PixelsToNative);
        private const string UIDocToNative = nameof(DensityConverters.UIDocToNative);
        private const string UIDocToPixels = nameof(DensityConverters.UIDocToPixels);
        private const string NativeToUIDoc = nameof(DensityConverters.NativeToUIDoc);
        private const string PixelsToUIDoc = nameof(DensityConverters.PixelsToUIDoc);

        private float? _originalScaleFactor;
        private float _originalUIDocScaleFactor;

        [SetUp]
        public void SetUp()
        {
            _originalScaleFactor = DensityConverters.ScaleFactor;
            _originalUIDocScaleFactor = DensityConverters._uiDocScaleFactor;

            DensityConverters.ScaleFactor = Platform;
            // Pre-seed the UIDoc factor to skip the UIDocument lookup
            DensityConverters._uiDocScaleFactor = UIDoc;
        }

        [TearDown]
        public void TearDown()
        {
            DensityConverters.ScaleFactor = _originalScaleFactor;
            DensityConverters._uiDocScaleFactor = _originalUIDocScaleFactor;
        }

        private static float Convert(string conversion, float value) => conversion switch
        {
            NativeToPixels => DensityConverters.NativeToPixels(value),
            PixelsToNative => DensityConverters.PixelsToNative(value),
            UIDocToNative => DensityConverters.UIDocToNative(value),
            UIDocToPixels => DensityConverters.UIDocToPixels(value),
            NativeToUIDoc => DensityConverters.NativeToUIDoc(value),
            PixelsToUIDoc => DensityConverters.PixelsToUIDoc(value),
            _ => throw new ArgumentOutOfRangeException(nameof(conversion), conversion, null)
        };

        [TestCase(NativeToPixels, Native, Native * Platform)]
        [TestCase(NativeToPixels, 0f, 0f)]
        [TestCase(NativeToPixels, NegativeValue, NegativeValue * Platform)]
        [TestCase(NativeToPixels, LargeValue, LargeValue * Platform)]
        [TestCase(PixelsToNative, Pixels, Pixels / Platform)]
        [TestCase(PixelsToNative, 0f, 0f)]
        [TestCase(PixelsToNative, NegativeValue, NegativeValue / Platform)]
        [TestCase(PixelsToNative, Tolerance, Tolerance / Platform)]
        [TestCase(UIDocToNative, UIDocValue, UIDocValue * UIDoc / Platform)]
        [TestCase(UIDocToNative, 0f, 0f)]
        [TestCase(UIDocToPixels, UIDocValue, UIDocValue * UIDoc)]
        [TestCase(UIDocToPixels, 0f, 0f)]
        [TestCase(NativeToUIDoc, Native, Native * Platform / UIDoc)]
        [TestCase(NativeToUIDoc, 0f, 0f)]
        [TestCase(PixelsToUIDoc, Pixels, Pixels / UIDoc)]
        [TestCase(PixelsToUIDoc, 0f, 0f)]
        public void ConversionUsesConfiguredScaleFactors(string conversion, float input, float expected)
        {
            Assert.AreEqual(expected, Convert(conversion, input), Tolerance);
        }

        [TestCase(true, X, Y, X * Platform, Y * Platform)]
        [TestCase(true, 0f, 0f, 0f, 0f)]
        [TestCase(true, NegativeValue, NegativeValue, NegativeValue * Platform, NegativeValue * Platform)]
        [TestCase(true, X, 0f, X * Platform, 0f)]
        [TestCase(false, X, Y, X / Platform, Y / Platform)]
        [TestCase(false, 0f, 0f, 0f, 0f)]
        public void Vector2ConversionAppliesPerAxis(bool nativeToPixels, float x, float y, float expectedX, float expectedY)
        {
            var input = new Vector2(x, y);

            var result = nativeToPixels ? DensityConverters.NativeToPixels(input) : DensityConverters.PixelsToNative(input);

            Assert.AreEqual(expectedX, result.x, Tolerance);
            Assert.AreEqual(expectedY, result.y, Tolerance);
        }

        [Test]
        public void NativeToPixelsAndPixelsToNativeAreInverseOperations()
        {
            var backToNative = DensityConverters.PixelsToNative(DensityConverters.NativeToPixels(Native));

            Assert.AreEqual(Native, backToNative, Tolerance);
        }

        [TestCase(NativeToPixels, Custom, Native, Native * Custom)]
        [TestCase(PixelsToNative, Custom, Pixels, Pixels / Custom)]
        [TestCase(NativeToPixels, 4.0f, Native, Native * 4.0f)]
        public void ScaleFactorOverrideIsUsed(string conversion, float scaleFactor, float input, float expected)
        {
            DensityConverters.ScaleFactor = scaleFactor;

            Assert.AreEqual(expected, Convert(conversion, input), Tolerance);
            Assert.AreEqual(scaleFactor, DensityConverters.ScaleFactor);
        }

        [Test]
        public void PlatformScaleFactorUsesDefaultWhenNull()
        {
            DensityConverters.ScaleFactor = null;

            Assert.AreEqual(Native * Platform, DensityConverters.NativeToPixels(Native), Tolerance);
        }

        // Unit tests never get a laid-out panel, so every case falls back to 1:1 without caching
        [TestCase(false, false, TestName = "UIDocScaleFactorReturnsOneWhenNoUIDocumentExists")]
        [TestCase(true, false, TestName = "UIDocScaleFactorReturnsOneWhenRootVisualElementOrPanelIsNull")]
        [TestCase(true, true, TestName = "UIDocScaleFactorFallsBackWhenPanelNotInitialized")]
        public void UIDocScaleFactorFallsBackToOne(bool createDocument, bool withPanelSettings)
        {
            DensityConverters._uiDocScaleFactor = 0f;

            if (!createDocument)
            {
                foreach (var doc in Object.FindObjectsOfType<UIDocument>())
                    Object.DestroyImmediate(doc.gameObject);
            }

            var documentObject = createDocument ? new GameObject(TestConstants.Unity.TestUIDocumentName) : null;
            PanelSettings panelSettings = null;
            if (documentObject != null)
            {
                var document = documentObject.AddComponent<UIDocument>();
                if (withPanelSettings)
                {
                    panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                    panelSettings.SetScreenToPanelSpaceFunction(screenPos => screenPos);
                    document.panelSettings = panelSettings;
                }
            }

            try
            {
                var result = DensityConverters.UIDocToPixels(UIDocValue);

                Assert.AreEqual(UIDocValue, result, Tolerance, "Should return 1:1 scale (fallback)");
                Assert.AreEqual(0f, DensityConverters._uiDocScaleFactor, Tolerance, "Scale factor should not be cached on fallback");
            }
            finally
            {
                if (panelSettings != null)
                    Object.DestroyImmediate(panelSettings);
                if (documentObject != null)
                    Object.DestroyImmediate(documentObject);
            }
        }

        // A disabled document loses its panel; the scale must come from a live one, not fall back to 1:1.
        [UnityTest]
        public IEnumerator UIDocScaleFactorMovesOnFromADisabledDocument()
        {
            DensityConverters._uiDocScaleFactor = 0f;
            var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            var first = CreateDocument(panelSettings);
            GameObject second = null;
            try
            {
                yield return null;
                var scaled = DensityConverters.UIDocToPixels(UIDocValue);
                Assume.That(scaled, Is.Not.EqualTo(UIDocValue).Within(Tolerance), "the panel needs a non-1:1 scale to tell the cases apart");

                first.SetActive(false);
                second = CreateDocument(panelSettings);
                yield return null;

                Assert.AreEqual(scaled, DensityConverters.UIDocToPixels(UIDocValue), Tolerance, "the live document's scale, not the 1:1 fallback");
            }
            finally
            {
                Object.DestroyImmediate(first);
                if (second != null)
                    Object.DestroyImmediate(second);
                Object.DestroyImmediate(panelSettings);
            }
        }

        // A BannerVisualElement converts with its own panel, not whichever UIDocument a global search finds.
        [UnityTest]
        public IEnumerator PanelScopedConversionsUseThatPanelsScale()
        {
            DensityConverters._uiDocScaleFactor = 0f;
            var single = ScriptableObject.CreateInstance<PanelSettings>();
            var doubled = ScriptableObject.CreateInstance<PanelSettings>();
            doubled.scale = 2f;
            var first = CreateDocument(single);
            var second = CreateDocument(doubled);
            try
            {
                yield return null;
                var singlePanel = first.GetComponent<UIDocument>().rootVisualElement.panel;
                var doubledPanel = second.GetComponent<UIDocument>().rootVisualElement.panel;

                var inSingle = DensityConverters.PixelsToUIDoc(Pixels, singlePanel);
                var inDoubled = DensityConverters.PixelsToUIDoc(Pixels, doubledPanel);

                Assert.AreEqual(inSingle / 2f, inDoubled, Tolerance, "a 2x panel fits half as many panel units in the same pixels");
            }
            finally
            {
                Object.DestroyImmediate(first);
                Object.DestroyImmediate(second);
                Object.DestroyImmediate(single);
                Object.DestroyImmediate(doubled);
            }
        }

        private static GameObject CreateDocument(PanelSettings panelSettings)
        {
            var documentObject = new GameObject(TestConstants.Unity.TestUIDocumentName);
            documentObject.AddComponent<UIDocument>().panelSettings = panelSettings;
            return documentObject;
        }
    }
}

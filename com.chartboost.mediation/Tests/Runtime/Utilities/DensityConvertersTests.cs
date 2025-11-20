using System.Reflection;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace Chartboost.Tests.Runtime.Utilities
{
    public class DensityConvertersTests
    {
        private const float NegativeValue = -50f;
        private const float LargeValue = 10000f;

        private float? _originalScaleFactor;
        private float _originalUIDocScaleFactor;

        [SetUp]
        public void SetUp()
        {
            // Save the original scale factor
            _originalScaleFactor = DensityConverters.ScaleFactor;

            // Reset to default for tests
            DensityConverters.ScaleFactor = TestConstants.DensityConverters.DefaultScaleFactor;

            // Save original UIDoc scale factor
            var uiDocScaleFactorField = typeof(DensityConverters).GetField(TestConstants.DensityConverters.UIDocScaleFactorFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            _originalUIDocScaleFactor = (float)(uiDocScaleFactorField?.GetValue(null) ?? 0f);

            // Set UIDoc scale factor to test value to avoid UIDocument lookup
            uiDocScaleFactorField?.SetValue(null, TestConstants.DensityConverters.TestUIDocScaleFactor);
        }

        [TearDown]
        public void TearDown()
        {
            // Restore original scale factor
            DensityConverters.ScaleFactor = _originalScaleFactor; 

            // Restore original UIDoc scale factor
            var uiDocScaleFactorField = typeof(DensityConverters).GetField(TestConstants.DensityConverters.UIDocScaleFactorFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            uiDocScaleFactorField?.SetValue(null, _originalUIDocScaleFactor);
        }

        [Test]
        public void NativeToPixelsConvertsCorrectly()
        {
            var result = DensityConverters.NativeToPixels(TestConstants.DensityConverters.TestNativeValue);

            var expected = TestConstants.DensityConverters.TestNativeValue * TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void NativeToPixelsWithZeroReturnsZero()
        {
            var result = DensityConverters.NativeToPixels(TestConstants.Dimensions.Zero);

            Assert.AreEqual(TestConstants.Dimensions.Zero, result);
        }

        [Test]
        public void NativeToPixelsWithNegativeValueHandlesCorrectly()
        {
            var result = DensityConverters.NativeToPixels(NegativeValue);

            var expected = NegativeValue * TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void NativeToPixelsWithLargeValueHandlesCorrectly()
        {
            var result = DensityConverters.NativeToPixels(LargeValue);

            var expected = LargeValue * TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void PixelsToNativeConvertsCorrectly()
        {
            var result = DensityConverters.PixelsToNative(TestConstants.DensityConverters.TestPixelsValue);

            var expected = TestConstants.DensityConverters.TestPixelsValue / TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void PixelsToNativeWithZeroReturnsZero()
        {
            var result = DensityConverters.PixelsToNative(TestConstants.Dimensions.Zero);

            Assert.AreEqual(TestConstants.Dimensions.Zero, result);
        }

        [Test]
        public void PixelsToNativeWithNegativeValueHandlesCorrectly()
        {
            var result = DensityConverters.PixelsToNative(NegativeValue);

            var expected = NegativeValue / TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void PixelsToNativeWithSmallValueHandlesCorrectly()
        {
            var result = DensityConverters.PixelsToNative(TestConstants.Tolerance.Standard);

            var expected = TestConstants.Tolerance.Standard / TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void NativeToPixelsVector2ConvertsCorrectly()
        {
            var nativeVector = new Vector2(TestConstants.Dimensions.VectorX, TestConstants.Dimensions.VectorY);

            var result = DensityConverters.NativeToPixels(nativeVector);

            var expectedX = TestConstants.Dimensions.VectorX * TestConstants.DensityConverters.DefaultScaleFactor;
            var expectedY = TestConstants.Dimensions.VectorY * TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expectedX, result.x, TestConstants.Tolerance.Standard);
            Assert.AreEqual(expectedY, result.y, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void NativeToPixelsVector2WithZeroReturnsZero()
        {
            var nativeVector = Vector2.zero;

            var result = DensityConverters.NativeToPixels(nativeVector);

            Assert.AreEqual(Vector2.zero, result);
        }

        [Test]
        public void NativeToPixelsVector2WithNegativeValuesHandlesCorrectly()
        {
            var nativeVector = new Vector2(NegativeValue, NegativeValue);

            var result = DensityConverters.NativeToPixels(nativeVector);

            var expected = NegativeValue * TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expected, result.x, TestConstants.Tolerance.Standard);
            Assert.AreEqual(expected, result.y, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void PixelsToNativeVector2ConvertsCorrectly()
        {
            var pixelsVector = new Vector2(TestConstants.Dimensions.VectorX, TestConstants.Dimensions.VectorY);

            var result = DensityConverters.PixelsToNative(pixelsVector);

            var expectedX = TestConstants.Dimensions.VectorX / TestConstants.DensityConverters.DefaultScaleFactor;
            var expectedY = TestConstants.Dimensions.VectorY / TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expectedX, result.x, TestConstants.Tolerance.Standard);
            Assert.AreEqual(expectedY, result.y, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void PixelsToNativeVector2WithZeroReturnsZero()
        {
            var pixelsVector = Vector2.zero;

            var result = DensityConverters.PixelsToNative(pixelsVector);

            Assert.AreEqual(Vector2.zero, result);
        }

        [Test]
        public void NativeToPixelsAndPixelsToNativeAreInverseOperations()
        {
            var native = TestConstants.DensityConverters.TestNativeValue;

            var pixels = DensityConverters.NativeToPixels(native);
            var backToNative = DensityConverters.PixelsToNative(pixels);

            Assert.AreEqual(native, backToNative, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void UIDocToNativeConvertsCorrectly()
        {
            var result = DensityConverters.UIDocToNative(TestConstants.DensityConverters.TestUIDocValue);

            // UIDocToNative = PixelsToNative(uiDoc * UIDocScaleFactor)
            var expected = (TestConstants.DensityConverters.TestUIDocValue * TestConstants.DensityConverters.TestUIDocScaleFactor) / TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void UIDocToPixelsConvertsCorrectly()
        {
            var result = DensityConverters.UIDocToPixels(TestConstants.DensityConverters.TestUIDocValue);

            var expected = TestConstants.DensityConverters.TestUIDocValue * TestConstants.DensityConverters.TestUIDocScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void NativeToUIDocConvertsCorrectly()
        {
            var result = DensityConverters.NativeToUIDoc(TestConstants.DensityConverters.TestNativeValue);

            // NativeToUIDoc = NativeToPixels(native) / UIDocScaleFactor
            var expected = (TestConstants.DensityConverters.TestNativeValue * TestConstants.DensityConverters.DefaultScaleFactor) / TestConstants.DensityConverters.TestUIDocScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void PixelsToUIDocConvertsCorrectly()
        {
            var result = DensityConverters.PixelsToUIDoc(TestConstants.DensityConverters.TestPixelsValue);

            var expected = TestConstants.DensityConverters.TestPixelsValue / TestConstants.DensityConverters.TestUIDocScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void UIDocToNativeWithZeroReturnsZero()
        {
            var result = DensityConverters.UIDocToNative(TestConstants.Dimensions.Zero);

            Assert.AreEqual(TestConstants.Dimensions.Zero, result);
        }

        [Test]
        public void UIDocToPixelsWithZeroReturnsZero()
        {
            var result = DensityConverters.UIDocToPixels(TestConstants.Dimensions.Zero);

            Assert.AreEqual(TestConstants.Dimensions.Zero, result);
        }

        [Test]
        public void NativeToUIDocWithZeroReturnsZero()
        {
            var result = DensityConverters.NativeToUIDoc(TestConstants.Dimensions.Zero);

            Assert.AreEqual(TestConstants.Dimensions.Zero, result);
        }

        [Test]
        public void PixelsToUIDocWithZeroReturnsZero()
        {
            var result = DensityConverters.PixelsToUIDoc(TestConstants.Dimensions.Zero);

            Assert.AreEqual(TestConstants.Dimensions.Zero, result);
        }

        [Test]
        public void UIDocConversionsUseConfiguredScaleFactor()
        {
            // Verify that UIDoc methods use the mocked scale factor
            var pixels = DensityConverters.UIDocToPixels(TestConstants.DensityConverters.TestUIDocValue);
            var expected = TestConstants.DensityConverters.TestUIDocValue * TestConstants.DensityConverters.TestUIDocScaleFactor;

            Assert.AreEqual(expected, pixels, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void UIDocScaleFactorReturnsOneWhenRootVisualElementIsNull()
        {
            // Reset the _uiDocScaleFactor to 0 to force recalculation
            var uiDocScaleFactorField = typeof(DensityConverters).GetField(TestConstants.DensityConverters.UIDocScaleFactorFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            uiDocScaleFactorField?.SetValue(null, 0f);

            // Create a UIDocument GameObject but don't initialize its rootVisualElement
            var uiDocumentGameObject = new GameObject("TestUIDocument");
            uiDocumentGameObject.AddComponent<UnityEngine.UIElements.UIDocument>();

            try
            {
                // Attempt to use UIDoc conversion
                // The UIDocScaleFactor getter will find the UIDocument, but rootVisualElement will be null
                // This should return 1 as fallback (line 58 in DensityConverters.cs)
                var result = DensityConverters.UIDocToPixels(TestConstants.DensityConverters.TestUIDocValue);

                // With rootVisualElement = null, UIDocScaleFactor should return 1
                // So UIDocToPixels(TestConstants.DensityConverters.TestUIDocValue) = TestConstants.DensityConverters.TestUIDocValue * 1 = TestConstants.DensityConverters.TestUIDocValue
                Assert.AreEqual(TestConstants.DensityConverters.TestUIDocValue, result, TestConstants.Tolerance.Standard);

                // Verify that _uiDocScaleFactor was not set (still 0) since we hit the fallback
                var scaleFactorValue = (float)(uiDocScaleFactorField?.GetValue(null) ?? 0f);
                Assert.AreEqual(0f, scaleFactorValue, TestConstants.Tolerance.Standard);
            }
            finally
            {
                // Clean up
                Object.DestroyImmediate(uiDocumentGameObject);

                // Restore the test scale factor for other tests
                uiDocScaleFactorField?.SetValue(null, TestConstants.DensityConverters.TestUIDocScaleFactor);
            }
        }

        [Test]
        public void UIDocScaleFactorReturnsOneWhenNoUIDocumentExists()
        {
            // Reset the _uiDocScaleFactor to 0 to force recalculation
            var uiDocScaleFactorField = typeof(DensityConverters).GetField(TestConstants.DensityConverters.UIDocScaleFactorFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            uiDocScaleFactorField?.SetValue(null, 0f);

            // Ensure no UIDocument exists in the scene
            var existingUIDocuments = Object.FindObjectsOfType<UnityEngine.UIElements.UIDocument>();
            foreach (var doc in existingUIDocuments)
            {
                Object.DestroyImmediate(doc.gameObject);
            }

            try
            {
                // Attempt to use UIDoc conversion when no UIDocument exists
                // The UIDocScaleFactor getter will not find any UIDocument and should return 1
                var result = DensityConverters.UIDocToPixels(TestConstants.DensityConverters.TestUIDocValue);

                // With no UIDocument, UIDocScaleFactor should return 1
                // So UIDocToPixels(TestConstants.DensityConverters.TestUIDocValue) = TestConstants.DensityConverters.TestUIDocValue * 1 = TestConstants.DensityConverters.TestUIDocValue
                Assert.AreEqual(TestConstants.DensityConverters.TestUIDocValue, result, TestConstants.Tolerance.Standard);

                // Verify that _uiDocScaleFactor was not set (still 0) since we hit the fallback
                var scaleFactorValue = (float)(uiDocScaleFactorField?.GetValue(null) ?? 0f);
                Assert.AreEqual(0f, scaleFactorValue, TestConstants.Tolerance.Standard);
            }
            finally
            {
                // Restore the test scale factor for other tests
                uiDocScaleFactorField?.SetValue(null, TestConstants.DensityConverters.TestUIDocScaleFactor);
            }
        }

        [Test]
        public void UIDocScaleFactorCachesValueAfterFirstCalculation()
        {
            // Set a specific value for _uiDocScaleFactor
            var uiDocScaleFactorField = typeof(DensityConverters).GetField(TestConstants.DensityConverters.UIDocScaleFactorFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            const float cachedValue = 3.5f;
            uiDocScaleFactorField?.SetValue(null, cachedValue);

            // Call UIDoc method - it should use the cached value without recalculation
            var result = DensityConverters.UIDocToPixels(TestConstants.DensityConverters.TestUIDocValue);

            var expected = TestConstants.DensityConverters.TestUIDocValue * cachedValue;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);

            // Verify the cached value wasn't changed
            var scaleFactorValue = (float)(uiDocScaleFactorField?.GetValue(null) ?? 0f);
            Assert.AreEqual(cachedValue, scaleFactorValue, TestConstants.Tolerance.Standard);

            // Restore test scale factor
            uiDocScaleFactorField?.SetValue(null, TestConstants.DensityConverters.TestUIDocScaleFactor);
        }

        [Test]
        public void UIDocScaleFactorReturnsOneWhenPanelIsNull()
        {
            // Reset the _uiDocScaleFactor to 0 to force recalculation
            var uiDocScaleFactorField = typeof(DensityConverters).GetField(TestConstants.DensityConverters.UIDocScaleFactorFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            uiDocScaleFactorField?.SetValue(null, 0f);

            // Create a UIDocument but its panel will be null since it's not properly initialized
            var uiDocumentGameObject = new GameObject("TestUIDocument");
            uiDocumentGameObject.AddComponent<UnityEngine.UIElements.UIDocument>();

            try
            {
                // The UIDocScaleFactor getter will find the UIDocument, and rootVisualElement might not be null,
                // but a panel will be null, so it should return 1
                var result = DensityConverters.UIDocToPixels(TestConstants.DensityConverters.TestUIDocValue);

                // With a panel = null, UIDocScaleFactor should return 1
                Assert.AreEqual(TestConstants.DensityConverters.TestUIDocValue, result, TestConstants.Tolerance.Standard);

                // Verify that _uiDocScaleFactor was not set (still 0) since we hit the fallback
                var scaleFactorValue = (float)(uiDocScaleFactorField?.GetValue(null) ?? 0f);
                Assert.AreEqual(0f, scaleFactorValue, TestConstants.Tolerance.Standard);
            }
            finally
            {
                // Clean up
                Object.DestroyImmediate(uiDocumentGameObject);

                // Restore the test scale factor for other tests
                uiDocScaleFactorField?.SetValue(null, TestConstants.DensityConverters.TestUIDocScaleFactor);
            }
        }

        [Test]
        public void NativeToPixelsWithCustomScaleFactorUsesCustomValue()
        {
            DensityConverters.ScaleFactor = TestConstants.DensityConverters.CustomScaleFactor;
            var result = DensityConverters.NativeToPixels(TestConstants.DensityConverters.TestNativeValue);

            var expected = TestConstants.DensityConverters.TestNativeValue * TestConstants.DensityConverters.CustomScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void PixelsToNativeWithCustomScaleFactorUsesCustomValue()
        {
            DensityConverters.ScaleFactor = TestConstants.DensityConverters.CustomScaleFactor;
            var result = DensityConverters.PixelsToNative(TestConstants.DensityConverters.TestPixelsValue);

            var expected = TestConstants.DensityConverters.TestPixelsValue / TestConstants.DensityConverters.CustomScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void PlatformScaleFactorUsesDefaultWhenNull()
        {
            DensityConverters.ScaleFactor = null;
            var result = DensityConverters.NativeToPixels(TestConstants.DensityConverters.TestNativeValue);

            // Should use EditorUIScaleFactor (2.5f) as default
            var expected = TestConstants.DensityConverters.TestNativeValue * TestConstants.DensityConverters.DefaultScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);
        }


        [Test]
        public void MultipleConversionsPreserveValue()
        {
            var original = TestConstants.DensityConverters.TestNativeValue;

            // Native -> Pixels -> Native
            var pixels = DensityConverters.NativeToPixels(original);
            var backToNative = DensityConverters.PixelsToNative(pixels);

            Assert.AreEqual(original, backToNative, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void NativeToPixelsVector2HandlesOneAxisZero()
        {
            var nativeVector = new Vector2(TestConstants.Dimensions.VectorX, TestConstants.Dimensions.Zero);

            var result = DensityConverters.NativeToPixels(nativeVector);

            Assert.AreEqual(TestConstants.Dimensions.VectorX * TestConstants.DensityConverters.DefaultScaleFactor, result.x, TestConstants.Tolerance.Standard);
            Assert.AreEqual(TestConstants.Dimensions.Zero, result.y);
        }

        [Test]
        public void ScaleFactorInitializesToEditorUIScaleFactor()
        {
            Assert.AreEqual(TestConstants.DensityConverters.DefaultScaleFactor, DensityConverters.ScaleFactor, TestConstants.Tolerance.Standard);
        }

        [Test]
        public void ScaleFactorPropertyCanBeSetExternally()
        {
            const float platformScaleFactor = 4.0f;
            DensityConverters.ScaleFactor = platformScaleFactor;


            var result = DensityConverters.NativeToPixels(TestConstants.DensityConverters.TestNativeValue);

            const float expected = TestConstants.DensityConverters.TestNativeValue * platformScaleFactor;
            Assert.AreEqual(expected, result, TestConstants.Tolerance.Standard);

            var currentValue = DensityConverters.ScaleFactor;
            Assert.AreEqual(platformScaleFactor, currentValue);
        }

        [Test]
        public void UIDocScaleFactorWithUIDocumentFallsBackWhenPanelNotInitialized()
        {
            // Verifies fallback when UIDocument panel is not fully initialized
            // NOTE: Lines 68-69 cannot be unit tested - require Unity's UI Toolkit layout engine running in a real scene

            var uiDocScaleFactorField = typeof(DensityConverters).GetField(TestConstants.DensityConverters.UIDocScaleFactorFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            uiDocScaleFactorField?.SetValue(null, 0f);

            var uiDocumentGameObject = new GameObject("TestUIDocument");
            var document = uiDocumentGameObject.AddComponent<UnityEngine.UIElements.UIDocument>();

            // Create PanelSettings - even with this, the panel won't fully initialize in a unit test
            var panelSettings = ScriptableObject.CreateInstance<UnityEngine.UIElements.PanelSettings>();
            panelSettings.SetScreenToPanelSpaceFunction(screenPos => screenPos);
            document.panelSettings = panelSettings;

            try
            {
                // Trigger the UIDocScaleFactor calculation
                var result = DensityConverters.UIDocToPixels(TestConstants.DensityConverters.TestUIDocValue);

                // Verify we hit the fallback path (lines 65-66: width validation fails)
                var scaleFactorValue = (float)(uiDocScaleFactorField?.GetValue(null) ?? 0f);

                // In unit tests, the panel's worldBound.width is NaN or 0, so we should get fallback value
                Assert.AreEqual(0f, scaleFactorValue, TestConstants.Tolerance.Standard, "Scale factor should remain 0 when hitting fallback");
                Assert.AreEqual(TestConstants.DensityConverters.TestUIDocValue, result, TestConstants.Tolerance.Standard, "Should return 1:1 scale (fallback) when panel not initialized");
            }
            finally
            {
                // Clean up
                Object.DestroyImmediate(panelSettings);
                Object.DestroyImmediate(uiDocumentGameObject);

                // Restore the test scale factor for other tests
                uiDocScaleFactorField?.SetValue(null, TestConstants.DensityConverters.TestUIDocScaleFactor);
            }
        }
    }
}

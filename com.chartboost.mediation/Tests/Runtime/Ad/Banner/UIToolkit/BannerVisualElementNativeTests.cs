using System.Collections;
using System.Collections.Generic;
using Chartboost.Logging;
using Chartboost.Mediation;
using Chartboost.Testing;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Ad.Banner.UIToolkit;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Tests.Runtime.Utilities;
using UnityEngine;
using UnityEngine.UIElements;
using NUnit.Framework;
using UnityEngine.TestTools;
// ReSharper disable PossibleInvalidOperationException

namespace Chartboost.Tests.Runtime.Ad.Banner.UIToolkit
{
    /// <summary>
    /// Native platform tests for BannerVisualElement (Android/iOS only).
    /// These tests use the actual Chartboost Mediation SDK and run in order.
    /// </summary>
    [TestFixture]
    public class BannerVisualElementNativeTests : NativeTestFixture
    {
        private static BannerVisualElement _sharedBannerVisualElement;
        private static bool _loaded;
        private static UIDocument _sharedUIDocument;
        private static GameObject _sharedGameObject;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _loaded = false;
            ChartboostMediation.TestMode = true;

            LogController.LoggingLevel = LogLevel.Verbose;

            // Initialize SDK for native platform tests
            TestInitializer.Initialize();

            // Create a UIDocument to host the BannerVisualElement (similar to Canvas for Unity UI)
            _sharedGameObject = new GameObject(TestConstants.UIToolkit.SharedTestUIDocumentName);
            _sharedUIDocument = _sharedGameObject.AddComponent<UIDocument>();

            // Configure PanelSettings for 1:1 mapping between panel coordinates and screen pixels
            // ConstantPixelSize with scale=1 means: 1 panel unit = 1 screen pixel (no scaling)
            var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            panelSettings.targetDisplay = 0;
            panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            panelSettings.scale = 1.0f; // 1:1 mapping with screen pixels
            _sharedUIDocument.panelSettings = panelSettings;

            // Configure the root visual element to fill the screen (like Canvas RectTransform)
            // Use Relative positioning for the root so children with Absolute positioning are relative to it
            // Don't set explicit width/height - let PanelSettings handle scaling via reference resolution
            var root = _sharedUIDocument.rootVisualElement;
            root.style.position = Position.Relative; // Relative so absolute children position relative to this root

            // Disable flex layout behaviors to match Canvas behavior
            root.style.flexDirection = FlexDirection.Column;
            root.style.alignItems = Align.FlexStart;
            root.style.justifyContent = Justify.FlexStart;
            root.style.flexWrap = Wrap.NoWrap;

            // Ensure no padding or margins that could offset children
            root.style.paddingLeft = 0;
            root.style.paddingRight = 0;
            root.style.paddingTop = 0;
            root.style.paddingBottom = 0;
            root.style.marginLeft = 0;
            root.style.marginRight = 0;
            root.style.marginTop = 0;
            root.style.marginBottom = 0;

            // Create the shared BannerVisualElement instance using ChartboostMediation factory method
            _sharedBannerVisualElement = ChartboostMediation.GetBannerVisualElement(TestConstants.Placements.Banner);

            // Add to the UIDocument root visual element
            root.Add(_sharedBannerVisualElement);

            // Subscribe to WillAppear to position the banner in a center and set the proper size once it loads
            _sharedBannerVisualElement.WillAppear += OnBannerWillAppearPositionAndSize;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ChartboostMediation.TestMode = false;

            if (!DevicePlatform.IsNative)
                return;

            // Cleanup shared a banner visual element
            _sharedBannerVisualElement?.Dispose();
            _sharedBannerVisualElement = null;

            // Cleanup UIDocument and GameObject
            if (_sharedUIDocument != null && _sharedUIDocument.panelSettings != null)
                Object.DestroyImmediate(_sharedUIDocument.panelSettings);

            if (_sharedGameObject != null)
                Object.DestroyImmediate(_sharedGameObject);

            _sharedUIDocument = null;
            _sharedGameObject = null;
        }

        /// <summary>
        /// Callback to position the banner in a center and set the proper size after it loads.
        /// Similar to UnityBannerAdNativeTests.OnBannerWillAppearSetSize.
        /// </summary>
        private void OnBannerWillAppearPositionAndSize(BannerVisualElement bannerVisualElement)
        {
            // Unsubscribe after the first call
            bannerVisualElement.WillAppear -= OnBannerWillAppearPositionAndSize;

            // Position the banner in center (0.5, 0.5) and resize to match actual banner dimensions
            bannerVisualElement.SetAbsolutePositionByAlignment(ScreenAnchor.Center.Pivot.x, ScreenAnchor.Center.Pivot.y);
        }

        #region Sequential Integration Tests - Banner Lifecycle (Creation → Configuration → Load → Verification → Reset)

        /// <summary>
        /// Verify that TestMode is enabled for these tests.
        /// </summary>
        [Test, Order(1)]
        public void TestMode_WhenSetInSetup_ShouldBeTrue()
        {
            Assert.IsTrue(ChartboostMediation.TestMode, TestConstants.AssertionMessages.TestModeShouldBeEnabled);
        }

        /// <summary>
        /// Verify the shared banner visual element was created in OneTimeSetUp.
        /// </summary>
        [Test, Order(2)]
        public void SharedBannerVisualElement_WhenCreatedInSetup_ShouldExist()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementCreated);
            Assert.IsNotNull(_sharedUIDocument, TestConstants.AssertionMessages.SharedUIDocumentExists);
        }

        /// <summary>
        /// Test that the PlacementName property works correctly on the shared instance before loading.
        /// Placement name should be set or retrieved after getting the banner but before loading it.
        /// </summary>
        [Test, Order(3)]
        public void PlacementName_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);

            Assert.AreEqual(TestConstants.Placements.Banner, _sharedBannerVisualElement.PlacementName);

            _sharedBannerVisualElement.PlacementName = TestConstants.Placements.Secondary;
            Assert.AreEqual(TestConstants.Placements.Secondary, _sharedBannerVisualElement.PlacementName);

            _sharedBannerVisualElement.PlacementName = TestConstants.Placements.Banner;
            Assert.AreEqual(TestConstants.Placements.Banner, _sharedBannerVisualElement.PlacementName);
        }

        /// <summary>
        /// Test that the Draggable property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(4)]
        public void Draggable_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);

            _sharedBannerVisualElement.Draggable = true;
            Assert.IsTrue(_sharedBannerVisualElement.Draggable);

            _sharedBannerVisualElement.Draggable = false;
            Assert.IsFalse(_sharedBannerVisualElement.Draggable);
        }

        /// <summary>
        /// Test that the HorizontalAlignment property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(5)]
        public void HorizontalAlignment_WhenSetToAllValues_ShouldBeRetrievable()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);

            _sharedBannerVisualElement.HorizontalAlignment = BannerHorizontalAlignment.Left;
            Assert.AreEqual(BannerHorizontalAlignment.Left, _sharedBannerVisualElement.HorizontalAlignment);

            _sharedBannerVisualElement.HorizontalAlignment = BannerHorizontalAlignment.Center;
            Assert.AreEqual(BannerHorizontalAlignment.Center, _sharedBannerVisualElement.HorizontalAlignment);

            _sharedBannerVisualElement.HorizontalAlignment = BannerHorizontalAlignment.Right;
            Assert.AreEqual(BannerHorizontalAlignment.Right, _sharedBannerVisualElement.HorizontalAlignment);
        }

        /// <summary>
        /// Test that the VerticalAlignment property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(6)]
        public void VerticalAlignment_WhenSetToAllValues_ShouldBeRetrievable()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);

            _sharedBannerVisualElement.VerticalAlignment = BannerVerticalAlignment.Top;
            Assert.AreEqual(BannerVerticalAlignment.Top, _sharedBannerVisualElement.VerticalAlignment);

            _sharedBannerVisualElement.VerticalAlignment = BannerVerticalAlignment.Center;
            Assert.AreEqual(BannerVerticalAlignment.Center, _sharedBannerVisualElement.VerticalAlignment);

            _sharedBannerVisualElement.VerticalAlignment = BannerVerticalAlignment.Bottom;
            Assert.AreEqual(BannerVerticalAlignment.Bottom, _sharedBannerVisualElement.VerticalAlignment);
        }

        /// <summary>
        /// Test that the Keywords property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(7)]
        public void Keywords_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);

            var keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _sharedBannerVisualElement.Keywords = keywords;

            var retrieved = _sharedBannerVisualElement.Keywords;
            Assert.IsNotNull(retrieved, TestConstants.AssertionMessages.KeywordsShouldNotBeNull);
            Assert.That(retrieved, Contains.Key(TestConstants.Keywords.Key), TestConstants.AssertionMessages.KeywordsShouldContainTestKey);
        }

        /// <summary>
        /// Test that the PartnerSettings property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(8)]
        public void PartnerSettings_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);

            var settings = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _sharedBannerVisualElement.PartnerSettings = settings;

            var retrieved = _sharedBannerVisualElement.PartnerSettings;
            Assert.IsNotNull(retrieved, TestConstants.AssertionMessages.PartnerSettingsShouldNotBeNull);
            Assert.That(retrieved, Contains.Key(TestConstants.Keywords.Key), TestConstants.AssertionMessages.PartnerSettingsShouldContainTestKey);
        }

        /// <summary>
        /// Load the shared banner visual element for the first time and verify it succeeds.
        /// Also verify that WillAppear and DidRecordImpression events are triggered.
        /// </summary>
        [UnityTest, Order(9)]
        public IEnumerator Load_WhenCalledWithValidRequest_ShouldSucceedAndTriggerEvents()
        {
            // Wait for SDK initialization on native platforms
            yield return TestInitializer.WaitForInitialization();

            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);

            // Track event calls
            var willAppearCalled = false;
            var didRecordImpressionCalled = false;
            BannerVisualElement willAppearBanner = null;
            BannerVisualElement impressionBanner = null;

            // Subscribe to events
            BannerVisualElementAdEvent willAppearHandler = banner =>
            {
                willAppearCalled = true;
                willAppearBanner = banner;
                Debug.Log(TestConstants.DebugLogPrefixes.WillAppearEvent);
            };

            BannerVisualElementAdEvent impressionHandler = banner =>
            {
                didRecordImpressionCalled = true;
                impressionBanner = banner;
                Debug.Log(TestConstants.DebugLogPrefixes.ImpressionEvent);
            };

            _sharedBannerVisualElement.WillAppear += willAppearHandler;
            _sharedBannerVisualElement.DidRecordImpression += impressionHandler;

            try
            {
                var adLoadRequest = new BannerAdLoadRequest(TestConstants.Placements.Banner, BannerSize.Standard);
                var loadTask = _sharedBannerVisualElement.Load(adLoadRequest);
                yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.NativeAdLoad);

                Assert.IsNotNull(loadTask.Result, TestConstants.AssertionMessages.LoadResultShouldNotBeNull);

                // Use AdLoadResultAssert to handle ad inventory errors as Inconclusive
                AdLoadResultAssert.AssertSuccess(loadTask.Result, TestConstants.AssertionMessages.LoadShouldSucceed);
                _loaded = true;

                yield return TestWait.Until(() => willAppearCalled && didRecordImpressionCalled, TestConstants.Timeouts.NativeEvent);

                // Verify WillAppear was triggered
                Assert.IsTrue(willAppearCalled, TestConstants.AssertionMessages.WillAppearShouldTrigger);
                Assert.AreSame(_sharedBannerVisualElement, willAppearBanner, TestConstants.AssertionMessages.WillAppearPassCorrectInstance);

                // Verify DidRecordImpression was triggered
                Assert.IsTrue(didRecordImpressionCalled, TestConstants.AssertionMessages.ImpressionShouldTrigger);
                Assert.AreSame(_sharedBannerVisualElement, impressionBanner, TestConstants.AssertionMessages.ImpressionPassCorrectInstance);
            }
            finally
            {
                // Defensive cleanup with null checks
                if (_sharedBannerVisualElement != null)
                {
                    _sharedBannerVisualElement.WillAppear -= willAppearHandler;
                    _sharedBannerVisualElement.DidRecordImpression -= impressionHandler;
                }
            }
        }

        /// <summary>
        /// Test that Request property is available after a successful load.
        /// </summary>
        [Test, Order(10)]
        public void Request_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var request = _sharedBannerVisualElement.Request;
            Assert.IsNotNull(request, TestConstants.AssertionMessages.RequestAvailableAfterLoad);
        }

        /// <summary>
        /// Test that LoadId is available after a successful load.
        /// </summary>
        [Test, Order(11)]
        public void LoadId_AfterSuccessfulLoad_ShouldNotBeNullOrEmpty()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var loadId = _sharedBannerVisualElement.LoadId;
            Assert.IsFalse(string.IsNullOrEmpty(loadId), TestConstants.AssertionMessages.LoadIdAvailableAfterLoad);
        }

        /// <summary>
        /// Test that LoadMetrics is available after a successful load.
        /// </summary>
        [Test, Order(12)]
        public void LoadMetrics_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var metrics = _sharedBannerVisualElement.LoadMetrics;
            Assert.IsNotNull(metrics, TestConstants.AssertionMessages.LoadMetricsAvailableAfterLoad);
        }

        /// <summary>
        /// Test that BannerSize is available after a successful load.
        /// </summary>
        [Test, Order(13)]
        public void BannerSize_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var bannerSize = _sharedBannerVisualElement.BannerSize;
            Assert.IsNotNull(bannerSize, TestConstants.AssertionMessages.BannerSizeAvailableAfterLoad);
        }

        /// <summary>
        /// Test that WinningBidInfo is available after a successful load.
        /// </summary>
        [Test, Order(14)]
        public void WinningBidInfo_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var bidInfo = _sharedBannerVisualElement.WinningBidInfo;
            Assert.IsNotNull(bidInfo, TestConstants.AssertionMessages.WinningBidInfoAvailableAfterLoad);
        }

        /// <summary>
        /// Test that ToString returns a valid JSON format.
        /// </summary>
        [Test, Order(15)]
        public void ToString_WhenCalled_ShouldReturnValidJson()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var jsonString = _sharedBannerVisualElement.ToString();

            Assert.IsNotNull(jsonString, TestConstants.AssertionMessages.ToStringShouldReturnNonNull);
            Assert.IsTrue(jsonString.Contains(TestConstants.Json.OpenBrace), TestConstants.AssertionMessages.JsonShouldContainOpenBrace);
            Assert.IsTrue(jsonString.Contains(TestConstants.Json.CloseBrace), TestConstants.AssertionMessages.JsonShouldContainCloseBrace);
        }

        /// <summary>
        /// The loaded banner can be absolutely positioned at each screen anchor.
        /// </summary>
        [UnityTest, Order(16)]
        public IEnumerator Position_WhenSetToAnchor_ShouldRelocateBanner([ValueSource(typeof(ScreenAnchor), nameof(ScreenAnchor.All))] ScreenAnchor anchor)
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            _sharedBannerVisualElement.SetAbsolutePositionByAlignment(anchor.Pivot.x, anchor.Pivot.y);
            // One frame for the layout pass that syncs the element to the native banner.
            yield return null;

            var size = _sharedBannerVisualElement.BannerSize.Value;
            var expected = BannerPositionCalculator.CalculateTopLeft(anchor.Pivot, DensityConverters.NativeToPixels(size.Width), DensityConverters.NativeToPixels(size.Height));
            Assert.AreEqual(Position.Absolute, _sharedBannerVisualElement.style.position.value, TestConstants.AssertionMessages.PositionShouldBeAbsolute);
            Assert.AreEqual(expected.x, _sharedBannerVisualElement.style.left.value.value, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.LeftPositionMatches);
            Assert.AreEqual(expected.y, _sharedBannerVisualElement.style.top.value.value, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.TopPositionMatches);
        }

        /// <summary>
        /// Test that the Reset method works and clears the loaded ad without throwing exceptions.
        /// </summary>
        [Test, Order(17)]
        public void Reset_WhenCalled_ShouldClearLoadedAdWithoutThrowing()
        {
            Assert.IsNotNull(_sharedBannerVisualElement, TestConstants.AssertionMessages.SharedBannerVisualElementMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            // Reset the banner to clear the loaded ad
            Assert.DoesNotThrow(() => _sharedBannerVisualElement.Reset(), TestConstants.AssertionMessages.ResetShouldNotThrow);
        }

        #endregion
    }
}

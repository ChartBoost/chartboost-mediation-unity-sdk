using System.Collections;
using System.Collections.Generic;
using Chartboost.Mediation;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Ad.Banner.Unity;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Tests.Runtime.Utilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Banner.Unity
{
    /// <summary>
    /// Native platform tests for UnityBannerAd (Android/iOS only).
    /// These tests use the actual Chartboost Mediation SDK and run in order.
    /// </summary>
    [TestFixture]
    public class UnityBannerAdNativeTests
    {
        private static UnityBannerAd _sharedBannerAd;
        private static Canvas _sharedCanvas;
        private const int TotalTests = 23;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ChartboostMediation.TestMode = true;

            // Skip setup if not on a native platform
            if (!IsNativePlatform())
            {
                Assert.Ignore(TestConstants.AssertionMessages.SkippingNonNativePlatform);
                return;
            }

            // Initialize SDK for native platform tests
            TestInitializer.Initialize();

            // Show test progress display on device
            TestProgressTracker.Show();

            // Ensure the EventSystem exists for UI interaction
            if (EventSystem.current == null)
            {
                var eventSystemObject = new GameObject(TestConstants.Unity.EventSystemName);
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }

            // Create a shared canvas for the shared banner ad instance
            var sharedCanvasObject = new GameObject(TestConstants.Unity.SharedTestCanvasName);
            _sharedCanvas = sharedCanvasObject.AddComponent<Canvas>();
            _sharedCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Configure canvas scaler for consistent behavior across different device resolutions
            // Using 1920x1080 as reference (16:9 aspect ratio - most common mobile/desktop)
            // matchWidthOrHeight = 0.5 balances width/height scaling
            var canvasScaler = sharedCanvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(TestConstants.Unity.ReferenceResolutionWidth, TestConstants.Unity.ReferenceResolutionHeight);
            canvasScaler.matchWidthOrHeight = TestConstants.Unity.CanvasMatchWidthOrHeight;

            // Add GraphicRaycaster for UI interaction
            sharedCanvasObject.AddComponent<GraphicRaycaster>();

            // Set the canvas RectTransform to fill the screen
            var canvasRect = sharedCanvasObject.GetComponent<RectTransform>();
            canvasRect.anchorMin = Vector2.zero;
            canvasRect.anchorMax = Vector2.one;
            canvasRect.offsetMin = Vector2.zero;
            canvasRect.offsetMax = Vector2.zero;

            // Create the shared UnityBannerAd instance using ChartboostMediation factory method
            _sharedBannerAd = ChartboostMediation.GetUnityBannerAd(TestConstants.Placements.Banner, _sharedCanvas.transform, BannerSize.Standard);

            // Position banner in the center of the screen
            var rectTransform = _sharedBannerAd.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Set anchors to center
                rectTransform.anchorMin = new Vector2(TestConstants.BannerAlignment.HorizontalCenter, TestConstants.BannerAlignment.UnityUI.VerticalCenter);
                rectTransform.anchorMax = new Vector2(TestConstants.BannerAlignment.HorizontalCenter, TestConstants.BannerAlignment.UnityUI.VerticalCenter);
                rectTransform.pivot = new Vector2(TestConstants.BannerAlignment.HorizontalCenter, TestConstants.BannerAlignment.UnityUI.VerticalCenter);
                // Position at a center using anchored position (relative to anchors)
                rectTransform.anchoredPosition = Vector2.zero;
            }

            // Subscribe to WillAppear to set the proper size once the banner loads
            _sharedBannerAd.WillAppear += OnBannerWillAppearSetSize;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ChartboostMediation.TestMode = false;

            if (!IsNativePlatform())
                return;

            // Clear test progress display content when this test fixture completes
            TestProgressTracker.Clear();

            // Cleanup shared banner ad instance
            if (_sharedBannerAd != null && _sharedBannerAd.gameObject != null)
                Object.DestroyImmediate(_sharedBannerAd.gameObject);
            _sharedBannerAd = null;

            // Cleanup shared canvas
            if (_sharedCanvas != null && _sharedCanvas.gameObject != null)
                Object.DestroyImmediate(_sharedCanvas.gameObject);
            _sharedCanvas = null;
        }

        private static bool IsNativePlatform() => TestPlatformUtilities.IsNativePlatform();

        /// <summary>
        /// Callback to properly set the banner size after it loads.
        /// Based on UnityBannerAdController.OnWillAppearBanner from the demo app.
        /// </summary>
        private void OnBannerWillAppearSetSize(UnityBannerAd unityBannerAd)
        {
            // Unsubscribe after the first call
            unityBannerAd.WillAppear -= OnBannerWillAppearSetSize;

            // Update size based on actual banner dimensions
            var canvas = unityBannerAd.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                var canvasScale = canvas.transform.localScale.x;
                var width = DensityConverters.NativeToPixels(unityBannerAd.BannerSize?.Width ?? 0) / canvasScale;
                var height = DensityConverters.NativeToPixels(unityBannerAd.BannerSize?.Height ?? 0) / canvasScale;

                var rectTransform = unityBannerAd.GetComponent<RectTransform>();
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }


        #region Sequential Integration Tests - Banner Lifecycle (Creation → Configuration → Load → Verification → Reset)

        /// <summary>
        /// Verify that TestMode is enabled for these tests.
        /// </summary>
        [Test, Order(1)]
        public void TestMode_WhenSetInSetup_ShouldBeTrue()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 1, TotalTests);
            Assert.IsTrue(ChartboostMediation.TestMode, TestConstants.AssertionMessages.TestModeShouldBeEnabled);
        }

        /// <summary>
        /// Verify the shared banner ad was created in OneTimeSetUp.
        /// </summary>
        [Test, Order(2)]
        public void SharedBannerAd_WhenCreatedInSetup_ShouldExist()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 2, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdCreated);
            Assert.IsNotNull(_sharedBannerAd.gameObject, TestConstants.AssertionMessages.SharedBannerAdGameObjectExists);
        }

        /// <summary>
        /// Test that the PlacementName property works correctly on the shared instance before loading.
        /// Placement name should be set or retrieved after getting the banner but before loading it.
        /// </summary>
        [Test, Order(3)]
        public void PlacementName_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 3, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            Assert.AreEqual(TestConstants.Placements.Banner, _sharedBannerAd.PlacementName);

            _sharedBannerAd.PlacementName = TestConstants.Placements.Secondary;
            Assert.AreEqual(TestConstants.Placements.Secondary, _sharedBannerAd.PlacementName);

            _sharedBannerAd.PlacementName = TestConstants.Placements.Banner;
            Assert.AreEqual(TestConstants.Placements.Banner, _sharedBannerAd.PlacementName);
        }

        /// <summary>
        /// Test that the Draggable property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(4)]
        public void Draggable_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 4, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            _sharedBannerAd.Draggable = true;
            Assert.IsTrue(_sharedBannerAd.Draggable);

            _sharedBannerAd.Draggable = false;
            Assert.IsFalse(_sharedBannerAd.Draggable);
        }

        /// <summary>
        /// Test that the HorizontalAlignment property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(5)]
        public void HorizontalAlignment_WhenSetToAllValues_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 5, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Left;
            Assert.AreEqual(BannerHorizontalAlignment.Left, _sharedBannerAd.HorizontalAlignment);

            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Center;
            Assert.AreEqual(BannerHorizontalAlignment.Center, _sharedBannerAd.HorizontalAlignment);

            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Right;
            Assert.AreEqual(BannerHorizontalAlignment.Right, _sharedBannerAd.HorizontalAlignment);
        }

        /// <summary>
        /// Test that the VerticalAlignment property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(6)]
        public void VerticalAlignment_WhenSetToAllValues_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 6, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Top;
            Assert.AreEqual(BannerVerticalAlignment.Top, _sharedBannerAd.VerticalAlignment);

            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Center;
            Assert.AreEqual(BannerVerticalAlignment.Center, _sharedBannerAd.VerticalAlignment);

            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Bottom;
            Assert.AreEqual(BannerVerticalAlignment.Bottom, _sharedBannerAd.VerticalAlignment);
        }

        /// <summary>
        /// Test that the Keywords property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(7)]
        public void Keywords_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 7, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _sharedBannerAd.Keywords = keywords;

            var retrieved = _sharedBannerAd.Keywords;
            Assert.IsNotNull(retrieved, TestConstants.AssertionMessages.KeywordsShouldNotBeNull);
            Assert.That(retrieved, Contains.Key(TestConstants.Keywords.Key), TestConstants.AssertionMessages.KeywordsShouldContainTestKey);
        }

        /// <summary>
        /// Test that the PartnerSettings property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(8)]
        public void PartnerSettings_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 8, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var settings = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _sharedBannerAd.PartnerSettings = settings;

            var retrieved = _sharedBannerAd.PartnerSettings;
            Assert.IsNotNull(retrieved, TestConstants.AssertionMessages.PartnerSettingsShouldNotBeNull);
            Assert.That(retrieved, Contains.Key(TestConstants.Keywords.Key), TestConstants.AssertionMessages.PartnerSettingsShouldContainTestKey);
        }

        /// <summary>
        /// Load the shared banner ad for the first time and verify it succeeds.
        /// Also verify that WillAppear and DidRecordImpression events are triggered.
        /// </summary>
        [UnityTest, Order(9)]
        public IEnumerator Load_WhenCalledWithValidRequest_ShouldSucceedAndTriggerEvents()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 9, TotalTests);

            // Wait for SDK initialization on native platforms
            yield return TestInitializer.WaitForInitialization();

            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Track event calls
            var willAppearCalled = false;
            var didRecordImpressionCalled = false;
            UnityBannerAd willAppearBanner = null;
            UnityBannerAd impressionBanner = null;

            // Subscribe to events
            UnityBannerAdEvent willAppearHandler = banner =>
            {
                willAppearCalled = true;
                willAppearBanner = banner;
                Debug.Log(TestConstants.DebugLogPrefixes.WillAppearEvent);
            };

            UnityBannerAdEvent impressionHandler = banner =>
            {
                didRecordImpressionCalled = true;
                impressionBanner = banner;
                Debug.Log(TestConstants.DebugLogPrefixes.ImpressionEvent);
            };

            _sharedBannerAd.WillAppear += willAppearHandler;
            _sharedBannerAd.DidRecordImpression += impressionHandler;

            try
            {
                var adLoadRequest = new BannerAdLoadRequest(TestConstants.Placements.Banner, BannerSize.Standard);
                var loadTask = _sharedBannerAd.Load(adLoadRequest);
                yield return new WaitUntil(() => loadTask.IsCompleted);

                Assert.IsNotNull(loadTask.Result, TestConstants.AssertionMessages.LoadResultShouldNotBeNull);

                // Use AdLoadResultAssert to handle ad inventory errors as Inconclusive
                AdLoadResultAssert.AssertSuccess(loadTask.Result, TestConstants.AssertionMessages.LoadShouldSucceed);

                // Wait to allow banner to be displayed and events to fire
                yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

                // Verify WillAppear was triggered
                Assert.IsTrue(willAppearCalled, TestConstants.AssertionMessages.WillAppearShouldTrigger);
                Assert.AreSame(_sharedBannerAd, willAppearBanner, TestConstants.AssertionMessages.WillAppearPassCorrectInstance);

                // Verify DidRecordImpression was triggered
                Assert.IsTrue(didRecordImpressionCalled, TestConstants.AssertionMessages.ImpressionShouldTrigger);
                Assert.AreSame(_sharedBannerAd, impressionBanner, TestConstants.AssertionMessages.ImpressionPassCorrectInstance);
            }
            finally
            {
                // Defensive cleanup with null checks
                if (_sharedBannerAd != null)
                {
                    _sharedBannerAd.WillAppear -= willAppearHandler;
                    _sharedBannerAd.DidRecordImpression -= impressionHandler;
                }
            }
        }

        /// <summary>
        /// Test that Request property is available after a successful load.
        /// </summary>
        [Test, Order(10)]
        public void Request_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 10, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var request = _sharedBannerAd.Request;
            Assert.IsNotNull(request, TestConstants.AssertionMessages.RequestAvailableAfterLoad);
        }

        /// <summary>
        /// Test that LoadId is available after a successful load.
        /// </summary>
        [Test, Order(11)]
        public void LoadId_AfterSuccessfulLoad_ShouldNotBeNullOrEmpty()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 11, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var loadId = _sharedBannerAd.LoadId;
            Assert.IsFalse(string.IsNullOrEmpty(loadId), TestConstants.AssertionMessages.LoadIdAvailableAfterLoad);
        }

        /// <summary>
        /// Test that LoadMetrics is available after a successful load.
        /// </summary>
        [Test, Order(12)]
        public void LoadMetrics_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 12, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var metrics = _sharedBannerAd.LoadMetrics;
            Assert.IsNotNull(metrics, TestConstants.AssertionMessages.LoadMetricsAvailableAfterLoad);
        }

        /// <summary>
        /// Test that BannerSize is available after a successful load.
        /// </summary>
        [Test, Order(13)]
        public void BannerSize_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 13, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var bannerSize = _sharedBannerAd.BannerSize;
            Assert.IsNotNull(bannerSize, TestConstants.AssertionMessages.BannerSizeAvailableAfterLoad);
        }

        /// <summary>
        /// Test that WinningBidInfo is available after a successful load.
        /// </summary>
        [Test, Order(14)]
        public void WinningBidInfo_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 14, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var bidInfo = _sharedBannerAd.WinningBidInfo;
            Assert.IsNotNull(bidInfo, TestConstants.AssertionMessages.WinningBidInfoAvailableAfterLoad);
        }

        /// <summary>
        /// Test that ToString returns a valid JSON format.
        /// </summary>
        [Test, Order(15)]
        public void ToString_WhenCalled_ShouldReturnValidJson()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 15, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var jsonString = _sharedBannerAd.ToString();

            Assert.IsNotNull(jsonString, TestConstants.AssertionMessages.ToStringShouldReturnNonNull);
            Assert.IsTrue(jsonString.Contains(TestConstants.Json.OpenBrace), TestConstants.AssertionMessages.JsonShouldContainOpenBrace);
            Assert.IsTrue(jsonString.Contains(TestConstants.Json.CloseBrace), TestConstants.AssertionMessages.JsonShouldContainCloseBrace);
        }

        /// <summary>
        /// Test that the GameObject has a RectTransform component for UI layout.
        /// </summary>
        [Test, Order(16)]
        public void GameObject_WhenBannerAdCreated_ShouldHaveRectTransformComponent()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 16, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var rectTransform = _sharedBannerAd.gameObject.GetComponent<RectTransform>();
            Assert.IsNotNull(rectTransform, TestConstants.AssertionMessages.RectTransformComponentRequired);
        }

        /// <summary>
        /// Test that the banner can be relocated to the top-left corner of the screen.
        /// </summary>
        [UnityTest, Order(17)]
        public IEnumerator Position_WhenSetToTopLeft_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 17, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var rectTransform = _sharedBannerAd.GetComponent<RectTransform>();
            UnityBannerTestUtilities.SetBannerPosition(rectTransform, TestConstants.BannerAlignment.Left, TestConstants.BannerAlignment.UnityUI.Top);

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            Assert.IsNotNull(rectTransform, TestConstants.AssertionMessages.RectTransformShouldNotBeNull);
            Assert.AreEqual(TestConstants.BannerAlignment.Left, rectTransform.anchorMin.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Top, rectTransform.anchorMin.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinYMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.Left, rectTransform.anchorMax.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Top, rectTransform.anchorMax.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxYMatches);
        }

        /// <summary>
        /// Test that the banner can be relocated to the top-right corner of the screen.
        /// </summary>
        [UnityTest, Order(18)]
        public IEnumerator Position_WhenSetToTopRight_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 18, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var rectTransform = _sharedBannerAd.GetComponent<RectTransform>();
            UnityBannerTestUtilities.SetBannerPosition(rectTransform, TestConstants.BannerAlignment.Right, TestConstants.BannerAlignment.UnityUI.Top);

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            Assert.IsNotNull(rectTransform, TestConstants.AssertionMessages.RectTransformShouldNotBeNull);
            Assert.AreEqual(TestConstants.BannerAlignment.Right, rectTransform.anchorMin.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Top, rectTransform.anchorMin.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinYMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.Right, rectTransform.anchorMax.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Top, rectTransform.anchorMax.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxYMatches);
        }

        /// <summary>
        /// Test that the banner can be relocated to the bottom-left corner of the screen.
        /// </summary>
        [UnityTest, Order(19)]
        public IEnumerator Position_WhenSetToBottomLeft_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 19, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var rectTransform = _sharedBannerAd.GetComponent<RectTransform>();
            UnityBannerTestUtilities.SetBannerPosition(rectTransform, TestConstants.BannerAlignment.Left, TestConstants.BannerAlignment.UnityUI.Bottom);

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            Assert.IsNotNull(rectTransform, TestConstants.AssertionMessages.RectTransformShouldNotBeNull);
            Assert.AreEqual(TestConstants.BannerAlignment.Left, rectTransform.anchorMin.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Bottom, rectTransform.anchorMin.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinYMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.Left, rectTransform.anchorMax.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Bottom, rectTransform.anchorMax.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxYMatches);
        }

        /// <summary>
        /// Test that the banner can be relocated to the bottom-right corner of the screen.
        /// </summary>
        [UnityTest, Order(20)]
        public IEnumerator Position_WhenSetToBottomRight_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 20, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var rectTransform = _sharedBannerAd.GetComponent<RectTransform>();
            UnityBannerTestUtilities.SetBannerPosition(rectTransform, TestConstants.BannerAlignment.Right, TestConstants.BannerAlignment.UnityUI.Bottom);

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            Assert.IsNotNull(rectTransform, TestConstants.AssertionMessages.RectTransformShouldNotBeNull);
            Assert.AreEqual(TestConstants.BannerAlignment.Right, rectTransform.anchorMin.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Bottom, rectTransform.anchorMin.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinYMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.Right, rectTransform.anchorMax.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Bottom, rectTransform.anchorMax.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxYMatches);
        }

        /// <summary>
        /// Test that the banner can be relocated to the top-center of the screen.
        /// </summary>
        [UnityTest, Order(21)]
        public IEnumerator Position_WhenSetToTopCenter_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 21, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var rectTransform = _sharedBannerAd.GetComponent<RectTransform>();
            UnityBannerTestUtilities.SetBannerPosition(rectTransform, TestConstants.BannerAlignment.HorizontalCenter, TestConstants.BannerAlignment.UnityUI.Top);

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            Assert.IsNotNull(rectTransform, TestConstants.AssertionMessages.RectTransformShouldNotBeNull);
            Assert.AreEqual(TestConstants.BannerAlignment.HorizontalCenter, rectTransform.anchorMin.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Top, rectTransform.anchorMin.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinYMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.HorizontalCenter, rectTransform.anchorMax.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Top, rectTransform.anchorMax.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxYMatches);
        }

        /// <summary>
        /// Test that the banner can be relocated to the bottom-center of the screen.
        /// </summary>
        [UnityTest, Order(22)]
        public IEnumerator Position_WhenSetToBottomCenter_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 22, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var rectTransform = _sharedBannerAd.GetComponent<RectTransform>();
            UnityBannerTestUtilities.SetBannerPosition(rectTransform, TestConstants.BannerAlignment.HorizontalCenter, TestConstants.BannerAlignment.UnityUI.Bottom);

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            Assert.IsNotNull(rectTransform, TestConstants.AssertionMessages.RectTransformShouldNotBeNull);
            Assert.AreEqual(TestConstants.BannerAlignment.HorizontalCenter, rectTransform.anchorMin.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Bottom, rectTransform.anchorMin.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinYMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.HorizontalCenter, rectTransform.anchorMax.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxXMatches);
            Assert.AreEqual(TestConstants.BannerAlignment.UnityUI.Bottom, rectTransform.anchorMax.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxYMatches);
        }

        /// <summary>
        /// Test that the Reset method works and clears the loaded ad without throwing exceptions.
        /// </summary>
        [Test, Order(23)]
        public void Reset_WhenCalled_ShouldClearLoadedAdWithoutThrowing()
        {
            TestProgressTracker.NotifyTestStart(nameof(UnityBannerAdNativeTests), 23, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Reset the banner ad to clear the loaded ad
            Assert.DoesNotThrow(() => _sharedBannerAd.Reset(), TestConstants.AssertionMessages.ResetShouldNotThrow);
        }

        #endregion
    }
}

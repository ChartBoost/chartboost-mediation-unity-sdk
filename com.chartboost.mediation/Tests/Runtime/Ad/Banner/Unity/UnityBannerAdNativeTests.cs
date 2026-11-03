using System.Collections;
using System.Collections.Generic;
using Chartboost.Mediation;
using Chartboost.Testing;
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
    public class UnityBannerAdNativeTests : NativeTestFixture
    {
        private static UnityBannerAd _sharedBannerAd;
        private static bool _loaded;
        private static Canvas _sharedCanvas;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _loaded = false;
            ChartboostMediation.TestMode = true;

            // Initialize SDK for native platform tests
            TestInitializer.Initialize();

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
                rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = ScreenAnchor.Center.UnityUIAnchor;
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

            if (!DevicePlatform.IsNative)
                return;

            // Cleanup shared banner ad instance
            if (_sharedBannerAd != null && _sharedBannerAd.gameObject != null)
                Object.DestroyImmediate(_sharedBannerAd.gameObject);
            _sharedBannerAd = null;

            // Cleanup shared canvas
            if (_sharedCanvas != null && _sharedCanvas.gameObject != null)
                Object.DestroyImmediate(_sharedCanvas.gameObject);
            _sharedCanvas = null;
        }

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
            Assert.IsTrue(ChartboostMediation.TestMode, TestConstants.AssertionMessages.TestModeShouldBeEnabled);
        }

        /// <summary>
        /// Verify the shared banner ad was created in OneTimeSetUp.
        /// </summary>
        [Test, Order(2)]
        public void SharedBannerAd_WhenCreatedInSetup_ShouldExist()
        {
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
                yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.NativeAdLoad);

                Assert.IsNotNull(loadTask.Result, TestConstants.AssertionMessages.LoadResultShouldNotBeNull);

                // Use AdLoadResultAssert to handle ad inventory errors as Inconclusive
                AdLoadResultAssert.AssertSuccess(loadTask.Result, TestConstants.AssertionMessages.LoadShouldSucceed);
                _loaded = true;

                yield return TestWait.Until(() => willAppearCalled && didRecordImpressionCalled, TestConstants.Timeouts.NativeEvent);

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
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var request = _sharedBannerAd.Request;
            Assert.IsNotNull(request, TestConstants.AssertionMessages.RequestAvailableAfterLoad);
        }

        /// <summary>
        /// Test that LoadId is available after a successful load.
        /// </summary>
        [Test, Order(11)]
        public void LoadId_AfterSuccessfulLoad_ShouldNotBeNullOrEmpty()
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var loadId = _sharedBannerAd.LoadId;
            Assert.IsFalse(string.IsNullOrEmpty(loadId), TestConstants.AssertionMessages.LoadIdAvailableAfterLoad);
        }

        /// <summary>
        /// Test that LoadMetrics is available after a successful load.
        /// </summary>
        [Test, Order(12)]
        public void LoadMetrics_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var metrics = _sharedBannerAd.LoadMetrics;
            Assert.IsNotNull(metrics, TestConstants.AssertionMessages.LoadMetricsAvailableAfterLoad);
        }

        /// <summary>
        /// Test that BannerSize is available after a successful load.
        /// </summary>
        [Test, Order(13)]
        public void BannerSize_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var bannerSize = _sharedBannerAd.BannerSize;
            Assert.IsNotNull(bannerSize, TestConstants.AssertionMessages.BannerSizeAvailableAfterLoad);
        }

        /// <summary>
        /// Test that WinningBidInfo is available after a successful load.
        /// </summary>
        [Test, Order(14)]
        public void WinningBidInfo_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var bidInfo = _sharedBannerAd.WinningBidInfo;
            Assert.IsNotNull(bidInfo, TestConstants.AssertionMessages.WinningBidInfoAvailableAfterLoad);
        }

        /// <summary>
        /// Test that ToString returns a valid JSON format.
        /// </summary>
        [Test, Order(15)]
        public void ToString_WhenCalled_ShouldReturnValidJson()
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

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
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var rectTransform = _sharedBannerAd.gameObject.GetComponent<RectTransform>();
            Assert.IsNotNull(rectTransform, TestConstants.AssertionMessages.RectTransformComponentRequired);
        }

        /// <summary>
        /// The loaded banner can be anchored to each screen anchor through its RectTransform.
        /// </summary>
        [UnityTest, Order(17)]
        public IEnumerator Position_WhenSetToAnchor_ShouldRelocateBanner([ValueSource(typeof(ScreenAnchor), nameof(ScreenAnchor.All))] ScreenAnchor anchor)
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var rectTransform = _sharedBannerAd.GetComponent<RectTransform>();
            Assert.IsNotNull(rectTransform, TestConstants.AssertionMessages.RectTransformShouldNotBeNull);
            var target = anchor.UnityUIAnchor;
            UnityBannerTestUtilities.SetBannerPosition(rectTransform, target.x, target.y);

            // One frame for UnityBannerAd.Update to sync the new layout to the native banner.
            yield return null;

            Assert.AreEqual(target.x, rectTransform.anchorMin.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinXMatches);
            Assert.AreEqual(target.y, rectTransform.anchorMin.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMinYMatches);
            Assert.AreEqual(target.x, rectTransform.anchorMax.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxXMatches);
            Assert.AreEqual(target.y, rectTransform.anchorMax.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.AnchorMaxYMatches);
        }

        /// <summary>
        /// Test that the Reset method works and clears the loaded ad without throwing exceptions.
        /// </summary>
        [Test, Order(18)]
        public void Reset_WhenCalled_ShouldClearLoadedAdWithoutThrowing()
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            // Reset the banner ad to clear the loaded ad
            Assert.DoesNotThrow(() => _sharedBannerAd.Reset(), TestConstants.AssertionMessages.ResetShouldNotThrow);
        }

        #endregion
    }
}

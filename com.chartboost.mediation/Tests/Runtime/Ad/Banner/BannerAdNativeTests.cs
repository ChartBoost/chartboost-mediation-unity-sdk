using System.Collections;
using System.Collections.Generic;
using Chartboost.Mediation;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using Chartboost.Tests.Runtime.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    /// <summary>
    /// Native platform tests for platform-specific BannerAd implementations (Android/iOS only).
    /// Tests the IBannerAd returned by ChartboostMediation.GetBannerAd(), which returns:
    /// - Chartboost.Mediation.iOS.Ad.Banner.BannerAd on iOS
    /// - Chartboost.Mediation.Android.Ad.Banner.BannerAd on Android
    /// These tests use the actual Chartboost Mediation SDK and run in order.
    /// </summary>
    [TestFixture]
    public class BannerAdNativeTests
    {
        private static IBannerAd _sharedBannerAd;
        private const int TotalTests = 28;

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

            // Create the shared IBannerAd instance using ChartboostMediation factory method
            // This will return a platform-specific implementation (iOS.BannerAd or Android.BannerAd)
            _sharedBannerAd = ChartboostMediation.GetBannerAd();

            // Position the banner at a screen center before loading (following BannerAdController pattern)
            // Position: center of screen in native coordinates (pixels converted to native)
            // Pivot: (0.5, 0.5) means the pivot is at the center of the banner
            _sharedBannerAd.Position = new Vector2(DensityConverters.PixelsToNative(Screen.width / TestConstants.Dimensions.ScreenCenterDivisor), DensityConverters.PixelsToNative(Screen.height / TestConstants.Dimensions.ScreenCenterDivisor));
            _sharedBannerAd.Pivot = new Vector2(TestConstants.Pivots.Center, TestConstants.Pivots.Center);
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
            _sharedBannerAd?.Dispose();
            _sharedBannerAd = null;
        }

        private static bool IsNativePlatform() => TestPlatformUtilities.IsNativePlatform();

        #region Sequential Integration Tests - Banner Lifecycle (Creation → Configuration → Load → Verification → Reset)

        /// <summary>
        /// Verify that TestMode is enabled for these tests.
        /// </summary>
        [Test, Order(1)]
        public void TestMode_WhenSetInSetup_ShouldBeTrue()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 1, TotalTests);
            Assert.IsTrue(ChartboostMediation.TestMode, TestConstants.AssertionMessages.TestModeShouldBeEnabled);
        }

        /// <summary>
        /// Verify the shared banner ad was created in OneTimeSetUp.
        /// </summary>
        [Test, Order(2)]
        public void SharedBannerAd_WhenCreatedInSetup_ShouldExist()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 2, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdCreated);
        }

        /// <summary>
        /// Test that the Draggable property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(3)]
        public void Draggable_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 3, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            _sharedBannerAd.Draggable = true;
            Assert.IsTrue(_sharedBannerAd.Draggable);

            _sharedBannerAd.Draggable = false;
            Assert.IsFalse(_sharedBannerAd.Draggable);
        }

        /// <summary>
        /// Test that the HorizontalAlignment property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(4)]
        public void HorizontalAlignment_WhenSetToAllValues_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 4, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Left;
            Assert.AreEqual(BannerHorizontalAlignment.Left, _sharedBannerAd.HorizontalAlignment);

            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Center;
            Assert.AreEqual(BannerHorizontalAlignment.Center, _sharedBannerAd.HorizontalAlignment);

            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Right;
            Assert.AreEqual(BannerHorizontalAlignment.Right, _sharedBannerAd.HorizontalAlignment);

            // Reset to center for the load test
            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Center;
        }

        /// <summary>
        /// Test that the VerticalAlignment property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(5)]
        public void VerticalAlignment_WhenSetToAllValues_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 5, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Top;
            Assert.AreEqual(BannerVerticalAlignment.Top, _sharedBannerAd.VerticalAlignment);

            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Center;
            Assert.AreEqual(BannerVerticalAlignment.Center, _sharedBannerAd.VerticalAlignment);

            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Bottom;
            Assert.AreEqual(BannerVerticalAlignment.Bottom, _sharedBannerAd.VerticalAlignment);

            // Reset to center for the load test
            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Center;
        }

        /// <summary>
        /// Test that the Keywords property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(6)]
        public void Keywords_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 6, TotalTests);
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
        [Test, Order(7)]
        public void PartnerSettings_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 7, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var settings = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _sharedBannerAd.PartnerSettings = settings;

            var retrieved = _sharedBannerAd.PartnerSettings;
            Assert.IsNotNull(retrieved, TestConstants.AssertionMessages.PartnerSettingsShouldNotBeNull);
            Assert.That(retrieved, Contains.Key(TestConstants.Keywords.Key), TestConstants.AssertionMessages.PartnerSettingsShouldContainTestKey);
        }

        /// <summary>
        /// Test that the Position property can be set and retrieved before loading.
        /// </summary>
        [Test, Order(8)]
        public void Position_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 8, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var testPosition = new Vector2(TestConstants.Dimensions.TestCoordX, TestConstants.Dimensions.TestCoordY);
            _sharedBannerAd.Position = testPosition;

            var retrieved = _sharedBannerAd.Position;
            Assert.AreEqual(testPosition.x, retrieved.x, TestConstants.Tolerance.Assertion);
            Assert.AreEqual(testPosition.y, retrieved.y, TestConstants.Tolerance.Assertion);

            // Reset to the screen center for the load test
            _sharedBannerAd.Position = new Vector2(DensityConverters.PixelsToNative(Screen.width / TestConstants.Dimensions.ScreenCenterDivisor), DensityConverters.PixelsToNative(Screen.height / TestConstants.Dimensions.ScreenCenterDivisor));
        }

        /// <summary>
        /// Test that the Pivot property can be set and retrieved before loading.
        /// </summary>
        [Test, Order(9)]
        public void Pivot_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 9, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var testPivot = new Vector2(TestConstants.Pivots.Center, TestConstants.Pivots.Center);
            _sharedBannerAd.Pivot = testPivot;

            var retrieved = _sharedBannerAd.Pivot;
            Assert.AreEqual(testPivot.x, retrieved.x, TestConstants.Tolerance.Assertion);
            Assert.AreEqual(testPivot.y, retrieved.y, TestConstants.Tolerance.Assertion);
        }

        /// <summary>
        /// Test that the Visible property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(10)]
        public void Visible_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 10, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            _sharedBannerAd.Visible = true;
            Assert.IsTrue(_sharedBannerAd.Visible);

            _sharedBannerAd.Visible = false;
            Assert.IsFalse(_sharedBannerAd.Visible);

            // Reset to visible for the load test
            _sharedBannerAd.Visible = true;
        }

        /// <summary>
        /// Test that the ContainerSize property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(11)]
        public void ContainerSize_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 11, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var testSize = new ContainerSize((int)TestConstants.Dimensions.StandardWidth, (int)TestConstants.Dimensions.StandardHeight);
            _sharedBannerAd.ContainerSize = testSize;

            var retrieved = _sharedBannerAd.ContainerSize;
            Assert.AreEqual(testSize.Width, retrieved.Width, TestConstants.AssertionMessages.ContainerSizeShouldMatch);
            Assert.AreEqual(testSize.Height, retrieved.Height, TestConstants.AssertionMessages.ContainerSizeShouldMatch);
        }

        /// <summary>
        /// Load the shared banner ad for the first time and verify it succeeds.
        /// Also verify that WillAppear and DidRecordImpression events are triggered.
        /// </summary>
        [UnityTest, Order(12)]
        public IEnumerator Load_WhenCalledWithValidRequest_ShouldSucceedAndTriggerEvents()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 12, TotalTests);

            // Wait for SDK initialization on native platforms
            yield return TestInitializer.WaitForInitialization();

            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Track event calls
            var willAppearCalled = false;
            var didRecordImpressionCalled = false;
            IBannerAd willAppearBanner = null;
            IBannerAd impressionBanner = null;

            // Subscribe to events
            BannerAdEvent willAppearHandler = banner =>
            {
                willAppearCalled = true;
                willAppearBanner = banner;
                Debug.Log(TestConstants.DebugLogPrefixes.WillAppearEvent);
            };

            BannerAdEvent impressionHandler = banner =>
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
        [Test, Order(13)]
        public void Request_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 13, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var request = _sharedBannerAd.Request;
            Assert.IsNotNull(request, TestConstants.AssertionMessages.RequestAvailableAfterLoad);
        }

        /// <summary>
        /// Test that LoadId is available after a successful load.
        /// </summary>
        [Test, Order(14)]
        public void LoadId_AfterSuccessfulLoad_ShouldNotBeNullOrEmpty()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 14, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var loadId = _sharedBannerAd.LoadId;
            Assert.IsFalse(string.IsNullOrEmpty(loadId), TestConstants.AssertionMessages.LoadIdAvailableAfterLoad);
        }

        /// <summary>
        /// Test that LoadMetrics is available after a successful load.
        /// </summary>
        [Test, Order(15)]
        public void LoadMetrics_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 15, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var metrics = _sharedBannerAd.LoadMetrics;
            Assert.IsNotNull(metrics, TestConstants.AssertionMessages.LoadMetricsAvailableAfterLoad);
        }

        /// <summary>
        /// Test that BannerSize is available after a successful load.
        /// </summary>
        [Test, Order(16)]
        public void BannerSize_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 16, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            var bannerSize = _sharedBannerAd.BannerSize;
            Assert.IsNotNull(bannerSize, TestConstants.AssertionMessages.BannerSizeAvailableAfterLoad);
        }

        /// <summary>
        /// Test that WinningBidInfo is available after a successful load.
        /// </summary>
        [Test, Order(17)]
        public void WinningBidInfo_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 17, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var bidInfo = _sharedBannerAd.WinningBidInfo;
            Assert.IsNotNull(bidInfo, TestConstants.AssertionMessages.WinningBidInfoAvailableAfterLoad);
        }

        // Note: ToString test is not included for raw IBannerAd implementations (Android.BannerAd, iOS.BannerAd)
        // ToString() is only implemented on wrapper classes (UnityBannerAd, BannerVisualElement)
        // See UnityBannerAdNativeTests.cs and BannerVisualElementNativeTests.cs for ToString tests

        /// <summary>
        /// Test that the banner can be repositioned to the top-left corner of the screen.
        /// </summary>
        [UnityTest, Order(19)]
        public IEnumerator Position_WhenSetToTopLeft_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 19, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Set pivot to top-left (0, 0) so banner's top-left corner aligns with position
            _sharedBannerAd.Pivot = new Vector2(TestConstants.Pivots.Min, TestConstants.Pivots.Min);
            // Position at top-left corner (0, 0)
            _sharedBannerAd.Position = new Vector2(TestConstants.Pivots.Min, TestConstants.Pivots.Min);

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            var position = _sharedBannerAd.Position;
            Assert.AreEqual(TestConstants.Pivots.Min, position.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionXMatches);
            Assert.AreEqual(TestConstants.Pivots.Min, position.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionYMatches);
        }

        /// <summary>
        /// Test that the banner can be repositioned to the top-right corner of the screen.
        /// </summary>
        [UnityTest, Order(20)]
        public IEnumerator Position_WhenSetToTopRight_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 20, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Set pivot to top-right (1, 0) so banner's top-right corner aligns with position
            _sharedBannerAd.Pivot = new Vector2(TestConstants.Pivots.Max, TestConstants.Pivots.Min);
            // Position at top-right corner (screen.width, 0)
            var topRightPosition = new Vector2(DensityConverters.PixelsToNative(Screen.width), TestConstants.Pivots.Min);
            _sharedBannerAd.Position = topRightPosition;

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            var position = _sharedBannerAd.Position;
            Assert.AreEqual(topRightPosition.x, position.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionXMatches);
            Assert.AreEqual(topRightPosition.y, position.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionYMatches);
        }

        /// <summary>
        /// Test that the banner can be repositioned to the bottom-left corner of the screen.
        /// </summary>
        [UnityTest, Order(21)]
        public IEnumerator Position_WhenSetToBottomLeft_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 21, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Set pivot to bottom-left (0, 1) so banner's bottom-left corner aligns with position
            _sharedBannerAd.Pivot = new Vector2(TestConstants.Pivots.Min, TestConstants.Pivots.Max);
            // Position at bottom-left corner (0, screen.height)
            var bottomLeftPosition = new Vector2(TestConstants.Pivots.Min, DensityConverters.PixelsToNative(Screen.height));
            _sharedBannerAd.Position = bottomLeftPosition;

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            var position = _sharedBannerAd.Position;
            Assert.AreEqual(bottomLeftPosition.x, position.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionXMatches);
            Assert.AreEqual(bottomLeftPosition.y, position.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionYMatches);
        }

        /// <summary>
        /// Test that the banner can be repositioned to the bottom-right corner of the screen.
        /// </summary>
        [UnityTest, Order(22)]
        public IEnumerator Position_WhenSetToBottomRight_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 22, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Set pivot to bottom-right (1, 1) so banner's bottom-right corner aligns with position
            _sharedBannerAd.Pivot = new Vector2(TestConstants.Pivots.Max, TestConstants.Pivots.Max);
            // Position at bottom-right corner (screen.width, screen.height)
            var bottomRightPosition = new Vector2(DensityConverters.PixelsToNative(Screen.width), DensityConverters.PixelsToNative(Screen.height));
            _sharedBannerAd.Position = bottomRightPosition;

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            var position = _sharedBannerAd.Position;
            Assert.AreEqual(bottomRightPosition.x, position.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionXMatches);
            Assert.AreEqual(bottomRightPosition.y, position.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionYMatches);
        }

        /// <summary>
        /// Test that the banner can be repositioned to the top-center of the screen.
        /// </summary>
        [UnityTest, Order(23)]
        public IEnumerator Position_WhenSetToTopCenter_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 23, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Set pivot to top-center (0.5, 0) so banner's top-center aligns with position
            var topCenterPivot = new Vector2(TestConstants.Pivots.Center, TestConstants.Pivots.Min);
            _sharedBannerAd.Pivot = topCenterPivot;
            // Position at top-center (screen.width/2, 0)
            var topCenterPosition = new Vector2(DensityConverters.PixelsToNative(Screen.width / TestConstants.Dimensions.ScreenCenterDivisor), TestConstants.Pivots.Min);
            _sharedBannerAd.Position = topCenterPosition;

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            var position = _sharedBannerAd.Position;
            Assert.AreEqual(topCenterPosition.x, position.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionXMatches);
            Assert.AreEqual(topCenterPosition.y, position.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionYMatches);
        }

        /// <summary>
        /// Test that the banner can be repositioned to the bottom-center of the screen.
        /// This matches BannerAdController's default positioning.
        /// </summary>
        [UnityTest, Order(24)]
        public IEnumerator Position_WhenSetToBottomCenter_ShouldRelocateBanner()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 24, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Set pivot to bottom-center (0.5, 1) so banner's bottom-center aligns with position
            _sharedBannerAd.Pivot = new Vector2(TestConstants.Pivots.Center, TestConstants.Pivots.Max);
            // Position at bottom-center (screen.width/2, screen.height)
            var bottomCenterPosition = new Vector2(DensityConverters.PixelsToNative(Screen.width / TestConstants.Dimensions.ScreenCenterDivisor), DensityConverters.PixelsToNative(Screen.height));
            _sharedBannerAd.Position = bottomCenterPosition;

            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);

            var position = _sharedBannerAd.Position;
            Assert.AreEqual(bottomCenterPosition.x, position.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionXMatches);
            Assert.AreEqual(bottomCenterPosition.y, position.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionYMatches);

            // Reset to center position and pivot for subsequent tests
            _sharedBannerAd.Pivot = new Vector2(TestConstants.Pivots.Center, TestConstants.Pivots.Center);
            _sharedBannerAd.Position = new Vector2(DensityConverters.PixelsToNative(Screen.width / TestConstants.Dimensions.ScreenCenterDivisor), DensityConverters.PixelsToNative(Screen.height / TestConstants.Dimensions.ScreenCenterDivisor));
        }

        /// <summary>
        /// Test that the Visible property can toggle banner visibility.
        /// </summary>
        [UnityTest, Order(25)]
        public IEnumerator Visible_WhenToggled_ShouldChangeVisibility()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 25, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Make banner invisible
            _sharedBannerAd.Visible = false;
            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);
            Assert.IsFalse(_sharedBannerAd.Visible);

            // Make banner visible again
            _sharedBannerAd.Visible = true;
            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);
            Assert.IsTrue(_sharedBannerAd.Visible);
        }

        /// <summary>
        /// Test that the HorizontalAlignment property can be changed after loading.
        /// </summary>
        [UnityTest, Order(26)]
        public IEnumerator HorizontalAlignment_WhenChangedAfterLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 26, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Right;
            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);
            Assert.AreEqual(BannerHorizontalAlignment.Right, _sharedBannerAd.HorizontalAlignment);

            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Center;
            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);
            Assert.AreEqual(BannerHorizontalAlignment.Center, _sharedBannerAd.HorizontalAlignment);
        }

        /// <summary>
        /// Test that the VerticalAlignment property can be changed after loading.
        /// </summary>
        [UnityTest, Order(27)]
        public IEnumerator VerticalAlignment_WhenChangedAfterLoad_ShouldBeRetrievable()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 27, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Bottom;
            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);
            Assert.AreEqual(BannerVerticalAlignment.Bottom, _sharedBannerAd.VerticalAlignment);

            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Top;
            yield return new WaitForSeconds(TestConstants.Timing.WaitForBannerDisplay);
            Assert.AreEqual(BannerVerticalAlignment.Top, _sharedBannerAd.VerticalAlignment);

            // Reset to center alignment for subsequent tests
            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Center;
        }

        /// <summary>
        /// Test that the Reset method works and clears the loaded ad without throwing exceptions.
        /// </summary>
        [Test, Order(28)]
        public void Reset_WhenCalled_ShouldClearLoadedAdWithoutThrowing()
        {
            TestProgressTracker.NotifyTestStart(nameof(BannerAdNativeTests), 28, TotalTests);
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            // Reset the banner ad to clear the loaded ad
            Assert.DoesNotThrow(() => _sharedBannerAd.Reset(), TestConstants.AssertionMessages.ResetShouldNotThrow);
        }

        #endregion
    }
}

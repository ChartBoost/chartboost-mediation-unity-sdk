using System.Collections;
using System.Collections.Generic;
using Chartboost.Mediation;
using Chartboost.Testing;
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
    public class BannerAdNativeTests : NativeTestFixture
    {
        private static IBannerAd _sharedBannerAd;
        private static bool _loaded;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ChartboostMediation.TestMode = true;
            _loaded = false;

            // Initialize SDK for native platform tests
            TestInitializer.Initialize();

            // Create the shared IBannerAd instance using ChartboostMediation factory method
            // This will return a platform-specific implementation (iOS.BannerAd or Android.BannerAd)
            _sharedBannerAd = ChartboostMediation.GetBannerAd();

            // Position the banner at a screen center before loading (following BannerAdController pattern)
            // Position: center of screen in native coordinates (pixels converted to native)
            // Pivot: (0.5, 0.5) means the pivot is at the center of the banner
            _sharedBannerAd.Position = ScreenAnchor.Center.NativePosition;
            _sharedBannerAd.Pivot = ScreenAnchor.Center.Pivot;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ChartboostMediation.TestMode = false;

            if (!DevicePlatform.IsNative)
                return;

            // Cleanup shared banner ad instance
            _sharedBannerAd?.Dispose();
            _sharedBannerAd = null;
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
        }

        /// <summary>
        /// Test that the Draggable property works correctly on the shared instance before loading.
        /// </summary>
        [Test, Order(3)]
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
        [Test, Order(4)]
        public void HorizontalAlignment_WhenSetToAllValues_ShouldBeRetrievable()
        {
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
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);

            var testPosition = new Vector2(TestConstants.Dimensions.TestCoordX, TestConstants.Dimensions.TestCoordY);
            _sharedBannerAd.Position = testPosition;

            var retrieved = _sharedBannerAd.Position;
            Assert.AreEqual(testPosition.x, retrieved.x, TestConstants.Tolerance.Assertion);
            Assert.AreEqual(testPosition.y, retrieved.y, TestConstants.Tolerance.Assertion);

            // Reset to the screen center for the load test
            _sharedBannerAd.Position = ScreenAnchor.Center.NativePosition;
        }

        /// <summary>
        /// Test that the Pivot property can be set and retrieved before loading.
        /// </summary>
        [Test, Order(9)]
        public void Pivot_WhenSetBeforeLoad_ShouldBeRetrievable()
        {
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
        [Test, Order(13)]
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
        [Test, Order(14)]
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
        [Test, Order(15)]
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
        [Test, Order(16)]
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
        [Test, Order(17)]
        public void WinningBidInfo_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var bidInfo = _sharedBannerAd.WinningBidInfo;
            Assert.IsNotNull(bidInfo, TestConstants.AssertionMessages.WinningBidInfoAvailableAfterLoad);
        }

        // Note: ToString test is not included for raw IBannerAd implementations (Android.BannerAd, iOS.BannerAd)
        // ToString() is only implemented on wrapper classes (UnityBannerAd, BannerVisualElement)
        // See UnityBannerAdNativeTests.cs and BannerVisualElementNativeTests.cs for ToString tests

        /// <summary>
        /// The loaded banner can be moved to each screen anchor, its pivot aligned to that anchor.
        /// </summary>
        [UnityTest, Order(18)]
        public IEnumerator Position_WhenSetToAnchor_ShouldRelocateBanner([ValueSource(typeof(ScreenAnchor), nameof(ScreenAnchor.All))] ScreenAnchor anchor)
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            var target = anchor.NativePosition;
            _sharedBannerAd.Pivot = anchor.Pivot;
            _sharedBannerAd.Position = target;
            yield return TestWait.Until(() => Vector2.Distance(_sharedBannerAd.Position, target) <= TestConstants.Tolerance.Assertion, TestConstants.Timeouts.NativeProperty);

            var position = _sharedBannerAd.Position;
            Assert.AreEqual(target.x, position.x, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionXMatches);
            Assert.AreEqual(target.y, position.y, TestConstants.Tolerance.Assertion, TestConstants.AssertionMessages.PositionYMatches);

            // Back to the screen center for the next case and the tests after this one.
            _sharedBannerAd.Pivot = ScreenAnchor.Center.Pivot;
            _sharedBannerAd.Position = ScreenAnchor.Center.NativePosition;
        }

        /// <summary>
        /// Test that the Visible property can toggle banner visibility.
        /// </summary>
        [UnityTest, Order(19)]
        public IEnumerator Visible_WhenToggled_ShouldChangeVisibility()
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            // Make banner invisible
            _sharedBannerAd.Visible = false;
            yield return TestWait.Until(() => !_sharedBannerAd.Visible, TestConstants.Timeouts.NativeProperty);
            Assert.IsFalse(_sharedBannerAd.Visible);

            // Make banner visible again
            _sharedBannerAd.Visible = true;
            yield return TestWait.Until(() => _sharedBannerAd.Visible, TestConstants.Timeouts.NativeProperty);
            Assert.IsTrue(_sharedBannerAd.Visible);
        }

        /// <summary>
        /// Test that the HorizontalAlignment property can be changed after loading.
        /// </summary>
        [UnityTest, Order(20)]
        public IEnumerator HorizontalAlignment_WhenChangedAfterLoad_ShouldBeRetrievable()
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Right;
            yield return TestWait.Until(() => _sharedBannerAd.HorizontalAlignment == BannerHorizontalAlignment.Right, TestConstants.Timeouts.NativeProperty);
            Assert.AreEqual(BannerHorizontalAlignment.Right, _sharedBannerAd.HorizontalAlignment);

            _sharedBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Center;
            yield return TestWait.Until(() => _sharedBannerAd.HorizontalAlignment == BannerHorizontalAlignment.Center, TestConstants.Timeouts.NativeProperty);
            Assert.AreEqual(BannerHorizontalAlignment.Center, _sharedBannerAd.HorizontalAlignment);
        }

        /// <summary>
        /// Test that the VerticalAlignment property can be changed after loading.
        /// </summary>
        [UnityTest, Order(21)]
        public IEnumerator VerticalAlignment_WhenChangedAfterLoad_ShouldBeRetrievable()
        {
            Assert.IsNotNull(_sharedBannerAd, TestConstants.AssertionMessages.SharedBannerAdMustExist);
            AdLoadResultAssert.RequireLoaded(_loaded);

            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Bottom;
            yield return TestWait.Until(() => _sharedBannerAd.VerticalAlignment == BannerVerticalAlignment.Bottom, TestConstants.Timeouts.NativeProperty);
            Assert.AreEqual(BannerVerticalAlignment.Bottom, _sharedBannerAd.VerticalAlignment);

            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Top;
            yield return TestWait.Until(() => _sharedBannerAd.VerticalAlignment == BannerVerticalAlignment.Top, TestConstants.Timeouts.NativeProperty);
            Assert.AreEqual(BannerVerticalAlignment.Top, _sharedBannerAd.VerticalAlignment);

            // Reset to center alignment for subsequent tests
            _sharedBannerAd.VerticalAlignment = BannerVerticalAlignment.Center;
        }

        /// <summary>
        /// Test that the Reset method works and clears the loaded ad without throwing exceptions.
        /// </summary>
        [Test, Order(22)]
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

using System.Collections;
using System.Collections.Generic;
using Chartboost.Mediation;
using Chartboost.Testing;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Requests;
using Chartboost.Tests.Runtime.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Fullscreen
{
    /// <summary>
    /// Native platform tests for platform-specific IFullscreenAd implementations (Android/iOS only).
    /// Tests the IFullscreenAd returned by ChartboostMediation.LoadFullscreenAd(), which returns:
    /// - Chartboost.Mediation.iOS.Ad.Fullscreen.FullscreenAd on iOS
    /// - Chartboost.Mediation.Android.Ad.Fullscreen.FullscreenAd on Android
    /// These tests use the actual Chartboost Mediation SDK and run in order.
    /// Note: Show() is NOT tested as we lack instrumentation to close the ad after showing.
    /// </summary>
    [TestFixture]
    public class FullscreenAdNativeTests : NativeTestFixture
    {
        private static IFullscreenAd _sharedFullscreenAd;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ChartboostMediation.TestMode = true;

            // Initialize SDK for native platform tests
            TestInitializer.Initialize();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ChartboostMediation.TestMode = false;

            if (!DevicePlatform.IsNative)
                return;

            // Cleanup shared fullscreen ad instance
            _sharedFullscreenAd?.Dispose();
            _sharedFullscreenAd = null;
        }

        #region Sequential Integration Tests - Fullscreen Ad Lifecycle (Load → Property Verification → Disposal)

        /// <summary>
        /// Verify that TestMode is enabled for these tests.
        /// </summary>
        [Test, Order(1)]
        public void TestMode_WhenSetInSetup_ShouldBeTrue()
        {
            Assert.IsTrue(ChartboostMediation.TestMode, TestConstants.AssertionMessages.TestModeShouldBeEnabled);
        }

        /// <summary>
        /// Load a fullscreen ad (interstitial) and verify it succeeds.
        /// Note: We cannot test Show() as we lack instrumentation to close the ad.
        /// </summary>
        [UnityTest, Order(2)]
        public IEnumerator Load_WhenCalledWithInterstitialRequest_ShouldSucceed()
        {
            // Wait for SDK initialization on native platforms
            yield return TestInitializer.WaitForInitialization();

            var adLoadRequest = new FullscreenAdLoadRequest(TestConstants.Placements.Interstitial, new Dictionary<string, string>());
            var loadTask = ChartboostMediation.LoadFullscreenAd(adLoadRequest);
            yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.NativeAdLoad);

            Assert.IsNotNull(loadTask.Result, TestConstants.AssertionMessages.LoadResultShouldNotBeNull);

            // Use AdLoadResultAssert to handle ad inventory errors as Inconclusive
            AdLoadResultAssert.AssertSuccess(loadTask.Result, TestConstants.AssertionMessages.FullscreenAdLoadShouldSucceed);

            // Store the loaded ad for further tests
            _sharedFullscreenAd = loadTask.Result.Ad;
            Assert.IsNotNull(_sharedFullscreenAd, TestConstants.AssertionMessages.LoadedFullscreenAdShouldNotBeNull);
        }

        /// <summary>
        /// Test that Request property is available after a successful load.
        /// </summary>
        [Test, Order(3)]
        public void Request_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            var request = _sharedFullscreenAd.Request;
            Assert.IsNotNull(request, TestConstants.AssertionMessages.RequestAvailableAfterLoad);
            Assert.AreEqual(TestConstants.Placements.Interstitial, request.PlacementName, TestConstants.AssertionMessages.RequestContainsCorrectPlacement);
        }

        /// <summary>
        /// Test that LoadId is available after a successful load.
        /// </summary>
        [Test, Order(4)]
        public void LoadId_AfterSuccessfulLoad_ShouldNotBeNullOrEmpty()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            var loadId = _sharedFullscreenAd.LoadId;
            Assert.IsFalse(string.IsNullOrEmpty(loadId), TestConstants.AssertionMessages.LoadIdShouldNotBeEmpty);
        }

        /// <summary>
        /// Test that WinningBidInfo is available after a successful load.
        /// </summary>
        [Test, Order(5)]
        public void WinningBidInfo_AfterSuccessfulLoad_ShouldNotBeNull()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            var bidInfo = _sharedFullscreenAd.WinningBidInfo;
            Assert.IsNotNull(bidInfo, TestConstants.AssertionMessages.WinningBidInfoAvailableAfterLoad);
        }

        /// <summary>
        /// Test that CustomData property can be set and retrieved.
        /// </summary>
        [Test, Order(6)]
        public void CustomData_WhenSetAndRetrieved_ShouldMatch()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            _sharedFullscreenAd.CustomData = TestConstants.TestData.CustomData;

            var retrieved = _sharedFullscreenAd.CustomData;
            Assert.AreEqual(TestConstants.TestData.CustomData, retrieved, TestConstants.AssertionMessages.CustomDataShouldMatch);
        }

        /// <summary>
        /// Test that CustomData can be set to null.
        /// </summary>
        [Test, Order(7)]
        public void CustomData_WhenSetToNull_ShouldBeNullOrEmpty()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            _sharedFullscreenAd.CustomData = null;

            var retrieved = _sharedFullscreenAd.CustomData;
            Assert.IsTrue(string.IsNullOrEmpty(retrieved), TestConstants.AssertionMessages.CustomDataShouldBeNullOrEmpty);
        }

        /// <summary>
        /// Test that DidClick event can be subscribed and unsubscribed without errors.
        /// Note: We cannot trigger the event as it requires user interaction.
        /// </summary>
        [Test, Order(8)]
        public void DidClick_WhenSubscribedAndUnsubscribed_ShouldNotThrow()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            FullscreenAdEvent testHandler = _ => Debug.Log(TestConstants.DebugLogPrefixes.FullscreenClickEvent);

            Assert.DoesNotThrow(() =>
            {
                _sharedFullscreenAd.DidClick += testHandler;
                _sharedFullscreenAd.DidClick -= testHandler;
            }, TestConstants.AssertionMessages.SubscribingDidClickShouldNotThrow);
        }

        /// <summary>
        /// Test that DidClose event can be subscribed and unsubscribed without errors.
        /// Note: We cannot trigger the event as it requires showing and closing the ad.
        /// </summary>
        [Test, Order(9)]
        public void DidClose_WhenSubscribedAndUnsubscribed_ShouldNotThrow()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            FullscreenAdEventWithError testHandler = (_, _) => Debug.Log(TestConstants.DebugLogPrefixes.FullscreenCloseEvent);

            Assert.DoesNotThrow(() =>
            {
                _sharedFullscreenAd.DidClose += testHandler;
                _sharedFullscreenAd.DidClose -= testHandler;
            }, TestConstants.AssertionMessages.SubscribingDidCloseShouldNotThrow);
        }

        /// <summary>
        /// Test that DidExpire event can be subscribed and unsubscribed without errors.
        /// Note: We cannot trigger the event as it requires the ad to expire.
        /// </summary>
        [Test, Order(10)]
        public void DidExpire_WhenSubscribedAndUnsubscribed_ShouldNotThrow()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            FullscreenAdEvent testHandler = _ => Debug.Log(TestConstants.DebugLogPrefixes.FullscreenExpireEvent);

            Assert.DoesNotThrow(() =>
            {
                _sharedFullscreenAd.DidExpire += testHandler;
                _sharedFullscreenAd.DidExpire -= testHandler;
            }, TestConstants.AssertionMessages.SubscribingDidExpireShouldNotThrow);
        }

        /// <summary>
        /// Test that DidRecordImpression event can be subscribed and unsubscribed without errors.
        /// Note: We cannot trigger the event as it requires showing the ad.
        /// </summary>
        [Test, Order(11)]
        public void DidRecordImpression_WhenSubscribedAndUnsubscribed_ShouldNotThrow()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            FullscreenAdEvent testHandler = _ => Debug.Log(TestConstants.DebugLogPrefixes.FullscreenImpressionEvent);

            Assert.DoesNotThrow(() =>
            {
                _sharedFullscreenAd.DidRecordImpression += testHandler;
                _sharedFullscreenAd.DidRecordImpression -= testHandler;
            }, TestConstants.AssertionMessages.SubscribingDidRecordImpressionShouldNotThrow);
        }

        /// <summary>
        /// Test that DidReward event can be subscribed and unsubscribed without errors.
        /// Note: This event is for rewarded ads. We cannot trigger it without showing the ad.
        /// </summary>
        [Test, Order(12)]
        public void DidReward_WhenSubscribedAndUnsubscribed_ShouldNotThrow()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            FullscreenAdEvent testHandler = _ => Debug.Log(TestConstants.DebugLogPrefixes.FullscreenRewardEvent);

            Assert.DoesNotThrow(() =>
            {
                _sharedFullscreenAd.DidReward += testHandler;
                _sharedFullscreenAd.DidReward -= testHandler;
            }, TestConstants.AssertionMessages.SubscribingDidRewardShouldNotThrow);
        }

        /// <summary>
        /// Test that Dispose can be called without throwing exceptions.
        /// </summary>
        [Test, Order(13)]
        public void Dispose_WhenCalled_ShouldNotThrow()
        {
            AdLoadResultAssert.RequireLoaded(_sharedFullscreenAd != null);

            // Dispose the ad
            Assert.DoesNotThrow(() => _sharedFullscreenAd.Dispose(), TestConstants.AssertionMessages.DisposeShouldNotThrow);

            // Clear the reference since it's now disposed
            _sharedFullscreenAd = null;
        }

        #endregion
    }
}

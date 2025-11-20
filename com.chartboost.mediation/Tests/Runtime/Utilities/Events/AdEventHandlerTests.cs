using System;
using System.Collections;
using Chartboost.Mediation.Utilities;
using Chartboost.Mediation.Utilities.Events;
using Chartboost.Tests.Runtime.Ad.Banner;
using Chartboost.Tests.Runtime.Ad.Fullscreen;
using Chartboost.Tests.Runtime.Ad.Fullscreen.Queue;
using Chartboost.Tests.Runtime.Requests;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Utilities.Events
{
    public class AdEventHandlerTests
    {
        private TestFullscreenAd _testFullscreenAd;
        private TestBannerAd _testBannerAd;
        private TestFullscreenAdQueue _testAdQueue;

        [SetUp]
        public void SetUp()
        {
            _testFullscreenAd = new TestFullscreenAd((IntPtr)TestConstants.UniqueIds.Primary);
            _testBannerAd = new TestBannerAd((IntPtr)TestConstants.UniqueIds.Primary);
            _testAdQueue = new TestFullscreenAdQueue((IntPtr)TestConstants.UniqueIds.Primary);

            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testFullscreenAd);
        }

        [TearDown]
        public void TearDown()
        {
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenEventRecordImpressionCallsOnRecordImpression()
        {
            AdEventHandler.ProcessFullscreenEvent(TestConstants.UniqueIds.Primary, FullscreenAdEvents.RecordImpression, string.Empty, string.Empty);

            yield return null;

            Assert.IsTrue(_testFullscreenAd.RecordImpressionCalled);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenEventClickCallsOnClick()
        {
            AdEventHandler.ProcessFullscreenEvent(TestConstants.UniqueIds.Primary, FullscreenAdEvents.Click, string.Empty, string.Empty);

            yield return null;

            Assert.IsTrue(_testFullscreenAd.ClickCalled);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenEventRewardCallsOnReward()
        {
            AdEventHandler.ProcessFullscreenEvent(TestConstants.UniqueIds.Primary, FullscreenAdEvents.Reward, string.Empty, string.Empty);

            yield return null;

            Assert.IsTrue(_testFullscreenAd.RewardCalled);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenEventExpireCallsOnExpireAndReleasesAd()
        {
            AdEventHandler.ProcessFullscreenEvent(TestConstants.UniqueIds.Primary, FullscreenAdEvents.Expire, string.Empty, string.Empty);

            yield return null;

            Assert.IsTrue(_testFullscreenAd.ExpireCalled);

            // Ad should be released from the cache
            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.FailedWeakReference));
            var ad = AdCache.GetAd(TestConstants.UniqueIds.Primary);
            Assert.IsNull(ad);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenEventCloseCallsOnCloseAndReleasesAd()
        {
            AdEventHandler.ProcessFullscreenEvent(TestConstants.UniqueIds.Primary, FullscreenAdEvents.Close, string.Empty, string.Empty);

            yield return null;

            Assert.IsTrue(_testFullscreenAd.CloseCalled);

            // Ad should be released from the cache
            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.FailedWeakReference));
            var ad = AdCache.GetAd(TestConstants.UniqueIds.Primary);
            Assert.IsNull(ad);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenEventCloseWithErrorPassesError()
        {
            AdEventHandler.ProcessFullscreenEvent(TestConstants.UniqueIds.Primary, FullscreenAdEvents.Close, TestConstants.Errors.CodeChartboost, TestConstants.Errors.Message);

            yield return null;

            Assert.IsTrue(_testFullscreenAd.CloseCalled);
            Assert.IsNotNull(_testFullscreenAd.CloseError);
            Assert.AreEqual(TestConstants.Errors.CodeChartboost, _testFullscreenAd.CloseError.Value.Code);
            Assert.AreEqual(TestConstants.Errors.Message, _testFullscreenAd.CloseError.Value.Message);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenEventCloseWithCodeOnlyPassesError()
        {
            AdEventHandler.ProcessFullscreenEvent(TestConstants.UniqueIds.Primary, FullscreenAdEvents.Close, TestConstants.Errors.CodeChartboost, string.Empty);

            yield return null;

            Assert.IsTrue(_testFullscreenAd.CloseCalled);
            Assert.IsNotNull(_testFullscreenAd.CloseError);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenEventCloseWithMessageOnlyPassesError()
        {
            AdEventHandler.ProcessFullscreenEvent(TestConstants.UniqueIds.Primary, FullscreenAdEvents.Close, string.Empty, TestConstants.Errors.Message);

            yield return null;

            Assert.IsTrue(_testFullscreenAd.CloseCalled);
            Assert.IsNotNull(_testFullscreenAd.CloseError);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenEventWithNonExistentAdLogsError()
        {
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.FullscreenEvent));

            AdEventHandler.ProcessFullscreenEvent(TestConstants.UniqueIds.ConstructorIntPtr, FullscreenAdEvents.RecordImpression, string.Empty, string.Empty);

            yield return null;
        }

        [UnityTest]
        public IEnumerator ProcessBannerEventLoadCallsOnWillAppear()
        {
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testBannerAd);

            AdEventHandler.ProcessBannerEvent(TestConstants.UniqueIds.Primary, BannerAdEvents.Load);

            yield return null;

            Assert.IsTrue(_testBannerAd.WillAppearCalled);
        }

        [UnityTest]
        public IEnumerator ProcessBannerEventClickCallsOnClick()
        {
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testBannerAd);

            AdEventHandler.ProcessBannerEvent(TestConstants.UniqueIds.Primary, BannerAdEvents.Click);

            yield return null;

            Assert.IsTrue(_testBannerAd.ClickCalled);
        }

        [UnityTest]
        public IEnumerator ProcessBannerEventRecordImpressionCallsOnRecordImpression()
        {
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testBannerAd);

            AdEventHandler.ProcessBannerEvent(TestConstants.UniqueIds.Primary, BannerAdEvents.RecordImpression);

            yield return null;

            Assert.IsTrue(_testBannerAd.RecordImpressionCalled);
        }

        [UnityTest]
        public IEnumerator ProcessBannerEventBeginDragCallsOnDragBeginWithPosition()
        {
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testBannerAd);

            AdEventHandler.ProcessBannerEvent(TestConstants.UniqueIds.Primary, BannerAdEvents.BeginDrag, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

            yield return null;

            Assert.IsTrue(_testBannerAd.DragBeginCalled);
            Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, _testBannerAd.DragBeginX);
            Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, _testBannerAd.DragBeginY);
        }

        [UnityTest]
        public IEnumerator ProcessBannerEventDragCallsOnDragWithPosition()
        {
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testBannerAd);

            AdEventHandler.ProcessBannerEvent(TestConstants.UniqueIds.Primary, BannerAdEvents.Drag, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

            yield return null;

            Assert.IsTrue(_testBannerAd.DragCalled);
            Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, _testBannerAd.DragX);
            Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, _testBannerAd.DragY);
        }

        [UnityTest]
        public IEnumerator ProcessBannerEventEndDragCallsOnDragEndWithPosition()
        {
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testBannerAd);

            AdEventHandler.ProcessBannerEvent(TestConstants.UniqueIds.Primary, BannerAdEvents.EndDrag, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

            yield return null;

            Assert.IsTrue(_testBannerAd.DragEndCalled);
            Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, _testBannerAd.DragEndX);
            Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, _testBannerAd.DragEndY);
        }

        [UnityTest]
        public IEnumerator ProcessBannerEventWithDefaultPositionUsesZero()
        {
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testBannerAd);

            AdEventHandler.ProcessBannerEvent(TestConstants.UniqueIds.Primary, BannerAdEvents.BeginDrag);

            yield return null;

            Assert.IsTrue(_testBannerAd.DragBeginCalled);
            Assert.AreEqual(TestConstants.Dimensions.Zero, _testBannerAd.DragBeginX);
            Assert.AreEqual(TestConstants.Dimensions.Zero, _testBannerAd.DragBeginY);
        }

        [UnityTest]
        public IEnumerator ProcessBannerEventWithNonExistentAdLogsError()
        {
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.BannerEvent));

            AdEventHandler.ProcessBannerEvent(TestConstants.UniqueIds.ConstructorIntPtr, BannerAdEvents.Load);

            yield return null;
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenAdQueueEventUpdateCallsOnDidUpdate()
        {
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testAdQueue);

            var testLoadResult = new TestAdLoadResult();
            AdEventHandler.ProcessFullscreenAdQueueEvent(TestConstants.UniqueIds.Primary, FullscreenAdQueueEvents.Update, testLoadResult, TestConstants.Queue.Capacity);

            yield return null;

            Assert.IsTrue(_testAdQueue.DidUpdateCalled);
            Assert.AreEqual(testLoadResult, _testAdQueue.UpdateLoadResult);
            Assert.AreEqual(TestConstants.Queue.Capacity, _testAdQueue.UpdateNumberOfAdsReady);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenAdQueueEventRemoveExpiredAdCallsOnDidRemoveExpiredAd()
        {
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testAdQueue);

            AdEventHandler.ProcessFullscreenAdQueueEvent(TestConstants.UniqueIds.Primary, FullscreenAdQueueEvents.RemoveExpiredAd, null, TestConstants.Queue.Capacity);

            yield return null;

            Assert.IsTrue(_testAdQueue.DidRemoveExpiredAdCalled);
            Assert.AreEqual(TestConstants.Queue.Capacity, _testAdQueue.RemoveExpiredAdNumberOfAdsReady);
        }

        [UnityTest]
        public IEnumerator ProcessFullscreenAdQueueEventWithNonExistentQueueLogsError()
        {
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.QueueEvent));

            AdEventHandler.ProcessFullscreenAdQueueEvent(TestConstants.UniqueIds.ConstructorIntPtr, FullscreenAdQueueEvents.Update, null, TestConstants.Queue.Capacity);

            yield return null;
        }
    }
}

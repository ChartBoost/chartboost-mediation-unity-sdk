using System;
using System.Collections;
using System.Collections.Generic;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    public class BannerAdBaseTests
    {

        private TestBannerAd _testBannerAd;
        private IntPtr _testUniqueId;
        private LogLevel _initialLogLevel;

        [SetUp]
        public void SetUp()
        {
            _initialLogLevel = LogController.LoggingLevel;
            LogController.LoggingLevel = LogLevel.Verbose;
            
            _testUniqueId = new IntPtr(TestConstants.UniqueIds.Primary);
            _testBannerAd = new TestBannerAd(_testUniqueId);
        }

        [TearDown]
        public void TearDown()
        {
            LogController.LoggingLevel = _initialLogLevel;
            _testBannerAd?.Dispose();
            AdCache.ReleaseAd(_testUniqueId.ToInt64());
        }

        [Test]
        public void ConstructorLogsCreation()
        {
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Creating));

            var uniqueId = new IntPtr(TestConstants.UniqueIds.ConstructorIntPtr);
            var bannerAd = new TestBannerAd(uniqueId);

            try
            {
                Assert.IsNotNull(bannerAd);
            }
            finally
            {
                bannerAd.Dispose();
                AdCache.ReleaseAd(uniqueId.ToInt64());
            }
        }

        [Test]
        public void ConstructorTracksAdInCache()
        {
            var uniqueId = new IntPtr(TestConstants.UniqueIds.ConstructorLong);
            var bannerAd = new TestBannerAd(uniqueId);

            try
            {
                var cachedAd = AdCache.GetAd(uniqueId.ToInt64());
                Assert.IsNotNull(cachedAd);
                Assert.AreSame(bannerAd, cachedAd);
            }
            finally
            {
                bannerAd.Dispose();
                AdCache.ReleaseAd(uniqueId.ToInt64());
            }
        }

        [UnityTest]
        public IEnumerator LoadLogsRequestAndReturnsError()
        {
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Load));

            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, BannerSize.Standard);
            var bannerLoadTask = _testBannerAd.Load(request);
            yield return bannerLoadTask;

            Assert.IsNotNull(bannerLoadTask.Result);
            Assert.IsNotNull(bannerLoadTask.Result.Error);
        }

        [UnityTest]
        public IEnumerator LoadStoresRequest()
        {
            var request = new BannerAdLoadRequest(TestConstants.Placements.Standard, BannerSize.Standard);
            var bannerLoadTask = _testBannerAd.Load(request);
            yield return bannerLoadTask;

            Assert.AreEqual(request, _testBannerAd.Request);
        }

        [Test]
        public void ResetLogsReset()
        {
            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Reset));

            _testBannerAd.Reset();
        }

        [Test]
        public void DisposeLogsDispose()
        {
            var uniqueId = new IntPtr(TestConstants.UniqueIds.Id77777);
            var bannerAd = new TestBannerAd(uniqueId);

            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Dispose));

            bannerAd.Dispose();

            AdCache.ReleaseAd(uniqueId.ToInt64());
        }

        [Test]
        public void DisposeCallsDisposeWithTrue()
        {
            var uniqueId = new IntPtr(TestConstants.UniqueIds.Dispose);
            var bannerAd = new TestBannerAd(uniqueId);

            bannerAd.Dispose();

            Assert.IsTrue(bannerAd.DisposeCalled);
            Assert.IsTrue(bannerAd.DisposingValue);

            AdCache.ReleaseAd(uniqueId.ToInt64());
        }

        [Test]
        public void DisposeMultipleTimesIsIdempotent()
        {
            _testBannerAd.Dispose();
            _testBannerAd.Dispose(); // The second call to dispose should not throw

            Assert.IsTrue(_testBannerAd.DisposeCalled);
        }

        [Test]
        public void IsDisposedPublicReturnsFalseInitially()
        {
            Assert.IsFalse(_testBannerAd.IsDisposedPublic);
        }

        [Test]
        public void IsDisposedPublicReturnsTrueAfterDispose()
        {
            _testBannerAd.Dispose();

            Assert.IsTrue(_testBannerAd.IsDisposedPublic);
        }

        [UnityTest]
        public IEnumerator OnWillAppearInvokesEvent()
        {
            var eventCalled = false;
            BannerAdEvent handler = _ => eventCalled = true;

            try
            {
                _testBannerAd.WillAppear += handler;

                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.WillAppear));

                _testBannerAd.TestOnWillAppear();

                yield return null;

                Assert.IsTrue(eventCalled);
            }
            finally
            {
                _testBannerAd.WillAppear -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnClickInvokesEvent()
        {
            var eventCalled = false;
            BannerAdEvent handler = _ => eventCalled = true;

            try
            {
                _testBannerAd.DidClick += handler;

                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Click));

                _testBannerAd.TestOnClick();

                yield return null;

                Assert.IsTrue(eventCalled);
            }
            finally
            {
                _testBannerAd.DidClick -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnRecordImpressionInvokesEvent()
        {
            var eventCalled = false;
            BannerAdEvent handler = _ => eventCalled = true;

            try
            {
                _testBannerAd.DidRecordImpression += handler;

                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.RecordImpression));

                _testBannerAd.TestOnRecordImpression();

                yield return null;

                Assert.IsTrue(eventCalled);
            }
            finally
            {
                _testBannerAd.DidRecordImpression -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnDragBeginInvokesEventWithPosition()
        {
            var eventCalled = false;
            var receivedX = 0f;
            var receivedY = 0f;

            BannerAdDragEvent handler = (_, x, y) =>
            {
                eventCalled = true;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _testBannerAd.DidBeginDrag += handler;

                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.DragBegin));

                _testBannerAd.TestOnDragBegin(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
            }
            finally
            {
                _testBannerAd.DidBeginDrag -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnDragInvokesEventWithPosition()
        {
            var eventCalled = false;
            var receivedX = 0f;
            var receivedY = 0f;

            BannerAdDragEvent handler = (_, x, y) =>
            {
                eventCalled = true;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _testBannerAd.DidDrag += handler;

                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Drag));

                _testBannerAd.TestOnDrag(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
            }
            finally
            {
                _testBannerAd.DidDrag -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnDragEndInvokesEventWithPosition()
        {
            var eventCalled = false;
            var receivedX = 0f;
            var receivedY = 0f;

            BannerAdDragEvent handler = (_, x, y) =>
            {
                eventCalled = true;
                receivedX = x;
                receivedY = y;
            };

            try
            {
                _testBannerAd.DidEndDrag += handler;

                LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.DragEnd));

                _testBannerAd.TestOnDragEnd(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, receivedX);
                Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, receivedY);
            }
            finally
            {
                _testBannerAd.DidEndDrag -= handler;
            }
        }

        [Test]
        public void SetContainerBackgroundColorLogsColor()
        {
            var color = new Color(TestConstants.Colors.Red, TestConstants.Colors.Green, TestConstants.Colors.Blue, TestConstants.Colors.Alpha);

            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.ContainerBackground));

            _testBannerAd.TestSetContainerBackgroundColor(color);
        }

        [Test]
        public void SetAdBackgroundColorLogsColor()
        {
            var color = new Color(TestConstants.Colors.Red, TestConstants.Colors.Green, TestConstants.Colors.Blue, TestConstants.Colors.Alpha);

            LogAssert.Expect(LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.AdBackground));

            _testBannerAd.TestSetAdBackgroundColor(color);
        }

        [Test]
        public void AdRelativePositionCanBeSetAndRetrieved()
        {
            var position = new Vector2(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

            _testBannerAd.TestSetAdRelativePosition(position);

            var result = _testBannerAd.TestGetAdRelativePosition();

            Assert.AreEqual(position, result);
        }

        [Test]
        public void KeywordsCanBeSetAndRetrieved()
        {
            var keywords = new Dictionary<string, string> { { "key1", "value1" } };

            _testBannerAd.Keywords = keywords;

            Assert.AreSame(keywords, _testBannerAd.Keywords);
        }

        [Test]
        public void PartnerSettingsCanBeSetAndRetrieved()
        {
            var settings = new Dictionary<string, string> { { "setting1", "value1" } };

            _testBannerAd.PartnerSettings = settings;

            Assert.AreSame(settings, _testBannerAd.PartnerSettings);
        }

        [Test]
        public void PositionCanBeSetAndRetrieved()
        {
            var position = new Vector2(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

            _testBannerAd.Position = position;

            Assert.AreEqual(position, _testBannerAd.Position);
        }

        [Test]
        public void PivotCanBeSetAndRetrieved()
        {
            var pivot = new Vector2(TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

            _testBannerAd.Pivot = pivot;

            Assert.AreEqual(pivot, _testBannerAd.Pivot);
        }

        [Test]
        public void VisibleCanBeSetAndRetrieved()
        {
            _testBannerAd.Visible = true;

            Assert.IsTrue(_testBannerAd.Visible);

            _testBannerAd.Visible = false;

            Assert.IsFalse(_testBannerAd.Visible);
        }

        [Test]
        public void DraggableCanBeSetAndRetrieved()
        {
            _testBannerAd.Draggable = true;

            Assert.IsTrue(_testBannerAd.Draggable);

            _testBannerAd.Draggable = false;

            Assert.IsFalse(_testBannerAd.Draggable);
        }

        [Test]
        public void ContainerSizeCanBeSetAndRetrieved()
        {
            var containerSize = new ContainerSize(320, 50);

            _testBannerAd.ContainerSize = containerSize;

            Assert.AreEqual(containerSize, _testBannerAd.ContainerSize);
        }

        [Test]
        public void HorizontalAlignmentCanBeSetAndRetrieved()
        {
            _testBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Center;

            Assert.AreEqual(BannerHorizontalAlignment.Center, _testBannerAd.HorizontalAlignment);

            _testBannerAd.HorizontalAlignment = BannerHorizontalAlignment.Right;

            Assert.AreEqual(BannerHorizontalAlignment.Right, _testBannerAd.HorizontalAlignment);
        }

        [Test]
        public void VerticalAlignmentCanBeSetAndRetrieved()
        {
            _testBannerAd.VerticalAlignment = BannerVerticalAlignment.Top;

            Assert.AreEqual(BannerVerticalAlignment.Top, _testBannerAd.VerticalAlignment);

            _testBannerAd.VerticalAlignment = BannerVerticalAlignment.Bottom;

            Assert.AreEqual(BannerVerticalAlignment.Bottom, _testBannerAd.VerticalAlignment);
        }
    }
}

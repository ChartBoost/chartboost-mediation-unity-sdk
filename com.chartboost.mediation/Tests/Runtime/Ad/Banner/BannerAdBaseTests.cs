using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Chartboost.Caching;
using Chartboost.Logging;
using Chartboost.Mediation.Ad;
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
            AdCache<IAd>.ReleaseAd(_testUniqueId.ToInt64());
        }

        [Test]
        public void ConstructorLogsCreation()
        {
            LogAssert.Expect(LogType.Log, new Regex(TestConstants.LogPatterns.Creating));

            var uniqueId = new IntPtr(TestConstants.UniqueIds.ConstructorIntPtr);
            var bannerAd = new TestBannerAd(uniqueId);

            try
            {
                Assert.IsNotNull(bannerAd);
            }
            finally
            {
                bannerAd.Dispose();
                AdCache<IAd>.ReleaseAd(uniqueId.ToInt64());
            }
        }

        [Test]
        public void ConstructorTracksAdInCache()
        {
            var uniqueId = new IntPtr(TestConstants.UniqueIds.ConstructorLong);
            var bannerAd = new TestBannerAd(uniqueId);

            try
            {
                var cachedAd = AdCache<IAd>.GetAd(uniqueId.ToInt64());
                Assert.IsNotNull(cachedAd);
                Assert.AreSame(bannerAd, cachedAd);
            }
            finally
            {
                bannerAd.Dispose();
                AdCache<IAd>.ReleaseAd(uniqueId.ToInt64());
            }
        }

        [UnityTest]
        public IEnumerator LoadLogsRequestAndReturnsError()
        {
            LogAssert.Expect(LogType.Log, new Regex(TestConstants.LogPatterns.Load));

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
            LogAssert.Expect(LogType.Log, new Regex(TestConstants.LogPatterns.Reset));

            _testBannerAd.Reset();
        }

        [Test]
        public void DisposeLogsDispose()
        {
            var uniqueId = new IntPtr(TestConstants.UniqueIds.Id77777);
            var bannerAd = new TestBannerAd(uniqueId);

            LogAssert.Expect(LogType.Log, new Regex(TestConstants.LogPatterns.Dispose));

            bannerAd.Dispose();

            AdCache<IAd>.ReleaseAd(uniqueId.ToInt64());
        }

        [Test]
        public void DisposeCallsDisposeWithTrue()
        {
            var uniqueId = new IntPtr(TestConstants.UniqueIds.Dispose);
            var bannerAd = new TestBannerAd(uniqueId);

            bannerAd.Dispose();

            Assert.IsTrue(bannerAd.DisposeCalled);
            Assert.IsTrue(bannerAd.DisposingValue);

            AdCache<IAd>.ReleaseAd(uniqueId.ToInt64());
        }

        [Test]
        public void DisposeMultipleTimesIsIdempotent()
        {
            _testBannerAd.Dispose();
            _testBannerAd.Dispose(); // The second call to dispose should not throw

            Assert.IsTrue(_testBannerAd.DisposeCalled);
        }

        [UnityTest]
        public IEnumerator OnWillAppearInvokesEvent()
        {
            var eventCalled = false;
            BannerAdEvent handler = _ => eventCalled = true;

            try
            {
                _testBannerAd.WillAppear += handler;

                LogAssert.Expect(LogType.Log, new Regex(TestConstants.LogPatterns.WillAppear));

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

                LogAssert.Expect(LogType.Log, new Regex(TestConstants.LogPatterns.Click));

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

                LogAssert.Expect(LogType.Log, new Regex(TestConstants.LogPatterns.RecordImpression));

                _testBannerAd.TestOnRecordImpression();

                yield return null;

                Assert.IsTrue(eventCalled);
            }
            finally
            {
                _testBannerAd.DidRecordImpression -= handler;
            }
        }

        // One row per drag phase: how to raise it, what TestBannerAd recorded, and the log it writes.
        private static readonly Dictionary<string, (Action<TestBannerAd, float, float> raise,
            Func<TestBannerAd, (bool called, float x, float y)> recorded, string log)> DragPhases = new()
        {
            ["DragBegin"] = ((ad, x, y) => ad.TestOnDragBegin(x, y), ad => (ad.DragBeginCalled, ad.DragBeginX, ad.DragBeginY), TestConstants.LogPatterns.DragBegin),
            ["Drag"] = ((ad, x, y) => ad.TestOnDrag(x, y), ad => (ad.DragCalled, ad.DragX, ad.DragY), TestConstants.LogPatterns.Drag),
            ["DragEnd"] = ((ad, x, y) => ad.TestOnDragEnd(x, y), ad => (ad.DragEndCalled, ad.DragEndX, ad.DragEndY), TestConstants.LogPatterns.DragEnd),
        };

        [TestCase("DragBegin")]
        [TestCase("Drag")]
        [TestCase("DragEnd")]
        public void DragEventsCarryThePosition(string phaseName)
        {
            var phase = DragPhases[phaseName];
            LogAssert.Expect(LogType.Log, new Regex(phase.log));

            phase.raise(_testBannerAd, TestConstants.Dimensions.XPositionPrecise, TestConstants.Dimensions.YPositionPrecise);

            var (called, x, y) = phase.recorded(_testBannerAd);
            Assert.IsTrue(called, $"{phaseName} was not raised");
            Assert.AreEqual(TestConstants.Dimensions.XPositionPrecise, x);
            Assert.AreEqual(TestConstants.Dimensions.YPositionPrecise, y);
        }

        [Test]
        public void SetContainerBackgroundColorLogsColor()
        {
            var color = new Color(TestConstants.Colors.Red, TestConstants.Colors.Green, TestConstants.Colors.Blue, TestConstants.Colors.Alpha);

            LogAssert.Expect(LogType.Log, new Regex(TestConstants.LogPatterns.ContainerBackground));

            _testBannerAd.TestSetContainerBackgroundColor(color);
        }

        [Test]
        public void SetAdBackgroundColorLogsColor()
        {
            var color = new Color(TestConstants.Colors.Red, TestConstants.Colors.Green, TestConstants.Colors.Blue, TestConstants.Colors.Alpha);

            LogAssert.Expect(LogType.Log, new Regex(TestConstants.LogPatterns.AdBackground));

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
    }
}

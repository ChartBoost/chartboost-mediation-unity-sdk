using System;
using System.Collections;
using System.Collections.Generic;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Fullscreen;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Fullscreen
{
    public class FullscreenAdBaseTests
    {
        private TestFullscreenAd _testFullscreenAd;
        private IntPtr _testUniqueId;
        private LogLevel _initialLogLevel;

        [SetUp]
        public void SetUp()
        {
            _initialLogLevel = LogController.LoggingLevel;
            LogController.LoggingLevel = LogLevel.Verbose;

            _testUniqueId = new IntPtr(TestConstants.UniqueIds.Primary);
            _testFullscreenAd = new TestFullscreenAd(_testUniqueId);
        }

        [TearDown]
        public void TearDown()
        {
            LogController.LoggingLevel = _initialLogLevel;
            _testFullscreenAd?.Dispose();
            AdCache.ReleaseAd(_testUniqueId.ToInt64());
        }

        [Test]
        public void ConstructorWithIntPtrLogsCreation()
        {
            LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Creating));

            var uniqueId = new IntPtr(TestConstants.UniqueIds.ConstructorIntPtr);
            var fullscreenAd = new TestFullscreenAd(uniqueId);

            try
            {
                Assert.IsNotNull(fullscreenAd);
            }
            finally
            {
                fullscreenAd.Dispose();
                AdCache.ReleaseAd(uniqueId.ToInt64());
            }
        }

        [Test]
        public void ConstructorWithLongLogsCreation()
        {
            LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Creating));

            const long uniqueId = TestConstants.UniqueIds.ConstructorLong;
            var fullscreenAd = new TestFullscreenAd(uniqueId);

            try
            {
                Assert.IsNotNull(fullscreenAd);
            }
            finally
            {
                fullscreenAd.Dispose();
                AdCache.ReleaseAd(uniqueId);
            }
        }

        [Test]
        public void ConstructorTracksAdInCache()
        {
            var uniqueId = new IntPtr(TestConstants.UniqueIds.Id77777);
            var fullscreenAd = new TestFullscreenAd(uniqueId);

            try
            {
                var cachedAd = AdCache.GetAd(uniqueId.ToInt64());
                Assert.IsNotNull(cachedAd);
                Assert.AreSame(fullscreenAd, cachedAd);
            }
            finally
            {
                fullscreenAd.Dispose();
                AdCache.ReleaseAd(uniqueId.ToInt64());
            }
        }

        [Test]
        public void DisposeCallsDisposeWithTrue()
        {
            var uniqueId = new IntPtr(TestConstants.UniqueIds.Dispose);
            var fullscreenAd = new TestFullscreenAd(uniqueId);

            fullscreenAd.Dispose();

            Assert.IsTrue(fullscreenAd.DisposeCalled);
            Assert.IsTrue(fullscreenAd.DisposingValue);

            AdCache.ReleaseAd(uniqueId.ToInt64());
        }

        [Test]
        public void DisposeMultipleTimesIsIdempotent()
        {
            _testFullscreenAd.Dispose();
            _testFullscreenAd.Dispose(); // The second call to dispose should not throw

            Assert.IsTrue(_testFullscreenAd.DisposeCalled);
        }

        [UnityTest]
        public IEnumerator OnClickInvokesEvent()
        {
            var eventCalled = false;
            FullscreenAdEvent handler = _ => eventCalled = true;

            try
            {
                _testFullscreenAd.DidClick += handler;

                LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Click));

                _testFullscreenAd.TestOnClick();

                yield return null;

                Assert.IsTrue(eventCalled);
            }
            finally
            {
                _testFullscreenAd.DidClick -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnCloseInvokesEvent()
        {
            var eventCalled = false;
            ChartboostMediationError? receivedError = null;

            FullscreenAdEventWithError handler = (_, error) =>
            {
                eventCalled = true;
                receivedError = error;
            };

            try
            {
                _testFullscreenAd.DidClose += handler;

                LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Close));

                var testError = new ChartboostMediationError(Errors.ShowFailureUnknown);
                _testFullscreenAd.TestOnClose(testError);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.IsNotNull(receivedError);
                Assert.AreEqual(testError.Code, receivedError.Value.Code);
            }
            finally
            {
                _testFullscreenAd.DidClose -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnCloseWithNullErrorInvokesEvent()
        {
            var eventCalled = false;
            ChartboostMediationError? receivedError = null;

            FullscreenAdEventWithError handler = (_, error) =>
            {
                eventCalled = true;
                receivedError = error;
            };

            try
            {
                _testFullscreenAd.DidClose += handler;

                LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Close));

                _testFullscreenAd.TestOnClose(null);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.IsNull(receivedError);
            }
            finally
            {
                _testFullscreenAd.DidClose -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnRecordImpressionInvokesEvent()
        {
            var eventCalled = false;
            FullscreenAdEvent handler = _ => eventCalled = true;

            try
            {
                _testFullscreenAd.DidRecordImpression += handler;

                LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.RecordImpression));

                _testFullscreenAd.TestOnRecordImpression();

                yield return null;

                Assert.IsTrue(eventCalled);
            }
            finally
            {
                _testFullscreenAd.DidRecordImpression -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnExpireInvokesEvent()
        {
            var eventCalled = false;
            FullscreenAdEvent handler = _ => eventCalled = true;

            try
            {
                _testFullscreenAd.DidExpire += handler;

                LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Expire));

                _testFullscreenAd.TestOnExpire();

                yield return null;

                Assert.IsTrue(eventCalled);
            }
            finally
            {
                _testFullscreenAd.DidExpire -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnRewardInvokesEvent()
        {
            var eventCalled = false;
            FullscreenAdEvent handler = _ => eventCalled = true;

            try
            {
                _testFullscreenAd.DidReward += handler;

                LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Reward));

                _testFullscreenAd.TestOnReward();

                yield return null;

                Assert.IsTrue(eventCalled);
            }
            finally
            {
                _testFullscreenAd.DidReward -= handler;
            }
        }

        [Test]
        public void CustomDataCanBeSetAndRetrieved()
        {
            _testFullscreenAd.CustomData = TestConstants.TestData.CustomData;

            Assert.AreEqual(TestConstants.TestData.CustomData, _testFullscreenAd.CustomData);
        }

        [Test]
        public void RequestReturnsNullBeforeSet()
        {
            Assert.IsNull(_testFullscreenAd.Request);
        }

        [Test]
        public void RequestReturnsSetValue()
        {
            var request = new FullscreenAdLoadRequest(TestConstants.Placements.Standard, new Dictionary<string, string>());
            _testFullscreenAd.SetRequest(request);

            Assert.AreSame(request, _testFullscreenAd.Request);
        }

        [Test]
        public void LoadIdReturnsNullBeforeSet()
        {
            Assert.IsNull(_testFullscreenAd.LoadId);
        }

        [Test]
        public void LoadIdReturnsSetValue()
        {
            _testFullscreenAd.SetLoadId(TestConstants.Auction.LineItemId);

            Assert.AreEqual(TestConstants.Auction.LineItemId, _testFullscreenAd.LoadId);
        }

        [Test]
        public void WinningBidInfoReturnsNonNull()
        {
            Assert.IsNotNull(_testFullscreenAd.WinningBidInfo);
        }

        [Test]
        public void WinningBidInfoReturnsSetValue()
        {
            var bidInfo = new BidInfo(null, null, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemId, TestConstants.Auction.LineItemNameTest);
            _testFullscreenAd.SetBidInfo(bidInfo);

            var result = _testFullscreenAd.WinningBidInfo;

            Assert.AreEqual(bidInfo.Price, result.Price);
            Assert.AreEqual(bidInfo.LineItemId, result.LineItemId);
            Assert.AreEqual(bidInfo.LineItemName, result.LineItemName);
        }

        [UnityTest]
        public IEnumerator ShowReturnsSuccessResult()
        {
            var showResult = new AdShowResult(null);
            _testFullscreenAd.ShowResult = showResult;

            var result = _testFullscreenAd.Show();
            yield return result;

            Assert.IsNotNull(result.Result);
            Assert.IsNull(result.Result.Error);
        }

        [UnityTest]
        public IEnumerator ShowReturnsErrorResult()
        {
            var testError = new ChartboostMediationError(Errors.ShowFailureAdNotReady);
            var showResult = new AdShowResult(testError);
            _testFullscreenAd.ShowResult = showResult;

            var result = _testFullscreenAd.Show();
            yield return result;

            Assert.IsNotNull(result.Result);
            Assert.IsNotNull(result.Result.Error);
            Assert.AreEqual(Errors.ShowFailureAdNotReady, result.Result.Error.Value.Message);
        }

        [UnityTest]
        public IEnumerator ShowAfterDisposeReturnsInvalidAdError()
        {
            _testFullscreenAd.Dispose();

            var result = _testFullscreenAd.Show();
            yield return result;

            Assert.IsNotNull(result.Result);
            Assert.IsNotNull(result.Result.Error);
            Assert.AreEqual(Errors.InvalidAdError, result.Result.Error.Value.Message);
        }

        [Test]
        public void IsDisposedPublicReturnsFalseInitially()
        {
            Assert.IsFalse(_testFullscreenAd.IsDisposedPublic);
        }

        [Test]
        public void IsDisposedPublicReturnsTrueAfterDispose()
        {
            _testFullscreenAd.Dispose();

            Assert.IsTrue(_testFullscreenAd.IsDisposedPublic);
        }
    }
}

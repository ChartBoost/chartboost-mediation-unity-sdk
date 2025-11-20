using System;
using System.Collections;
using System.Collections.Generic;
using Chartboost.Logging;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;
using UnityEngine.TestTools;
// ReSharper disable AssignNullToNotNullAttribute

namespace Chartboost.Tests.Runtime.Ad.Fullscreen.Queue
{
    public class FullscreenAdQueueBaseTests
    {

        private TestFullscreenAdQueue _testQueue;
        private IntPtr _testUniqueId;
        private LogLevel _initialLogLevel;

        [SetUp]
        public void SetUp()
        {
            _initialLogLevel = LogController.LoggingLevel;
            LogController.LoggingLevel = LogLevel.Verbose;

            _testUniqueId = new IntPtr(TestConstants.UniqueIds.Tertiary);
            _testQueue = new TestFullscreenAdQueue(_testUniqueId);
        }

        [TearDown]
        public void TearDown()
        {
            LogController.LoggingLevel = _initialLogLevel;
            AdCache.ReleaseAd(_testUniqueId.ToInt64());
        }

        [Test]
        public void ConstructorCreatesInstance()
        {
            Assert.IsNotNull(_testQueue);
        }

        [Test]
        public void ConstructorTracksQueueInCache()
        {
            var uniqueId = new IntPtr(TestConstants.UniqueIds.Cache);
            var queue = new TestFullscreenAdQueue(uniqueId);

            try
            {
                var cachedQueue = AdCache.GetAd(uniqueId.ToInt64());
                Assert.IsNotNull(cachedQueue);
                Assert.AreSame(queue, cachedQueue);
            }
            finally
            {
                AdCache.ReleaseAd(uniqueId.ToInt64());
            }
        }

        [Test]
        public void ConstructorStoresUniqueId()
        {
            Assert.AreEqual(_testUniqueId, _testQueue.UniqueIdPublic);
        }

        [Test]
        public void KeywordsCanBeSetAndRetrieved()
        {
            var keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _testQueue.Keywords = keywords;

            Assert.AreSame(keywords, _testQueue.Keywords);
        }

        [Test]
        public void QueueCapacityCanBeSetAndRetrieved()
        {
            _testQueue.QueueCapacity = TestConstants.Queue.Capacity;

            Assert.AreEqual(TestConstants.Queue.Capacity, _testQueue.QueueCapacity);
        }

        [Test]
        public void NumberOfAdsReadyReturnsSetValue()
        {
            _testQueue.NumberOfAdsReadyValue = TestConstants.Queue.NumberOfAdsReady;

            Assert.AreEqual(TestConstants.Queue.NumberOfAdsReady, _testQueue.NumberOfAdsReady);
        }

        [Test]
        public void IsRunningReturnsFalseInitially()
        {
            Assert.IsFalse(_testQueue.IsRunning);
        }

        [Test]
        public void IsRunningReturnsSetValue()
        {
            _testQueue.IsRunningValue = true;

            Assert.IsTrue(_testQueue.IsRunning);
        }

        [Test]
        public void GetNextAdLogsAttempt()
        {
            LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.GetNextAd));

            _testQueue.GetNextAd();
        }

        [Test]
        public void GetNextAdReturnsNull()
        {
            var result = _testQueue.GetNextAd();

            Assert.IsNull(result);
        }

        [Test]
        public void GetNextAdReturnsSetAd()
        {
            var mockAd = new TestFullscreenAd(new IntPtr(99999));
            _testQueue.NextAd = mockAd;

            try
            {
                var result = _testQueue.GetNextAd();

                Assert.IsNotNull(result);
                Assert.AreSame(mockAd, result);
            }
            finally
            {
                mockAd.Dispose();
                AdCache.ReleaseAd(99999);
            }
        }

        [Test]
        public void HasNextAdLogsCheck()
        {
            LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.HasNextAd));

            _testQueue.HasNextAd();
        }

        [Test]
        public void HasNextAdReturnsFalse()
        {
            var result = _testQueue.HasNextAd();

            Assert.IsFalse(result);
        }

        [Test]
        public void HasNextAdReturnsSetValue()
        {
            _testQueue.HasNextAdValue = true;

            var result = _testQueue.HasNextAd();

            Assert.IsTrue(result);
        }

        [Test]
        public void StartLogsStarting()
        {
            LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Start));

            _testQueue.Start();
        }

        [Test]
        public void StartSetsStartCalledFlag()
        {
            _testQueue.Start();

            Assert.IsTrue(_testQueue.StartCalled);
        }

        [Test]
        public void StartSetsIsRunningToTrue()
        {
            _testQueue.Start();

            Assert.IsTrue(_testQueue.IsRunning);
        }

        [Test]
        public void StopLogsStopping()
        {
            LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Stop));

            _testQueue.Stop();
        }

        [Test]
        public void StopSetsStopCalledFlag()
        {
            _testQueue.Stop();

            Assert.IsTrue(_testQueue.StopCalled);
        }

        [Test]
        public void StopSetsIsRunningToFalse()
        {
            _testQueue.IsRunningValue = true;

            _testQueue.Stop();

            Assert.IsFalse(_testQueue.IsRunning);
        }

        [UnityTest]
        public IEnumerator OnDidUpdateInvokesEvent()
        {
            var eventCalled = false;
            IFullscreenAdQueue receivedQueue = null;
            IAdLoadResult receivedResult = null;
            var receivedNumberOfAdsReady = 0;

            FullscreenAdQueueUpdateEvent handler = (queue, result, numberOfAdsReady) =>
            {
                eventCalled = true;
                receivedQueue = queue;
                receivedResult = result;
                receivedNumberOfAdsReady = numberOfAdsReady;
            };

            try
            {
                _testQueue.DidUpdate += handler;

                var loadResult = new FullscreenAdLoadResult(null,
                    TestConstants.LoadIds.Primary,
                    new Metrics(),
                    new BidInfo()
                );

                _testQueue.TestOnDidUpdate(loadResult, TestConstants.Queue.NumberOfAdsReady);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_testQueue, receivedQueue);
                Assert.IsNotNull(receivedResult);
                Assert.AreEqual(TestConstants.LoadIds.Primary, receivedResult.LoadId);
                Assert.AreEqual(TestConstants.Queue.NumberOfAdsReady, receivedNumberOfAdsReady);
            }
            finally
            {
                _testQueue.DidUpdate -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnDidUpdateWithNullResultInvokesEvent()
        {
            var eventCalled = false;
            IFullscreenAdQueue receivedQueue = null;
            IAdLoadResult receivedResult = null;
            var receivedNumberOfAdsReady = 0;

            FullscreenAdQueueUpdateEvent handler = (queue, result, numberOfAdsReady) =>
            {
                eventCalled = true;
                receivedQueue = queue;
                receivedResult = result;
                receivedNumberOfAdsReady = numberOfAdsReady;
            };

            try
            {
                _testQueue.DidUpdate += handler;

                _testQueue.TestOnDidUpdate(null, TestConstants.Queue.NumberOfAdsReady);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_testQueue, receivedQueue);
                Assert.IsNull(receivedResult);
                Assert.AreEqual(TestConstants.Queue.NumberOfAdsReady, receivedNumberOfAdsReady);
            }
            finally
            {
                _testQueue.DidUpdate -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnDidRemoveExpiredAdInvokesEvent()
        {
            var eventCalled = false;
            IFullscreenAdQueue receivedQueue = null;
            var receivedNumberOfAdsReady = 0;

            FullscreenAdQueueRemoveExpiredAdEvent handler = (queue, numberOfAdsReady) =>
            {
                eventCalled = true;
                receivedQueue = queue;
                receivedNumberOfAdsReady = numberOfAdsReady;
            };

            try
            {
                _testQueue.DidRemoveExpiredAd += handler;

                _testQueue.TestOnDidRemoveExpiredAd(TestConstants.Queue.NumberOfAdsReady);

                yield return null;

                Assert.IsTrue(eventCalled);
                Assert.AreSame(_testQueue, receivedQueue);
                Assert.AreEqual(TestConstants.Queue.NumberOfAdsReady, receivedNumberOfAdsReady);
            }
            finally
            {
                _testQueue.DidRemoveExpiredAd -= handler;
            }
        }

        [UnityTest]
        public IEnumerator OnDidUpdateDoesNotThrowWithNoSubscribers()
        {
            Assert.DoesNotThrow(() => _testQueue.TestOnDidUpdate(null, TestConstants.Queue.NumberOfAdsReady));
            yield return null;
        }

        [UnityTest]
        public IEnumerator OnDidRemoveExpiredAdDoesNotThrowWithNoSubscribers()
        {
            Assert.DoesNotThrow(() => _testQueue.TestOnDidRemoveExpiredAd(TestConstants.Queue.NumberOfAdsReady));
            yield return null;
        }
    }
}

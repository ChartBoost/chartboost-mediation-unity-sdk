using System;
using System.Collections.Generic;
using Chartboost.Logging;
using Chartboost.Mediation.Default.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Fullscreen.Queue
{
    public class FullscreenAdQueueDefaultTests
    {

        private FullscreenAdQueueDefault _queue;
        private IntPtr _testUniqueId;
        private LogLevel _initialLogLevel;

        [SetUp]
        public void SetUp()
        {
            _initialLogLevel = LogController.LoggingLevel;
            LogController.LoggingLevel = LogLevel.Verbose;

            _testUniqueId = new IntPtr(TestConstants.UniqueIds.Secondary);
            _queue = new FullscreenAdQueueDefault(_testUniqueId);
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
            Assert.IsNotNull(_queue);
        }

        [Test]
        public void KeywordsInitiallyNull()
        {
            Assert.IsNull(_queue.Keywords);
        }

        [Test]
        public void KeywordsCanBeSetAndRetrieved()
        {
            var keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            _queue.Keywords = keywords;

            Assert.AreSame(keywords, _queue.Keywords);
        }

        [Test]
        public void QueueCapacityCanBeSetAndRetrieved()
        {
            _queue.QueueCapacity = TestConstants.Queue.Capacity;

            Assert.AreEqual(TestConstants.Queue.Capacity, _queue.QueueCapacity);
        }

        [Test]
        public void NumberOfAdsReadyReturnsZero()
        {
            Assert.AreEqual(0, _queue.NumberOfAdsReady);
        }

        [Test]
        public void IsRunningReturnsFalse()
        {
            Assert.IsFalse(_queue.IsRunning);
        }

        [Test]
        public void GetNextAdReturnsNull()
        {
            var result = _queue.GetNextAd();

            Assert.IsNull(result);
        }

        [Test]
        public void HasNextAdReturnsFalse()
        {
            var result = _queue.HasNextAd();

            Assert.IsFalse(result);
        }

        [Test]
        public void StartLogsStarting()
        {
            LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Start));

            _queue.Start();
        }

        [Test]
        public void StopLogsStopping()
        {
            LogAssert.Expect(UnityEngine.LogType.Log, new System.Text.RegularExpressions.Regex(TestConstants.LogPatterns.Stop));

            _queue.Stop();
        }

        [Test]
        public void IsRunningRemainsFalseAfterStart()
        {
            _queue.Start();

            Assert.IsFalse(_queue.IsRunning);
        }

        [Test]
        public void NumberOfAdsReadyRemainsZeroAfterStart()
        {
            _queue.Start();

            Assert.AreEqual(0, _queue.NumberOfAdsReady);
        }
    }
}

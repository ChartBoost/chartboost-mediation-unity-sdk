using System.Collections;
using System.Collections.Generic;
using Chartboost.Mediation;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Requests;
using Chartboost.Testing;
using Chartboost.Tests.Runtime.Utilities;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Fullscreen.Queue
{
    /// <summary>
    /// The native <see cref="IFullscreenAdQueue"/> on Android and iOS: configure, start, take an ad, stop.
    /// Runs in order against one queue for the interstitial placement.
    /// </summary>
    [TestFixture]
    public class FullscreenAdQueueNativeTests : NativeTestFixture
    {
        // One ad: once full, a running queue has nothing left to load.
        private const int Capacity = 1;
        // Never cleared: the ad cache holds queues weakly and a load still in flight reports after the test.
        private static IFullscreenAdQueue _queue;
        private static bool _filled;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ChartboostMediation.TestMode = true;
            _filled = false;
            TestInitializer.Initialize();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            ChartboostMediation.TestMode = false;
            _queue?.Stop();
        }

        [UnityTest, Order(1)]
        public IEnumerator GetFullscreenAdQueue_ReturnsOneStoppedQueuePerPlacement()
        {
            yield return TestInitializer.WaitForInitialization();

            _queue = ChartboostMediation.GetFullscreenAdQueue(TestConstants.Placements.Interstitial);

            Assert.IsNotNull(_queue);
            Assert.AreSame(_queue, ChartboostMediation.GetFullscreenAdQueue(TestConstants.Placements.Interstitial), "the same placement returned a different queue");
            Assert.IsFalse(_queue.IsRunning, "a new queue is already running");
        }

        [Test, Order(2)]
        public void Settings_AreKeptByTheNativeQueue()
        {
            RequireQueue();

            _queue.QueueCapacity = Capacity;
            _queue.Keywords = new Dictionary<string, string> { { TestConstants.Keywords.Key, TestConstants.Keywords.Value } };

            Assert.AreEqual(Capacity, _queue.QueueCapacity);
            Assert.That(_queue.Keywords, Contains.Key(TestConstants.Keywords.Key));
        }

        [UnityTest, Order(3)]
        public IEnumerator Start_LoadsAnAdAndReportsIt()
        {
            RequireQueue();
            IAdLoadResult update = null;
            var readyAfterUpdate = -1;
            void OnUpdate(IFullscreenAdQueue queue, IAdLoadResult result, int ready)
            {
                update = result;
                readyAfterUpdate = ready;
            }

            _queue.DidUpdate += OnUpdate;
            try
            {
                _queue.Start();
                Assert.IsTrue(_queue.IsRunning, "the queue didn't report running after Start");

                yield return TestWait.Until(() => update != null, TestConstants.Timeouts.NativeAdLoad);

                Assert.IsNotNull(update, "the queue never reported a load");
                AdLoadResultAssert.AssertSuccess(update, "queue load failed");
                Assert.GreaterOrEqual(readyAfterUpdate, 1, "a successful load reported no ready ads");
                _filled = true;
                Assert.IsTrue(_queue.HasNextAd(), "HasNextAd is false with an ad ready");
            }
            finally
            {
                _queue.DidUpdate -= OnUpdate;
            }
        }

        [Test, Order(4)]
        public void Stop_StopsTheQueue()
        {
            RequireQueue();

            _queue.Stop();

            Assert.IsFalse(_queue.IsRunning, "the queue still reports running after Stop");
        }

        // After Stop, so taking the ad doesn't start a refill load that would report after the fixture.
        [Test, Order(5)]
        public void GetNextAd_HandsOutTheLoadedAd()
        {
            RequireQueue();
            AdLoadResultAssert.RequireLoaded(_filled);

            var ad = _queue.GetNextAd();
            try
            {
                Assert.IsNotNull(ad, "GetNextAd returned null with an ad ready");
            }
            finally
            {
                ad?.Dispose();
            }
        }

        private static void RequireQueue() => Assert.IsNotNull(_queue, "the queue from Order(1) is missing");
    }
}

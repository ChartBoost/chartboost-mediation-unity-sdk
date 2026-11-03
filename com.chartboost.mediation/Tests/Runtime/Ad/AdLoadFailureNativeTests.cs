using System.Collections;
using System.Collections.Generic;
using Chartboost.Mediation;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Fullscreen.Queue;
using Chartboost.Mediation.Requests;
using Chartboost.Testing;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad
{
    /// <summary>
    /// A load for a placement that doesn't exist must come back as an error, never an exception or a hang,
    /// whatever the live demand. One test per ad format.
    /// </summary>
    [TestFixture]
    public class AdLoadFailureNativeTests : NativeTestFixture
    {
        private const string UnknownPlacement = "unity_sdk_tests_unknown_placement";
        // Kept for the whole run: the ad cache holds queues weakly and its retries can report after the test.
        private static IFullscreenAdQueue _queue;

        [OneTimeSetUp]
        public void OneTimeSetUp() => TestInitializer.Initialize();

        [UnityTest]
        public IEnumerator Banner_UnknownPlacement_ReturnsAnError()
        {
            yield return TestInitializer.WaitForInitialization();
            var bannerAd = ChartboostMediation.GetBannerAd();
            try
            {
                var loadTask = bannerAd.Load(new BannerAdLoadRequest(UnknownPlacement, BannerSize.Standard));
                yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.NativeAdLoad);

                Assert.IsTrue(loadTask.Result.Error.HasValue, "an unknown placement loaded a banner");
            }
            finally
            {
                bannerAd.Dispose();
            }
        }

        [UnityTest]
        public IEnumerator Fullscreen_UnknownPlacement_ReturnsAnError()
        {
            yield return TestInitializer.WaitForInitialization();

            var loadTask = ChartboostMediation.LoadFullscreenAd(new FullscreenAdLoadRequest(UnknownPlacement, new Dictionary<string, string>()));
            yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.NativeAdLoad);

            Assert.IsTrue(loadTask.Result.Error.HasValue, "an unknown placement loaded a fullscreen ad");
            Assert.IsNull(loadTask.Result.Ad, "a failed load still returned an ad");
        }

        [UnityTest]
        public IEnumerator Queue_UnknownPlacement_ReportsAnErrorAndNoAds()
        {
            yield return TestInitializer.WaitForInitialization();
            _queue = ChartboostMediation.GetFullscreenAdQueue(UnknownPlacement);
            IAdLoadResult update = null;
            void OnUpdate(IFullscreenAdQueue adQueue, IAdLoadResult result, int ready) => update ??= result;

            _queue.DidUpdate += OnUpdate;
            try
            {
                _queue.Start();
                yield return TestWait.Until(() => update != null, TestConstants.Timeouts.NativeAdLoad);

                Assert.IsNotNull(update, "the queue never reported its load");
                Assert.IsTrue(update.Error.HasValue, "an unknown placement filled the queue");
                Assert.IsFalse(_queue.HasNextAd(), "a queue with only failed loads has an ad");
            }
            finally
            {
                _queue.DidUpdate -= OnUpdate;
                _queue.Stop();
            }
        }
    }
}

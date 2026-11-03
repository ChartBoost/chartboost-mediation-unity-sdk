using System.Collections;
using Chartboost.Mediation;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using Chartboost.Testing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Ad.Banner
{
    /// <summary>
    /// A NaN banner size must fail the load instead of crashing the native Android wrapper.
    /// </summary>
    [UnityPlatform(RuntimePlatform.Android)]
    public class BannerAdInvalidSizeNativeTests : NativeTestFixture
    {
        [UnityTest]
        public IEnumerator LoadWithNaNSizeReturnsErrorInsteadOfCrashing()
        {
            var bannerAd = ChartboostMediation.GetBannerAd();
            try
            {
                var request = new BannerAdLoadRequest(TestConstants.Placements.Banner, BannerSize.Adaptive(float.NaN, float.NaN));
                var loadTask = bannerAd.Load(request);
                yield return TestWait.Until(() => loadTask.IsCompleted, "the load", TestConstants.Timeouts.EditorTask);

                Assert.IsFalse(loadTask.IsFaulted, loadTask.Exception?.ToString());
                Assert.AreEqual(Errors.InvalidBannerSize, loadTask.Result.Error?.Message);
            }
            finally
            {
                bannerAd.Dispose();
            }
        }
    }
}

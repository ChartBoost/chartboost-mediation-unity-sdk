using System;
using System.Text.RegularExpressions;
using Chartboost.Caching;
using Chartboost.Logging;
using Chartboost.Mediation;
using Chartboost.Mediation.Ad;
using NUnit.Framework;
using UnityEngine.TestTools;
// ReSharper disable UnusedMember.Local

namespace Chartboost.Tests.Runtime.Utilities
{
    // The generic mechanics of AdCache<T> (track/get/release/overwrite/isolation/CacheInfo) are owned and
    // tested by com.chartboost.unity.utilities. These tests cover only Mediation's integration with the
    // closed generic AdCache<IAd> and its logging via ChartboostMediation.
    public class AdCacheTests
    {
        private TestAd _testAd;

        // Release only this fixture's id: clearing the shared cache drops live native ads and queues,
        // whose late callbacks then fail whichever test is running.
        [SetUp]
        public void SetUp()
        {
            AdCache<IAd>.ReleaseAd(TestConstants.UniqueIds.Primary);
            _testAd = new TestAd();
        }

        [TearDown]
        public void TearDown() => AdCache<IAd>.ReleaseAd(TestConstants.UniqueIds.Primary);

        [Test]
        public void TrackAndGetAdWithIAd()
        {
            AdCache<IAd>.TrackAd(TestConstants.UniqueIds.Primary, _testAd);

            var retrievedAd = AdCache<IAd>.GetAd(TestConstants.UniqueIds.Primary);

            Assert.IsNotNull(retrievedAd);
            Assert.AreEqual(_testAd, retrievedAd);
        }

        [Test]
        public void GetAdWithNonExistentIdReturnsNullAndWarns()
        {
            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedWeakReference));

            var retrievedAd = AdCache<IAd>.GetAd(TestConstants.UniqueIds.Primary);

            Assert.IsNull(retrievedAd);
        }

        [Test]
        public void TrackAdLogsVerboseMessage()
        {
            ChartboostMediation.LogLevel = LogLevel.Verbose;
            LogAssert.Expect(UnityEngine.LogType.Log, new Regex(TestConstants.LogPatterns.Tracking));

            AdCache<IAd>.TrackAd(TestConstants.UniqueIds.Primary, _testAd);
        }

        // Simple test implementation of IAd for testing purposes
        private class TestAd : IAd
        {
            public string PlacementName => TestConstants.Placements.Standard;
            public string Request => string.Empty;
            public IntPtr UniqueId => IntPtr.Zero;
            public string WinningBidInfo => string.Empty;
            public string CustomData { get; set; }
        }
    }
}

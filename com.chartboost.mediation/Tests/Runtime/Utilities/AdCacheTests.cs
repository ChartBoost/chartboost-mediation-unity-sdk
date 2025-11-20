using System;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using Chartboost.Logging;
using Chartboost.Mediation;
using Chartboost.Mediation.Ad;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;
using UnityEngine.TestTools;
// ReSharper disable UnusedMember.Local

namespace Chartboost.Tests.Runtime.Utilities
{
    public class AdCacheTests
    {

        private TestAd _testAd;
        private TestAd _testAd2;
        private FullscreenAdLoadRequest _testRequest;
        private FullscreenAdLoadRequest _testRequest2;

        [SetUp]
        public void SetUp()
        {
            // Clear the static dictionaries before each test
            ClearAdCache();
            ClearAdLoadRequestCache();

            _testAd = new TestAd();
            _testAd2 = new TestAd();
            _testRequest = new FullscreenAdLoadRequest(TestConstants.Placements.Standard);
            _testRequest2 = new FullscreenAdLoadRequest(TestConstants.Placements.Secondary);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            ClearAdCache();
            ClearAdLoadRequestCache();
        }

        [Test]
        public void TrackAdWithLongIdAddsAdToCache()
        {
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testAd);

            var retrievedAd = AdCache.GetAd(TestConstants.UniqueIds.Primary);

            Assert.IsNotNull(retrievedAd);
            Assert.AreEqual(_testAd, retrievedAd);
        }

        [Test]
        public void TrackAdWithIntPtrIdAddsAdToCache()
        {
            var intPtrId = new IntPtr(TestConstants.UniqueIds.Primary);

            AdCache.TrackAd(intPtrId, _testAd);

            var retrievedAd = AdCache.GetAd(intPtrId);

            Assert.IsNotNull(retrievedAd);
            Assert.AreEqual(_testAd, retrievedAd);
        }

        [Test]
        public void GetAdWithNonExistentIdReturnsNull()
        {
            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedWeakReference));

            var retrievedAd = AdCache.GetAd(TestConstants.UniqueIds.Primary);

            Assert.IsNull(retrievedAd);
        }

        [Test]
        public void GetAdWithIntPtrNonExistentIdReturnsNull()
        {
            var intPtrId = new IntPtr(TestConstants.UniqueIds.Primary);
            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedWeakReference));

            var retrievedAd = AdCache.GetAd(intPtrId);

            Assert.IsNull(retrievedAd);
        }

        [Test]
        public void ReleaseAdWithLongIdRemovesAdFromCache()
        {
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testAd);
            AdCache.ReleaseAd(TestConstants.UniqueIds.Primary);

            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedWeakReference));
            var retrievedAd = AdCache.GetAd(TestConstants.UniqueIds.Primary);

            Assert.IsNull(retrievedAd);
        }

        [Test]
        public void ReleaseAdWithIntPtrIdRemovesAdFromCache()
        {
            var intPtrId = new IntPtr(TestConstants.UniqueIds.Primary);

            AdCache.TrackAd(intPtrId, _testAd);
            AdCache.ReleaseAd(intPtrId);

            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedWeakReference));
            var retrievedAd = AdCache.GetAd(intPtrId);

            Assert.IsNull(retrievedAd);
        }

        [Test]
        public void ReleaseAdWithNonExistentIdDoesNotThrow()
        {
            Assert.DoesNotThrow(() => AdCache.ReleaseAd(TestConstants.UniqueIds.Primary));
        }

        [Test]
        public void TrackAdLoadRequestAddsRequestToCache()
        {
            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);

            var retrievedRequest = AdCache.GetAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.IsNotNull(retrievedRequest);
            Assert.AreEqual(_testRequest, retrievedRequest);
        }

        [Test]
        public void TrackAdLoadRequestSetsAssociatedProxy()
        {
            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);

            Assert.AreEqual(TestConstants.UniqueIds.Primary, _testRequest.AssociatedProxy);
        }

        [Test]
        public void GetAdLoadRequestRemovesRequestFromCache()
        {
            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
            var retrievedRequest = AdCache.GetAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.IsNotNull(retrievedRequest);

            // The second call should return null and log warning
            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedWeakReference));
            var secondRetrievedRequest = AdCache.GetAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.IsNull(secondRetrievedRequest);
        }

        [Test]
        public void GetAdLoadRequestWithNonExistentIdReturnsNull()
        {
            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedWeakReference));

            var retrievedRequest = AdCache.GetAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.IsNull(retrievedRequest);
        }

        [Test]
        public void ReleaseAdLoadRequestRemovesRequestFromCache()
        {
            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
            AdCache.ReleaseAdLoadRequest(TestConstants.UniqueIds.Primary);

            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedWeakReference));
            var retrievedRequest = AdCache.GetAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.IsNull(retrievedRequest);
        }

        [Test]
        public void ReleaseAdLoadRequestWithNonExistentIdDoesNotThrow()
        {
            Assert.DoesNotThrow(() => AdCache.ReleaseAdLoadRequest(TestConstants.UniqueIds.Primary));
        }

        [Test]
        public void CacheInfoReturnsCorrectFormat()
        {
            var cacheInfo = AdCache.CacheInfo();

            Assert.IsNotNull(cacheInfo);
            Assert.IsTrue(cacheInfo.Contains(TestConstants.AdCache.ExpectedCacheInfoPrefix));
            Assert.IsTrue(cacheInfo.Contains(TestConstants.AdCache.ExpectedFullscreenCacheText));
            Assert.IsTrue(cacheInfo.Contains(TestConstants.AdCache.ExpectedAdLoadRequestText));
        }

        [Test]
        public void CacheInfoReturnsCorrectCountsWhenEmpty()
        {
            var cacheInfo = AdCache.CacheInfo();

            Assert.IsTrue(cacheInfo.Contains(TestConstants.AdCache.ExpectedFullscreenCacheText + TestConstants.Counts.ZeroString));
            Assert.IsTrue(cacheInfo.Contains(TestConstants.AdCache.ExpectedAdLoadRequestText + TestConstants.Counts.ZeroString));
        }

        [Test]
        public void CacheInfoReturnsCorrectCountsWithAds()
        {
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testAd);
            AdCache.TrackAd(TestConstants.UniqueIds.Secondary, _testAd2);

            var cacheInfo = AdCache.CacheInfo();

            Assert.IsTrue(cacheInfo.Contains(TestConstants.AdCache.ExpectedFullscreenCacheText + TestConstants.Counts.TwoString));
        }

        [Test]
        public void CacheInfoReturnsCorrectCountsWithRequests()
        {
            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Secondary, _testRequest2);

            var cacheInfo = AdCache.CacheInfo();

            Assert.IsTrue(cacheInfo.Contains(TestConstants.AdCache.ExpectedAdLoadRequestText + TestConstants.Counts.TwoString));
        }

        [Test]
        public void TrackMultipleAdsWithDifferentIds()
        {
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testAd);
            AdCache.TrackAd(TestConstants.UniqueIds.Secondary, _testAd2);

            var retrievedAd1 = AdCache.GetAd(TestConstants.UniqueIds.Primary);
            var retrievedAd2 = AdCache.GetAd(TestConstants.UniqueIds.Secondary);

            Assert.AreEqual(_testAd, retrievedAd1);
            Assert.AreEqual(_testAd2, retrievedAd2);
        }

        [Test]
        public void TrackAdWithSameIdOverwritesPreviousAd()
        {
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testAd);
            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testAd2);

            var retrievedAd = AdCache.GetAd(TestConstants.UniqueIds.Primary);

            Assert.AreEqual(_testAd2, retrievedAd);
        }

        [Test]
        public void TrackMultipleAdLoadRequestsWithDifferentIds()
        {
            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Secondary, _testRequest2);

            var retrievedRequest1 = AdCache.GetAdLoadRequest(TestConstants.UniqueIds.Primary);
            var retrievedRequest2 = AdCache.GetAdLoadRequest(TestConstants.UniqueIds.Secondary);

            Assert.AreEqual(_testRequest, retrievedRequest1);
            Assert.AreEqual(_testRequest2, retrievedRequest2);
        }

        [Test]
        public void TrackAdLoadRequestWithSameIdOverwritesPreviousRequest()
        {
            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest2);

            var retrievedRequest = AdCache.GetAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.AreEqual(_testRequest2, retrievedRequest);
        }

        [Test]
        public void TrackAdLogsVerboseMessage()
        {
            ChartboostMediation.LogLevel = LogLevel.Verbose;
            LogAssert.Expect(UnityEngine.LogType.Log, new Regex(TestConstants.LogPatterns.Tracking));

            AdCache.TrackAd(TestConstants.UniqueIds.Primary, _testAd);
        }

        [Test]
        public void TrackAdLoadRequestLogsVerboseMessage()
        {
            ChartboostMediation.LogLevel = LogLevel.Verbose;
            LogAssert.Expect(UnityEngine.LogType.Log, new Regex(TestConstants.LogPatterns.TrackingAdLoadRequest));

            AdCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
        }

        private void ClearAdCache()
        {
            var adCacheType = typeof(AdCache);
            var adsField = adCacheType.GetField(TestConstants.AdCache.AdsFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (adsField?.GetValue(null) is IDictionary ads)
                ads.Clear();
        }

        private void ClearAdLoadRequestCache()
        {
            var adCacheType = typeof(AdCache);
            var adLoadRequestsField = adCacheType.GetField(TestConstants.AdCache.AdLoadRequestsFieldName, BindingFlags.NonPublic | BindingFlags.Static);
            if (adLoadRequestsField?.GetValue(null) is IDictionary adLoadRequests)
                adLoadRequests.Clear();
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

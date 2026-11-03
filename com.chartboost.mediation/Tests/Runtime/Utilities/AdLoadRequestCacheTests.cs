using System.Text.RegularExpressions;
using Chartboost.Logging;
using Chartboost.Mediation;
using Chartboost.Mediation.Requests;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;
using UnityEngine.TestTools;
// ReSharper disable UnusedMember.Local

namespace Chartboost.Tests.Runtime.Utilities
{
    public class AdLoadRequestCacheTests
    {
        private FullscreenAdLoadRequest _testRequest;
        private FullscreenAdLoadRequest _testRequest2;

        [SetUp]
        public void SetUp()
        {
            AdLoadRequestCache.ClearForTesting();

            _testRequest = new FullscreenAdLoadRequest(TestConstants.Placements.Standard);
            _testRequest2 = new FullscreenAdLoadRequest(TestConstants.Placements.Secondary);
        }

        [TearDown]
        public void TearDown()
        {
            AdLoadRequestCache.ClearForTesting();
        }

        [Test]
        public void TrackAdLoadRequestAddsRequestToCache()
        {
            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);

            var retrievedRequest = AdLoadRequestCache.TakeAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.IsNotNull(retrievedRequest);
            Assert.AreEqual(_testRequest, retrievedRequest);
        }

        [Test]
        public void TrackAdLoadRequestSetsAssociatedProxy()
        {
            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);

            Assert.AreEqual(TestConstants.UniqueIds.Primary, _testRequest.AssociatedProxy);
        }

        [Test]
        public void TakeAdLoadRequestRemovesRequestFromCache()
        {
            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
            var retrievedRequest = AdLoadRequestCache.TakeAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.IsNotNull(retrievedRequest);

            // The second call should return null and log warning
            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedAdLoadRequest));
            var secondRetrievedRequest = AdLoadRequestCache.TakeAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.IsNull(secondRetrievedRequest);
        }

        [Test]
        public void TakeAdLoadRequestWithNonExistentIdReturnsNull()
        {
            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedAdLoadRequest));

            var retrievedRequest = AdLoadRequestCache.TakeAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.IsNull(retrievedRequest);
        }

        [Test]
        public void ReleaseAdLoadRequestRemovesRequestFromCache()
        {
            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
            AdLoadRequestCache.ReleaseAdLoadRequest(TestConstants.UniqueIds.Primary);

            LogAssert.Expect(UnityEngine.LogType.Warning, new Regex(TestConstants.LogPatterns.FailedAdLoadRequest));
            var retrievedRequest = AdLoadRequestCache.TakeAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.IsNull(retrievedRequest);
        }

        [Test]
        public void ReleaseAdLoadRequestWithNonExistentIdDoesNotThrow()
        {
            Assert.DoesNotThrow(() => AdLoadRequestCache.ReleaseAdLoadRequest(TestConstants.UniqueIds.Primary));
        }

        [Test]
        public void CacheInfoReturnsCorrectFormat()
        {
            var cacheInfo = AdLoadRequestCache.CacheInfo();

            Assert.IsNotNull(cacheInfo);
            Assert.IsTrue(cacheInfo.Contains(TestConstants.AdLoadRequestCache.CacheInfoLabel));
            Assert.IsTrue(cacheInfo.Contains(TestConstants.AdLoadRequestCache.CacheInfoSuffix));
        }

        [Test]
        public void CountIsZeroWhenEmpty()
        {
            Assert.AreEqual(0, AdLoadRequestCache.Count);
        }

        [Test]
        public void CountReflectsTrackedRequests()
        {
            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Secondary, _testRequest2);

            Assert.AreEqual(2, AdLoadRequestCache.Count);
        }

        [Test]
        public void TrackMultipleAdLoadRequestsWithDifferentIds()
        {
            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Secondary, _testRequest2);

            var retrievedRequest1 = AdLoadRequestCache.TakeAdLoadRequest(TestConstants.UniqueIds.Primary);
            var retrievedRequest2 = AdLoadRequestCache.TakeAdLoadRequest(TestConstants.UniqueIds.Secondary);

            Assert.AreEqual(_testRequest, retrievedRequest1);
            Assert.AreEqual(_testRequest2, retrievedRequest2);
        }

        [Test]
        public void TrackAdLoadRequestWithSameIdOverwritesPreviousRequest()
        {
            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest2);

            var retrievedRequest = AdLoadRequestCache.TakeAdLoadRequest(TestConstants.UniqueIds.Primary);

            Assert.AreEqual(_testRequest2, retrievedRequest);
        }

        [Test]
        public void TrackAdLoadRequestLogsVerboseMessage()
        {
            ChartboostMediation.LogLevel = LogLevel.Verbose;
            LogAssert.Expect(UnityEngine.LogType.Log, new Regex(TestConstants.LogPatterns.TrackingAdLoadRequest));

            AdLoadRequestCache.TrackAdLoadRequest(TestConstants.UniqueIds.Primary, _testRequest);
        }
    }
}

using System.Collections.Generic;
using Chartboost.Mediation.Data;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Data
{
    public class MetricsTests
    {

        [Test]
        public void MetricsStructInitializesCorrectly()
        {
            var metrics = new Metrics
            {
                auctionId = TestConstants.Auction.AuctionId,
                result = TestConstants.Metrics.TestResult,
                metrics = new List<MetricsData>(),
                error = null
            };

            Assert.AreEqual(TestConstants.Auction.AuctionId, metrics.auctionId);
            Assert.AreEqual(TestConstants.Metrics.TestResult, metrics.result);
            Assert.IsNotNull(metrics.metrics);
            Assert.IsNull(metrics.error);
        }

        [Test]
        public void MetricsDataStructInitializesCorrectly()
        {
            var metricsData = new MetricsData
            {
                loadId = TestConstants.LoadIds.Secondary,
                networkType = TestConstants.Metrics.TestNetworkType,
                lineItemId = TestConstants.Auction.LineItemId,
                partnerPlacement = TestConstants.Metrics.TestPartnerPlacement,
                partner = TestConstants.Partners.Standard,
                start = TestConstants.Metrics.TestStart,
                end = TestConstants.Metrics.TestEnd,
                duration = TestConstants.Metrics.TestDuration,
                isSuccess = true,
                partnerSdkVersion = TestConstants.Versions.PartnerSDK,
                partnerAdapterVersion = TestConstants.Versions.PartnerAdapter
            };

            Assert.AreEqual(TestConstants.LoadIds.Secondary, metricsData.loadId);
            Assert.AreEqual(TestConstants.Metrics.TestNetworkType, metricsData.networkType);
            Assert.AreEqual(TestConstants.Auction.LineItemId, metricsData.lineItemId);
            Assert.AreEqual(TestConstants.Metrics.TestPartnerPlacement, metricsData.partnerPlacement);
            Assert.AreEqual(TestConstants.Partners.Standard, metricsData.partner);
            Assert.AreEqual(TestConstants.Metrics.TestStart, metricsData.start);
            Assert.AreEqual(TestConstants.Metrics.TestEnd, metricsData.end);
            Assert.AreEqual(TestConstants.Metrics.TestDuration, metricsData.duration);
            Assert.IsTrue(metricsData.isSuccess);
            Assert.AreEqual(TestConstants.Versions.PartnerSDK, metricsData.partnerSdkVersion);
            Assert.AreEqual(TestConstants.Versions.PartnerAdapter, metricsData.partnerAdapterVersion);
        }

        [Test]
        public void MetricsDataWithFailureInitializesCorrectly()
        {
            var metricsData = new MetricsData
            {
                partner = TestConstants.Partners.Standard,
                isSuccess = false
            };

            Assert.IsFalse(metricsData.isSuccess);
        }

        [Test]
        public void MetricsErrorStructInitializesCorrectly()
        {
            var errorDetails = new ErrorDetails
            {
                type = TestConstants.Metrics.TestErrorType,
                description = TestConstants.Metrics.TestErrorDescription,
                data = TestConstants.Metrics.TestErrorData
            };

            var metricsError = new MetricsError
            {
                code = TestConstants.Errors.CodeChartboost,
                details = errorDetails
            };

            Assert.AreEqual(TestConstants.Errors.CodeChartboost, metricsError.code);
            Assert.AreEqual(TestConstants.Metrics.TestErrorType, metricsError.details.type);
            Assert.AreEqual(TestConstants.Metrics.TestErrorDescription, metricsError.details.description);
            Assert.AreEqual(TestConstants.Metrics.TestErrorData, metricsError.details.data);
        }

        [Test]
        public void ErrorDetailsStructInitializesCorrectly()
        {
            var errorDetails = new ErrorDetails
            {
                type = TestConstants.Metrics.TestErrorType,
                description = TestConstants.Metrics.TestErrorDescription,
                data = null
            };

            Assert.AreEqual(TestConstants.Metrics.TestErrorType, errorDetails.type);
            Assert.AreEqual(TestConstants.Metrics.TestErrorDescription, errorDetails.description);
            Assert.IsNull(errorDetails.data);
        }

        [Test]
        public void MetricsWithMultipleMetricsDataInitializesCorrectly()
        {
            var metricsData1 = new MetricsData { partner = TestConstants.Partners.Standard, isSuccess = true };
            var metricsData2 = new MetricsData { partner = "another_partner", isSuccess = false };

            var metrics = new Metrics
            {
                auctionId = TestConstants.Auction.AuctionId,
                metrics = new List<MetricsData> { metricsData1, metricsData2 }
            };

            Assert.AreEqual(2, metrics.metrics.Count);
            Assert.AreEqual(TestConstants.Partners.Standard, metrics.metrics[0].partner);
            Assert.IsTrue(metrics.metrics[0].isSuccess);
            Assert.IsFalse(metrics.metrics[1].isSuccess);
        }

        [Test]
        public void MetricsWithErrorInitializesCorrectly()
        {
            var errorDetails = new ErrorDetails
            {
                type = TestConstants.Metrics.TestErrorType,
                description = TestConstants.Metrics.TestErrorDescription
            };

            var metricsError = new MetricsError
            {
                code = TestConstants.Errors.CodeChartboost,
                details = errorDetails
            };

            var metrics = new Metrics
            {
                auctionId = TestConstants.Auction.AuctionId,
                result = "failure",
                error = metricsError
            };

            Assert.IsNotNull(metrics.error);
            Assert.AreEqual(TestConstants.Errors.CodeChartboost, metrics.error.Value.code);
            Assert.AreEqual(TestConstants.Metrics.TestErrorType, metrics.error.Value.details.type);
        }

        [Test]
        public void MetricsDeserializesFromJsonCorrectly()
        {
            var json = @"{
                ""auction_id"": ""auction_123"",
                ""result"": ""success"",
                ""metrics"": [
                    {
                        ""load_id"": ""load_id_456"",
                        ""partner"": ""test_partner"",
                        ""is_success"": true
                    }
                ]
            }";

            var metrics = JsonConvert.DeserializeObject<Metrics>(json);

            Assert.AreEqual(TestConstants.Auction.AuctionId, metrics.auctionId);
            Assert.AreEqual(TestConstants.Metrics.TestResult, metrics.result);
            Assert.AreEqual(1, metrics.metrics.Count);
            Assert.AreEqual(TestConstants.LoadIds.Secondary, metrics.metrics[0].loadId);
            Assert.AreEqual(TestConstants.Partners.Standard, metrics.metrics[0].partner);
            Assert.IsTrue(metrics.metrics[0].isSuccess);
        }

        [Test]
        public void MetricsDataWithNullableFieldsInitializesCorrectly()
        {
            var metricsData = new MetricsData
            {
                partner = TestConstants.Partners.Standard,
                start = null,
                end = null,
                duration = null
            };

            Assert.IsNull(metricsData.start);
            Assert.IsNull(metricsData.end);
            Assert.IsNull(metricsData.duration);
        }
    }
}

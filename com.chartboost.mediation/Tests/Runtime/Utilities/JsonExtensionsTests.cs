using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Ad.Banner.Enums;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chartboost.Tests.Runtime.Utilities
{
    public class JsonExtensionsTests
    {
        private const string TestAdapterName = "TestAdapter";
        private const string TestListItem1 = "item1";
        private const string TestListItem2 = "item2";
        private const string TestListItem3 = "item3";
        private const string EmptyJson = "";
        private const string NullJson = null;
        private const string WhitespaceJson = "   ";
        private const string StandardSizeJson = "\"size\":{\"sizeType\":0,\"width\":320,\"height\":50}";

        private static IEnumerable<TestCaseData> NullOrEmptyJsonCases()
        {
            yield return Case(nameof(JsonExtensions.ToChartboostMediationError), json => json.ToChartboostMediationError(), EmptyJson, null);
            yield return Case(nameof(JsonExtensions.ToChartboostMediationError), json => json.ToChartboostMediationError(), NullJson, null);
            yield return Case(nameof(JsonExtensions.ToBidInfo), json => json.ToBidInfo(), EmptyJson, null);
            yield return Case(nameof(JsonExtensions.ToMetrics), json => json.ToMetrics(), EmptyJson, null);
            yield return Case(nameof(JsonExtensions.ToMetrics), json => json.ToMetrics(), NullJson, null);
            yield return Case(nameof(JsonExtensions.ToFullscreenAdLoadRequest), json => json.ToFullscreenAdLoadRequest(), EmptyJson, null);
            yield return Case(nameof(JsonExtensions.ToAdaptersInfo), json => json.ToAdaptersInfo(), EmptyJson, null);
            yield return Case(nameof(JsonExtensions.ToBannerAdLoadRequest), json => json.ToBannerAdLoadRequest(), EmptyJson, null);
            yield return Case(nameof(JsonExtensions.ToBannerAdLoadRequest), json => json.ToBannerAdLoadRequest(), NullJson, null);
            yield return Case(nameof(JsonExtensions.ToDictionary), json => json.ToDictionary(), EmptyJson, null);
            yield return Case(nameof(JsonExtensions.ToList), json => json.ToList(), EmptyJson, null);
            yield return Case(nameof(JsonExtensions.ToBannerSize), json => json.ToBannerSize(), EmptyJson, null);
            yield return Case(nameof(JsonExtensions.ToBannerSize), json => json.ToBannerSize(), NullJson, null);
            yield return Case(nameof(JsonExtensions.ToVector2), json => json.ToVector2(), EmptyJson, Vector2.zero);
            yield return Case(nameof(JsonExtensions.ToVector2), json => json.ToVector2(), NullJson, Vector2.zero);
            yield return Case(nameof(JsonExtensions.ToVector2), json => json.ToVector2(), WhitespaceJson, Vector2.zero);

            static TestCaseData Case(string method, Func<string, object> deserialize, string json, object expected)
                => new TestCaseData(deserialize, json, expected).SetName($"{method}ReturnsDefaultFor{(json == null ? "Null" : json.Length == 0 ? "Empty" : "Whitespace")}Json");
        }

        [TestCaseSource(nameof(NullOrEmptyJsonCases))]
        public void ReturnsDefaultAndWarnsForNullOrEmptyJson(Func<string, object> deserialize, string json, object expected)
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            Assert.AreEqual(expected, deserialize(json));
        }

        [Test]
        public void ToChartboostMediationErrorDeserializesValidJson()
        {
            var json = $"{{\"code\":\"{TestConstants.Errors.CodeChartboost}\",\"message\":\"{TestConstants.Errors.Message}\"}}";

            var error = json.ToChartboostMediationError();

            Assert.IsNotNull(error);
            Assert.AreEqual(TestConstants.Errors.CodeChartboost, error.Value.Code);
            Assert.AreEqual(TestConstants.Errors.Message, error.Value.Message);
        }

        [Test]
        public void ToBidInfoDeserializesValidJson()
        {
            const string testAuctionId = "auction123";
            const string testPartnerId = "partner";
            const double testPrice = 1.5;
            const string json = "{\"auction_id\":\"" + testAuctionId + "\",\"partner_id\":\"" + testPartnerId + "\",\"price\":1.5,\"line_item_id\":\"item\",\"line_item_name\":\"name\"}";

            var bidInfo = json.ToBidInfo();

            if (!bidInfo.HasValue)
                Assert.Fail("BidInfo was null");

            Assert.AreEqual(testAuctionId, bidInfo.Value.AuctionId);
            Assert.AreEqual(testPartnerId, bidInfo.Value.PartnerId);
            Assert.AreEqual(testPrice, bidInfo.Value.Price);
        }

        [Test]
        public void ToMetricsDeserializesValidJson()
        {
            const string testAuctionId = "auction_123";
            const string json = "{\"auction_id\":\"" + testAuctionId + "\",\"result\":\"success\",\"metrics\":[]}";

            var metrics = json.ToMetrics();

            Assert.IsNotNull(metrics);
            Assert.AreEqual(testAuctionId, metrics.Value.auctionId);
        }

        [Test]
        public void ToFullscreenAdLoadRequestDeserializesValidJson()
        {
            var json = $"{{\"placement\":\"{TestConstants.Placements.Standard}\"}}";

            var request = json.ToFullscreenAdLoadRequest();

            Assert.IsNotNull(request);
            Assert.AreEqual(TestConstants.Placements.Standard, request.PlacementName);
        }

        [Test]
        public void ToAdaptersInfoDeserializesValidJson()
        {
            var json = $"[{{\"adapterVersion\":\"{TestConstants.Versions.Adapter}\",\"adapterName\":\"{TestAdapterName}\",\"partnerId\":\"{TestConstants.Partners.Id}\",\"partnerVersion\":\"{TestConstants.Versions.Partner}\"}}]";

            var adaptersInfo = json.ToAdaptersInfo();

            Assert.IsNotNull(adaptersInfo);
            Assert.AreEqual(1, adaptersInfo.Length);
            Assert.AreEqual(TestConstants.Versions.Adapter, adaptersInfo[0].AdapterVersion);
        }

        [Test]
        public void ToBannerAdLoadRequestDeserializesValidJson()
        {
            var json = $"{{\"placement\":\"{TestConstants.Placements.Standard}\",{StandardSizeJson}}}";

            var request = json.ToBannerAdLoadRequest();

            Assert.IsNotNull(request);
            Assert.AreEqual(TestConstants.Placements.Standard, request.PlacementName);
        }

        [TestCase("\"placement\":\"\",", TestName = "ToBannerAdLoadRequestReturnsNullWhenPlacementNameIsEmpty")]
        [TestCase("\"placement\":null,", TestName = "ToBannerAdLoadRequestReturnsNullWhenPlacementNameIsNull")]
        [TestCase("", TestName = "ToBannerAdLoadRequestReturnsNullWhenPlacementNameIsMissing")]
        public void ToBannerAdLoadRequestReturnsNullWithoutPlacementName(string placementField)
        {
            var json = $"{{{placementField}{StandardSizeJson}}}";

            Assert.IsNull(json.ToBannerAdLoadRequest());
        }

        [Test]
        public void ToDictionaryDeserializesValidJson()
        {
            var json = $"{{\"{TestConstants.Keywords.Key1}\":\"{TestConstants.Keywords.Value1}\",\"{TestConstants.Keywords.Key2}\":\"{TestConstants.Keywords.Value2}\"}}";

            var dictionary = json.ToDictionary();

            Assert.IsNotNull(dictionary);
            Assert.AreEqual(2, dictionary.Count);
            Assert.AreEqual(TestConstants.Keywords.Value1, dictionary[TestConstants.Keywords.Key1]);
            Assert.AreEqual(TestConstants.Keywords.Value2, dictionary[TestConstants.Keywords.Key2]);
        }

        [Test]
        public void ToDictionaryDeserializesEmptyObject()
        {
            var dictionary = TestConstants.Json.EmptyObject.ToDictionary();

            Assert.IsNotNull(dictionary);
            Assert.AreEqual(0, dictionary.Count);
        }

        [Test]
        public void ToListDeserializesValidJson()
        {
            var json = $"[\"{TestListItem1}\",\"{TestListItem2}\",\"{TestListItem3}\"]";

            var list = json.ToList();

            Assert.IsNotNull(list);
            CollectionAssert.AreEqual(new[] { TestListItem1, TestListItem2, TestListItem3 }, list);
        }

        [Test]
        public void ToListDeserializesEmptyArray()
        {
            var list = "[]".ToList();

            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void ToBannerSizeDeserializesValidJson()
        {
            var bannerSize = "{\"sizeType\":0,\"width\":320,\"height\":50}".ToBannerSize();

            Assert.IsNotNull(bannerSize);
            Assert.AreEqual(BannerSizeType.Standard, bannerSize.Value.SizeType);
        }

        [Test]
        public void ToVector2ThrowsExceptionForInvalidJson()
        {
            Assert.Throws<ArgumentException>(() => TestConstants.ChartboostMediation.InvalidJsonString.ToVector2());
        }

        // Literal JSON keeps parsing culture-independent; missing axes default to 0
        [TestCase("{\"x\":10.5,\"y\":20.3}", 10.5f, 20.3f, 0f, TestName = "ToVector2DeserializesValidJson")]
        [TestCase("{\"y\":20.3}", 0f, 20.3f, 0f, TestName = "ToVector2HandlesMissingXValue")]
        [TestCase("{\"x\":10.5}", 10.5f, 0f, 0f, TestName = "ToVector2HandlesMissingYValue")]
        [TestCase(TestConstants.Json.EmptyObject, 0f, 0f, 0f, TestName = "ToVector2HandlesMissingBothValues")]
        [TestCase("{\"x\":-15.5,\"y\":-25.3}", -15.5f, -25.3f, 0f, TestName = "ToVector2HandlesNegativeValues")]
        [TestCase("{\"x\":0,\"y\":0}", 0f, 0f, 0f, TestName = "ToVector2HandlesZeroValues")]
        [TestCase("{\"x\":999999.9,\"y\":888888.8}", 999999.9f, 888888.8f, 0.1f, TestName = "ToVector2HandlesLargeValues")]
        public void ToVector2ParsesAxes(string json, float expectedX, float expectedY, float tolerance)
        {
            var vector = json.ToVector2();

            Assert.AreEqual(expectedX, vector.x, tolerance);
            Assert.AreEqual(expectedY, vector.y, tolerance);
        }
    }
}

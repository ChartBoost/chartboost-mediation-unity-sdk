using System;
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
        private const float TestX = 10.5f;
        private const float TestY = 20.3f;

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
        public void ToChartboostMediationErrorReturnsNullForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var error = EmptyJson.ToChartboostMediationError();

            Assert.IsNull(error);
        }

        [Test]
        public void ToChartboostMediationErrorReturnsNullForNullJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var error = NullJson.ToChartboostMediationError();

            Assert.IsNull(error);
        }

        [Test]
        public void ToBidInfoDeserializesValidJson()
        {
            const string testAuctionId = "auction123";
            const string testPartnerId = "partner";
            const double testPrice = 1.5;
            const string testLineItemId = "item";
            const string testLineItemName = "name";
            var json = $"{{\"auction_id\":\"{testAuctionId}\",\"partner_id\":\"{testPartnerId}\",\"price\":{testPrice},\"line_item_id\":\"{testLineItemId}\",\"line_item_name\":\"{testLineItemName}\"}}";

            var bidInfo = json.ToBidInfo();
            
            if (!bidInfo.HasValue)
                Assert.Fail("BidInfo was null");

            Assert.AreEqual(testAuctionId, bidInfo.Value.AuctionId);
            Assert.AreEqual(testPartnerId, bidInfo.Value.PartnerId);
            Assert.AreEqual(testPrice, bidInfo.Value.Price);
        }

        [Test]
        public void ToBidInfoReturnsNullForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var bidInfo = EmptyJson.ToBidInfo();

            Assert.AreEqual(null, bidInfo);
        }

        [Test]
        public void ToMetricsDeserializesValidJson()
        {
            const string testAuctionId = "auction_123";
            var json = "{\"auction_id\":\"" + testAuctionId + "\",\"result\":\"success\",\"metrics\":[]}";

            var metrics = json.ToMetrics();

            Assert.IsNotNull(metrics);
            Assert.AreEqual(testAuctionId, metrics.Value.auctionId);
        }

        [Test]
        public void ToMetricsReturnsNullForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var metrics = EmptyJson.ToMetrics();

            Assert.IsNull(metrics);
        }

        [Test]
        public void ToMetricsReturnsNullForNullJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var metrics = NullJson.ToMetrics();

            Assert.IsNull(metrics);
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
        public void ToFullscreenAdLoadRequestReturnsNullForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var request = EmptyJson.ToFullscreenAdLoadRequest();

            Assert.IsNull(request);
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
        public void ToAdaptersInfoReturnsNullForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var adaptersInfo = EmptyJson.ToAdaptersInfo();

            Assert.IsNull(adaptersInfo);
        }

        [Test]
        public void ToBannerAdLoadRequestDeserializesValidJson()
        {
            var json = $"{{\"placement\":\"{TestConstants.Placements.Standard}\",\"size\":{{\"sizeType\":0,\"width\":{(int)TestConstants.Dimensions.StandardWidth},\"height\":{(int)TestConstants.Dimensions.StandardHeight}}}}}";

            var request = json.ToBannerAdLoadRequest();

            Assert.IsNotNull(request);
            Assert.AreEqual(TestConstants.Placements.Standard, request.PlacementName);
        }

        [Test]
        public void ToBannerAdLoadRequestReturnsNullForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var request = EmptyJson.ToBannerAdLoadRequest();

            Assert.IsNull(request);
        }

        [Test]
        public void ToBannerAdLoadRequestReturnsNullForNullJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var request = NullJson.ToBannerAdLoadRequest();

            Assert.IsNull(request);
        }

        [Test]
        public void ToBannerAdLoadRequestReturnsNullWhenPlacementNameIsEmpty()
        {
            var json = $"{{\"placement\":\"\",\"size\":{{\"sizeType\":0,\"width\":{(int)TestConstants.Dimensions.StandardWidth},\"height\":{(int)TestConstants.Dimensions.StandardHeight}}}}}";

            var request = json.ToBannerAdLoadRequest();

            Assert.IsNull(request);
        }

        [Test]
        public void ToBannerAdLoadRequestReturnsNullWhenPlacementNameIsNull()
        {
            var json = $"{{\"placement\":null,\"size\":{{\"sizeType\":0,\"width\":{(int)TestConstants.Dimensions.StandardWidth},\"height\":{(int)TestConstants.Dimensions.StandardHeight}}}}}";

            var request = json.ToBannerAdLoadRequest();

            Assert.IsNull(request);
        }

        [Test]
        public void ToBannerAdLoadRequestReturnsNullWhenPlacementNameIsMissing()
        {
            var json = $"{{\"size\":{{\"sizeType\":0,\"width\":{(int)TestConstants.Dimensions.StandardWidth},\"height\":{(int)TestConstants.Dimensions.StandardHeight}}}}}";

            var request = json.ToBannerAdLoadRequest();

            Assert.IsNull(request);
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
        public void ToDictionaryReturnsNullForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var dictionary = EmptyJson.ToDictionary();

            Assert.IsNull(dictionary);
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
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(TestListItem1, list[0]);
            Assert.AreEqual(TestListItem2, list[1]);
            Assert.AreEqual(TestListItem3, list[2]);
        }

        [Test]
        public void ToListReturnsNullForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var list = EmptyJson.ToList();

            Assert.IsNull(list);
        }

        [Test]
        public void ToListDeserializesEmptyArray()
        {
            const string emptyArrayJson = "[]";

            var list = emptyArrayJson.ToList();

            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void ToBannerSizeDeserializesValidJson()
        {
            var json = $"{{\"sizeType\":0,\"width\":{(int)TestConstants.Dimensions.StandardWidth},\"height\":{(int)TestConstants.Dimensions.StandardHeight}}}";

            var bannerSize = json.ToBannerSize();

            Assert.IsNotNull(bannerSize);
            Assert.AreEqual(BannerSizeType.Standard, bannerSize.Value.SizeType);
        }

        [Test]
        public void ToBannerSizeReturnsNullForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var bannerSize = EmptyJson.ToBannerSize();

            Assert.IsNull(bannerSize);
        }

        [Test]
        public void ToBannerSizeReturnsNullForNullJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var bannerSize = NullJson.ToBannerSize();

            Assert.IsNull(bannerSize);
        }

        [Test]
        public void ToContainerSizeDeserializesValidJson()
        {
            const string json = "{\"width\":320,\"height\":50}";

            var containerSize = json.ToContainerSize();

            Assert.AreEqual(320, containerSize.Width);
            Assert.AreEqual(50, containerSize.Height);
        }

        [Test]
        public void ToContainerSizeReturnsDefaultForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var containerSize = EmptyJson.ToContainerSize();

            Assert.AreEqual(default(ContainerSize), containerSize);
        }

        [Test]
        public void ToVector2DeserializesValidJson()
        {
            var json = $"{{\"x\":{TestX},\"y\":{TestY}}}";

            var vector = json.ToVector2();

            Assert.AreEqual(TestX, vector.x);
            Assert.AreEqual(TestY, vector.y);
        }

        [Test]
        public void ToVector2ReturnsZeroForEmptyJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var vector = EmptyJson.ToVector2();

            Assert.AreEqual(Vector2.zero, vector);
        }

        [Test]
        public void ToVector2ReturnsZeroForNullJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var vector = NullJson.ToVector2();

            Assert.AreEqual(Vector2.zero, vector);
        }

        [Test]
        public void ToVector2ReturnsZeroForWhitespaceJson()
        {
            LogAssert.Expect(LogType.Warning, new Regex(TestConstants.LogPatterns.CannotBeNullOrEmpty));

            var vector = WhitespaceJson.ToVector2();

            Assert.AreEqual(Vector2.zero, vector);
        }

        [Test]
        public void ToVector2ThrowsExceptionForInvalidJson()
        {
            Assert.Throws<ArgumentException>(() => TestConstants.ChartboostMediation.InvalidJsonString.ToVector2());
        }

        [Test]
        public void ToVector2HandlesMissingXValue()
        {
            var json = $"{{\"y\":{TestY}}}";

            var vector = json.ToVector2();

            Assert.AreEqual(TestConstants.Dimensions.Zero, vector.x);
            Assert.AreEqual(TestY, vector.y);
        }

        [Test]
        public void ToVector2HandlesMissingYValue()
        {
            var json = $"{{\"x\":{TestX}}}";

            var vector = json.ToVector2();

            Assert.AreEqual(TestX, vector.x);
            Assert.AreEqual(TestConstants.Dimensions.Zero, vector.y);
        }

        [Test]
        public void ToVector2HandlesMissingBothValues()
        {
            var vector = TestConstants.Json.EmptyObject.ToVector2();

            Assert.AreEqual(TestConstants.Dimensions.Zero, vector.x);
            Assert.AreEqual(TestConstants.Dimensions.Zero, vector.y);
        }

        [Test]
        public void ToVector2HandlesNegativeValues()
        {
            const float negativeX = -15.5f;
            const float negativeY = -25.3f;
            var json = $"{{\"x\":{negativeX},\"y\":{negativeY}}}";

            var vector = json.ToVector2();

            Assert.AreEqual(negativeX, vector.x);
            Assert.AreEqual(negativeY, vector.y);
        }

        [Test]
        public void ToVector2HandlesZeroValues()
        {
            var json = $"{{\"x\":{TestConstants.Dimensions.Zero},\"y\":{TestConstants.Dimensions.Zero}}}";

            var vector = json.ToVector2();

            Assert.AreEqual(TestConstants.Dimensions.Zero, vector.x);
            Assert.AreEqual(TestConstants.Dimensions.Zero, vector.y);
        }

        [Test]
        public void ToVector2HandlesLargeValues()
        {
            const float largeX = 999999.9f;
            const float largeY = 888888.8f;
            var json = $"{{\"x\":{largeX},\"y\":{largeY}}}";

            var vector = json.ToVector2();

            Assert.AreEqual(largeX, vector.x, 0.1f);
            Assert.AreEqual(largeY, vector.y, 0.1f);
        }
    }
}

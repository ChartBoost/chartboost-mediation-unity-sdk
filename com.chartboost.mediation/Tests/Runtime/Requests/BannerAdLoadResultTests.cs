using Chartboost.Mediation.Ad.Banner;
using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Requests
{
    public class BannerAdLoadResultTests
    {
        private const string TestLoadId = "banner_load_id_123";

        [Test]
        public void ConstructorForSuccessfulLoadInitializesCorrectly()
        {
            var metrics = new Metrics();
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);
            var size = BannerSize.Standard;
            var result = new BannerAdLoadResult(TestLoadId, metrics, bidInfo, null, size);

            Assert.AreEqual(TestLoadId, result.LoadId);
            Assert.IsNotNull(result.Metrics);
            Assert.IsNotNull(result.WinningBidInfo);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Size);
            Assert.AreEqual(size, result.Size);
        }

        [Test]
        public void ConstructorForSuccessfulLoadWithErrorInitializesCorrectly()
        {
            var metrics = new Metrics();
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);
            var error = new ChartboostMediationError(TestConstants.Errors.CodeBanner, TestConstants.Errors.Message);
            var size = BannerSize.Standard;
            var result = new BannerAdLoadResult(TestLoadId, metrics, bidInfo, error, size);

            Assert.AreEqual(TestLoadId, result.LoadId);
            Assert.IsNotNull(result.Metrics);
            Assert.IsNotNull(result.WinningBidInfo);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(TestConstants.Errors.CodeBanner, result.Error.Value.Code);
            Assert.IsNotNull(result.Size);
        }

        [Test]
        public void ConstructorForFailedLoadInitializesCorrectly()
        {
            var error = new ChartboostMediationError(TestConstants.Errors.CodeBanner, TestConstants.Errors.Message);
            var result = new BannerAdLoadResult(error);

            Assert.AreEqual(string.Empty, result.LoadId);
            Assert.IsNull(result.Metrics);
            Assert.IsNull(result.WinningBidInfo);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(TestConstants.Errors.CodeBanner, result.Error.Value.Code);
            Assert.AreEqual(TestConstants.Errors.Message, result.Error.Value.Message);
            Assert.IsNull(result.Size);
        }

        [Test]
        public void ConstructorForSuccessfulLoadWithNullSizeInitializesCorrectly()
        {
            var metrics = new Metrics();
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);
            var result = new BannerAdLoadResult(TestLoadId, metrics, bidInfo, null);

            Assert.AreEqual(TestLoadId, result.LoadId);
            Assert.IsNotNull(result.Metrics);
            Assert.IsNotNull(result.WinningBidInfo);
            Assert.IsNull(result.Error);
            Assert.IsNull(result.Size);
        }

        [Test]
        public void ConstructorForSuccessfulLoadWithNullMetricsInitializesCorrectly()
        {
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);
            var size = BannerSize.MediumRect;
            var result = new BannerAdLoadResult(TestLoadId, null, bidInfo, null, size);

            Assert.AreEqual(TestLoadId, result.LoadId);
            Assert.IsNull(result.Metrics);
            Assert.IsNotNull(result.WinningBidInfo);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Size);
            Assert.AreEqual(size, result.Size);
        }

        [Test]
        public void ConstructorForSuccessfulLoadWithNullBidInfoInitializesCorrectly()
        {
            var metrics = new Metrics();
            var size = BannerSize.Leaderboard;
            var result = new BannerAdLoadResult(TestLoadId, metrics, null, null, size);

            Assert.AreEqual(TestLoadId, result.LoadId);
            Assert.IsNotNull(result.Metrics);
            Assert.IsNull(result.WinningBidInfo);
            Assert.IsNull(result.Error);
            Assert.IsNotNull(result.Size);
            Assert.AreEqual(size, result.Size);
        }

        [Test]
        public void ConstructorForSuccessfulLoadWithAdaptiveSizeInitializesCorrectly()
        {
            var metrics = new Metrics();
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);
            var size = BannerSize.Adaptive(TestConstants.Dimensions.StandardWidth);
            var result = new BannerAdLoadResult(TestLoadId, metrics, bidInfo, null, size);

            Assert.IsNotNull(result.Size);
            Assert.AreEqual(TestConstants.Dimensions.StandardWidth, result.Size.Value.Width);
        }

        [Test]
        public void ConstructorForFailedLoadWithEmptyErrorMessageInitializesCorrectly()
        {
            var error = new ChartboostMediationError(string.Empty);
            var result = new BannerAdLoadResult(error);

            Assert.IsNotNull(result.Error);
            Assert.AreEqual(string.Empty, result.Error.Value.Message);
            Assert.IsNull(result.Size);
        }

        [Test]
        public void ImplementsIAdLoadResultInterface()
        {
            var error = new ChartboostMediationError(TestConstants.Errors.Message);
            var result = new BannerAdLoadResult(error);

            Assert.IsInstanceOf<IAdLoadResult>(result);
        }
    }
}

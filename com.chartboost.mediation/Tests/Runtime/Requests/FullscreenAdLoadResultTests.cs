using Chartboost.Mediation.Data;
using Chartboost.Mediation.Error;
using Chartboost.Mediation.Requests;
using NUnit.Framework;
// ReSharper disable AssignNullToNotNullAttribute

namespace Chartboost.Tests.Runtime.Requests
{
    public class FullscreenAdLoadResultTests
    {

        [Test]
        public void ConstructorForSuccessfulLoadInitializesCorrectly()
        {
            var metrics = new Metrics();
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);
            var result = new FullscreenAdLoadResult(null, TestConstants.LoadIds.Primary, metrics, bidInfo);

            Assert.AreEqual(TestConstants.LoadIds.Primary, result.LoadId);
            Assert.IsNotNull(result.Metrics);
            Assert.IsNotNull(result.WinningBidInfo);
            Assert.IsNull(result.Error);
            Assert.IsNull(result.Ad);
        }

        [Test]
        public void ConstructorForSuccessfulLoadWithErrorInitializesCorrectly()
        {
            var metrics = new Metrics();
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);
            var error = new ChartboostMediationError(TestConstants.Errors.CodeGeneric, TestConstants.Errors.Message);
            var result = new FullscreenAdLoadResult(null, TestConstants.LoadIds.Primary, metrics, bidInfo, error);

            Assert.AreEqual(TestConstants.LoadIds.Primary, result.LoadId);
            Assert.IsNotNull(result.Metrics);
            Assert.IsNotNull(result.WinningBidInfo);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(TestConstants.Errors.CodeGeneric, result.Error.Value.Code);
        }

        [Test]
        public void ConstructorForFailedLoadInitializesCorrectly()
        {
            var error = new ChartboostMediationError(TestConstants.Errors.CodeGeneric, TestConstants.Errors.Message);
            var result = new FullscreenAdLoadResult(error);

            Assert.IsNull(result.Ad);
            Assert.IsNull(result.Metrics);
            Assert.IsNull(result.WinningBidInfo);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(TestConstants.Errors.CodeGeneric, result.Error.Value.Code);
            Assert.AreEqual(TestConstants.Errors.Message, result.Error.Value.Message);
        }

        [Test]
        public void ConstructorForSuccessfulLoadWithNullMetricsInitializesCorrectly()
        {
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);
            var result = new FullscreenAdLoadResult(null, TestConstants.LoadIds.Primary, null, bidInfo);

            Assert.AreEqual(TestConstants.LoadIds.Primary, result.LoadId);
            Assert.IsNull(result.Metrics);
            Assert.IsNotNull(result.WinningBidInfo);
            Assert.IsNull(result.Error);
        }

        [Test]
        public void ConstructorForSuccessfulLoadWithNullBidInfoInitializesCorrectly()
        {
            var metrics = new Metrics();
            var result = new FullscreenAdLoadResult(null, TestConstants.LoadIds.Primary, metrics, null);

            Assert.AreEqual(TestConstants.LoadIds.Primary, result.LoadId);
            Assert.IsNotNull(result.Metrics);
            Assert.IsNull(result.WinningBidInfo);
            Assert.IsNull(result.Error);
        }

        [Test]
        public void ConstructorForSuccessfulLoadWithNullLoadIdInitializesCorrectly()
        {
            var metrics = new Metrics();
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);
            var result = new FullscreenAdLoadResult(null, null, metrics, bidInfo);

            Assert.IsNull(result.LoadId);
            Assert.IsNotNull(result.Metrics);
            Assert.IsNotNull(result.WinningBidInfo);
        }

        [Test]
        public void ConstructorForFailedLoadWithEmptyErrorMessageInitializesCorrectly()
        {
            var error = new ChartboostMediationError(string.Empty);
            var result = new FullscreenAdLoadResult(error);

            Assert.IsNull(result.Ad);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(string.Empty, result.Error.Value.Message);
        }

        [Test]
        public void ImplementsIAdLoadResultInterface()
        {
            var error = new ChartboostMediationError(TestConstants.Errors.Message);
            var result = new FullscreenAdLoadResult(error);

            Assert.IsInstanceOf<IAdLoadResult>(result);
        }
    }
}

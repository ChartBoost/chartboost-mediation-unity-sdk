using Chartboost.Mediation.Data;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime.Data
{
    public class BidInfoTests
    {

        [Test]
        public void ConstructorInitializesAllFields()
        {
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.IdAlt, TestConstants.Auction.Price, TestConstants.Auction.LineItemName, TestConstants.Auction.LineItemId);

            Assert.AreEqual(TestConstants.Auction.AuctionId, bidInfo.AuctionId);
            Assert.AreEqual(TestConstants.Partners.IdAlt, bidInfo.PartnerId);
            Assert.AreEqual(TestConstants.Auction.Price, bidInfo.Price);
            Assert.AreEqual(TestConstants.Auction.LineItemName, bidInfo.LineItemName);
            Assert.AreEqual(TestConstants.Auction.LineItemId, bidInfo.LineItemId);
        }

        [Test]
        public void ConstructorHandlesNullValues()
        {
            var bidInfo = new BidInfo(null, null, 0.0, null, null);

            Assert.IsNull(bidInfo.AuctionId);
            Assert.IsNull(bidInfo.PartnerId);
            Assert.AreEqual(0.0, bidInfo.Price);
            Assert.IsNull(bidInfo.LineItemName);
            Assert.IsNull(bidInfo.LineItemId);
        }

        [Test]
        public void ConstructorHandlesEmptyStrings()
        {
            var bidInfo = new BidInfo(string.Empty, string.Empty, 0.0, string.Empty, string.Empty);

            Assert.AreEqual(string.Empty, bidInfo.AuctionId);
            Assert.AreEqual(string.Empty, bidInfo.PartnerId);
            Assert.AreEqual(0.0, bidInfo.Price);
            Assert.AreEqual(string.Empty, bidInfo.LineItemName);
            Assert.AreEqual(string.Empty, bidInfo.LineItemId);
        }

        [Test]
        public void ConstructorHandlesNegativePrice()
        {
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceNegative, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);

            Assert.AreEqual(TestConstants.Auction.PriceNegative, bidInfo.Price);
        }

        [Test]
        public void ConstructorHandlesZeroPrice()
        {
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, 0.0, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);

            Assert.AreEqual(0.0, bidInfo.Price);
        }

        [Test]
        public void ConstructorHandlesLargePrice()
        {
            var bidInfo = new BidInfo(TestConstants.Auction.AuctionIdGeneric, TestConstants.Partners.Generic, TestConstants.Auction.PriceLarge, TestConstants.Auction.LineItemIdGeneric, TestConstants.Auction.LineItemIdGeneric);

            Assert.AreEqual(TestConstants.Auction.PriceLarge, bidInfo.Price);
        }

        [Test]
        public void StructsWithSameValuesAreEqual()
        {
            var bid1 = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.IdAlt, TestConstants.Auction.Price, TestConstants.Auction.LineItemName, TestConstants.Auction.LineItemId);
            var bid2 = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.IdAlt, TestConstants.Auction.Price, TestConstants.Auction.LineItemName, TestConstants.Auction.LineItemId);

            Assert.AreEqual(bid1, bid2);
        }

        [Test]
        public void StructsWithDifferentAuctionIdsAreNotEqual()
        {
            var bid1 = new BidInfo("auction1", TestConstants.Partners.Partner1, TestConstants.Auction.Price, TestConstants.Auction.LineItemName, TestConstants.Auction.LineItemId);
            var bid2 = new BidInfo("auction2", TestConstants.Partners.Partner1, TestConstants.Auction.Price, TestConstants.Auction.LineItemName, TestConstants.Auction.LineItemId);

            Assert.AreNotEqual(bid1, bid2);
        }

        [Test]
        public void StructsWithDifferentPartnerIdsAreNotEqual()
        {
            var bid1 = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.Partner1, TestConstants.Auction.Price, TestConstants.Auction.LineItemName, TestConstants.Auction.LineItemId);
            var bid2 = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.Partner2, TestConstants.Auction.Price, TestConstants.Auction.LineItemName, TestConstants.Auction.LineItemId);

            Assert.AreNotEqual(bid1, bid2);
        }

        [Test]
        public void StructsWithDifferentPricesAreNotEqual()
        {
            var bid1 = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.IdAlt, TestConstants.Auction.PriceAlt, TestConstants.Auction.LineItemName, TestConstants.Auction.LineItemId);
            var bid2 = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.IdAlt, TestConstants.Auction.PriceAlt2, TestConstants.Auction.LineItemName, TestConstants.Auction.LineItemId);

            Assert.AreNotEqual(bid1, bid2);
        }

        [Test]
        public void StructsWithDifferentLineItemNamesAreNotEqual()
        {
            var bid1 = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.Id, TestConstants.Auction.Price, "item1", TestConstants.Auction.LineItemId);
            var bid2 = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.Id, TestConstants.Auction.Price, "item2", TestConstants.Auction.LineItemId);

            Assert.AreNotEqual(bid1, bid2);
        }

        [Test]
        public void StructsWithDifferentLineItemIdsAreNotEqual()
        {
            var bid1 = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.IdAlt, TestConstants.Auction.Price, TestConstants.Auction.LineItemName, "id1");
            var bid2 = new BidInfo(TestConstants.Auction.AuctionId, TestConstants.Partners.IdAlt, TestConstants.Auction.Price, TestConstants.Auction.LineItemName, "id2");

            Assert.AreNotEqual(bid1, bid2);
        }
    }
}

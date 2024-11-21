using Chartboost.Mediation.Data;
using NUnit.Framework;

namespace Chartboost.Tests.Runtime
{
    public class BidInfoValidator
    {
        private BidInfo _testBidInfo;

        private const string TestAuctionId = "test_auction_id";
        private const string TestPartnerId = "test_partner_id";
        private const string TestLineItemName = "test_line_item_name";
        private const string TestLineItemId = "test_line_item_id";
        private const double TestPrice = 0.1f;

        [Test, Order(0)]
        public void Constructor()
        {
            _testBidInfo = new BidInfo(TestAuctionId, TestPartnerId, TestPrice, TestLineItemName, TestLineItemId);
            Assert.IsNotNull(_testBidInfo);
        }

        [Test, Order(1)]
        public void AuctionId() => Assert.AreEqual(TestAuctionId, _testBidInfo.AuctionId);
        
        [Test, Order(2)]
        public void PartnerId() => Assert.AreEqual(TestPartnerId, _testBidInfo.PartnerId);

        [Test, Order(3)]
        public void Price() => Assert.AreEqual(TestPrice, _testBidInfo.Price);
        
        [Test, Order(4)]
        public void LineItemName() => Assert.AreEqual(TestLineItemName, _testBidInfo.LineItemName);
        
        [Test, Order(5)]
        public void LineItemId() => Assert.AreEqual(TestLineItemId, _testBidInfo.LineItemId);
    }
}

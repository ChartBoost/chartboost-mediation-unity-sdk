namespace Chartboost
{
    public struct BidInfo
    {
        public readonly string AuctionId;
        public readonly string PartnerId;
        public readonly double Price;

        public BidInfo(string auctionId, string partnerId, double price)
        {
            AuctionId = auctionId;
            PartnerId = partnerId;
            Price = price;
        }
    }
}

namespace Helium
{
    public struct HeliumBidInfo
    {
        public readonly string AuctionId;
        public readonly string PartnerId;
        public readonly double Price;

        public HeliumBidInfo(string auctionId, string partnerId, double price)
        {
            AuctionId = auctionId;
            PartnerId = partnerId;
            Price = price;
        }
    }
}

namespace Helium
{
    public struct HeliumBidInfo
    {
        public string AuctionId;
        public string PartnerPlacementName;
        public double Price;
        public string Seat;

        public HeliumBidInfo(string auctionId, string partnerPlacementName, double price, string seat)
        {
            AuctionId = auctionId;
            PartnerPlacementName = partnerPlacementName;
            Price = price;
            Seat = seat;
        }
    }
}

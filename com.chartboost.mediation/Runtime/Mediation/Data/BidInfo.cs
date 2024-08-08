using System;
using Newtonsoft.Json;

namespace Chartboost.Mediation.Data
{
    /// <summary>
    /// Information about the bid that won the auction.
    /// </summary>
    [Serializable]
    public struct BidInfo
    {
        [JsonProperty("auction_id")] public readonly string AuctionId;
        [JsonProperty("partner_id")] public readonly string PartnerId;
        [JsonProperty("price")] public readonly double Price;
        [JsonProperty("line_item_name")] public readonly string LineItemName;
        [JsonProperty("line_item_id")] public readonly string LineItemId;

        public BidInfo(string auctionId, string partnerId, double price, string lineItemName, string lineItemId)
        {
            AuctionId = auctionId;
            PartnerId = partnerId;
            Price = price;
            LineItemName = lineItemName;
            LineItemId = lineItemId;
        }
    }
}

using System;
using Newtonsoft.Json;

namespace Chartboost
{
    [Serializable]
    #nullable enable
    public struct BidInfo
    {
        [JsonProperty("auction-id")] public readonly string AuctionId;
        [JsonProperty("partner-id")] public readonly string PartnerId;
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
    #nullable disable
}

using System;
using Newtonsoft.Json;

namespace Chartboost
{
    [Serializable]
    #nullable enable
    public struct BidInfo
    {
        [JsonProperty("auction-id")]
        public readonly string AuctionId;
        [JsonProperty("partner-id")]
        public readonly string PartnerId;
        [JsonProperty("price")]
        public readonly double Price;

        public BidInfo(string auctionId, string partnerId, double price)
        {
            AuctionId = auctionId;
            PartnerId = partnerId;
            Price = price;
        }
    }
    #nullable disable
}

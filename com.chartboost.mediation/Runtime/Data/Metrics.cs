using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chartboost.Placements
{
    #nullable enable
    [Serializable]
    public struct Metrics
    {
        [JsonProperty("auction_id")]
        public string? auctionId;

        [JsonProperty("result")]
        public string? result;

        [JsonProperty("metrics")]
        public List<MetricsData> metrics;

        [JsonProperty("error")]
        public MetricsError? error;
    }

    [Serializable]
    public struct MetricsData
    {
        [JsonProperty("network_type")]
        public string? networkType;
        
        [JsonProperty("line_item_id")]
        public string? lineItemId;
        
        [JsonProperty("partner_placement")]
        public string? partnerPlacement;
        
        [JsonProperty("partner")]
        public string? partner;
        
        [JsonProperty("start")]
        public long? start;
        
        [JsonProperty("end")]
        public long? end;
        
        [JsonProperty("duration")]
        public long? duration;
        
        [JsonProperty("is_success")]
        public bool isSuccess;
        
        [JsonProperty("partner_sdk_version")]
        public string? partnerSdkVersion;
        
        [JsonProperty("partner_adapter_version")]
        public string? partnerAdapterVersion;
    }

    [Serializable]
    public struct MetricsError
    {
        [JsonProperty("cm_code")]
        public string code;

        [JsonProperty("details")]
        public ErrorDetails details;
    }

    [Serializable]
    public struct ErrorDetails
    {
        [JsonProperty("type")]
        public string type;
        
        [JsonProperty("description")]
        public string description;
        
        [JsonProperty("data_as_string")]
        public string? data;
    }
#nullable disable
}

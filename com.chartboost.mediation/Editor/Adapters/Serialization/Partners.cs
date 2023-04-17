using System;
using Newtonsoft.Json;

namespace Chartboost.Editor.Adapters.Serialization
{
    #nullable enable
    [Serializable]
    public class Partners
    {
        /// <summary>
        /// Timestamp of last update
        /// </summary>
        public string? lastUpdated;

        /// <summary>
        /// All certified Ad Adapters.
        /// </summary>
        [JsonProperty("partners")]
        public Adapter[]? adapters;
    }
    #nullable disable
}

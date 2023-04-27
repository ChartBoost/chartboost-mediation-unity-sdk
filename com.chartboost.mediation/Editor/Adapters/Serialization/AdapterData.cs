using System;
using Newtonsoft.Json;

namespace Chartboost.Editor.Adapters.Serialization
{
    /// <summary>
    /// Root level structure for S3 json file containing adapter information.
    /// </summary>
    #nullable enable
    [Serializable]
    public struct AdapterData
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

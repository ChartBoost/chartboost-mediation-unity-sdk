using System;
using Newtonsoft.Json;

namespace Chartboost.Adapters
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

    [Serializable]
    public struct Adapter
    {
        /// <summary>
        /// The string that defines the adapter name.
        /// </summary>
        public string name;
        
        /// <summary>
        /// The string that defines the adapter id.
        /// </summary>
        public string id;
        
        /// <summary>
        /// Url to the logo identifying the adapter
        /// </summary>
        public string logoUrl;

        /// <summary>
        /// Timestamp of last update
        /// </summary>
        public string lastUpdated;

        /// <summary>
        /// Container to adapter documentation links
        /// </summary>
        public Documentation documentationUrl;

        /// <summary>
        /// Android Ad Adapter properties
        /// </summary>
        public NativeAdapter android;

        /// <summary>
        /// IOS Ad Adapter properties
        /// </summary>
        public NativeAdapter ios;
    }

    #nullable enable
    [Serializable]
    public struct NativeAdapter
    {
        /// <summary>
        /// Chartboost mediation naming convention for Native Ad Adapter.
        /// </summary>
        public string adapter;

        /// <summary>
        /// SDK dependency name of the Native Ad Adapter.
        /// </summary>
        public string sdk;
        
        /// <summary>
        /// Additional dependencies for the certified Ad Adapter
        /// </summary>
        public string[]? dependencies;
        
        /// <summary>
        /// Additional repositories to fetch dependencies. Some providers have their own repos, they can be found here.
        /// </summary>
        public string[]? repositories;

        /// <summary>
        /// All certified versions of the Ad Adapter
        /// </summary>
        public string[] versions;

        /// <summary>
        /// Indicates if Native SDK dependency should be added to all targets, this is defined on a case by case basis.
        /// </summary>
        public bool? allTargets;
    }
    #nullable disable

    [Serializable]
    public struct Documentation
    {
        public string chartboost;

        public string partner;
    }
}

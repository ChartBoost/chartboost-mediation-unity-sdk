using System;

namespace Chartboost.Adapters
{
    #nullable enable
    [Serializable]
    public class UnityAdapters
    {
        /// <summary>
        /// Current version of the Adapter config file.
        /// </summary>
        public string? version;

        /// <summary>
        /// All certified Ad Adapters.
        /// </summary>
        public AdAdapter[]? adapters;
    }
    #nullable disable

    [Serializable]
    public struct AdAdapter
    {
        /// <summary>
        /// The string that defines the adapter id.
        /// </summary>
        public string id;
        
        /// <summary>
        /// The string that defines the adapter name.
        /// </summary>
        public string name;

        /// <summary>
        /// Android Ad Adapter properties
        /// </summary>
        public NativeAdAdapter android;

        /// <summary>
        /// IOS Ad Adapter properties
        /// </summary>
        public NativeAdAdapter ios;
    }

    [Serializable]
    public struct NativeAdAdapter
    {
        /// <summary>
        /// Chartboost mediation naming convention for Native Ad Adapter.
        /// </summary>
        public string name;

        /// <summary>
        /// SDK dependency name of the Native Ad Adapter.
        /// </summary>
        public string sdk;

        /// <summary>
        /// Latest certified version of the Ad Adapter.
        /// </summary>
        public string latest;

        /// <summary>
        /// All certified versions of the Ad Adapter
        /// </summary>
        public string[] versions;

        /// <summary>
        /// Additional dependencies for the certified Ad Adapter
        /// </summary>
        public string[] dependencies;
        
        /// <summary>
        /// Additional repositories to fetch dependencies. Some providers have their own repos, they can be found here.
        /// </summary>
        public string[] repositories;

        /// <summary>
        /// Indicates if Native SDK dependency should be added to all targets, this is defined on a case by case basis.
        /// </summary>
        public bool? addToAllTargets;
    }
}

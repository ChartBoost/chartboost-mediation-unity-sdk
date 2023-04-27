using System;

namespace Chartboost.Editor.Adapters.Serialization
{
    /// <summary>
    /// Base structure for native adapters. 
    /// </summary>
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
}

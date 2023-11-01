using System;

namespace Chartboost.Editor.EditorWindows.Adapters.Serialization
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
        /// Additional repositories to fetch dependencies. Some providers have their own repos, they can be found here.
        /// </summary>
        public string[]? repositories;
        
        /// <summary>
        /// All certified versions of the Ad Adapter
        /// </summary>
        public AdapterVersion[]? versions;
    }
    #nullable disable
}

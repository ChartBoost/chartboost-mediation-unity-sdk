using System;
using System.Collections.Generic;

namespace Chartboost.Editor.Adapters.Serialization
{
    #nullable enable
    /// <summary>
    /// Represents all versions compatible with an specific adapter dependency set. This is relevant to those adapters that have changed their SDK dependencies naming conventions and versions.
    /// </summary>
    [Serializable]
    public struct AdapterVersion
    {
        /// <summary>
        /// Additional dependencies for the certified Ad Adapter
        /// </summary>
        public List<string>? dependencies;
        
        /// <summary>
        /// All certified versions of the Ad Adapter
        /// </summary>
        public List<string>? versions;
        
        /// <summary>
        /// Indicates if Native SDK dependency should be added to all targets, this is defined on a case by case basis.
        /// </summary>
        public bool? allTargets;
    }
    #nullable disable
}
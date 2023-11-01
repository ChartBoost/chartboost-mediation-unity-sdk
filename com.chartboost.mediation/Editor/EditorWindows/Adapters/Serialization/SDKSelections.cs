using System;

namespace Chartboost.Editor.EditorWindows.Adapters.Serialization
{
    /// <summary>
    /// Root structure for User selections. Saved into a json file.
    /// </summary>
    #nullable enable
    [Serializable]
    public sealed class SDKSelections
    {
        /// <summary>
        /// Current Chartboost Mediation version dependencies, this value is generated off the Chartboost Mediation UPM package.
        /// </summary>
        public string? mediationVersion;

        /// <summary>
        /// User specific Ad Adapter selections.
        /// </summary>
        public AdapterSelection[]? adapterSelections;
    }
    #nullable disable
}

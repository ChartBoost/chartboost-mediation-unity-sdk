using System;

namespace Chartboost.Editor.Adapters.Serialization
{
    #nullable enable
    [Serializable]
    public class SDKSelections
    {
        public string? mediationVersion;

        public AdapterSelection[]? adapterSelections;
    }
    #nullable disable
}

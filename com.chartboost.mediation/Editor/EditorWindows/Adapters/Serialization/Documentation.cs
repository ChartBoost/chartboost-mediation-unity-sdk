using System;

namespace Chartboost.Editor.EditorWindows.Adapters.Serialization
{
    /// <summary>
    /// Contains references to Adapters Documentation.
    /// </summary>
    [Serializable]
    public struct Documentation
    {
        /// <summary>
        /// Chartboost specific documentation.
        /// </summary>
        public string chartboost;
        
        /// <summary>
        /// Partner network specific documentation.
        /// </summary>
        public string partner;
    }
}

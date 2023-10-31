using System;

namespace Chartboost.Editor.EditorWindows.Adapters.Serialization
{
    /// <summary>
    /// Serializable class indicating Adapter change.
    /// </summary>
    [Serializable]
    public struct AdapterChange
    {
        /// <summary>
        /// string defining adapter id.
        /// </summary>
        public string id;
        
        /// <summary>
        /// Adapter platform, Android or iOS.
        /// </summary>
        public string platform;
        
        /// <summary>
        /// Old adapter version, before change.
        /// </summary>
        public string oldVersion;
        
        /// <summary>
        /// New adapter version, after change.
        /// </summary>
        public string newVersion;
        
        public AdapterChange(string id, Platform platform, string oldVersion, string newVersion)
        {
            this.id = id;
            this.platform = platform.ToString();
            this.oldVersion = oldVersion;
            this.newVersion = newVersion;
        }
    }
}

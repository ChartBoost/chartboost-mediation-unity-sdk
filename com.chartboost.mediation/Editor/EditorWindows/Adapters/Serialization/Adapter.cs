using System;

namespace Chartboost.Editor.EditorWindows.Adapters.Serialization
{
    /// <summary>
    /// Base Unity friendly Adapter structure. Contains native platforms adapter specific information. 
    /// </summary>
    [Serializable]
    public struct Adapter
    {
        /// <summary>
        /// string defining adapter name.
        /// </summary>
        public string name;
        
        /// <summary>
        /// string defining adapter id.
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
}

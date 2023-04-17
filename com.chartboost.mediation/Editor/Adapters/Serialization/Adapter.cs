using System;

namespace Chartboost.Editor.Adapters.Serialization
{
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
}

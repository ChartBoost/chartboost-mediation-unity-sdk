using System;

namespace Chartboost.Editor.Adapters.Serialization
{
    [Serializable]
    public class AdapterChange
    {
        public string id;
        public string platform;
        public string oldVersion;
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

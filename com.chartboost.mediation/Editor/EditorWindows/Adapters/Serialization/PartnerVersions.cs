using System.Collections.Generic;

namespace Chartboost.Editor.EditorWindows.Adapters.Serialization
{
    /// <summary>
    /// Provides platform specific partner SDK versions from the Chartboost Mediation Adapter versions.
    /// </summary>
    public struct PartnerVersions
    {
        private const string Unselected = "Unselected";
        /// <summary>
        /// Android versions from newest to older. [Unselected, Newest, Older, ...]
        /// </summary>
        public readonly string[] android;
        
        /// <summary>
        /// IOS versions from newest to older. [Unselected, Newest, Older, ...]
        /// </summary> 
        public readonly string[] ios;

        /// <summary>
        /// Generates partner network user readable versions
        /// </summary>
        /// <param name="androidAdapters">Partner Adapter Android versions.</param>
        /// <param name="iosAdapters">Partner Adapter IOS versions.</param>
        public PartnerVersions(AdapterVersion[] androidAdapters, AdapterVersion[] iosAdapters)
        {
            android = GetSupportedVersions(androidAdapters);
            ios = GetSupportedVersions(iosAdapters);
        }

        private static string[] GetSupportedVersions(AdapterVersion[] adapters)
        {
            var temp = new List<string> { Unselected };

            foreach (var adapterVersion in adapters)
            {
                foreach (var version in adapterVersion.versions)
                {
                    var partnerVersion = GetPartnerSDKVersion(version);
                    if (!temp.Contains(partnerVersion))
                        temp.Add(partnerVersion);
                }
            }

            return temp.ToArray();
        }

        private static string GetPartnerSDKVersion(string adapterVersion)
        {
            const int removalIndex = 2;
            adapterVersion = adapterVersion.Remove(0, removalIndex);
            adapterVersion =  adapterVersion.Remove(adapterVersion.Length - removalIndex, removalIndex);
            return adapterVersion;
        }
    }
}

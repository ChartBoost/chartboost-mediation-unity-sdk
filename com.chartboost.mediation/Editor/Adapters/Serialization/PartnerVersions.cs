using System.Collections.Generic;

namespace Chartboost.Editor.Adapters.Serialization
{
    /// <summary>
    /// Provides platform specific partner SDK versions from the Chartboost Mediation Adapter versions.
    /// </summary>
    public class PartnerVersions
    {
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
        public PartnerVersions(IEnumerable<string> androidAdapters, IEnumerable<string> iosAdapters)
        {
            android = GetSupportedVersions(androidAdapters);
            ios = GetSupportedVersions(iosAdapters);
        }

        private static string[] GetSupportedVersions(IEnumerable<string> adapters)
        {
            var temp = new List<string> { Constants.Unselected };

            foreach (var platformVersion in adapters)
            {
                var partnerVersion = GetPartnerSDKVersion(platformVersion);
                if (!temp.Contains(partnerVersion))
                    temp.Add(partnerVersion);
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

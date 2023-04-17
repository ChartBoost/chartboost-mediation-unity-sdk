using System.Collections.Generic;

namespace Chartboost.Editor.Adapters.Serialization
{
    /// <summary>
    /// Provides platform specific partner SDK versions from the Chartboost Mediation Adapter versions.
    /// </summary>
    public class PartnerVersions
    {
        public readonly string[] android;
        public readonly string[] ios;

        public PartnerVersions(IEnumerable<string> androidAdapters, string[] iosAdapters)
        {
            android = GetSupportedVersions(androidAdapters);
            ios = GetSupportedVersions(iosAdapters);
        }

        private string[] GetSupportedVersions(IEnumerable<string> adapters)
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

        private string GetPartnerSDKVersion(string adapterVersion)
        {
            const int removalIndex = 2;
            adapterVersion = adapterVersion.Remove(0, removalIndex);
            adapterVersion =  adapterVersion.Remove(adapterVersion.Length - removalIndex, removalIndex);
            return adapterVersion;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Chartboost.Adapters
{
    public partial class AdaptersWindow
    {
        /// <summary>
        /// Supported platforms for Adapters.
        /// </summary>
        private enum Platform
        {
            Android,
            IOS
        }

        /// <summary>
        /// Contains selected versions in the project, used to update, modify, existing selections. 
        /// </summary>
        [Serializable]
        private class ProjectSelectedVersions
        {
            public string id;
            public string android = UNSELECTED;
            public string ios = UNSELECTED;

            public ProjectSelectedVersions(string id)
            {
                this.id = id;
            }
        }

        /// <summary>
        /// Provides platform specific partner SDK versions from the Chartboost Mediation Adapter versions.
        /// </summary>
        private class PartnerVersions
        {
            public readonly string[] Android;
            public readonly string[] IOS;

            public PartnerVersions(string[] androidAdapters, string[] iosAdapters)
            {
                Android = GetSupportedVersions(androidAdapters);
                IOS = GetSupportedVersions(iosAdapters);
            }

            private static string[] GetSupportedVersions(string[] adapters)
            {
                var temp = new List<string> { UNSELECTED };

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
                adapterVersion = adapterVersion.Remove(0, 2);
                adapterVersion = adapterVersion.Remove(adapterVersion.Length - 2, 2);
                return adapterVersion;
            }
        }

        private static void LoadSelection()
        {
            const string selectionsJson = "Assets/com.chartboost.mediation/Editor/selections.json";
            
            if (!File.Exists(selectionsJson))
                return;

            var jsonContents = File.ReadAllText(selectionsJson);

            var selections = JsonConvert.DeserializeObject<ProjectSelectedVersions[]>(jsonContents);

            foreach (var versionSelection in selections)
                Instance._selectedVersions[versionSelection.id] = versionSelection;
        }

        private static void SaveSelection()
        {
            var allSelections = Instance._selectedVersions.Values.Where(x => x.android != UNSELECTED || x.ios != UNSELECTED);
            
            var selectionsJson = JsonConvert.SerializeObject(allSelections);

            var packageAssetsInAssets = Path.Combine(Application.dataPath, "com.chartboost.mediation");

            if (!Directory.Exists(packageAssetsInAssets))
                Directory.CreateDirectory(packageAssetsInAssets);

            var editorFolder = Path.Combine(packageAssetsInAssets, "Editor");

            if (!Directory.Exists(editorFolder))
                Directory.CreateDirectory(editorFolder);

            var selectionsFile = Path.Combine(editorFolder, "selections.json");
            
            File.WriteAllText(selectionsFile, selectionsJson);
            AssetDatabase.Refresh();
            _saveButton.RemoveFromHierarchy();
        }
    }
}

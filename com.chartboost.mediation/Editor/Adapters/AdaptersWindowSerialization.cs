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
        public class SelectedVersions
        {
            public string id;
            public string android = Unselected;
            public string ios = Unselected;

            public SelectedVersions(string id)
            {
                this.id = id;
            }
        }

        /// <summary>
        /// Provides platform specific partner SDK versions from the Chartboost Mediation Adapter versions.
        /// </summary>
        public class PartnerVersions
        {
            public readonly string[] android;
            public readonly string[] ios;

            public PartnerVersions(string[] androidAdapters, string[] iosAdapters)
            {
                android = GetSupportedVersions(androidAdapters);
                ios = GetSupportedVersions(iosAdapters);
            }

            private static string[] GetSupportedVersions(string[] adapters)
            {
                var temp = new List<string> { Unselected };

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

        public static void LoadSelections()
        {
            const string selectionsJson = "Assets/com.chartboost.mediation/Editor/selections.json";
            
            if (!File.Exists(selectionsJson))
                return;

            var jsonContents = File.ReadAllText(selectionsJson);

            var selections = JsonConvert.DeserializeObject<SelectedVersions[]>(jsonContents);

            foreach (var versionSelection in selections)
                UserSelectedVersions[versionSelection.id] = versionSelection;

            SavedVersions = UserSelectedVersions.ToDictionary(k => k.Key, v =>
            {
                var newSelection = new SelectedVersions(v.Key.ToString())
                {
                    android = v.Value.android,
                    ios = v.Value.ios
                };
                return newSelection;
            });
        }

        public static void SaveSelections()
        {
            var allSelections = UserSelectedVersions.Values.Where(x => x.android != Unselected || x.ios != Unselected);
            
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
        
        public class DictionaryComparer<TKey, TValue> : IEqualityComparer<Dictionary<TKey, TValue>>
        {
            private readonly IEqualityComparer<TValue> _valueComparer;

            public DictionaryComparer(IEqualityComparer<TValue> valueComparer) 
                => _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            public bool Equals(Dictionary<TKey, TValue> x, Dictionary<TKey, TValue> y)
            {
                if (x.Count != y.Count)
                    return false;
                if (x.Keys.Except(y.Keys).Any())
                    return false;
                if (y.Keys.Except(x.Keys).Any())
                    return false;
                foreach (var pair in x)
                    if (!_valueComparer.Equals(pair.Value, y[pair.Key]))
                        return false;
                return true;
            }

            public int GetHashCode(Dictionary<TKey, TValue> obj)
            {
                throw new NotImplementedException();
            }
        }

        public class SelectedVersionsComparer : IEqualityComparer<SelectedVersions>
        {
            public bool Equals(SelectedVersions x, SelectedVersions y)
            {
                if (x.id != y.id)
                    return false;
                if (x.android != y.android)
                    return false;
                if (x.ios != y.ios)
                    return false;
                return true;
            }

            public int GetHashCode(SelectedVersions obj)
            {
                unchecked
                {
                    var hashCode = (obj.id != null ? obj.id.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.android != null ? obj.android.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.ios != null ? obj.ios.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
}

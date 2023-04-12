using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Chartboost.Adapters
{
    public partial class AdaptersWindow
    {
        private static void Refresh()
        {
            Instance.rootVisualElement.Clear();
            AdapterDataSource.Update();
            Instance.Initialize();
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Chartboost Mediation", "Adapter update completed.", "ok");
        }
        
        public static bool CheckForChanges()
        {
            var root = Instance.rootVisualElement;
            var same =
                new DictionaryComparer<string, AdapterSelection>(new SelectedVersionsComparer()).Equals(UserSelectedVersions, SavedVersions);
            switch (same)
            {
                case false when !root.Contains(_saveButton):
                    root.Add(_saveButton);
                    return true;
                case true:
                    _saveButton.RemoveFromHierarchy();
                    break;
            }
            return false;
        }

        public static void UpgradeSelectionsToLatest()
        {
            var cancel = false;
            
            if (!Application.isBatchMode) { 
                cancel = EditorUtility.DisplayDialog(
                    "Chartboost Mediation", 
                    "Doing this will update all of your selected adapters to their latest version, do you wish to continue?", "Yes", "No");
            }
            
            if (!cancel)
                return;

            var currentSelections = UserSelectedVersions.ToDictionary(k => k.Key, v =>
            {
                var newSelection = new AdapterSelection(v.Key.ToString())
                {
                    android = v.Value.android,
                    ios = v.Value.ios
                };
                return newSelection;
            });
            
            foreach (var selection in currentSelections)
            {
                var adapterId = selection.Key;
                UpdateSelection(PartnerSDKVersions[adapterId].android, adapterId, selection.Value.android, Platform.Android);
                UpdateSelection(PartnerSDKVersions[adapterId].ios,  adapterId, selection.Value.ios, Platform.IOS);
            }
            
            var changes = CheckForChanges();

            if (!changes && !Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    "Chartboost Mediation", 
                    "No adapters updated, everything is already up to date!\n\n Do you think this is incorrect? Try using the refresh button.",
                    "Ok");
            }


            void UpdateSelection(IReadOnlyList<string> versions, string id, string startValue, Platform platform)
            {
                if (startValue.Equals(Unselected))
                    return;

                var latest = versions[1];

                if (latest == startValue)
                    return;
                
                switch (platform)
                {
                    case Platform.Android:
                        UserSelectedVersions[id].android = latest;
                        UserSelectedVersions[id].androidDropdown.text = latest;
                        break;
                    case Platform.IOS:
                        UserSelectedVersions[id].ios = latest;
                        UserSelectedVersions[id].iosDropdown.text = latest;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
                }
            }
        }
        
        public static PackageInfo FindPackage(string packageName)
        {
            var packageJsons = AssetDatabase.FindAssets("package")
                .Select(AssetDatabase.GUIDToAssetPath).Where(x => AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
                .Select(PackageInfo.FindForAssetPath).ToList();

            return packageJsons.Find(x => x.name == packageName);
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

        public class SelectedVersionsComparer : IEqualityComparer<AdaptersWindow.AdapterSelection>
        {
            public bool Equals(AdaptersWindow.AdapterSelection x, AdaptersWindow.AdapterSelection y)
            {
                if (x.id != y.id)
                    return false;
                if (x.android != y.android)
                    return false;
                if (x.ios != y.ios)
                    return false;
                return true;
            }

            public int GetHashCode(AdaptersWindow.AdapterSelection obj)
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

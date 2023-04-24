using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Chartboost.Editor.Adapters.Comparers;
using Chartboost.Editor.Adapters.Serialization;
using Newtonsoft.Json;

namespace Chartboost.Editor.Adapters
{
    public partial class AdaptersWindow
    {
        private static void Refresh(bool ignore = true)
        {
            if (!Application.isBatchMode)
                Instance.rootVisualElement.Clear();
            AdapterDataSource.Update();
            Initialize();
            if (!Application.isBatchMode|| !ignore)
                EditorUtility.DisplayDialog("Chartboost Mediation", "Adapter update completed.", "ok");
        }
        
        public static bool CheckForChanges()
        {
            var same = new DictionaryComparer<string, AdapterSelection>(new SelectedVersionsComparer()).Equals(UserSelectedVersions, SavedVersions);

            if (Application.isBatchMode)
                return same;

            var root = Instance.rootVisualElement;
            if (!same && _saveButton != null && !root.Contains(_saveButton))
                root.Add(_saveButton);
            else
                _saveButton?.RemoveFromHierarchy();
            return same;
        }

        public static void AddNewNetworks()
        {
            foreach (var network in PartnerSDKVersions)
            {
                var id = network.Key;
                if (UserSelectedVersions.ContainsKey(id)) 
                    continue;
                
                const int latestVersion = 1;
                const int unselected = 0;
                
                var selection = new AdapterSelection(id);
                var androidVersions = network.Value.android;
                if (androidVersions.Length > unselected)
                    selection.android = androidVersions[latestVersion];
                var iosVersions = network.Value.ios;
                if (iosVersions.Length > unselected)
                    selection.ios = iosVersions[latestVersion];
                UserSelectedVersions.Add(id, selection);
            }
            
            var package = Utilities.FindPackage(Constants.ChartboostMediationPackageName);
            MediationSelection = package.version;
            GenerateChartboostMediationDependency();
            GenerateDependenciesFromSelections();
        }

        public static List<AdapterChange> UpgradeAndroidSelectionsToLatest() => UpgradePlatformToLatest(Platform.Android);

        public static List<AdapterChange> UpgradeIOSSelectionsToLatest() => UpgradePlatformToLatest(Platform.IOS);

        public static List<AdapterChange> UpgradeSelectionsToLatest() => UpgradePlatformToLatest(Platform.Android | Platform.IOS);

        private static List<AdapterChange> UpgradePlatformToLatest(Platform platform)
        {
            var selectionChanges = new List<AdapterChange>();
            if (!WarningDialog())
                return selectionChanges;

            var currentSelections = UserSelectedVersions.ToDictionary(kv => kv.Key, kv => kv.Value);
            
            foreach (var selection in currentSelections)
            {
                var adapterId = selection.Key;
                
                var updateAndroid = new Action(() => UpdateSelection(PartnerSDKVersions[adapterId].android, selectionChanges,  adapterId, selection.Value.android, Platform.Android));;
                var updateIOS = new Action(() => UpdateSelection(PartnerSDKVersions[adapterId].ios, selectionChanges, adapterId, selection.Value.ios, Platform.IOS));

                switch (platform)
                {
                    case Platform.Android | Platform.IOS:
                        updateAndroid();
                        updateIOS();
                        break;
                    case Platform.Android:
                        updateAndroid();
                        break;
                    default:
                        updateIOS();
                        break;
                }
            }

            NoChangesDialog();
            return selectionChanges;
        }

        private static bool WarningDialog()
        {
            var cancel = false;
            if (!Application.isBatchMode) { 
                cancel = EditorUtility.DisplayDialog(
                    "Chartboost Mediation", 
                    "Doing this will update all of your selected adapters to their latest version, do you wish to continue?", "Yes", "No");
            }

            return cancel;
        }

        private static void NoChangesDialog()
        {
            if (CheckForChanges() && !Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    "Chartboost Mediation", 
                    "No adapters updated, everything is already up to date!\n\n Do you think this is incorrect? Try using the refresh button.",
                    "Ok");
            }
        }

        private static void UpdateSelection(IReadOnlyList<string> versions, List<AdapterChange> selectionChanges,  string id, string startValue, Platform platform)
        {
            if (startValue.Equals(Constants.Unselected))
                return;
            
            if (versions.Count <= 0)
                return;
            var latest = versions[1];
            
            Debug.Log($"[Adapters] Current: {startValue} {JsonConvert.SerializeObject(versions)}");

            if (latest == startValue)
                return;
                
            switch (platform)
            {
                case Platform.Android:
                    selectionChanges.Add(new AdapterChange(id, Platform.Android, startValue, latest));
                    UserSelectedVersions[id].android = latest;
                    Console.WriteLine($"[Adapters] Setting {id} to {UserSelectedVersions[id].android}");
                    var androidDropdown = UserSelectedVersions[id].androidDropdown;
                    if (androidDropdown != null)
                        androidDropdown.text = latest;
                    break;
                case Platform.IOS:
                    selectionChanges.Add(new AdapterChange(id, Platform.IOS, startValue, latest));
                    UserSelectedVersions[id].ios = latest;
                    Console.WriteLine($"[Adapters] Setting {id} to {UserSelectedVersions[id].ios}");
                    var iosDropdown = UserSelectedVersions[id].iosDropdown;
                    if (iosDropdown != null)
                        iosDropdown.text = latest;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
            }
        }
    }
}

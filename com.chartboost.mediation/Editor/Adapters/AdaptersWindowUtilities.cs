using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Chartboost.Editor.Adapters.Comparers;
using Chartboost.Editor.Adapters.Serialization;

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
        
        private static bool CheckForChanges()
        {
            var same = new DictionaryComparer<string, AdapterSelection>(new SelectedVersionsComparer()).Equals(UserSelectedVersions, SavedVersions);

            if (Application.isBatchMode)
                return same;

            var root = Instance.rootVisualElement;
            switch (same)
            {
                case false when _saveButton != null && !root.Contains(_saveButton):
                    root.Add(_saveButton);
                    break;
                case true:
                    _saveButton?.RemoveFromHierarchy();
                    break;
            }
            return same;
        }
        
        public static List<AdapterSelection> AddNewNetworks(Platform platform, Func<string, Dictionary<string, AdapterSelection>, bool> condition)
        {
            var newNetworks = new List<AdapterSelection>();

            foreach (var network in PartnerSDKVersions)
            {
                var id = network.Key;
                if (condition(id, UserSelectedVersions))
                    continue;
                
                const int latestVersion = 1;
                const int unselected = 0;

                var selection = !UserSelectedVersions.ContainsKey(id) ? new AdapterSelection(id) : UserSelectedVersions[id];
                 
                var addAndroid = new Action(() => {
                    var androidVersions = network.Value.android;
                    if (androidVersions.Length > unselected)
                        selection.android = androidVersions[latestVersion];
                });

                var addIOS = new Action(() => {
                    var iosVersions = network.Value.ios;
                    if (iosVersions.Length > unselected)
                        selection.ios = iosVersions[latestVersion];
                });

                switch (platform)
                {
                    case Platform.Android | Platform.IOS:
                        addAndroid();
                        addIOS();
                        break;
                    case Platform.Android:
                        addAndroid();
                        break;
                    default:
                        addIOS();
                        break;
                }

                newNetworks.Add(selection);
                UserSelectedVersions[id] = selection;
            }
            
            var package = Utilities.FindPackage(Constants.ChartboostMediationPackageName);
            MediationSelection = package.version;
            GenerateChartboostMediationDependency();
            GenerateDependenciesFromSelections();
            return newNetworks;
        }
        
        public static List<AdapterChange> UpgradePlatformToLatest(Platform platform)
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
            if (Application.isBatchMode)
                return true;
            
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
            
            if (latest == startValue)
                return;
                
            switch (platform)
            {
                case Platform.Android:
                    selectionChanges.Add(new AdapterChange(id, Platform.Android, startValue, latest));
                    UserSelectedVersions[id].android = latest;
                    var androidDropdown = UserSelectedVersions[id].androidDropdown;
                    if (androidDropdown != null)
                        androidDropdown.text = latest;
                    break;
                case Platform.IOS:
                    selectionChanges.Add(new AdapterChange(id, Platform.IOS, startValue, latest));
                    UserSelectedVersions[id].ios = latest;
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

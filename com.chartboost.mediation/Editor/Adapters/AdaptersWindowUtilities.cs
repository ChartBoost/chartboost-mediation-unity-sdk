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
                if (startValue.Equals(Constants.Unselected))
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
    }
}

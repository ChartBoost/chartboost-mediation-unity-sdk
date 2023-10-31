#if !NO_ADAPTERS_WINDOW
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chartboost.Editor.EditorWindows.Adapters.Serialization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor.EditorWindows.Adapters
{
    public partial class AdaptersWindow
    {
        /// <summary>
        /// Loads partner versions and current adapter selections.
        /// </summary>
        public static void LoadSelections()
        {
            if (AdapterDataSource.LoadedAdapters.adapters != null)
                foreach (var partnerAdapter in AdapterDataSource.LoadedAdapters.adapters)
                    PartnerSDKVersions[partnerAdapter.id] = new PartnerVersions(partnerAdapter.android.versions, partnerAdapter.ios.versions);
            
            if (!PathToSelectionsFile.FileExist())
                return;

            var jsonContents = PathToSelectionsFile.ReadAllText();
            var selections = JsonConvert.DeserializeObject<SDKSelections>(jsonContents);

            if (selections == null)
            {
                MediationSelection = ChartboostMediationPackage.version;
                UpdateSavedVersions();
                return;
            }

            MediationSelection = selections.mediationVersion;

            if (selections.adapterSelections != null)
                foreach (var versionSelection in selections.adapterSelections)
                    UserSelectedVersions[versionSelection.id] = versionSelection;

            UpdateSavedVersions();
        }
        
        /// <summary>
        /// Saves current user selections.
        /// </summary>
        public static void SaveSelections()
        {
            var allSelections = UserSelectedVersions.Values.Where(x => x.android != Unselected || x.ios != Unselected).ToArray();

            var sdkSelections = new SDKSelections
            {
                adapterSelections = allSelections,
                mediationVersion = MediationSelection
            };

            var selectionsJson = JsonConvert.SerializeObject(sdkSelections, Formatting.Indented);

            PathToPackageGeneratedFiles.DirectoryCreate();
            PathToEditorInGeneratedFiles.DirectoryCreate();
            
            if (UserSelectedVersions.Count <= 0 && string.IsNullOrEmpty(MediationSelection))
                PathToSelectionsFile.DeleteFileWithMeta();
            else
                PathToSelectionsFile.FileCreate(selectionsJson);
            
            if (!Application.isBatchMode && _saveButton != null)
                _saveButton.RemoveFromHierarchy();
            GenerateDependenciesFromSelections();
            UpdateSavedVersions();
        }

        private static void GenerateDependenciesFromSelections()
        {
            var adaptersTemplate = Resources.Load<TextAsset>(AdapterTemplate);
            
            if (adaptersTemplate == null)
            {
                Debug.LogError("[Chartboost Mediation] Unable to create dependencies from selection, TemplateAdapter.xml could not be found.");
                return;
            }

            var defaultTemplateContents = TextAssetToList(adaptersTemplate);
            var adapters = AdapterDataSource.LoadedAdapters.adapters;

            if (adapters == null)
            {
                Debug.LogError("[Chartboost Mediation] Unable to Load Adapters information, make sure you have an active internet connection");
                return;
            }


            
            foreach (var adapter in adapters)
            {
                if (UserSelectedVersions.ContainsKey(adapter.id)) 
                    continue;
                
                var pathToAdapter = Path.Combine(PathToAdaptersDirectory, $"{adapter.name.RemoveWhitespace()}Dependencies.xml");
                pathToAdapter.DeleteFileWithMeta();
            }

            if (UserSelectedVersions.Count <= 0)
            {
                PathToAdaptersDirectory.DeleteDirectoryWithMeta();
                
                if (!PathToSelectionsFile.FileExist() && !PathToMainDependency.FileExist())
                    PathToEditorInGeneratedFiles.DeleteDirectoryWithMeta();
            }
            
            foreach (var selection in UserSelectedVersions)
            {
                var template = new List<string>(defaultTemplateContents);
                var adapter = adapters.First(x => x.id == selection.Key);
                var adapterId = adapter.id;
                var pathToAdapter = Path.Combine(PathToAdaptersDirectory, $"{adapter.name.RemoveWhitespace()}Dependencies.xml");
                
                var androidAdapterIndexInTemplate = template.FindIndex(x => x.Contains(AndroidAdapterInTemplate));
                var extraDependenciesStartPoint = template.FindIndex(x => x.Contains(AndroidDependenciesInTemplate));
                var androidSDKVersion = selection.Value.android;
                var androidSelected = androidSDKVersion != Unselected;
                
                // Android SDK Adapter Version
                if (androidSelected)
                {
                    // handling IronSource SDK versioning quirk
                    if (adapterId.Equals(IronSource) && androidSDKVersion[androidSDKVersion.Length - 1] == '0')
                        androidSDKVersion = androidSDKVersion.Remove(androidSDKVersion.Length - 2, 2);
                    
                    // Set Adapter versioning
                    template[androidAdapterIndexInTemplate] =  $"        <androidPackage spec=\"{adapter.android.adapter}:4.{androidSDKVersion}+@aar\"/>";
                    
                    // Set all remaining dependencies
                    var versionSet = GetAdapterVersionSet(androidSDKVersion, adapter.android.versions);
                    if (versionSet.dependencies != null && versionSet.dependencies.Count > 0)
                    {
                        var dependenciesToAdd = new List<string>();
                        
                        foreach (var dependency in versionSet.dependencies)
                        {
                            var hasVersionNumber = NeedsVersionNumber.IsMatch(dependency);

                            var toInsert = dependency;
                            if (!hasVersionNumber)
                                toInsert = $"{dependency}:{androidSDKVersion}";
                            dependenciesToAdd.Add($"        <androidPackage spec=\"{toInsert}\"/>");
                        }

                        // Android Extra Dependencies
                        template.RemoveAt(extraDependenciesStartPoint);
                        template.InsertRange(extraDependenciesStartPoint, dependenciesToAdd);
                    }
                }
                else
                {
                    var message = $"        <!-- Android {adapter.name} Adapter has not been selected. Choose a version to fill this field -->";
                    template[androidAdapterIndexInTemplate] = message;
                    template[extraDependenciesStartPoint] = message;
                }
            
                // Android Extra Repos
                var androidRepos = adapter.android.repositories;
                var repositoriesIndexStartPoint = template.FindIndex(x => x.Contains(AndroidRepositoriesInTemplate));
                
                if (androidSelected && androidRepos != null && androidRepos.Length > 0)
                {
                    var repos = new List<string>();
                    repos.Add($"        <!-- {adapter.name} Android Repositories -->");
                    repos.Add("        <repositories>");
                    repos.AddRange(androidRepos.Select(repo => $"          <repository>{repo}</repository>"));
                    repos.Add("        </repositories>");
                    template.InsertRange(repositoriesIndexStartPoint, repos);
                    repositoriesIndexStartPoint += repos.Count;
                    
                    template.RemoveAt(repositoriesIndexStartPoint);
                }
                else
                    template.RemoveRange(repositoriesIndexStartPoint - 1, 2);
                
                var iosAdapterIndexInTemplate = template.FindIndex(x => x.Contains(IOSAdapterInTemplate));
                var iosSDKIndexInTemplate = template.FindIndex(x => x.Contains(IOSDependenciesInTemplate));
                
                var iosSDKVersion = selection.Value.ios;
                if (iosSDKVersion != Unselected)
                {
                    var versionSet = GetAdapterVersionSet(iosSDKVersion, adapter.ios.versions);
                    template[iosAdapterIndexInTemplate] = $"        <iosPod name=\"{adapter.ios.adapter}\" version=\"~> 4.{iosSDKVersion}\"/>";
                    
                    if (versionSet.dependencies != null && versionSet.dependencies.Count > 0)
                    {
                        var dependenciesToAdd = new List<string>();
                        foreach (var dependency in versionSet.dependencies)
                            dependenciesToAdd.Add($"        <iosPod name=\"{dependency}\" version=\"~> {iosSDKVersion}\" addToAllTargets=\"{versionSet.allTargets.ToString().ToLower()}\"/>");

                        template.RemoveAt(iosSDKIndexInTemplate);
                        template.InsertRange(iosSDKIndexInTemplate, dependenciesToAdd);
                    }
                }
                else
                {
                    var message = $"        <!-- IOS {adapter.name} Adapter has not been selected. Choose a version to fill this field -->";
                    template[iosAdapterIndexInTemplate] = message;
                    template[iosSDKIndexInTemplate] = message;
                }
            
                PathToPackageGeneratedFiles.DirectoryCreate();
                PathToEditorInGeneratedFiles.DirectoryCreate();
                PathToAdaptersDirectory.DirectoryCreate();
                pathToAdapter.FileCreate(template);
            }
            
            AssetDatabase.Refresh();

            AdapterVersion GetAdapterVersionSet(string sdkVersion, AdapterVersion[] allVersions)
            {
                var versionSet = new AdapterVersion();
                foreach (var version in allVersions)
                {
                    versionSet = version;
                    if (versionSet.versions.Find(x => x.Contains(sdkVersion)) != null)
                        return versionSet;
                }
                return versionSet;
            }
        }

        private static void GenerateChartboostMediationDependency()
        {
            var mainTemplate = Resources.Load<TextAsset>(MainTemplate);
            
            if (mainTemplate == null)
            {
                Debug.LogError("[Chartboost Mediation] Unable to create dependencies for Chartboost Mediation, TemplateChartboostMediation.xml could not be found.");
                return;
            }

            var defaultTemplateContents = TextAssetToList(mainTemplate);
            var androidVersionIndex = defaultTemplateContents.FindIndex(x => x.Contains(AndroidVersionInMainTemplate));
            var iosVersionIndex = defaultTemplateContents.FindIndex(x => x.Contains(IOSVersionInMainTemplate));
            
            var androidOptimisticVersion = MediationSelection.Remove(MediationSelection.Length - 2);
            defaultTemplateContents[androidVersionIndex] = defaultTemplateContents[androidVersionIndex].Replace(AndroidVersionInMainTemplate, androidOptimisticVersion);
            
            var lastDigit = MediationSelection.Length - 1;
            var iosOptimisticVersion = MediationSelection.Remove(lastDigit).Insert(lastDigit, "0");
            defaultTemplateContents[iosVersionIndex] = defaultTemplateContents[iosVersionIndex].Replace(IOSVersionInMainTemplate, iosOptimisticVersion);

            PathToPackageGeneratedFiles.DirectoryCreate();
            PathToEditorInGeneratedFiles.DirectoryCreate();
            PathToMainDependency.FileCreate(defaultTemplateContents);
            SaveSelections();
            AssetDatabase.Refresh();
        }

        private static void UpdateSavedVersions()
        {
            SavedVersions = UserSelectedVersions.ToDictionary(k => k.Key, v =>
            {
                var newSelection = new AdapterSelection(v.Key.ToString())
                {
                    android = v.Value.android,
                    ios = v.Value.ios
                };
                return newSelection;
            });
        }
        
        private static List<string> TextAssetToList(TextAsset ta) {
            return new List<string>(ta.text.Split('\n'));
        }
    }
}
#endif

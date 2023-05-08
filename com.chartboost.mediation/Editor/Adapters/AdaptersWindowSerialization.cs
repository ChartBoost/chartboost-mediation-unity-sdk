using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Chartboost.Editor.Adapters.Serialization;

namespace Chartboost.Editor.Adapters
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
            
            if (!Constants.PathToSelectionsFile.FileExist())
                return;

            var jsonContents = Constants.PathToSelectionsFile.ReadAllText();
            var selections = JsonConvert.DeserializeObject<SDKSelections>(jsonContents);

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
            var allSelections = UserSelectedVersions.Values.Where(x => x.android != Constants.Unselected || x.ios != Constants.Unselected).ToArray();

            var sdkSelections = new SDKSelections
            {
                adapterSelections = allSelections,
                mediationVersion = MediationSelection
            };

            var selectionsJson = JsonConvert.SerializeObject(sdkSelections, Formatting.Indented);

            Constants.PathToPackageGeneratedFiles.DirectoryCreate();
            Constants.PathToEditorInGeneratedFiles.DirectoryCreate();
            
            if (UserSelectedVersions.Count <= 0 && string.IsNullOrEmpty(MediationSelection))
                Constants.PathToSelectionsFile.DeleteFileWithMeta();
            else
                Constants.PathToSelectionsFile.FileCreate(selectionsJson);
            
            if (!Application.isBatchMode && _saveButton != null)
                _saveButton.RemoveFromHierarchy();
            GenerateDependenciesFromSelections();
            UpdateSavedVersions();
        }

        private static void GenerateDependenciesFromSelections()
        {
            if (!Constants.PathToAdapterTemplate.FileExist())
            {
                Debug.LogError("[Chartboost Mediation] Unable to create dependencies from selection, TemplateAdapter.xml could not be found.");
                return;
            }

            var defaultTemplateContents = Constants.PathToAdapterTemplate.ReadAllLines().ToList();
            var adapters = AdapterDataSource.LoadedAdapters.adapters;

            if (adapters == null)
            {
                Debug.LogError("[Chartboost Mediation] Unable to Load Adapters information, make sure you have an active internet connection");
                return;
            }

            const string androidAdapterInTemplate = "%ANDROID_ADAPTER%";
            const string iosAdapterInTemplate = "%IOS_ADAPTER%";
            const string androidSDKInTemplate = "%ANDROID_SDK%";
            const string androidRepositoriesInTemplate = "%REPOSITORIES%";
            const string androidDependenciesInTemplate = "%DEPENDENCIES%";
            const string iosSDKInTemplate = "%IOS_SDK%";
            const string iosAdapterVersionInTemplate = "%IOS_CBMA_VERSION%";
            const string iosSDKVersionInTemplate = "%IOS_SDK_VERSION%";
            const string iosAllTargetsInTemplate = "%ALL_TARGETS%";
            
            foreach (var adapter in adapters)
            {
                if (UserSelectedVersions.ContainsKey(adapter.id)) 
                    continue;
                
                var pathToAdapter = Path.Combine(Constants.PathToAdaptersDirectory, $"{adapter.name.RemoveWhitespace()}Dependencies.xml");
                pathToAdapter.DeleteFileWithMeta();
            }

            if (UserSelectedVersions.Count <= 0)
            {
                Constants.PathToAdaptersDirectory.DeleteDirectoryWithMeta();
                
                if (!Constants.PathToSelectionsFile.FileExist() && !Constants.PathToMainDependency.FileExist())
                    Constants.PathToEditorInGeneratedFiles.DeleteDirectoryWithMeta();
            }

            foreach (var selection in UserSelectedVersions)
            {
                var template = new List<string>(defaultTemplateContents);
                var adapter = adapters.First(x => x.id == selection.Key);
                var adapterId = adapter.id;
                var pathToAdapter = Path.Combine(Constants.PathToAdaptersDirectory, $"{adapter.name.RemoveWhitespace()}Dependencies.xml");
                
                var androidAdapterIndexInTemplate = template.FindIndex(x => x.Contains(androidAdapterInTemplate));
                var androidSDKIndexInTemplate = template.FindIndex(x => x.Contains(androidSDKInTemplate));

                var androidSelected = selection.Value.android != Constants.Unselected;
                
                // Android SDK Adapter Version
                if (androidSelected)
                {
                    string sdkVersion;
                    var adapterVersion = sdkVersion = selection.Value.android;
                    var sdk = adapter.android.sdk;
                  
                    // handling IronSource SDK versioning quirk
                    if (adapterId.Equals(Constants.IronSource) && sdkVersion[sdkVersion.Length - 1] == '0')
                        sdkVersion = sdkVersion.Remove(sdkVersion.Length - 2, 2);
                    
                    var androidDependency = $"{adapter.android.adapter}:4.{adapterVersion}+@aar";
                    
                    if (adapterId.Equals(Constants.InMobi))
                    {
                        var version = new Version(sdkVersion);
                        if (version >= Constants.InMobiNewSDK)
                            sdk = $"{sdk}-kotlin";
                    }

                    var androidSDKDependency = $"{sdk}:{sdkVersion}";
                    
                    template[androidAdapterIndexInTemplate] = template[androidAdapterIndexInTemplate].Replace(androidAdapterInTemplate, androidDependency);
                    
                    if (!string.IsNullOrEmpty(sdk))
                        template[androidSDKIndexInTemplate] = template[androidSDKIndexInTemplate].Replace(androidSDKInTemplate, androidSDKDependency);
                    else
                        template[androidSDKIndexInTemplate] = $"        <!-- {adapter.name} does not provide a single SDK. -->";
                }
                else
                {
                    var message = $"        <!-- Android {adapter.name} Adapter has not been selected. Choose a version to fill this field -->";
                    template[androidAdapterIndexInTemplate] = message;
                    template[androidSDKIndexInTemplate] = message;
                }

                // Android Extra Repos
                var androidRepos = adapter.android.repositories;
                var extraDependencies = adapter.android.dependencies;

                var repositoriesIndexStartPoint = template.FindIndex(x => x.Contains(androidRepositoriesInTemplate));

                if (androidSelected && androidRepos != null && androidRepos.Length > 0)
                {
                    var repos = new List<string>();
                    repos.Add($"        <!-- {adapter.name} Android Repositories -->");
                    repos.Add("        <repositories>");
                    repos.AddRange(androidRepos.Select(repo => $"          <repository>{repo}</repository>"));
                    repos.Add("        </repositories>");
                    if (extraDependencies != null && extraDependencies.Length > 0)
                        repos.Add("");
                    template.InsertRange(repositoriesIndexStartPoint, repos);
                    repositoriesIndexStartPoint += repos.Count;
                }
                
                if (repositoriesIndexStartPoint >= 0)
                    template.RemoveRange(repositoriesIndexStartPoint, 2);

                // Android Extra Dependencies
                var extraDependenciesStartPoint = template.FindIndex(x => x.Contains(androidDependenciesInTemplate));

                if (androidSelected && extraDependencies != null && extraDependencies.Length > 0)
                {
                    var extra = new List<string>();
                    foreach (var dependency in extraDependencies)
                    {
                        // handling Mintegral quirk
                        var formatted = adapterId.Equals(Constants.Mintegral) ? $"{dependency}:{selection.Value.android}" : dependency;
                        extra.Add($"        <androidPackage spec=\"{formatted}\"/>");
                    }
                    template.InsertRange(extraDependenciesStartPoint, extra);
                    extraDependenciesStartPoint += extra.Count;
                }
                
                if (extraDependenciesStartPoint >= 0)
                    template.RemoveAt(extraDependenciesStartPoint);

                var iosAdapterIndexInTemplate = template.FindIndex(x => x.Contains(iosAdapterInTemplate));
                var iosSDKIndexInTemplate = template.FindIndex(x => x.Contains(iosSDKInTemplate));

                var iosSDKVersion = selection.Value.ios;
                if (iosSDKVersion != Constants.Unselected)
                {
                    var iosAdapterVersion = $"4.{iosSDKVersion}";
                    template[iosAdapterIndexInTemplate] = template[iosAdapterIndexInTemplate].Replace(iosAdapterInTemplate, adapter.ios.adapter).Replace(iosAdapterVersionInTemplate, iosAdapterVersion);

                    var sdkNaming = adapter.ios.sdk;
                    
                    if (adapterId.Equals(Constants.InMobi))
                    {
                        var version = new Version(iosSDKVersion);
                        if (version >= Constants.InMobiNewSDK)
                            sdkNaming = $"{sdkNaming}-Swift";
                    }
                    
                    template[iosSDKIndexInTemplate] = template[iosSDKIndexInTemplate].Replace(iosSDKInTemplate, sdkNaming).Replace(iosSDKVersionInTemplate, iosSDKVersion).Replace(iosAllTargetsInTemplate, adapter.ios.allTargets.ToString().ToLower());
                }
                else
                {
                    var message = $"        <!-- IOS {adapter.name} Adapter has not been selected. Choose a version to fill this field -->";
                    template[iosAdapterIndexInTemplate] = message;
                    template[iosSDKIndexInTemplate] = message;
                }

                Constants.PathToPackageGeneratedFiles.DirectoryCreate();
                Constants.PathToEditorInGeneratedFiles.DirectoryCreate();
                Constants.PathToAdaptersDirectory.DirectoryCreate();
                pathToAdapter.FileCreate(template);
            }
            
            AssetDatabase.Refresh();
        }

        private static void GenerateChartboostMediationDependency()
        {
            if (!Constants.PathToMainTemplate.FileExist())
            {
                Debug.LogError("[Chartboost Mediation] Unable to create dependencies for Chartboost Mediation, TemplateChartboostMediation.xml could not be found.");
                return;
            }
            
            var defaultTemplateContents = Constants.PathToMainTemplate.ReadAllLines().ToList();
            var androidVersionIndex = defaultTemplateContents.FindIndex(x => x.Contains(Constants.AndroidVersionInMainTemplate));
            var iosVersionIndex = defaultTemplateContents.FindIndex(x => x.Contains(Constants.IOSVersionInMainTemplate));

            var optimisticVersion = MediationSelection.Remove(MediationSelection.Length - 2);
            defaultTemplateContents[androidVersionIndex] = defaultTemplateContents[androidVersionIndex].Replace(Constants.AndroidVersionInMainTemplate, optimisticVersion);
            defaultTemplateContents[iosVersionIndex] = defaultTemplateContents[iosVersionIndex].Replace(Constants.IOSVersionInMainTemplate, optimisticVersion);

            Constants.PathToPackageGeneratedFiles.DirectoryCreate();
            Constants.PathToEditorInGeneratedFiles.DirectoryCreate();
            Constants.PathToMainDependency.FileCreate(defaultTemplateContents);
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
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UIElements;
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
        
        #nullable enable
        [Serializable]
        public struct SDKSelections
        {
            public string? mediationVersion;

            public AdapterSelection[]? adapterSelections;
        }
        #nullable disable

        /// <summary>
        /// Contains adapter selected versions in the project, used to update, modify, existing selections. 
        /// </summary>
        [Serializable]
        public class AdapterSelection
        {
            public string id;
            public string android = Unselected;
            public string ios = Unselected;
            
            [NonSerialized]
            public ToolbarMenu androidDropdown;
            [NonSerialized]
            public ToolbarMenu iosDropdown;

            public AdapterSelection(string id)
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

        private static string PathToPackageGeneratedFiles => Path.Combine(Application.dataPath, "com.chartboost.mediation");
        private static string PathToEditorInGeneratedFiles => Path.Combine(PathToPackageGeneratedFiles, "Editor");
        private static string PathToAdaptersDirectory => Path.Combine(PathToEditorInGeneratedFiles, "Adapters");

        private static string PathToMainDependency => Path.Combine(PathToEditorInGeneratedFiles, "ChartboostMediationDependencies.xml");
        
        public static void LoadSelections()
        {
            const string selectionsJson = "Assets/com.chartboost.mediation/Editor/selections.json";
            
            if (!File.Exists(selectionsJson))
                return;

            var jsonContents = File.ReadAllText(selectionsJson);

            var selections = JsonConvert.DeserializeObject<SDKSelections>(jsonContents);

            MediationSelection = selections.mediationVersion;

            foreach (var versionSelection in selections.adapterSelections)
                UserSelectedVersions[versionSelection.id] = versionSelection;

            UpdateSavedVersions();
        }
        
        public static void SaveSelections()
        {
            var selectionsFile = Path.Combine(PathToEditorInGeneratedFiles, "selections.json");

            var allSelections = UserSelectedVersions.Values.Where(x => x.android != Unselected || x.ios != Unselected).ToArray();

            var sdkSelections = new SDKSelections
            {
                adapterSelections = allSelections,
                mediationVersion = MediationSelection
            };

            var selectionsJson = JsonConvert.SerializeObject(sdkSelections, Formatting.Indented);

            if (!Directory.Exists(PathToPackageGeneratedFiles))
                Directory.CreateDirectory(PathToPackageGeneratedFiles);

            if (!Directory.Exists(PathToEditorInGeneratedFiles))
                Directory.CreateDirectory(PathToEditorInGeneratedFiles);
            
            if (UserSelectedVersions.Count <= 0 && string.IsNullOrEmpty(MediationSelection))
                DeleteFileWithMeta(selectionsFile);
            else
                File.WriteAllText(selectionsFile, selectionsJson);
            
            if (!Application.isBatchMode)
                _saveButton.RemoveFromHierarchy();
            GenerateDependenciesFromSelections();
            AssetDatabase.Refresh();
            UpdateSavedVersions();
        }

        public static void GenerateDependenciesFromSelections()
        {
            const string templatePath = "Packages/com.chartboost.mediation/Editor/Adapters/DependencyTemplate.xml";

            if (!File.Exists(templatePath))
            {
                Debug.LogError("[Chartboost Mediation] Unable to create dependencies from selection, DependencyTemplate.xml could not be found.");
                return;
            }

            var defaultTemplateContents = File.ReadAllLines(templatePath).ToList();

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
                
                var pathToAdapter = Path.Combine(PathToAdaptersDirectory, $"{RemoveWhitespace(adapter.name)}Dependencies.xml");
                DeleteFileWithMeta(pathToAdapter);
            }

            if (UserSelectedVersions.Count <= 0)
            {
                DeleteDirectoryWithMeta(PathToAdaptersDirectory);
                DeleteDirectoryWithMeta(PathToEditorInGeneratedFiles);
            }

            foreach (var selection in UserSelectedVersions)
            {
                var template = new List<string>(defaultTemplateContents);
                var adapter = adapters.First(x => x.id == selection.Key);
                var pathToAdapter = Path.Combine(PathToAdaptersDirectory, $"{RemoveWhitespace(adapter.name)}Dependencies.xml");
                
                var androidAdapterIndexInTemplate = template.FindIndex(x => x.Contains(androidAdapterInTemplate));
                var androidSDKIndexInTemplate = template.FindIndex(x => x.Contains(androidSDKInTemplate));

                var androidSelected = selection.Value.android != Unselected;
                
                // Android SDK Adapter Version
                if (androidSelected)
                {
                    var androidVersion = selection.Value.android;
                    var sdk = adapter.android.sdk;
                    var androidDependency = $"{adapter.android.adapter}:4.{androidVersion}+@aar";
                    var androidSDKDependency = $"{sdk}:{androidVersion}";
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
                var repositoriesIndexStartPoint = template.FindIndex(x => x.Contains(androidRepositoriesInTemplate));

                if (androidSelected && androidRepos != null && androidRepos.Length > 0)
                {
                    var repos = new List<string>();
                    repos.Add($"        <!-- {adapter.name} Android Repositories -->");
                    repos.Add("        <repositories>");
                    repos.AddRange(androidRepos.Select(repo => $"          <repository>{repo}</repository>"));
                    repos.Add("        </repositories>");
                    repos.Add("");
                    template.InsertRange(repositoriesIndexStartPoint, repos);
                    repositoriesIndexStartPoint += repos.Count;
                }
                
                if (repositoriesIndexStartPoint >= 0)
                    template.RemoveRange(repositoriesIndexStartPoint, 2);

                // Android Extra Dependencies
                var extraDependencies = adapter.android.dependencies;
                var extraDependenciesStartPoint = template.FindIndex(x => x.Contains(androidDependenciesInTemplate));

                if (androidSelected && extraDependencies != null && extraDependencies.Length > 0)
                {
                    var extra = new List<string>();
                    foreach (var dependency in extraDependencies)
                    {
                        var formatted = adapter.id == "mintegral" ? $"{dependency}:{selection.Value.android}" : dependency;
                        extra.Add($"        <androidPackage spec=\"{formatted}\"/>");
                    }
                    extra.Add("");
                    template.InsertRange(extraDependenciesStartPoint, extra);
                    extraDependenciesStartPoint += extra.Count;
                }
                
                if (extraDependenciesStartPoint >= 0)
                    template.RemoveAt(extraDependenciesStartPoint);

                var iosAdapterIndexInTemplate = template.FindIndex(x => x.Contains(iosAdapterInTemplate));
                var iosSDKIndexInTemplate = template.FindIndex(x => x.Contains(iosSDKInTemplate));

                if (selection.Value.ios != Unselected)
                {
                    var iosVersion = selection.Value.ios;
                    var iosDependency = $"4.{iosVersion}";
                    template[iosAdapterIndexInTemplate] = template[iosAdapterIndexInTemplate].Replace(iosAdapterInTemplate, adapter.ios.adapter).Replace(iosAdapterVersionInTemplate, iosDependency);
                    template[iosSDKIndexInTemplate] = template[iosSDKIndexInTemplate].Replace(iosSDKInTemplate, adapter.ios.sdk).Replace(iosSDKVersionInTemplate, iosVersion).Replace(iosAllTargetsInTemplate, adapter.ios.allTargets.ToString().ToLower());
                }
                else
                {
                    var message = $"        <!-- IOS {adapter.name} Adapter has not been selected. Choose a version to fill this field -->";
                    template[iosAdapterIndexInTemplate] = message;
                    template[iosSDKIndexInTemplate] = message;
                }

                if (!Directory.Exists(PathToPackageGeneratedFiles))
                    Directory.CreateDirectory(PathToPackageGeneratedFiles);
                
                if (!Directory.Exists(PathToEditorInGeneratedFiles))
                    Directory.CreateDirectory(PathToEditorInGeneratedFiles);
                
                if (!Directory.Exists(PathToAdaptersDirectory))
                    Directory.CreateDirectory(PathToAdaptersDirectory);
                
                File.WriteAllLines(pathToAdapter, template);
            }
            
            AssetDatabase.Refresh();
        }

        public static void GenerateChartboostMediationDependency()
        {
            const string templatePath = "Packages/com.chartboost.mediation/Editor/Adapters/MediationTemplate.xml";
            const string androidVersionInTemplate = "%ANDROID_VERSION%";
            const string iosVersionInTemplate = "%IOS_VERSION%";
            
            if (!File.Exists(templatePath))
            {
                Debug.LogError("[Chartboost Mediation] Unable to create dependencies for Chartboost Mediation, MediationTemplate.xml could not be found.");
                return;
            }
            
            var defaultTemplateContents = File.ReadAllLines(templatePath).ToList();

            var androidVersionIndex = defaultTemplateContents.FindIndex(x => x.Contains(androidVersionInTemplate));
            var iosVersionIndex = defaultTemplateContents.FindIndex(x => x.Contains(iosVersionInTemplate));

            var optimisticVersion = MediationSelection.Remove(MediationSelection.Length - 2);

            defaultTemplateContents[androidVersionIndex] = defaultTemplateContents[androidVersionIndex].Replace(androidVersionInTemplate, optimisticVersion);
            defaultTemplateContents[iosVersionIndex] = defaultTemplateContents[iosVersionIndex].Replace(iosVersionInTemplate, optimisticVersion);

            File.WriteAllLines(PathToMainDependency, defaultTemplateContents);
            SaveSelections();
        }

        private static void DeleteFileWithMeta(string path) => DeleteWithFunc(File.Exists, File.Delete, path);

        private static void DeleteDirectoryWithMeta(string path) => DeleteWithFunc(Directory.Exists, Directory.Delete, path);

        private static void DeleteWithFunc(Func<string, bool> exist, Action<string> delete, string path)
        {
            if (exist(path))
                delete(path);
            var metaPath = $"{path}.meta";
            if (File.Exists(metaPath))
                File.Delete(metaPath);
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
        
        private static readonly Regex SWhitespace = new Regex(@"\s+");
        public static string RemoveWhitespace(string input) 
        {
            return SWhitespace.Replace(input, "");
        }
    }
}

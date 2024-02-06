using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Chartboost.Editor.EditorWindows.Adapters;
using Chartboost.Editor.EditorWindows.Adapters.Serialization;
using GooglePlayServices;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Editor
{
    public class Builder
    {
        // Mediation Specific Variables
        private const string PathToAdapterUpdates = "adapter_updates.txt";
        
        private static readonly string EOL = System.Environment.NewLine;
        private const string PathToMainTemplate = "Assets/Plugins/Android/mainTemplate.gradle";
        
        private const string MenuItemBuildAndroid = "Build/Android";
        private const string MenuItemBuildIOS = "Build/IOS";

        private const string DateFormat = "yyyy.M.dd";
        // if we have a passed version number, use that, otherwise generate one based off time
        private const string ReleaseCandidateRegex = @"\d+\.\d+\.\d+(\.?[A-Za-z0-9]+)*-rc\d+";
        // regex for live release builds.
        private const string VersionNumberRegex = @"\d+\.\d+\.\d+";
        private const string ArgPackageId = "package_id";
        private const string ArgRCVersion = "rc_version";
        private const string ArgPackageGitLocation = "package_git_location";
        private const string ArgPackageGitHubToken = "github_token";
        private const string ArgUpdatePackage = "update_package";
        private const string ArgPodRepos = "pod_repos";
        private const string ArgPrivateMavenRepository = "private_maven_repository";
        private const string ArgBuildName = "build_name";

        [MenuItem(MenuItemBuildAndroid)]
        public static void BuildAndroid()
        {
            UpdateAdapters(Platform.Android, AdaptersWindow.UpgradePlatformToLatest);
            ResolveAndroidDependencies();
            ParseCommandLineArgumentsToUnity(out var args);
            Build(BuildTargetGroup.Android, BuildTarget.Android, args);
        }

        [MenuItem(MenuItemBuildIOS)]
        public static void BuildIOS()
        {
            UpdateAdapters(Platform.IOS, AdaptersWindow.UpgradePlatformToLatest);
            ParseCommandLineArgumentsToUnity(out var args);
            Build(BuildTargetGroup.iOS, BuildTarget.iOS, args);
        }

        private static void UpdateAdapters(Platform platform, Func<Platform, List<AdapterChange>> upgrade)
        {
            var adapterUpdates = string.Empty;
            AdapterDataSource.Update();
            AdaptersWindow.LoadSelections();
            
            if(File.Exists(adapterUpdates))    
                File.Delete(adapterUpdates);
            
            var upgrades = upgrade(platform);
            if (upgrades.Count > 0)
            {
                adapterUpdates = $"Upgraded: \n {JsonConvert.SerializeObject(upgrades, Formatting.Indented)}";
                File.WriteAllText(PathToAdapterUpdates, adapterUpdates);
            }
            Log(upgrades.Count > 0 ? $"[Adapters] {adapterUpdates}" : "[Adapters] No Upgrades.");
            
            var newNetworks =  AdaptersWindow.AddNewNetworks(platform);
            if (newNetworks.Count > 0)
            {
                adapterUpdates = $"New Networks: \n {JsonConvert.SerializeObject(newNetworks, Formatting.Indented)}";
                File.AppendAllText(PathToAdapterUpdates, $"\n{adapterUpdates}");
            }
            Log(newNetworks.Count > 0 ? $"[Adapters] {adapterUpdates}" :  "[Adapters] No New Networks");
            
            AdaptersWindow.SaveSelections();
            
            var changed = AdaptersWindow.CheckChartboostMediationVersion();
            Log(changed ? $"[Adapters] Chartboost Mediation Version Has Been Updated" :  "[Adapters] Chartboost Mediation Version is Up to Date");
        }

        private static void ResolveAndroidDependencies()
        {
            PlayServicesResolver.ResolveSync(true);
            AssetDatabase.Refresh();
        }

        private static void Build(BuildTargetGroup targetGroup, BuildTarget target, Dictionary<string, string> args)
        {
            var now = DateTime.Now;
            var timeFormatted = now.ToString(DateFormat);
            var t = now - new DateTime(1970, 1, 1);
            var secondsSinceEpoch = (int)t.TotalSeconds;
            var isRcBuild = false;
            var isNightly = false;

            if (!args.TryGetValue(ArgBuildName, out var buildName))
            {
                if (Application.isBatchMode)
                    ExitEditorWithResult(BuildResult.Failed);
                else
                {
                    Debug.LogError("Cannot Build, Parameters Are Missing");
                    return;
                }
            }

            var packageId = args[ArgPackageId];
            
            if (args.TryGetValue(ArgRCVersion, out var rcVersionInput)) 
            {
                if (!string.IsNullOrEmpty(rcVersionInput))
                {
                    // Building Release Candidate
                    if (Regex.IsMatch(rcVersionInput, ReleaseCandidateRegex))
                    {
                        isRcBuild = true;

                        if (args.ContainsKey(ArgUpdatePackage) && args.TryGetValue(ArgPackageGitLocation, out var gitPackageLocation))
                        {
                            var chartboostMediationRcTag = $"{gitPackageLocation}#{rcVersionInput.Trim('"')}";
                            var request = Client.Add(chartboostMediationRcTag);
                            Log($"[Chartboost Canary Builder] Requesting Update of Package to -> {chartboostMediationRcTag}");
                            while (!request.IsCompleted)
                            {
                            }
                            var result = request.Result;
                            Log($"[Chartboost Canary Builder] Package Update Finished: Hash: {result.git.hash}, Revision: {result.git.revision}");
                            Client.Resolve();
                        }
                    }
                    // Building Live
                    else if (Regex.IsMatch(rcVersionInput, VersionNumberRegex) && args.ContainsKey(ArgUpdatePackage))
                    {
                        var chartboostMediationRcTag = $"{packageId}@{rcVersionInput.Trim('"')}";
                        var request = Client.Add(chartboostMediationRcTag);
                        Log($"[Chartboost Canary Builder] Requesting Update of Package to -> {chartboostMediationRcTag}");
                        while (!request.IsCompleted)
                        { }
                        var result = request.Result;
                        Log($"[Chartboost Canary Builder] Package Update Finished: Version: {result.version}");
                        Client.Resolve();
                    }
                    var versionNumber = Regex.Match(rcVersionInput, VersionNumberRegex);
                    PlayerSettings.bundleVersion = versionNumber.Value;
                }
                else
                {
                    Log($"[Chartboost Canary Builder] Version Input is null or empty.");
                    EditorApplication.Exit(101);
                }
            }
            // Nightly Build
            else
            {
                PlayerSettings.bundleVersion = timeFormatted;
                isNightly = true;
            }
            Debug.Log($"Is Nightly Build? {isNightly}");

            switch (target)
            {
                case BuildTarget.Android:
                    PlayerSettings.Android.bundleVersionCode = secondsSinceEpoch;
                    PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.IL2CPP);
                    if (isRcBuild || isNightly)
                    {
                        var privateRepository = args[ArgPrivateMavenRepository];
                        UpdateMainTemplate(privateRepository);
                    }
                    break;
                case BuildTarget.iOS:
                    PlayerSettings.iOS.buildNumber = secondsSinceEpoch.ToString();
                    PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.IL2CPP);
                    if (isRcBuild || isNightly)
                        UpdateCocoaPodRepos(isNightly);
                    break;
            }

            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(s => s.path).ToArray();
            var buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                target = target,
                targetGroup = targetGroup,
                locationPathName = buildName,
            };

            var bOptions = BuildOptions.None;
            bOptions |= BuildOptions.CompressWithLz4HC;
            bOptions |= BuildOptions.Development;

            if (!Application.isBatchMode)
                bOptions |= BuildOptions.ShowBuiltPlayer;
            
            buildOptions.options = bOptions;

            var chartboostMediation = FindPackage(packageId);

            if (chartboostMediation != null)
            {
                switch (chartboostMediation.source)
                {
                    case PackageSource.Git:
                        Log($"[Chartboost Mediation Builder] Building with Git Package: Hash:{chartboostMediation.git.hash}, Revision: {chartboostMediation.git.revision}");
                        break;
                    case PackageSource.Registry:
                        Log($"[Chartboost Mediation Builder] Building with npm Package: Name: {chartboostMediation.registry.name}, URL: {chartboostMediation.registry.url}");
                        break;
                }
            }
            else
                Log($"[Chartboost Mediation Builder] Package: {packageId} was not found");

            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
            AssetDatabase.Refresh();
            
            var buildSummary = BuildPipeline.BuildPlayer(buildOptions).summary;

            if (target == BuildTarget.Android)
            {
                var privateRepository = args[ArgPrivateMavenRepository]; 
                CleanUpMainTemplate(privateRepository);
            }

            ReportBuildSummary(buildSummary);
            if (Application.isBatchMode)
                ExitEditorWithResult(buildSummary.result);
        }
        private static void ParseCommandLineArgumentsToUnity(out Dictionary<string, string> providedArguments)
        {
            providedArguments = new Dictionary<string, string>();
            var args = System.Environment.GetCommandLineArgs();
            Log(
                $"{EOL}" +
                $"#####################{EOL}" +
                $"# Parsing Arguments #{EOL}" +
                $"#####################{EOL}" +
                $"{EOL}"
            );

            for (int current = 0, next = 1; current < args.Length; current++, next++)
            {
                // Parse any flags
                var isFlag = args[current].StartsWith("-");
                if (!isFlag)
                    continue;
                var flag = args[current].TrimStart('-');

                // Parse optional value
                var flagHasValue = next < args.Length && !args[next].StartsWith("-");
                var flagValue = flagHasValue ? args[next].TrimStart('-') : string.Empty;
                var displayValue = $"{flagValue}";

                // Assign Value
                Log($"Found Flag \"{flag}\" with value \"{displayValue}\"");
                providedArguments.Add(flag, displayValue);
            }
        }

        private static void ExitEditorWithResult(BuildResult result)
        {
            switch (result)
            {
                case BuildResult.Succeeded:
                    Log("Build Succeeded!");
                    EditorApplication.Exit(0);
                    break;
                case BuildResult.Failed:
                    Log("Build Failed!");
                    EditorApplication.Exit(101);
                    break;
                case BuildResult.Cancelled:
                    Log("Build Cancelled!");
                    EditorApplication.Exit(102);
                    break;
                case BuildResult.Unknown:
                default:
                    Log("Build Result is Unknown!");
                    EditorApplication.Exit(103);
                    break;
            }
        }

        private static void ReportBuildSummary(BuildSummary summary)
        {
            Log(
                $"{EOL}" +
                $"#################{EOL}" +
                $"# Build Summary #{EOL}" +
                $"#################{EOL}" +
                $"{EOL}" +
                $"Output Path: {summary.outputPath}{EOL}" +
                $"Duration: {summary.totalTime}{EOL}" +
                $"Warnings: {summary.totalWarnings}{EOL}" +
                $"Errors: {summary.totalErrors}{EOL}" +
                $"Size: {summary.totalSize}{EOL}" +
                $"{EOL}"
            );
        }
        
        private static void Log(string message)
        {
            if (Application.isBatchMode)
                Console.WriteLine(message);
            else
                Debug.Log(message);
        }

        private static void UpdateMainTemplate(string privateRepository)
        {
            if (!File.Exists(PathToMainTemplate))
            {
                EditorApplication.Exit(1);
                return;
            }

            var contents = File.ReadAllLines(PathToMainTemplate).ToList();
            var containsRepository = contents.FindIndex(line => line.Contains(privateRepository));
            if (containsRepository >= 0)
            {
                Log($"Private maven repo: {privateRepository} already in place, no need to do it again.");
                return;
            }

            var lastRepositoryLocation = contents.FindIndex(line => line.Contains("mavenCentral()"));
            var localContents = 
                @"        maven {
            url 'https://cboost.jfrog.io/artifactory/$'
            credentials {
                username System.getenv('JFROG_USER')
                password System.getenv('JFROG_PASS')
            }
        }";
            localContents = localContents.Replace("$", privateRepository);
            contents.Insert(lastRepositoryLocation + 1, localContents);
            File.WriteAllLines(PathToMainTemplate, contents);
            AssetDatabase.Refresh();
        }    


        private static void CleanUpMainTemplate(string privateRepository)
        {
            if (!File.Exists(PathToMainTemplate))
            {
                EditorApplication.Exit(1);
                return;
            }
            
            var contents = File.ReadAllLines(PathToMainTemplate).ToList();
            var containsRepository = contents.FindIndex(line => line.Contains(privateRepository));
            if (containsRepository < 0)
            {
                Log($"Private maven repo {privateRepository} not in place.");
                return;
            }

            contents.RemoveRange(containsRepository - 1, 7);
            File.WriteAllLines(PathToMainTemplate, contents);
            AssetDatabase.Refresh();
        }

        private static void UpdateCocoaPodRepos(bool isNightlyBuild = true)
        {
            UpdateCocoaPodsRepo("chartboost-pods.git", "<!-- Private Cocoapods Repo -->");
            if(isNightlyBuild)
                UpdateCocoaPodsRepo("helium-ios-sdk-nightly.git", "<!-- Nightly Cocoapods Repo -->");
            AssetDatabase.Refresh();
        }
        
        private static void UpdateCocoaPodsRepo(string repository, string location)
        {
            const string pathToInternalDependencies = "Assets/Editor/InternalDependencies.xml";
            
            if (!File.Exists(pathToInternalDependencies))
            {
                EditorApplication.Exit(1);
                return;
            }
            
            var contents = File.ReadAllLines(pathToInternalDependencies).ToList();
            var containsRepository = contents.FindIndex(line => line.Contains(repository));
            if (containsRepository >= 0)
            {
                Log("Cocoapods Repos already updated, no need to do it again.");
                return;
            }
            
            ParseCommandLineArgumentsToUnity(out var args);
            if (args.TryGetValue(ArgPackageGitHubToken, out var token))
                token = token.Replace("\"", string.Empty);
            else
            {
                Log("github_token not found, should not add repos.");
                return;
            }

            var mod = $"{token}@";
            var privateCocoapodsRepoLocation = contents.FindIndex(line => line.Contains(location));
            var newContents = 
                $@"        <sources>
            <source>https://{mod}github.com/ChartBoost/{repository}</source>
        </sources>";
            contents.Insert(privateCocoapodsRepoLocation + 1, newContents);
            File.WriteAllLines(pathToInternalDependencies, contents);
        }
        
        private static PackageInfo FindPackage(string packageName)
        {
            var packages = Client.List(false, false);
            while (!packages.IsCompleted) { }
            var packageInfos = packages.Result.ToList();
            var desiredPackage = packageInfos.Find(x => x.name == packageName);
            return desiredPackage;
        }
    }
}

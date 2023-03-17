using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Chartboost.Editor;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using ImportOptions = UnityEditor.PackageManager.UI.Sample.ImportOptions;

namespace Chartboost.Editor
{
    public class ChartboostMediationIntegrationChecker
    {
        private const string UnityAds = "UnityAds";
        private const string ChartboostMediation = "Chartboost Mediation";
        private const string WindowTitle = "Chartboost Mediation Unity SDK - Integration Status Checker";
        private const string ChartboostMediationPackageName = "com.chartboost.mediation";
        private const string ChartboostMediationSamplesInAssets = "Assets/Samples/Chartboost Mediation";
        private const string UnityAdsPackageName = "com.unity.ads";
        private const string UnityAdsUncommentWindow = "unity-ads-uncomment-window";
        private static readonly string ChartboostMediationSamplesMetaInAssets = $"{ChartboostMediationSamplesInAssets}.meta";
        private static readonly Version UnityAdsSupportedVersion = new Version(4, 6, 0);
        private static readonly string UnityAdsSDKCommented = $"<!-- <androidPackage spec=\"com.unity3d.ads:unity-ads:{UnityAdsSupportedVersion}\"/> -->";
        private static readonly string UnityAdsSDKUncommented = $"        <androidPackage spec=\"com.unity3d.ads:unity-ads:{UnityAdsSupportedVersion}\"/>";

        /// <summary>
        /// Finds a package in the Unity project non-restricted to the Unity Registry. Any package on the package.json file can be loaded with this method.
        /// </summary>
        /// <param name="packageName">Name of the package to fetch</param>
        /// <returns>PackageInfo of the found package, null if not found</returns>
        public static PackageInfo FindPackage(string packageName)
        {
            var packageJsons = AssetDatabase.FindAssets("package")
                .Select(AssetDatabase.GUIDToAssetPath).Where(x => AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
                .Select(PackageInfo.FindForAssetPath).ToList();

            return packageJsons.Find(x => x.name == packageName);
        }

        /// <summary>
        /// Imports a sample in the Chartboost Mediation  Unity SDK package
        /// </summary>
        /// <param name="sampleName">Sample to include into project</param>
        /// <param name="version">Chartboost Mediation package version to use, must coincide with the currently installed version.</param>
        /// <returns>Import success status</returns>
        public static bool ImportSample(string sampleName, string version)
        {
            var sample = Sample.FindByPackage(ChartboostMediationPackageName, version).Single(x => x.displayName.Equals(sampleName));
            return sample.Import(ImportOptions.HideImportWindow | ImportOptions.OverridePreviousImports);
        }

        /// <summary>
        /// Re-imports a series of Samples based of a collection of Samples names. This is to only update what it's currently in place regardless of the version.
        /// </summary>
        /// <param name="existingSamples">Existing Samples to re-import regardless of the version. Name based</param>
        /// <param name="version">Chartboost Mediation package version to use, must coincide with the currently installed version.</param>
        public static void ReimportExistingSampleSet(ICollection<string> existingSamples, string version)
        {
            if (!Directory.Exists(ChartboostMediationSamplesInAssets))
            {
                Debug.Log($"[Chartboost Mediation Checker] {ChartboostMediationSamplesInAssets} does not exist.");
                return;
            }

            Directory.Delete(ChartboostMediationSamplesInAssets, true);
            File.Delete(ChartboostMediationSamplesMetaInAssets);
            AssetDatabase.Refresh();

            var allSamples = Sample.FindByPackage(ChartboostMediationPackageName, version);
            var sb = new StringBuilder();
            sb.AppendLine("<color='green'>[Chartboost Mediation Checker] Ad Adapter Reimport Started!</color>");
            foreach (var sample in allSamples)
            {
                if (!existingSamples.Contains(sample.displayName))
                {
                    sb.AppendLine($"<color='yellow'> * Skipping Ad Adapter: <b>{sample.displayName}</b></color>");
                    continue;
                }

                sample.Import(ImportOptions.HideImportWindow | ImportOptions.OverridePreviousImports);
                sb.AppendLine($"<color='green'> * Importing Ad Adapter: <b>{sample.displayName}</b></color>");
            }

            sb.AppendLine("<color='green'>[Chartboost Mediation Integration Checker] Ad Adapter Reimport Completed</color>");
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, sb.ToString());
        }

        /// <summary>
        /// Used to attempt to update all existing Ad Adapters without extra input. Good for CI/CD usage and update of Adapters.
        /// </summary>
        /// <returns>Update attempt status</returns>
        public static bool ReimportAllExistingAdapters()
        {
            var chartboostMediation = FindPackage(ChartboostMediationPackageName);

            if (!Directory.Exists(ChartboostMediationSamplesInAssets))
                return false;

            var subdirectories = Directory.GetDirectories(ChartboostMediationSamplesInAssets);
            if (subdirectories.Length <= 0)
                return false;

            var versionDirectory = subdirectories[0];
            var importedDependencies = new HashSet<string>();
            // find all samples/ad adapters
            foreach (var imported in Directory.GetDirectories(versionDirectory))
            {
                var sampleName = Path.GetFileName(imported);
                importedDependencies.Add(sampleName);
            }

            ReimportExistingSampleSet(importedDependencies, chartboostMediation.version);
            return true;
        }

        /// <summary>
        /// Uncomment UnityAds dependency on Optional-UnityAdsDependencies.xml if present.
        /// </summary>
        /// <param name="skipDialog">Optional parameter to skip any dialog windows, if true, UnityAds dependency will be uncommented if possible.</param>
        public static void UncommentUnityAdsDependency(bool skipDialog = false)
        {
            var chartboostMediation = FindPackage(ChartboostMediationPackageName);
            if (!Version.TryParse(chartboostMediation.version, out var chartboostMediationFoundVersion))
            {
                if (!skipDialog)
                {
                    EditorUtility.DisplayDialog(
                        WindowTitle,
                        $"Failed to parse version {chartboostMediation.version} in Package.\n\n**This is probably a bad setup, contact Chartboost Mediation Support at support@chartboost.com**",
                        "Ok");
                }
                return;
            }

            var chartboostMediationVersion = chartboostMediationFoundVersion.ToString();
            var unityAdsDependencyPath = $"Assets/Samples/Chartboost Mediation/{chartboostMediationVersion}/UnityAds/Editor/Optional-UnityAdsDependencies.xml";

            // Check if UnityAds is integrated
            if (!File.Exists(unityAdsDependencyPath))
                return;

            var unityAdsDependencyLines = File.ReadLines(unityAdsDependencyPath).ToList();
            var commentedLineIndex = unityAdsDependencyLines.FindIndex(line => line.Contains(UnityAdsSDKCommented));
            if (commentedLineIndex == -1)
                return;

            var updateUnityAdsSample = true;
            if (!skipDialog)
            {
                updateUnityAdsSample = EditorUtility.DisplayDialog(
                    WindowTitle,
                    "Chartboost Mediation UnityAds Adapter found, but UnityAds dependency is commented. This will lead to a non-functional adapter.\n\nDo you wish to uncomment it?",
                    "Yes", "No", DialogOptOutDecisionType.ForThisMachine, UnityAdsUncommentWindow);
            }

            if (!updateUnityAdsSample)
                return;
            unityAdsDependencyLines[commentedLineIndex] = UnityAdsSDKUncommented;
            File.WriteAllLines(unityAdsDependencyPath, unityAdsDependencyLines);
        }

        [MenuItem("Chartboost Mediation/Integration/UnityAds Check", false, 1)]
        public static void CheckUnityAdsIntegration()
        {
            var chartboostMediation = FindPackage(ChartboostMediationPackageName);
            if (!Version.TryParse(chartboostMediation.version, out var chartboostMediationFoundVersion))
            {
                EditorUtility.DisplayDialog(
                    WindowTitle,
                    $"Failed to parse version {chartboostMediation.version} in Package.\n\n**This is probably a bad setup, contact Chartboost Mediation Support at support@chartboost.com**",
                    "Ok");
                return;
            }

            var chartboostMediationVersion = chartboostMediationFoundVersion.ToString();
            var unityAdsDependencyPath = $"Assets/Samples/Chartboost Mediation/{chartboostMediationVersion}/UnityAds/Editor/Optional-UnityAdsDependencies.xml";

            // check if UnityAds is integrated
            if (!File.Exists(unityAdsDependencyPath))
                return;

            var unityAdsPackage = FindPackage(UnityAdsPackageName);

            if (unityAdsPackage != null)
            {
                if (!Version.TryParse(unityAdsPackage.version, out var unityAdsVersion))
                    return;
                if (!unityAdsVersion.Equals(UnityAdsSupportedVersion))
                {
                    EditorUtility.DisplayDialog(
                        WindowTitle,
                        $"UnityAds integrated through Unity Package Manager with version: {unityAdsPackage.version}. Chartboost Mediation recommended version is {UnityAdsSupportedVersion}.\n\nUnexpected behaviors can occur.",
                        "Ok");
                }
                return;
            }

            UncommentUnityAdsDependency();
            AssetDatabase.Refresh();
            Log("[Chartboost Mediation Integration Checker] UnityAds Ad Adapter Check Completed!", "green");
        }

        /// <summary>
        /// Used to update all existing adapters by Devs choice. This will utilize current's Chartboost Mediation Package version to override all adapters with such version.
        /// </summary>
        [MenuItem("Chartboost Mediation/Integration/Force Reimport Adapters", false, 2)]
        public static void ForceReimportExistingAdapters()
        {
            var confirmUpdate = EditorUtility.DisplayDialog(WindowTitle,
                "Attempting to force reimport all existing adapters.\n\nIs this intentional?", "Yes", "No");

            if (confirmUpdate)
                ReimportAllExistingAdapters();
            CheckUnityAdsIntegration();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Used to detect and address general Chartboost Mediation Integration issues
        /// </summary>
        [MenuItem("Chartboost Mediation/Integration/Status Check", false, 0)]
        public static void CheckIntegration()
        {
            var chartboostMediation = FindPackage(ChartboostMediationPackageName);

            // check if Chartboost Mediation Samples exists
            if (Directory.Exists(ChartboostMediationSamplesInAssets))
            {
                var subDirectories = Directory.GetDirectories(ChartboostMediationSamplesInAssets);

                // no versioning folder
                if (subDirectories.Length <= 0)
                {
                    var addChartboostMediationSample = EditorUtility.DisplayDialog(
                        WindowTitle,
                        "Chartboost Mediation Adapters directory found, but not ad adapters in place.\n\nMake sure to include at least the Chartboost Mediation dependencies.\n\nWould you like to add them?",
                        "Yes", "No");

                    if (addChartboostMediationSample)
                        ImportSample(ChartboostMediation, chartboostMediation.version);
                }
                // at least one versioning sample
                else
                {
                    // we have found a directory with dependencies
                    var versionDirectory = subDirectories[0];

                    // get the version of the dependencies found
                    var versionStr = Path.GetFileName(versionDirectory);

                    // parse versioning folder version
                    if (!Version.TryParse(versionStr, out var versionInAssets))
                    {
                        EditorUtility.DisplayDialog(
                            WindowTitle,
                            $"Failed to parse version {versionStr} in Assets. \n\n**This is probably a bad setup, contact Chartboost Mediation Support at support@chartboost.com**",
                            "Ok");
                        return;
                    }

                    var importedDependencies = new HashSet<string>();

                    // find all samples/ad adapters
                    foreach (var imported in Directory.GetDirectories(versionDirectory))
                    {
                        var sampleName = Path.GetFileName(imported);
                        importedDependencies.Add(sampleName);
                    }

                    // no samples/ad adapters
                    if (importedDependencies.Count <= 0)
                    {
                        var addChartboostMediationSamples = EditorUtility.DisplayDialog(
                            WindowTitle,
                            $"Chartboost Mediation Ad Adapters directory found for version {versionInAssets}, but not Ad Adapters in place.\n\nYou must at least include the Chartboost Mediation dependencies.\n\nWould you like to add them?",
                            "Yes", "No");

                        if (addChartboostMediationSamples)
                            ImportSample(ChartboostMediation, chartboostMediation.version);
                    }
                    // at least one sample
                    else
                    {
                        if (!importedDependencies.Contains(ChartboostMediation))
                        {
                            var addChartboostMediationSamples = EditorUtility.DisplayDialog(
                                WindowTitle,
                                "Chartboost Mediation dependencies not found in Assets.\n\nMake sure to include at least the Chartboost Mediation dependencies.\n\nWould you like to add them?",
                                "Yes", "No");

                            if (addChartboostMediationSamples)
                                ImportSample(ChartboostMediation, chartboostMediation.version);
                        }
                    }

                    // parse package version
                    if (!Version.TryParse(chartboostMediation.version, out var versionInPackage))
                    {
                        EditorUtility.DisplayDialog(
                            WindowTitle,
                            $"Failed to parse version {versionStr} in Package.\n\n**This is probably a bad setup, contact Chartboost Mediation Support at support@chartboost.com**",
                            "Ok");
                    }

                    // act based off version
                    if (versionInAssets < versionInPackage)
                    {
                        var dialogInput = EditorUtility.DisplayDialog(
                            WindowTitle,
                            $"Ad Adapters for version {versionInAssets}, Chartboost Mediation  Unity SDK is using higher version {versionInPackage}.\n\nDo you wish to upgrade your existing Ad Adapters?",
                            "Yes", "No");

                        if (dialogInput)
                            ReimportExistingSampleSet(importedDependencies, chartboostMediation.version);
                    }
                    else if (versionInAssets > versionInPackage)
                    {
                        var dialogInput = EditorUtility.DisplayDialog(
                            WindowTitle,
                            $"Ad Adapters for version {versionInAssets} found, Chartboost Mediation  Unity SDK is using lower version {versionInPackage}.\n\nDo you wish to downgrade your existing Ad Adapters?\n\n**This is probably a bad setup, contact Chartboost Mediation  Support at support@chartboost.com**",
                            "Yes", "No");

                        if (dialogInput)
                            ReimportExistingSampleSet(importedDependencies, chartboostMediation.version);
                    }

                    // check for Unity Ads integration
                    if (importedDependencies.Contains(UnityAds))
                        CheckUnityAdsIntegration();

                    AssetDatabase.Refresh();
                }
            }
            // no samples at all!
            else
            {
                var addChartboostMediationSample = EditorUtility.DisplayDialog(
                    WindowTitle,
                    "No Ad Adapters directory found.\n\nMake sure to include at least the Chartboost Mediation  dependencies.\n\nWould you like to add them?",
                    "Yes", "No");

                if (addChartboostMediationSample)
                    ImportSample(ChartboostMediation, chartboostMediation.version);
            }

            Log("[Chartboost Mediation  Integration Checker] Status Check Completed!", "green");
        }

        private static void Log(string message, string color = "white")
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, $"<color='{color}'>{message}</color>");
        }
    }
}

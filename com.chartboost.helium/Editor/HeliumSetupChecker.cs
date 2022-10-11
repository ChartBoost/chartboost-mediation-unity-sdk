using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using ImportOptions = UnityEditor.PackageManager.UI.Sample.ImportOptions;

namespace Editor
{
    public class HeliumSetupChecker
    {
        private const string Helium = "Helium";
        private const string HeliumWindowTitle = "Helium Unity SDK - Integration Status Checker";
        private const string HeliumPackageName = "com.chartboost.helium";
        private const string HeliumSamplesInAssets = "Assets/Samples/Helium SDK";

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
        /// Imports a sample in the Helium Unity SDK package
        /// </summary>
        /// <param name="sampleName">Sample to include into project</param>
        /// <param name="version">Helium package version to use, must coincide with the currently installed version.</param>
        /// /// <returns>Import success status</returns>
        public static bool ImportSample(string sampleName, string version)
        {
            var sample = Sample.FindByPackage(HeliumPackageName, version).Single(x => x.displayName.Equals(sampleName));
            return sample.Import(ImportOptions.HideImportWindow | ImportOptions.OverridePreviousImports);
        }

        /// <summary>
        /// Re-imports a series of Samples based of a collection of Samples names. This is to only update what it's currently in place regardless of the version.
        /// </summary>
        /// <param name="existingSamples">Existing Samples to re-import regardless of the version. Name based</param>
        /// <param name="version">Helium package version to use, must coincide with the currently installed version.</param>
        public static void ReimportExistingHeliumSamples(ICollection<string> existingSamples, string version)
        {
            Directory.Delete(HeliumSamplesInAssets, true);
            File.Delete($"{HeliumSamplesInAssets}.meta");
            AssetDatabase.Refresh();

            var allSamples = Sample.FindByPackage(HeliumPackageName, version);

            foreach (var sample in allSamples)
            {
                if (existingSamples.Contains(sample.displayName))
                    sample.Import(ImportOptions.HideImportWindow | ImportOptions.OverridePreviousImports);
            }
        }

        [MenuItem("Helium/Integration/Reimport Existing Adapters")]
        public static void ReimportExistingAdapters()
        {
            var helium = FindPackage(HeliumPackageName);

            if (!Directory.Exists(HeliumSamplesInAssets))
                return;

            var subdirectories = Directory.GetDirectories(HeliumSamplesInAssets);
            if (subdirectories.Length <= 0)
                return;
            
            var versionDirectory = subdirectories[0];
            var importedDependencies = new HashSet<string>();
            // find all samples/ad adapters
            foreach (var imported in Directory.GetDirectories(versionDirectory))
            {
                var sampleName = Path.GetFileName(imported);
                importedDependencies.Add(sampleName);
            }

            ReimportExistingHeliumSamples(importedDependencies, helium.version);
        }

        [MenuItem("Helium/Integration/Status Check")]
        public static void CheckHeliumIntegration()
        {
            var helium = FindPackage(HeliumPackageName);

            // check if Helium Samples exists
            if (Directory.Exists(HeliumSamplesInAssets))
            {
                var subDirectories = Directory.GetDirectories(HeliumSamplesInAssets);

                // no versioning folder
                if (subDirectories.Length <= 0)
                {
                    var addHeliumSample = EditorUtility.DisplayDialog(
                        HeliumWindowTitle,
                        "Helium Samples directory found, but not ad adapters in place.\n\nMake sure to include at least the Helium dependencies.\n\nWould you like to add them?",
                        "Yes", "No");

                    if (addHeliumSample)
                        ImportSample(Helium, helium.version);
                }
                // at least one versioning sample
                else
                {
                    // we have found a directory with dependencies
                    var versionDirectory = subDirectories[0];

                    // get the version of the dependencies found
                    var heliumVersionStr = Path.GetFileName(versionDirectory);

                    // parse versioning folder vesion
                    if (!Version.TryParse(heliumVersionStr, out var versionInAssets))
                    {
                        EditorUtility.DisplayDialog(
                            HeliumWindowTitle,
                            $"Failed to parse version {heliumVersionStr} in Assets, please contact Helium Support.",
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
                        var addHeliumSamples = EditorUtility.DisplayDialog(
                            HeliumWindowTitle,
                            $"Helium Samples/Dependencies directory found for version {versionInAssets}, but not Ad adapters in place.\n\nYou must at least include the Helium dependencies.\n\nWould you like to add them?",
                            "Yes", "No");

                        if (addHeliumSamples)
                            ImportSample(Helium, helium.version);
                    }
                    // at least one sample
                    else
                    {
                        if (!importedDependencies.Contains(Helium))
                        {
                            var addHeliumSamples = EditorUtility.DisplayDialog(
                                HeliumWindowTitle,
                                $"Helium Samples/Dependencies not found in Assets.\n\nWould you like to add them?",
                                "Yes", "No");

                            if (addHeliumSamples)
                                ImportSample(Helium, helium.version);
                        }
                    }

                    // parse package version
                    if (!Version.TryParse(helium.version, out var versionInPackage))
                    {
                        EditorUtility.DisplayDialog(
                            HeliumWindowTitle,
                            $"Failed to parse version {heliumVersionStr} in Package, please contact Helium Support.",
                            "Ok");
                    }

                    // act based off version
                    if (versionInAssets < versionInPackage)
                    {
                        var dialogInput = EditorUtility.DisplayDialog(
                            HeliumWindowTitle,
                            $"Newer Samples/Dependencies for version {versionInAssets}, package is using version {versionInPackage}.\n\nDo you wish to update your existing Ad adapters?",
                            "Yes", "No");

                        if (dialogInput)
                            ReimportExistingHeliumSamples(importedDependencies, helium.version);
                    }
                    else if (versionInAssets > versionInPackage)
                    {
                        var dialogInput = EditorUtility.DisplayDialog(
                            HeliumWindowTitle,
                            $"Older Samples/Dependencies for version {versionInAssets} found, package is using newer version {versionInPackage}.\n\nDo you wish to downgrade your existing Ad adapters?\n\n**This is probably a bad setup, contact Helium Support**",
                            "Yes", "No");

                        if (dialogInput)
                            ReimportExistingHeliumSamples(importedDependencies, helium.version);
                    }
                }
            }
            // no samples at all!
            else
            {
                var addHeliumSample = EditorUtility.DisplayDialog(
                    HeliumWindowTitle,
                    "No Samples directory found.\n\nMake sure to include at least the Helium dependencies.\n\nWould you like to add them?",
                    "Yes", "No");

                if (addHeliumSample)
                    ImportSample(Helium, helium.version);
            }
        }
    }
}

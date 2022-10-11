using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Editor
{
    public class HeliumSetupChecker
    {
        private const string Helium = "Helium";
        private const string HeliumWindowTitle = "Helium Unity SDK - Integration Status Checker";
        private const string HeliumPackageName = "com.chartboost.helium";
        private const string HeliumSamplesInAssets = "Assets/Samples/Helium SDK";

        private static PackageInfo FindPackage(string packageName)
        {
            var packageJsons = AssetDatabase.FindAssets("package")
                .Select(AssetDatabase.GUIDToAssetPath).Where(x => AssetDatabase.LoadAssetAtPath<TextAsset>(x) != null)
                .Select(PackageInfo.FindForAssetPath).ToList();
        
            return packageJsons.Find(x => x.name == packageName);
        }

        [MenuItem("Helium/Check Integration")]
        public static void CheckHeliumIntegration()
        {
            if (Directory.Exists(HeliumSamplesInAssets))
            {
                var subDirectories = Directory.GetDirectories(HeliumSamplesInAssets);
                if (subDirectories.Length <= 0)
                {
                    EditorUtility.DisplayDialog(
                        HeliumWindowTitle, 
                        "Helium Samples directory found, but not ad adapters have been found.\n\nMake sure to include at least the Helium dependencies.",
                        "Ok");
                }
                else
                {
                    // we have found a directory with dependencies
                    var versionDirectory = subDirectories[0];

                    // get the version of the dependencies found
                    var heliumVersionStr = Path.GetFileName(versionDirectory);

                    if (!Version.TryParse(heliumVersionStr, out var versionInAssets))
                    {
                        EditorUtility.DisplayDialog(
                            HeliumWindowTitle, 
                            $"Failed to parse version {heliumVersionStr} in Assets, please contact Helium Support.",
                            "Ok");
                        return;
                    }
                    
                    var helium = FindPackage(HeliumPackageName);

                    var optionalDependencies = Directory.GetDirectories(versionDirectory);

                    var importedDependencies = new HashSet<string>();

                    foreach (var imported in optionalDependencies)
                    {
                        var sampleName = Path.GetFileName(imported);
                        importedDependencies.Add(sampleName);
                    }

                    if (optionalDependencies.Length <= 0)
                    {
                        EditorUtility.DisplayDialog(
                            HeliumWindowTitle, 
                            $"Helium Samples version {versionInAssets} found, but not Ad adapters in place.\n\nMake sure to include at least the Helium dependencies.",
                            "Ok");
                    } 
                    else
                    {
                        var allSamples = Sample.FindByPackage(HeliumPackageName, helium.version);

                        if (!importedDependencies.Contains(Helium))
                        {
                            var addHeliumDependencies = EditorUtility.DisplayDialog(
                                HeliumWindowTitle, 
                                $"Helium Dependencies not found in Assets.\n\nWould you like to add them?",
                                "Yes", "No");

                            if (addHeliumDependencies)
                            {
                                var heliumSample = allSamples.Single(x => x.displayName.Equals(Helium));
                                heliumSample.Import(Sample.ImportOptions.HideImportWindow |
                                                    Sample.ImportOptions.OverridePreviousImports);
                            }
                        }
                    }
                
                    if (!Version.TryParse(helium.version, out var versionInPackage))
                    {
                        EditorUtility.DisplayDialog(
                            HeliumWindowTitle, 
                            $"Failed to parse version {heliumVersionStr} in Package, please contact Helium Support.",
                            "Ok");
                    }

                    if (versionInAssets < versionInPackage)
                    {
                        var dialogInput = EditorUtility.DisplayDialog(
                            HeliumWindowTitle,
                            $"Samples/Dependencies for version {versionInAssets}, package is on version {versionInPackage}.\n\nDo you wish to update your existing Ad adapters?",
                            "Yes", "No");
                        
                        if (dialogInput)
                            ReimportExistingHeliumSamples(importedDependencies, helium.version);
                    }
                    else if (versionInAssets > versionInPackage)
                    {
                        var dialogInput = EditorUtility.DisplayDialog(
                            HeliumWindowTitle,
                            $"Samples/Dependencies for version {versionInAssets}, package is on version {versionInPackage}.\n\nDo you wish to downgrade your existing Ad adapters?\n\n**This is probably a bad setup, contact Helium Support**",
                            "Yes", "No");
                        
                        if (dialogInput)
                            ReimportExistingHeliumSamples(importedDependencies, helium.version);
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog(
                    HeliumWindowTitle, 
                    "No Samples directory found.\n\nMake sure to include at least the Helium dependencies.",
                    "Ok");
            }
            
            void ReimportExistingHeliumSamples(ICollection<string> existingSamples, string version)
            {
                File.Delete(Path.Combine(HeliumSamplesInAssets, ".meta"));
                Directory.Delete(HeliumSamplesInAssets, true);
                AssetDatabase.Refresh();
                            
                var allSamples = Sample.FindByPackage(HeliumPackageName, version);
                            
                foreach (var sample in allSamples)
                {
                    if (existingSamples.Contains(sample.displayName))
                        sample.Import(Sample.ImportOptions.HideImportWindow | Sample.ImportOptions.OverridePreviousImports);
                }
            }
        }
    }
}

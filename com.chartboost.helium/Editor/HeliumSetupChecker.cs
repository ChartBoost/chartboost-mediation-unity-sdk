using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Editor
{
    public class HeliumSetupChecker
    {
        private const string HeliumWindowTitle = "Helium Unity SDK - Integration Status Checker";
        private const string HeliumPackageName = "com.chartboost.helium";
        private const string HeliumSamplesAssets = "Assets/Samples/Helium SDK";

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
            if (Directory.Exists(HeliumSamplesAssets))
            {
                var subDirectories = Directory.GetDirectories(HeliumSamplesAssets);
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
                    
                    var optionalDependencies = Directory.GetDirectories(versionDirectory);

                    if (optionalDependencies.Length <= 0)
                    {
                        EditorUtility.DisplayDialog(
                            HeliumWindowTitle, 
                            $"Helium Samples version {versionInAssets} found, but not Ad adapters in place.\n\nMake sure to include at least the Helium dependencies.",
                            "Ok");
                    } 
                    else
                    {
                        Debug.LogError("At least one adapter has been added");
                    }
                
                    var helium = FindPackage(HeliumPackageName);
                
                    if (!Version.TryParse(helium.version, out var versionInPackage))
                    {
                        EditorUtility.DisplayDialog(
                            HeliumWindowTitle, 
                            $"Failed to parse version {heliumVersionStr} in Package, please contact Helium Support.",
                            "Ok");
                    }

                    if (versionInAssets < versionInPackage)
                    {
                        if (EditorUtility.DisplayDialog(
                                HeliumWindowTitle,
                                $"Samples/Dependencies for version {versionInAssets}, package is on version {versionInPackage}.\n\nDo you wish to update your existing Ad adapters?",
                                "Yes", "No"))
                        {
                            Debug.LogError("Updating Dependencies");
                        }
                    }
                    else if (versionInAssets > versionInPackage)
                    {
                        if (EditorUtility.DisplayDialog(
                                HeliumWindowTitle,
                                $"Samples/Dependencies for version {versionInAssets}, package is on version {versionInPackage}.\n\nDo you wish to downgrade your existing Ad adapters?\n\n**This is probably a bad setup, contact Helium Support**",
                                "Yes", "No"))
                        {
                            Debug.LogError("Updating Dependencies");
                        }
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
        }
    }
}

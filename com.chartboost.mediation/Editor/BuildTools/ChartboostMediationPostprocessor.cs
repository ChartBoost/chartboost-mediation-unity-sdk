using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using System.IO;
using System.Linq;
using Chartboost.Editor.SKAdNetwork;
#endif

namespace Chartboost.Editor.BuildTools
{
    internal sealed class ChartboostMediationPostprocessor : IPostprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPostprocessBuild(BuildReport report)
        {
            var buildTarget = report.summary.platform;
            var pathToBuiltProject = report.summary.outputPath;
            
            if (buildTarget != BuildTarget.iOS)
                return;
            
            PListModifications(pathToBuiltProject);
            PBXProjectModifications(pathToBuiltProject);
        }

        private static void PBXProjectModifications(string pathToBuiltProject)
        {
            #if UNITY_IOS
            var pbxProjectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            var pbxProject = new PBXProject();
            pbxProject.ReadFromFile(pbxProjectPath);

            var mods = new HashSet<bool>
            {
                DisableBitcode(pbxProject)
            };

            if (mods.Any(x => x))
                pbxProject.WriteToFile(pbxProjectPath);

            static bool DisableBitcode(PBXProject pbxProject)
            {
                if (!ChartboostMediationSettings.DisableBitcode)
                    return false;

                const string bitcodeKey = "ENABLE_BITCODE";
                const string bitcodeValue = "NO";

                pbxProject.SetBuildProperty(pbxProject.ProjectGuid(), bitcodeKey, bitcodeValue);
                pbxProject.SetBuildProperty(pbxProject.GetUnityMainTargetGuid(), bitcodeKey, bitcodeValue);
                pbxProject.SetBuildProperty(pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName()), bitcodeKey, bitcodeValue);
                pbxProject.SetBuildProperty(pbxProject.GetUnityFrameworkTargetGuid(), bitcodeKey, bitcodeValue);
                Debug.Log("[ChartboostMediationPostprocessor] Disabled Bitcode");
                return true;
            }
            #endif
        }

        private static void PListModifications(string pathToBuiltProject)
        {
            #if UNITY_IOS
            var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            var mods = new HashSet<bool>
            {
                IncludeSKAdNetwork(plist),
                IncludeGoogleAppId(plist)
            };
            
            if (mods.Any(x => x))
                plist.WriteToFile(plistPath);
            
            static bool IncludeSKAdNetwork(PlistDocument plist)
            {
                if (!ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled)
                    return false;
            
                var ids = SKAdNetworkRequest.GetSKAdNetworkIds();
                var skanItems = plist.root.CreateArray(SKAdNetworkConstants.SKAdNetworkItemsKey);

                foreach (var skadnetworkID in ids)
                {
                    var skanId = skanItems.AddDict();
                    skanId.SetString(SKAdNetworkConstants.SKAdNetworkIdKey, skadnetworkID.ToLower());
                }
                Debug.Log($"[ChartboostMediationPostprocessor] Added {skanItems.values.Count} Items to the SKAdNetwork Dictionary in Info.plist");
                return true;
            }

            static bool IncludeGoogleAppId(PlistDocument plist)
            {
                if (string.IsNullOrEmpty(ChartboostMediationSettings.IOSGoogleAppId) || ChartboostMediationSettings.IOSGoogleAppId.Equals(ChartboostMediationSettings.DefaultSDKKeyValue))
                    return false;
            
                const string googleAppIdKey = "GADApplicationIdentifier";
                plist.root.SetString(googleAppIdKey, ChartboostMediationSettings.IOSGoogleAppId);
                Debug.Log($"[ChartboostMediationPostprocessor] Added {googleAppIdKey} to Info.plist");
                return true;
            }
            #endif
        }
    }
}

#if UNITY_IOS
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chartboost.Editor.SKAdNetwork;
using Chartboost.Logging;
using Chartboost.Mediation.Editor.iOS.SKAdNetwork;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace Chartboost.Mediation.Editor.iOS
{
    internal sealed class IOSPostprocessor : IPostprocessBuildWithReport
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
                const string bitcodeKey = "ENABLE_BITCODE";
                const string bitcodeValue = "NO";

                pbxProject.SetBuildProperty(pbxProject.ProjectGuid(), bitcodeKey, bitcodeValue);
                pbxProject.SetBuildProperty(pbxProject.GetUnityMainTargetGuid(), bitcodeKey, bitcodeValue);
                pbxProject.SetBuildProperty(pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName()), bitcodeKey, bitcodeValue);
                pbxProject.SetBuildProperty(pbxProject.GetUnityFrameworkTargetGuid(), bitcodeKey, bitcodeValue);
                LogController.Log("Disabled Bitcode", LogLevel.Info);
                return true;
            }
        }

        private static void PListModifications(string pathToBuiltProject)
        {
            var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            var mods = new HashSet<bool>
            {
                IncludeSKAdNetwork(plist),
            };
            
            if (mods.Any(x => x))
                plist.WriteToFile(plistPath);
            
            static bool IncludeSKAdNetwork(PlistDocument plist)
            {
                var ids = SKAdNetworkRequest.GetSKAdNetworkIds();
                var skanItems = plist.root.CreateArray(SKAdNetworkConstants.SKAdNetworkItemsKey);

                foreach (var skadnetworkID in ids)
                {
                    var skanId = skanItems.AddDict();
                    skanId.SetString(SKAdNetworkConstants.SKAdNetworkIdKey, skadnetworkID.ToLower());
                }
                LogController.Log($"Added {skanItems.values.Count} Items to the SKAdNetwork Dictionary in Info.plist", LogLevel.Info);
                return true;
            }
        }
    }
}
#endif

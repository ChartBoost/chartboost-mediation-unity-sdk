using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
#endif

namespace Editor
{
    public class CanaryPostprocessor : IPostprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPostprocessBuild(BuildReport report)
        {
            var buildTarget = report.summary.platform;
            var pathToBuiltProject = report.summary.outputPath;

            if (buildTarget != BuildTarget.iOS)
                return;

            PBXProjectModifications(pathToBuiltProject);
            PBXProjectSchemaModifications(pathToBuiltProject);
            PListModifications(pathToBuiltProject);
        }

        private static void PBXProjectModifications(string pathToBuiltProject)
        {
            #if UNITY_IOS
            var pbxProjectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            var pbxProject = new PBXProject();
            pbxProject.ReadFromFile(pbxProjectPath);

            var mods = new HashSet<bool>
            {
                AddAppTrackingTransparencyFramework(pbxProject),
                AddQrCodeScannerStoryboard(pbxProject, pathToBuiltProject),
                IntegrateSwiftBridging(pbxProject)
            };

            if (mods.Any(x => x))
                pbxProject.WriteToFile(pbxProjectPath);

            static bool AddAppTrackingTransparencyFramework(PBXProject pbxProject)
            {
                if (pbxProject == null)
                    return false;

                var frameworkGUI = pbxProject.GetUnityFrameworkTargetGuid();
                pbxProject.AddFrameworkToProject(frameworkGUI, "AppTrackingTransparency.framework", false);
                return true;
            }

            static bool AddQrCodeScannerStoryboard(PBXProject pbxProject, string pathToBuiltProject)
            {
                var storyboardFileName = "QRCodeScanner.storyboard";
                var storyboardDestinationPathRelative = $"Libraries/Plugins/iOS";

                var storyboardSourcePathAbsolute = Path.Combine(Application.dataPath, "Plugins/iOS", storyboardFileName);
                var storyboardDestinationPathAbsolute = Path.Combine(pathToBuiltProject, storyboardDestinationPathRelative, storyboardFileName);
                FileUtil.CopyFileOrDirectory(storyboardSourcePathAbsolute, storyboardDestinationPathAbsolute);

                Debug.Log($"Adding {storyboardSourcePathAbsolute} to Xcode project at {storyboardDestinationPathRelative}");
                var storyboardGuid = pbxProject.AddFile(storyboardDestinationPathAbsolute, Path.Combine(storyboardDestinationPathRelative, storyboardFileName), PBXSourceTree.Absolute);
                
                string targetGuid = pbxProject.GetUnityFrameworkTargetGuid();
                pbxProject.AddFileToBuild(targetGuid, storyboardGuid);
                
                return true;
            }

            static bool IntegrateSwiftBridging(PBXProject pbxProject)
            {
                Debug.Log("Integrating Swift bridging");
                string targetGuid = pbxProject.GetUnityFrameworkTargetGuid();
                pbxProject.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "Canary-Swift.h");
                pbxProject.AddBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
                pbxProject.AddBuildProperty(targetGuid, "DEFINES_MODULE", "YES");
                pbxProject.AddBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");
                pbxProject.AddBuildProperty(targetGuid, "COREML_CODEGEN_LANGUAGE", "Swift");
                return true;
            }
            #endif
        }

        private static void PBXProjectSchemaModifications(string pathToBuiltProject)
        {
            #if UNITY_IOS
            var pathToSchema = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj", "xcshareddata", "xcschemes",
                "Unity-iPhone.xcscheme");
            var xcshemeFile = XDocument.Load(pathToSchema);

            var mods = new HashSet<bool>
            {
                AddPreBuildActionSteps(xcshemeFile)
            };


            if (mods.Any(x => x))
                xcshemeFile.Save(pathToSchema);

            static bool AddPreBuildActionSteps(XDocument xcsheme)
            {
                if (xcsheme == null)
                    return false;

                var scheme = xcsheme.Root;
                var buildActionNode = scheme?.Element("BuildAction");
                var buildActionEntriesNode = buildActionNode?.Element("BuildActionEntries");
                var buildActionEntryNode = buildActionEntriesNode?.Element("BuildActionEntry");
                var buildableReferenceNode = buildActionEntryNode?.Element("BuildableReference");

                Debug.Log(buildableReferenceNode);

                var environmentBuildable = new XElement("EnvironmentBuildable", buildableReferenceNode);
                var actionContentNode = new XElement("ActionContent", environmentBuildable);
                actionContentNode.SetAttributeValue("title", "Encoding File Fix");
                const string SchemeModificationsScript = "sed -i -e 's/\\r$//' \"${PROJECT_DIR}/Pods/Fyber_Marketplace_SDK/IASDKCore/IASDKCore.xcframework/ios-arm64/IASDKCore.framework/Modules/IASDKCore.swiftmodule/arm64-apple-ios.swiftinterface\"\n\nsed -i -e 's/\\r$//' \"${PROJECT_DIR}/Pods/ChartboostMediationAdapterDigitalTurbineExchange/Source/DigitalTurbineExchangeAdapter.swift\" \n\nsed -i -e 's/\\r$//' \"${PROJECT_DIR}/Pods/FirebaseCrashlytics/run\"\n";
                actionContentNode.SetAttributeValue("scriptText", SchemeModificationsScript);
                var executionActionNode = new XElement("ExecutionAction", actionContentNode);
                executionActionNode.SetAttributeValue("ActionType",
                    "Xcode.IDEStandardExecutionActionsCore.ExecutionActionType.ShellScriptAction");
                var preActionNode = new XElement("PreActions", executionActionNode);

                buildActionNode?.AddFirst(preActionNode);
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
                AddNSUserTrackingUsageDescription(plist)
            };

            if (mods.Any(x => x))
                plist.WriteToFile(plistPath);

            static bool AddNSUserTrackingUsageDescription(PlistDocument plist)
            {
                if (plist == null)
                    return false;

                var plistRoot = plist.root;
                
                const string TrackingDescription = "Your data will be used to provide you a better and personalized ad experience.";
                plistRoot.SetString("NSUserTrackingUsageDescription", TrackingDescription);

                const string CameraDescription = "Scan your credentials with a QR code.";
                plistRoot.SetString("NSCameraUsageDescription", CameraDescription);
                
                return true;
            }
            #endif
        }
    }
}

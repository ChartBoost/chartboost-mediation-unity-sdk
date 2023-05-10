#if UNITY_IOS
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace Editor
{
    internal class DisableBitcode : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS)
                return;

            var buildPath = report.summary.outputPath;
            var pbxProjectPath = PBXProject.GetPBXProjectPath(buildPath);
            var pbxProject = new PBXProject();
            pbxProject.ReadFromFile(pbxProjectPath);

            //Disabling Bitcode on all targets
            //Main
            var target = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            //Unity Tests
            target = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            //Unity Framework
            target = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            pbxProject.WriteToFile(pbxProjectPath);
        }
    }
}
#endif
using System.IO;
using Chartboost.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor
{
    public class CanaryPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
            StoreBuildCode();
        }

        private static void StoreBuildCode()
        {
            var buildCodePath = Path.Combine(Application.dataPath, "Resources/buildcode.txt");
            
            var buildCode = string.Empty;
            #if UNITY_ANDROID
            buildCode = PlayerSettings.Android.bundleVersionCode.ToString();
            #elif UNITY_IOS
            buildCode = PlayerSettings.iOS.buildNumber;
            #endif

            buildCodePath.FileCreate(buildCode);
        }
    }
}

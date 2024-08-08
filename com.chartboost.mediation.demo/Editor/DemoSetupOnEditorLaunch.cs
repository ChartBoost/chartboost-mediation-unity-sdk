using Chartboost.Mediation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Chartboost.Editor {
    
    [InitializeOnLoad]
    public class DemoSetupOnEditorLaunch {
        private const string CompanyName = "Chartboost";
        private const string ProductName = "Chartboost Mediation Unity SDK Demo";
        private const string ApplicationBundleIdentifier = "com.chartboost.mediation.unity.demo";
        private const string IconChartboostMediation = "Icon-Chartboost-Mediation";

        private static readonly string[] CompilerFlags = {
            "-warnaserror"
        };

        static DemoSetupOnEditorLaunch() => SetupDemoApp();

        [MenuItem("Chartboost Mediation/Setup Demo")]
        private static void SetupDemoApp() {
            Debug.Log($"Configuring {ProductName}.");

            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;
            PlayerSettings.bundleVersion = ChartboostMediation.SDKVersion;

            var mediationIcon = Resources.Load<Texture2D>(IconChartboostMediation);

            var icons = new[] {
                mediationIcon
            };

            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34;
            PlayerSettings.iOS.targetOSVersionString = "11.0";

            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iOS, icons);
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);

            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, ApplicationBundleIdentifier);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, ApplicationBundleIdentifier);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, ApplicationBundleIdentifier);

            PlayerSettings.SetAdditionalCompilerArgumentsForGroup(BuildTargetGroup.Android, CompilerFlags);
            PlayerSettings.SetAdditionalCompilerArgumentsForGroup(BuildTargetGroup.iOS, CompilerFlags);
            PlayerSettings.SetAdditionalCompilerArgumentsForGroup(BuildTargetGroup.Standalone, CompilerFlags);

            // Addresses an issue with 2022 LTS where development builds using Vulkan API will crash.
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] {
                GraphicsDeviceType.OpenGLES3,
                GraphicsDeviceType.Vulkan
            });
        }
    }
}

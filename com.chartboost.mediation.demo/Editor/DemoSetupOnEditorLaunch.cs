using Chartboost.Mediation;
using UnityEditor;
using UnityEditor.Build;
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

            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)34;
            PlayerSettings.iOS.targetOSVersionString = "13.0";
            
            PlayerSettings.SetIcons(NamedBuildTarget.Android, icons, IconKind.Application);
            PlayerSettings.SetIcons(NamedBuildTarget.iOS, icons, IconKind.Application);
            PlayerSettings.SetIcons(NamedBuildTarget.Unknown, icons, IconKind.Application);
            
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationBundleIdentifier);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, ApplicationBundleIdentifier);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, ApplicationBundleIdentifier);
            
            PlayerSettings.SetAdditionalCompilerArguments(NamedBuildTarget.Android, CompilerFlags);
            PlayerSettings.SetAdditionalCompilerArguments(NamedBuildTarget.iOS, CompilerFlags);
            PlayerSettings.SetAdditionalCompilerArguments(NamedBuildTarget.Standalone, CompilerFlags);

            // Addresses an issue with 2022 LTS where development builds using Vulkan API will crash.
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] {
                GraphicsDeviceType.OpenGLES3,
                GraphicsDeviceType.Vulkan
            });
        }
    }
}

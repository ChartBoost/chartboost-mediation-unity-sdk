using Chartboost.Mediation.Demo;
using UnityEditor;
using UnityEngine;

namespace Chartboost.Editor
{
   [InitializeOnLoad]
   public class DemoSetupOnEditorLaunch
   {
      private const string CompanyName = "Chartboost";
      private const string ProductName = "Chartboost Mediation Unity SDK Demo";
      private const string ApplicationBundleIdentifier = "com.chartboost.mediation.unity.demo";
      private const string IconChartboostMediation = "Icon-Chartboost-Mediation";

      private static readonly string[] CompilerFlags = { 
         "-warnaserror"
      };

      static DemoSetupOnEditorLaunch() => SetupDemoApp();

      [MenuItem("Chartboost Mediation/Setup Demo")]
      private static void SetupDemoApp()
      {
         Debug.Log($"Configuring {ProductName}.");
         
         PlayerSettings.companyName = CompanyName;
         PlayerSettings.productName = ProductName;
         PlayerSettings.bundleVersion = ChartboostMediation.Version;

         var mediationIcon = Resources.Load<Texture2D>(IconChartboostMediation);

         var icons = new[]
         {
            mediationIcon
         };

         PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
         PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)33;
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

         ChartboostMediationSettings.AndroidAppId = DefaultEnvironment.AndroidAppId;
         ChartboostMediationSettings.AndroidAppSignature = DefaultEnvironment.AndroidAppSignature;
         ChartboostMediationSettings.IOSAppId = DefaultEnvironment.IOSAppId;
         ChartboostMediationSettings.IOSAppSignature = DefaultEnvironment.IOSAppSignature;
         ChartboostMediationSettings.IsLoggingEnabled = DefaultEnvironment.IsLoggingEnabled;
         ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled = DefaultEnvironment.IsSkAdNetworkResolutionEnabled;
         ChartboostMediationSettings.DisableBitcode = DefaultEnvironment.DisableBitcode;
      }
   }
}

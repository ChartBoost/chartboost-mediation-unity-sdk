using System;
using System.IO;
using UnityEngine;

namespace Chartboost.Editor.Adapters
{
    public class Constants
    {
        public const string Unselected = "Unselected";
        public const string ChartboostMediationPackageName = "com.chartboost.mediation";
        public const string AndroidVersionInMainTemplate = "%ANDROID_VERSION%";
        public const string IOSVersionInMainTemplate = "%IOS_VERSION%";
        
        public const string PathToMainTemplate = "Packages/com.chartboost.mediation/Editor/Adapters/Templates/TemplateChartboostMediation.xml";
        public const string PathToAdapterTemplate = "Packages/com.chartboost.mediation/Editor/Adapters/Templates/TemplateAdapter.xml";

        public static readonly string PathToPackageGeneratedFiles = Path.Combine(Application.dataPath, "com.chartboost.mediation");
        public static readonly string PathToEditorInGeneratedFiles = Path.Combine(PathToPackageGeneratedFiles, "Editor");
        public static readonly string PathToAdaptersDirectory = Path.Combine(PathToEditorInGeneratedFiles, "Adapters");
        public static readonly string PathToMainDependency = Path.Combine(PathToEditorInGeneratedFiles, "ChartboostMediationDependencies.xml");
        public static readonly string PathToSelectionsFile = Path.Combine(PathToEditorInGeneratedFiles, "selections.json");
        
        public static readonly string PathToLibrary = Path.Combine(Directory.GetCurrentDirectory(), "Library");
        public static readonly string PathToLibraryCacheDirectory = Path.Combine(PathToLibrary, "com.chartboost.mediation");
        public static readonly string PathToAdaptersCachedJson = Path.Combine(PathToLibraryCacheDirectory, "partners.json");
        
        // Networks with special handling due to different versioning, dependencies, or multiple sdks
        public const string InMobi = "inmobi";
        public const string IronSource = "ironsource";
        public const string Mintegral = "mintegral";
        public static readonly Version InMobiNewSDK = new Version("10.5.0");

        public const string StyleSheet = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/AdaptersWindow.uss";
        public const string LogoPNG = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/Logo.png";
        public const string UpgradePNG = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/Upgrade.png";
        public const string SavePNG = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/Save.png";
        public const string WarningPNG = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/Warning.png";
        public const string RefreshPNG = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/Refresh.png";
    }
}

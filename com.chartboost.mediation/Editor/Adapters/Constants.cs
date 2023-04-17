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
        
        public const string PathToMainTemplate = "Packages/com.chartboost.mediation/Editor/Adapters/TemplateChartboostMediation.xml";
        public const string PathToAdapterTemplate = "Packages/com.chartboost.mediation/Editor/Adapters/TemplateAdapter.xml";

        public static readonly string PathToPackageGeneratedFiles = Path.Combine(Application.dataPath, "com.chartboost.mediation");
        public static readonly string PathToEditorInGeneratedFiles = Path.Combine(PathToPackageGeneratedFiles, "Editor");
        public static readonly string PathToAdaptersDirectory = Path.Combine(PathToEditorInGeneratedFiles, "Adapters");
        public static readonly string PathToMainDependency = Path.Combine(PathToEditorInGeneratedFiles, "ChartboostMediationDependencies.xml");
        public static readonly string PathToSelectionsFile = Path.Combine(PathToEditorInGeneratedFiles, "selections.json");
        
        // Quirky networks
        public const string InMobi = "inmobi";
        public const string IronSource = "ironsource";
        public const string Mintegral = "mintegral";

        public const string StyleSheet = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/AdaptersWindow.uss";
        public const string LogoPNG = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/Logo.png";
        public const string UpgradePNG = "Packages/com.chartboost.mediation/Editor/Adapters/Visual/Upgrade.png";
        public const string SavePNG = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/Save.png";
        public const string WarningPNG = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/Warning.png";
        public const string RefreshPNG = "Packages/com.chartboost.mediation/Editor/Adapters/Visuals/Refresh.png";

    }
}

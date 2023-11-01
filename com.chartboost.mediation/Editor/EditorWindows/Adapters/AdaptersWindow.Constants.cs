using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Chartboost.Editor.EditorWindows.Adapters
{
    public sealed partial class AdaptersWindow
    {
        private const string AndroidVersionInMainTemplate = "%ANDROID_VERSION%";
        private const string IOSVersionInMainTemplate = "%IOS_VERSION%";
        private const string AndroidAdapterInTemplate = "%ANDROID_ADAPTER%";
        private const string AndroidRepositoriesInTemplate = "%REPOSITORIES%";
        private const string AndroidDependenciesInTemplate = "%ANDROID_DEPENDENCIES%";
            
        private const string IOSAdapterInTemplate = "%IOS_ADAPTER%";
        private const string IOSDependenciesInTemplate = "%IOS_DEPENDENCIES%";
        
        private const string MainTemplate = "TemplateChartboostMediation";
        private const string AdapterTemplate = "TemplateAdapter";

        private static string PathToPackageGeneratedFiles => Path.Combine(Application.dataPath, "com.chartboost.mediation");
        private static string PathToEditorInGeneratedFiles => Path.Combine(PathToPackageGeneratedFiles, "Editor");
        private static string PathToAdaptersDirectory => Path.Combine(PathToEditorInGeneratedFiles, "Adapters");
        private static string PathToMainDependency => Path.Combine(PathToEditorInGeneratedFiles, "ChartboostMediationDependencies.xml");
        private static string PathToSelectionsFile => Path.Combine(PathToEditorInGeneratedFiles, "selections.json");
        
        // Networks with special handling due to different versioning, dependencies, or multiple sdks
        private const string IronSource = "ironsource";

        private const string VersionNumberIsPresent = @"(?<=:)(?<major>\d+)\.(?<minor>\d+)(\.(?<build>\d+))?(-(?<suffix>\w+(\.\w+)*))?(?=[^.\d]|$)";
        private static readonly Regex NeedsVersionNumber = new Regex(VersionNumberIsPresent);

        private const string Unselected = "Unselected";
        private const string ChartboostMediationPackageName = "com.chartboost.mediation";
        private const string StyleSheet = "AdaptersWindowStyleSheet";
        private const string ClassBody = "body";
        private const string ClassIcon = "icon";
        private const string ClassWarningButton = "warning-button";
        private const string ClassMediationLogo = "mediation-logo";
        private const string ClassUpgradeButton = "upgrade-button";
        private const string ClassRefreshButton = "refresh-button";
        private const string ClassSaveButton = "save-button";
        private const string ClassVersionCol = "version-col";
        private const string ClassAdapterCol = "adapter-col";
        private const string ClassFlexGrid = "flex-grid";
        private const string ClassHeaderNetwork = "header-network";
        private const string ClassHeaderVersion = "header-version";
        
        private const string LogoPNG = "ChartboostMediationLogo";
        private const string UpgradePNG = "AdaptersUpgrade";
        private const string SavePNG = "AdaptersSave";
        private const string WarningPNG = "AdaptersWarning";
        private const string RefreshPNG = "AdaptersRefresh";
        
        private const string ToolTipUpgradeButton = "Upgrade all adapter selections to their latest version!";
        private const string ToolTipRefreshButton = "Fetch new configurations for adapter updates!";
        private const string ToolTipSaveButton = "You have unsaved changes! Press to save.";
        private const string ToolTipNetwork = "Network Associated with Ad Adapter.";
        private const string ToolTipAndroidVersion = "Android Version of Ad Adapters.";
        private const string ToolTipIOSVersion = "iOS Version of Ad Adapters.";

        private const string LabelNetwork = "Network";
        private const string LabelAndroid = "Android";
        private const string LabelIOS = "iOS";
    }
}

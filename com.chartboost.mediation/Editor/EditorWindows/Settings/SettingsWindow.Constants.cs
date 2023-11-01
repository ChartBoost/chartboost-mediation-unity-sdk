#if !NO_SETTINGS_WINDOW
using UnityEngine.UIElements;

namespace Chartboost.Editor.EditorWindows.Settings
{
    public sealed partial class SettingsWindow
    {
        private const int WindowPriority = 1;
        private const string SettingsWindowStyleSheet = "SettingsWindowStyleSheet";
        private const string ClassBody = "body";
        private const string ClassTitle = "title";
        private const string SettingsLabel = "Mediation SDK Settings";
        private const string SettingsTooltip = "Chartboost Mediation Settings Window. Modify your integration settings through this window.";
        private const string BuildToolsLabel = "Build-Processing Tools";
        private const string BuildToolsToolTip = "Build pre-processor & post-processor tools offered through the Chartboost Mediation Unity SDK.";
        private const string HelpBoxContents = "The fields below toggle pre-processor & post-processor tools offered through the Chartboost Mediation Unity SDK. Use with caution.";
        private const string IdHeaders = "headers";
        private const string ClassFlexGrid = "flex-grid";
        private const string ClassHeader = "header";
        private const string IdentifiersLabel = "Identifiers";
        private const string IdentifiersToolTip = "Chartboost Mediation Unity SDK required identifiers.";
        private const string AndroidLabel = "Android";
        private const string AndroidToolTip = "Android identifiers.";  
        private const string IOSLabel = "iOS";
        private const string IOSToolTip = "iOS identifiers.";
        private const string AppIdentifier = "App Id";
        private const string AppSignature = "App Signature";
        private const string ClassCol = "col";
        private const string PartialIdToolTip = "for Chartboost Mediation Unity SDK.";
        private const string PartialFieldToolTip = "Fill this field to modify the";
        private const string SDKDebuggingLabel = "SDK Debugging";
        private const string SDKDebuggingToolTip = "Chartboost Mediation Unity SDK logging status.";
        private const string AutomaticInitializationLabel = "Automatic Initialization";
        private const string AutomaticInitializationToolTip = "Initialize Chartboost Mediation Unity SDK automatically.";
        private const string SDKKeysLabel = "SDK Keys";
        private const string SDKKeysToolTip = "Adapter associated SDK Keys with Build-Processing step features.";
        private const string GoogleAppIdAndroidLabel = "Google App Id - Android";
        private const string GoogleAppIOSAndroidLabel = "Google App Id - iOS";
        private const string AppLovinSDKKeyLabel = "Applovin SDK Key";
        private const string ClassKeyCol = "key-col";
        private const string PartialInputToolTip = "Fill this field to modify the";
        private const string SKAdNetworkLabel = "SKAdNetwork Resolution";
        private const string SKAdNetworkToolTip = "Use Chartboost Mediation Unity SDK SKAdNetwork identifier resolution.";
        private const string DisableBitCodeLabel = "Disable Bitcode";
        private const string DisableBitCodeToolTip = "Disables BitCode for XCode projects/workspaces.";
        private const string ClassSeparator = "separator";
        private const string ClassConfigCol = "config-col";
        private const string ClassUnityBaseFieldInput = "unity-base-field__input";
        private static readonly StyleEnum<FlexDirection> Direction = new StyleEnum<FlexDirection> { value = FlexDirection.Column };
        private static readonly StyleEnum<Align> Alignment = new StyleEnum<Align> { value = Align.Center };
    }
}
#endif

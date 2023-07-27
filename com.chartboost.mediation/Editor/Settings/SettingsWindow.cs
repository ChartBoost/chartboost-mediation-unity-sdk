using System;
using System.Linq;
using Chartboost.Editor.Adapters;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chartboost.Editor.Settings
{
    public class SettingsWindow : EditorWindow
    {
        private static SettingsWindow _instance;
        
        internal static SettingsWindow Instance {
            get
            {
                if (_instance != null)
                    return _instance;
                
                var settingsWindow = GetWindow<SettingsWindow>("Settings", typeof(AdaptersWindow));
                settingsWindow.minSize = Constants.MinWindowSize;
                _instance = settingsWindow;
                var scriptableInstance = ChartboostMediationSettings.Instance == null;
                if (!scriptableInstance)
                    Debug.LogWarning($"[Settings Window] Could not fetch ChartboostMediationSettings instance.");
                return _instance;
            }
        }
        
        public void CreateGUI() => Initialize();

        private void Initialize()
        {
            var root = rootVisualElement;
            root.styleSheets.Add(SettingsWindowConstants.StyleSheet.LoadAsset<StyleSheet>());
            root.name = "body";
            
            var scrollView = new ScrollView();
            scrollView.contentContainer.style.flexDirection = FlexDirection.Column;
            scrollView.contentContainer.style.flexWrap = Wrap.NoWrap;
            
            var settingsLabel = new Label("SDK Integration Settings") {
                name = "title",
                tooltip = "Chartboost Mediation Settings Window. Modify your integration settings through this window."
            };
            scrollView.Add(settingsLabel);
            scrollView.Add(CreateSeparator());
            scrollView.Add(CreateMediationIdsTable());
            scrollView.Add(CreateSeparator());
            scrollView.Add(CreateSDKConfigTogglesTable());
            scrollView.Add(CreateSeparator());

            var buildToolingLabel = new Label("Build-Processing Tools")
            {
                name = "title",
                tooltip = "Build pre-processor & post-processor tools offered through the Chartboost Mediation Unity SDK."
            };

            var helpBox = new HelpBox("The fields below toggle pre-processor & post-processor tools offered through the Chartboost Mediation Unity SDK. Use with caution.", HelpBoxMessageType.Warning);
            
            scrollView.Add(buildToolingLabel);
            scrollView.Add(CreateSeparator());
            scrollView.Add(helpBox);
            scrollView.Add(CreateSeparator());
            scrollView.Add(CreateSDKKeysTable());
            scrollView.Add(CreateSeparator());
            scrollView.Add(CreateBuildProcessingTogglesTable());
            
            root.Add(scrollView);
        }

        private static TemplateContainer CreateMediationIdsTable()
        {
            var container = new TemplateContainer();
            
            var headers = new TemplateContainer("headers") {
                name = "flex-grid"
            };

            var identifiersLabels = new Label("Identifiers") {
                name = "header",
                tooltip = "Chartboost Mediation Unity SDK required identifiers."
            };
            headers.Add(identifiersLabels);
            
            var androidIdentifiersLabel = new Label("Android") {
                name = "header",
                tooltip = "Android identifiers."
            };
            headers.Add(androidIdentifiersLabel);
            
            var iosIdentifiersLabel = new Label("iOS") {
                name = "header",
                tooltip = "IOS identifiers."
            };
            headers.Add(iosIdentifiersLabel);

            container.Add(headers);
            container.Add(CreateIdentifierTableRow("App Id", 
                (ChartboostMediationSettings.AndroidAppId, newValue => ChartboostMediationSettings.AndroidAppId = newValue), 
                (ChartboostMediationSettings.IOSAppId, newValue => ChartboostMediationSettings.IOSAppId = newValue)));
            container.Add(CreateIdentifierTableRow("App Signature", 
                (ChartboostMediationSettings.AndroidAppSignature, newValue => ChartboostMediationSettings.AndroidAppSignature = newValue), 
                (ChartboostMediationSettings.IOSAppSignature, newValue => ChartboostMediationSettings.IOSAppSignature = newValue)));

            return container;
            
            TemplateContainer CreateIdentifierTableRow(string label, (string, Action<string>) onAndroidChange, (string, Action<string>) onIOSChange) 
            {
                var retContainer = new TemplateContainer {
                    name = "flex-grid"
                };
                var idLabel = new Label(label) {
                    name = "col",
                    tooltip = $"{label} for Chartboost Mediation Unity SDK."
                };
                
                var androidAppIdInput = new TextField {
                    name = "col",
                    value = onAndroidChange.Item1,
                    tooltip = $"Fill this field to modify the Android {label}."
                };
                androidAppIdInput.RegisterValueChangedCallback(changeEvent => onAndroidChange.Item2?.Invoke(changeEvent.newValue));
                
                var iosAppIdInput = new TextField {
                    name = "col",
                    value = onIOSChange.Item1,
                    tooltip = $"Fill this field to modify the iOS {label}."
                };
                iosAppIdInput.RegisterValueChangedCallback(changeEvent => onIOSChange.Item2?.Invoke(changeEvent.newValue));
            
                retContainer.Add(idLabel);
                retContainer.Add(androidAppIdInput);
                retContainer.Add(iosAppIdInput);

                return retContainer;
            }
        }

        private static TemplateContainer CreateSDKConfigTogglesTable()
        {
            var retContainer = new TemplateContainer { name = "flex-grid" };
            retContainer.Add(CreateToggle("SDK Debugging", "Chartboost Mediation logging status.", (ChartboostMediationSettings.IsLoggingEnabled, newValue => ChartboostMediationSettings.IsLoggingEnabled = newValue)));
            retContainer.Add(CreateToggle("Automatic Initialization", "Initialize Chartboost Mediation automatically.", (ChartboostMediationSettings.IsAutomaticInitializationEnabled, newValue => ChartboostMediationSettings.IsAutomaticInitializationEnabled = newValue)));
            return retContainer;
        }

        private static TemplateContainer CreateSDKKeysTable()
        {
            var retContainer = new TemplateContainer();
            
            var identifiersLabels = new Label("SDK Keys") {
                name = "header",
                tooltip = "Adapter associated SDK Keys with Build-Processing step features."
            };
            
            retContainer.Add(identifiersLabels);
            retContainer.Add(CreateSDKKeyTableRow("Google App Id - Android", (ChartboostMediationSettings.AndroidGoogleAppId, newValue => ChartboostMediationSettings.AndroidGoogleAppId = newValue)));
            retContainer.Add(CreateSDKKeyTableRow("Google App Id - iOS", (ChartboostMediationSettings.IOSGoogleAppId, newValue => ChartboostMediationSettings.IOSGoogleAppId = newValue)));
            retContainer.Add(CreateSDKKeyTableRow("Applovin SDK Key", (ChartboostMediationSettings.AppLovinSDKKey, newValue => ChartboostMediationSettings.AppLovinSDKKey = newValue)));

            return retContainer;

            TemplateContainer CreateSDKKeyTableRow(string label, (string, Action<string>) onKeyChange)
            {
                var retRowContainer = new TemplateContainer {
                    name = "flex-grid"
                };
                var keyLabel = new Label(label) {
                    name = "key-col"
                };
                var sdkKeyInput = new TextField {
                    name = "col",
                    value = onKeyChange.Item1,
                    tooltip = $"Fill this field to modify the {label}."
                };
                sdkKeyInput.RegisterValueChangedCallback(changeEvent => onKeyChange.Item2?.Invoke(changeEvent.newValue));
                
                retRowContainer.Add(keyLabel);
                retRowContainer.Add(sdkKeyInput);

                return retRowContainer;
            }
        }

        private static TemplateContainer CreateBuildProcessingTogglesTable()
        {
            var retContainer = new TemplateContainer { name = "flex-grid" };
            retContainer.Add(CreateToggle("SKAdNetwork Resolution", "Use Chartboost Mediation SKAdNetwork identifier resolution.", (ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled, newValue => ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled = newValue)));
            retContainer.Add(CreateToggle("Disable Bitcode", "Disables BitCode for XCode projects/workspaces.", (ChartboostMediationSettings.DisableBitcode, newValue => ChartboostMediationSettings.DisableBitcode = newValue)));
            return retContainer;
        }

        private static VisualElement CreateSeparator()
        {
            var retSeparator = new VisualElement {
                name = "separator"
            };
            return retSeparator;
        }

        private static Toggle CreateToggle(string label, string toolTip, (bool, Action<bool>) toggleChanged)
        {
            var direction = new StyleEnum<FlexDirection> { value = FlexDirection.Column };
            var alignment = new StyleEnum<Align> { value = Align.Center };
            
            var retToggle = new Toggle(label) { name = "config-col", tooltip = toolTip, value = toggleChanged.Item1};
            retToggle.RegisterValueChangedCallback(changeEvent => toggleChanged.Item2?.Invoke(changeEvent.newValue));

            try
            {
                var fieldInputCustomToggle = retToggle.Children().First(x => x.ClassListContains("unity-base-field__input"));
                fieldInputCustomToggle.style.flexDirection = direction;
                fieldInputCustomToggle.style.alignSelf = alignment;
            }
            catch (NullReferenceException e)
            {
                Debug.LogWarning($"[Settings Window]Could not properly align toggles, with exception: {e}");
            }

            return retToggle;
        }
    }
}

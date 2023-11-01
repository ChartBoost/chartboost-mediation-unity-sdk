#if !NO_SETTINGS_WINDOW
using System;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chartboost.Editor.EditorWindows.Settings
{
    [CustomEditorWindow(typeof(SettingsWindow), WindowPriority)]
    public sealed partial class SettingsWindow : CustomEditorWindow<SettingsWindow>
    {
        public void CreateGUI()
        {
            var styleSheet = Resources.Load<StyleSheet>(SettingsWindowStyleSheet);
            rootVisualElement.styleSheets.Add(styleSheet);
            rootVisualElement.AddToClassList(ClassBody);
            
            var scrollView = new ScrollView();
            scrollView.contentContainer.style.flexDirection = FlexDirection.Column;
            scrollView.contentContainer.style.flexWrap = Wrap.NoWrap;

            var objectField = new ObjectField {
                objectType = typeof(ChartboostMediationSettings),
                value = ChartboostMediationSettings.Instance
            };

            objectField.RegisterValueChangedCallback(value =>
            {
                ChartboostMediationSettings.Instance = value.newValue as ChartboostMediationSettings;
                rootVisualElement.Clear();
                CreateGUI();
            });
            
            scrollView.Add(objectField);
            
            
            var settingsLabel = new Label(SettingsLabel) {
                tooltip = SettingsTooltip
            };
            settingsLabel.AddToClassList(ClassTitle);
            
            scrollView.Add(settingsLabel);
            scrollView.Add(CreateSeparator());
            scrollView.Add(CreateMediationIdsTable());
            scrollView.Add(CreateSeparator());
            scrollView.Add(CreateSDKConfigTogglesTable());
            scrollView.Add(CreateSeparator());

            var buildToolingLabel = new Label(BuildToolsLabel)
            {
                tooltip = BuildToolsToolTip
            };
            buildToolingLabel.AddToClassList(ClassTitle);

            var helpBox = new HelpBox(HelpBoxContents, HelpBoxMessageType.Warning);
            
            scrollView.Add(buildToolingLabel);
            scrollView.Add(CreateSeparator());
            scrollView.Add(helpBox);
            scrollView.Add(CreateSeparator());
            scrollView.Add(CreateSDKKeysTable());
            scrollView.Add(CreateSeparator());
            scrollView.Add(CreateBuildProcessingTogglesTable());
            
            rootVisualElement.Add(scrollView);
        }

        private static TemplateContainer CreateMediationIdsTable()
        {
            var container = new TemplateContainer();

            var headers = new TemplateContainer(IdHeaders);
            headers.AddToClassList(ClassFlexGrid);

            var identifiersLabels = new Label(IdentifiersLabel) {
                tooltip = IdentifiersToolTip
            };
            identifiersLabels.AddToClassList(ClassHeader);
            headers.Add(identifiersLabels);
            
            var androidIdentifiersLabel = new Label(AndroidLabel) {
                tooltip = AndroidToolTip
            };
            androidIdentifiersLabel.AddToClassList(ClassHeader);
            headers.Add(androidIdentifiersLabel);
            
            var iosIdentifiersLabel = new Label(IOSLabel) {
                tooltip = IOSToolTip
            };
            iosIdentifiersLabel.AddToClassList(ClassHeader);
            headers.Add(iosIdentifiersLabel);

            container.Add(headers);
            container.Add(CreateIdentifierTableRow(AppIdentifier, 
                (ChartboostMediationSettings.AndroidAppId, newValue => ChartboostMediationSettings.AndroidAppId = newValue), 
                (ChartboostMediationSettings.IOSAppId, newValue => ChartboostMediationSettings.IOSAppId = newValue)));
            container.Add(CreateIdentifierTableRow(AppSignature, 
                (ChartboostMediationSettings.AndroidAppSignature, newValue => ChartboostMediationSettings.AndroidAppSignature = newValue), 
                (ChartboostMediationSettings.IOSAppSignature, newValue => ChartboostMediationSettings.IOSAppSignature = newValue)));

            return container;
            
            TemplateContainer CreateIdentifierTableRow(string label, (string, Action<string>) onAndroidChange, (string, Action<string>) onIOSChange) 
            {
                var retContainer = new TemplateContainer();
                retContainer.AddToClassList(ClassFlexGrid);
                
                var idLabel = new Label(label) {
                    tooltip = $"{label} for {PartialIdToolTip}"
                };
                idLabel.AddToClassList(ClassCol);
                
                var androidAppIdInput = new TextField {
                    value = onAndroidChange.Item1,
                    tooltip = $"{PartialFieldToolTip} the Android {label}."
                };
                androidAppIdInput.AddToClassList(ClassCol);
                androidAppIdInput.RegisterValueChangedCallback(changeEvent => onAndroidChange.Item2?.Invoke(changeEvent.newValue));
                
                var iosAppIdInput = new TextField {
                    value = onIOSChange.Item1,
                    tooltip = $"{PartialFieldToolTip} the iOS {label}."
                };
                iosAppIdInput.AddToClassList(ClassCol);
                iosAppIdInput.RegisterValueChangedCallback(changeEvent => onIOSChange.Item2?.Invoke(changeEvent.newValue));
            
                retContainer.Add(idLabel);
                retContainer.Add(androidAppIdInput);
                retContainer.Add(iosAppIdInput);

                return retContainer;
            }
        }

        private static TemplateContainer CreateSDKConfigTogglesTable()
        {
            var retContainer = new TemplateContainer();
            retContainer.AddToClassList(ClassFlexGrid);
            retContainer.Add(CreateToggle(SDKDebuggingLabel, SDKDebuggingToolTip, (ChartboostMediationSettings.IsLoggingEnabled, newValue => ChartboostMediationSettings.IsLoggingEnabled = newValue)));
            retContainer.Add(CreateToggle(AutomaticInitializationLabel, AutomaticInitializationToolTip, (ChartboostMediationSettings.IsAutomaticInitializationEnabled, newValue => ChartboostMediationSettings.IsAutomaticInitializationEnabled = newValue)));
            return retContainer;
        }

        private static TemplateContainer CreateSDKKeysTable()
        {
            var retContainer = new TemplateContainer();
            
            var identifiersLabels = new Label(SDKKeysLabel) {
                tooltip = SDKKeysToolTip
            };
            identifiersLabels.AddToClassList(ClassHeader);
            
            retContainer.Add(identifiersLabels);
            retContainer.Add(CreateSDKKeyTableRow(GoogleAppIdAndroidLabel, (ChartboostMediationSettings.AndroidGoogleAppId, newValue => ChartboostMediationSettings.AndroidGoogleAppId = newValue)));
            retContainer.Add(CreateSDKKeyTableRow(GoogleAppIOSAndroidLabel, (ChartboostMediationSettings.IOSGoogleAppId, newValue => ChartboostMediationSettings.IOSGoogleAppId = newValue)));
            retContainer.Add(CreateSDKKeyTableRow(AppLovinSDKKeyLabel, (ChartboostMediationSettings.AppLovinSDKKey, newValue => ChartboostMediationSettings.AppLovinSDKKey = newValue)));

            return retContainer;

            TemplateContainer CreateSDKKeyTableRow(string label, (string, Action<string>) onKeyChange)
            {
                var retRowContainer = new TemplateContainer();
                retRowContainer.AddToClassList(ClassFlexGrid);
                
                var keyLabel = new Label(label);
                keyLabel.AddToClassList(ClassKeyCol);
                
                var sdkKeyInput = new TextField {
                    value = onKeyChange.Item1,
                    tooltip = $"{PartialInputToolTip} {label}."
                };
                sdkKeyInput.AddToClassList(ClassCol);
                sdkKeyInput.RegisterValueChangedCallback(changeEvent => onKeyChange.Item2?.Invoke(changeEvent.newValue));
                
                retRowContainer.Add(keyLabel);
                retRowContainer.Add(sdkKeyInput);

                return retRowContainer;
            }
        }

        private static TemplateContainer CreateBuildProcessingTogglesTable()
        {
            var retContainer = new TemplateContainer();
            retContainer.AddToClassList(ClassFlexGrid);
            retContainer.Add(CreateToggle(SKAdNetworkLabel, SKAdNetworkToolTip, (ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled, newValue => ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled = newValue)));
            retContainer.Add(CreateToggle(DisableBitCodeLabel, DisableBitCodeToolTip, (ChartboostMediationSettings.DisableBitcode, newValue => ChartboostMediationSettings.DisableBitcode = newValue)));
            return retContainer;
        }

        private static VisualElement CreateSeparator()
        {
            var retSeparator = new VisualElement();
            retSeparator.AddToClassList(ClassSeparator);
            return retSeparator;
        }

        private static Toggle CreateToggle(string label, string toolTip, (bool, Action<bool>) toggleChanged)
        {
            var retToggle = new Toggle(label) { tooltip = toolTip, value = toggleChanged.Item1 };
            retToggle.AddToClassList(ClassConfigCol);
            retToggle.RegisterValueChangedCallback(changeEvent => toggleChanged.Item2?.Invoke(changeEvent.newValue));

            try
            {
                var fieldInputCustomToggle = retToggle.Children().First(x => x.ClassListContains(ClassUnityBaseFieldInput));
                fieldInputCustomToggle.style.flexDirection = Direction;
                fieldInputCustomToggle.style.alignSelf = Alignment;
            }
            catch (NullReferenceException e)
            {
                Debug.LogWarning($"[Settings Window] Could not properly align toggles, with exception: {e}");
            }
            return retToggle;
        }
    }
}
#endif

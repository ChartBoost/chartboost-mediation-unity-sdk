using System;
using System.Collections.Generic;
using Chartboost.Editor.Adapters.Serialization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chartboost.Editor.Adapters
{
    public partial class AdaptersWindow : EditorWindow
    {
        [MenuItem("Chartboost Mediation/Adapters")]
        public static void ShowAdapterWindow()
        {
            AdaptersWindow wnd = GetWindow<AdaptersWindow>();
            wnd.titleContent = new GUIContent("Chartboost Mediation Adapters");
            var windowSize =new Vector2(420, 520);
            wnd.minSize = windowSize;
        }

        public static Dictionary<string, PartnerVersions> PartnerSDKVersions { get; set; } = new Dictionary<string, PartnerVersions>();
        public static Dictionary<string, AdapterSelection> UserSelectedVersions { get; set; } = new Dictionary<string, AdapterSelection>();
        public static Dictionary<string, AdapterSelection> SavedVersions { get; set; } = new Dictionary<string, AdapterSelection>();
        private static string MediationSelection { get; set; }
       
        private static Button _saveButton;

        private static AdaptersWindow Instance { get; set; }

        public void CreateGUI()
        {
            Initialize();
        }

        private void Initialize()
        {
            Instance = this;
            PartnerSDKVersions.Clear();
            UserSelectedVersions.Clear();
            SavedVersions.Clear();
            _saveButton = CreateSaveButton();
            LoadSelections();
            
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;
            root.styleSheets.Add(Constants.StyleSheet.LoadAsset<StyleSheet>());
            root.name = "body";
            
            CreateTableHeaders(root);
            CreateAdapterTable(root);
            CheckMediationVersion();
        }

        private void CheckMediationVersion()
        {
            var logo = new Image
            {
                name = "icon",
                image = Constants.WarningPNG.LoadAsset<Texture>(),
                scaleMode = ScaleMode.ScaleToFit
            };
            
            var package = Utilities.FindPackage(Constants.ChartboostMediationPackageName);
            
            var warningFixButton = new Button();
            warningFixButton.name = "warning-button";
            warningFixButton.Add(logo);
            warningFixButton.clicked += FixWarning;
            
            if (string.IsNullOrEmpty(MediationSelection) || !Constants.PathToMainDependency.FileExist())
            {
                warningFixButton.tooltip = $"Dependencies for Chartboost Mediation {package.version} have not been found. Press to add.";
                rootVisualElement.Add(warningFixButton);
                return;
            }
            
            var version = new Version(MediationSelection);
            var packageVersion = new Version(package.version);
            
            if (version != packageVersion)
            {
                warningFixButton.tooltip = $"Your selected dependencies for Chartboost Mediation {version} do not match your current package version {packageVersion}. Press to fix.";
                rootVisualElement.Add(warningFixButton);
            }

            void FixWarning()
            {
                warningFixButton.clicked -= FixWarning;
                warningFixButton.RemoveFromHierarchy();
                MediationSelection = package.version;
                GenerateChartboostMediationDependency();
            }
        }

        private static void CreateTableHeaders(VisualElement root)
        {
            var logo = new Image
            {
                name = "mediation-logo",
                image = Constants.LogoPNG.LoadAsset<Texture>(),
                scaleMode = ScaleMode.ScaleToFit
            };
            
            var upgradeImage = new Image
            {
                name = "icon",
                image = Constants.UpgradePNG.LoadAsset<Texture>(),
                scaleMode = ScaleMode.ScaleToFit
            };

            var upgradeButton = new Button(UpgradeSelectionsToLatest);
            upgradeButton.name = "upgrade-button";
            upgradeButton.tooltip = "Upgrade all adapter selections to their latest version!";
            upgradeButton.Add(upgradeImage);
            
            var refreshImage = new Image
            {
                name = "icon",
                image = Constants.RefreshPNG.LoadAsset<Texture>(),
                scaleMode = ScaleMode.ScaleToFit
            };

            var refreshButton = new Button(Refresh);
            refreshButton.name = "refresh-button";
            refreshButton.tooltip = "Fetch adapter updates!";
            refreshButton.Add(refreshImage);
            
            root.Add(logo);
            root.Add(upgradeButton);
            root.Add(refreshButton);
        }

        private static void CreateAdapterTable(VisualElement root)
        {
            var adapters = AdapterDataSource.LoadedAdapters.adapters;

            if (adapters == null)
                return;
            
            var scrollView = new ScrollView();
            scrollView.contentContainer.style.flexDirection = FlexDirection.Column;
            scrollView.contentContainer.style.flexWrap = Wrap.NoWrap;
            
            var headers = new TemplateContainer("headers");
            headers.name = "flex-grid";

            var adapterNameLabel = new Label("Network");
            adapterNameLabel.name = "header-network";
            adapterNameLabel.tooltip = "Network Associated with Ad Adapter.";
            headers.Add(adapterNameLabel);
        
            var androidVersionLabel = new Label("Android");
            androidVersionLabel.name = "header-version";
            adapterNameLabel.tooltip = "Android Version of Ad Adapters.";
            headers.Add(androidVersionLabel);
        
            var iosVersionLabel = new Label("iOS");
            iosVersionLabel.name = "header-version";
            iosVersionLabel.tooltip = "iOS Version of Ad Adapters.";
            headers.Add(iosVersionLabel);
        
            scrollView.Add(headers);
            
            foreach (var partnerAdapter in adapters)
                PartnerSDKVersions[partnerAdapter.id] = new PartnerVersions(partnerAdapter.android.versions, partnerAdapter.ios.versions);

            foreach (var adapter in adapters)
            {
                var container = new TemplateContainer(adapter.id);
                container.name = "flex-grid";

                var adapterLabel = new Label(adapter.name);
                adapterLabel.name = "adapter-col";
                container.Add(adapterLabel);

                var adapterId = adapter.id;
                var androidVersions = PartnerSDKVersions[adapterId].android;
                var iosVersions = PartnerSDKVersions[adapterId].ios;

                var hasSelection = UserSelectedVersions.ContainsKey(adapterId);
                var androidStartValue = hasSelection && UserSelectedVersions[adapterId] != null ? UserSelectedVersions[adapterId].android : Constants.Unselected;
                var iosStartValue = hasSelection && UserSelectedVersions[adapterId] != null ? UserSelectedVersions[adapterId].ios : Constants.Unselected;

                var androidDropdown = CreateAdapterVersionDropdown(root, adapter, androidVersions, Platform.Android, androidStartValue);
                var iosDropdown = CreateAdapterVersionDropdown(root, adapter, iosVersions, Platform.IOS, iosStartValue);

                if (hasSelection)
                {
                    UserSelectedVersions[adapterId].androidDropdown = androidDropdown;
                    UserSelectedVersions[adapterId].iosDropdown = iosDropdown;
                }

                container.Add(androidDropdown);
                container.Add(iosDropdown);
 
                scrollView.Add(container);
            }
            
            root.Add(scrollView);
        }
        
        private static ToolbarMenu CreateAdapterVersionDropdown(VisualElement root, Adapter adapter, IEnumerable<string> versions, Platform platform, string startValue)
        {
            var toolbar = new ToolbarMenu {
                name = "version-col",
                text = startValue,
                tooltip = $"{adapter.name} {platform} SDK Version."
            };
                
            foreach (var version in versions)
            {
                toolbar.menu.AppendAction(version, (dropdownEvent) =>
                {
                    var selection = dropdownEvent.name;
                    
                    // User selected the currently set value
                    if (selection.Equals(toolbar.text))
                        return;

                    toolbar.text = selection;
                    
                    if (!UserSelectedVersions.ContainsKey(adapter.id))
                        UserSelectedVersions[adapter.id] = new AdapterSelection(adapter.id);
                    
                    switch (platform)
                    {
                        case Platform.Android:
                            UserSelectedVersions[adapter.id].android = selection;
                            UserSelectedVersions[adapter.id].androidDropdown = toolbar;
                            break;
                        case Platform.IOS:
                            UserSelectedVersions[adapter.id].ios = selection;
                            UserSelectedVersions[adapter.id].iosDropdown = toolbar;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
                    }

                    if (selection.Equals(Constants.Unselected) && UserSelectedVersions.ContainsKey(adapter.id))
                    {
                        if (UserSelectedVersions[adapter.id].android == Constants.Unselected && UserSelectedVersions[adapter.id].ios == Constants.Unselected)
                            UserSelectedVersions.Remove(adapter.id);
                    }
                    
                    CheckForChanges();
                });
                
                toolbar.menu.AppendSeparator();
            }
            return toolbar;
        }

        private static Button CreateSaveButton()
        {
            var saveIconImage = new Image
            {
                name = "icon",
                image = Constants.SavePNG.LoadAsset<Texture>(),
                scaleMode = ScaleMode.ScaleToFit
            };

            var saveButton = new Button(SaveSelections);
            saveButton.name = "save-button";
            saveButton.tooltip = "You have unsaved changes!";
            saveButton.Add(saveIconImage);
            return saveButton;
        }
    }
}

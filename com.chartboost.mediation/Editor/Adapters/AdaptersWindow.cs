using System;
using System.Collections.Generic;
using Chartboost.Editor.Adapters.Serialization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Chartboost.Editor.Adapters
{
    public partial class AdaptersWindow : EditorWindow
    {
        /// <summary>
        /// Currently loaded partner networks versions.
        /// </summary>
        public static Dictionary<string, PartnerVersions> PartnerSDKVersions { get; } = new Dictionary<string, PartnerVersions>();
        
        /// <summary>
        /// Currently loaded user selections.
        /// </summary>
        public static Dictionary<string, AdapterSelection> UserSelectedVersions { get; } = new Dictionary<string, AdapterSelection>();
        
        /// <summary>
        /// Currently saved user selections.
        /// </summary>
        public static Dictionary<string, AdapterSelection> SavedVersions { get; set; } = new Dictionary<string, AdapterSelection>();
        
        /// <summary>
        /// Currently selected Chartboost Mediation version.
        /// </summary>
        private static string MediationSelection { get; set; }
       
        private static Button _saveButton;
        private static Button _warningButton;
        
        private static PackageInfo ChartboostMediationPackage => _mediationPackage ??= Utilities.FindPackage(AdapterWindowConstants.ChartboostMediationPackageName);
        private static PackageInfo _mediationPackage;
        
        internal static AdaptersWindow Instance {
            get
            {
                if (_instance != null)
                    return _instance;
                
                var adaptersWindow = GetWindow<AdaptersWindow>("Adapters");
                adaptersWindow.minSize = Constants.MinWindowSize;
                _instance = adaptersWindow;
                return _instance;
            }
        }

        private static AdaptersWindow _instance;

        public void CreateGUI() => Initialize();

        /// <summary>
        /// Initializes GUIComponents
        /// </summary>
        private static void Initialize()
        {
            PartnerSDKVersions.Clear();
            UserSelectedVersions.Clear();
            SavedVersions.Clear();
            _saveButton = CreateSaveButton();
            LoadSelections();
            
            // Each editor window contains a root VisualElement object
            var root = Instance.rootVisualElement;
            root.styleSheets.Add(AdapterWindowConstants.StyleSheet.LoadAsset<StyleSheet>());
            root.name = "body";
            
            CreateTableHeaders(root);
            CreateAdapterTable(root);
            CheckMediationVersion(root);
        }
        
        private static void CheckMediationVersion(VisualElement root)
        {
            var logo = new Image
            {
                name = "icon",
                image = AdapterWindowConstants.WarningPNG.LoadAsset<Texture>(),
                scaleMode = ScaleMode.ScaleToFit
            };
            
            _warningButton = new Button(() => CheckChartboostMediationVersion());
            _warningButton.name = "warning-button";
            _warningButton.Add(logo);
            
            if (string.IsNullOrEmpty(MediationSelection) || !AdapterWindowConstants.PathToMainDependency.FileExist())
            {
                _warningButton.tooltip = $"Dependencies for Chartboost Mediation {ChartboostMediationPackage.version} have not been found. Press to add.";
                root.Add(_warningButton);
                return;
            }
            
            var version = new Version(MediationSelection);
            var packageVersion = new Version(ChartboostMediationPackage.version);

            if (version.Minor != packageVersion.Minor)
            {
                _warningButton.tooltip = $"Your selected dependencies for Chartboost Mediation {version} do not match your current package version {packageVersion}. Press to fix.";
                root.Add(_warningButton);
            }
        }

        private static void CreateTableHeaders(VisualElement root)
        {
            var logo = new Image
            {
                name = "mediation-logo",
                image = AdapterWindowConstants.LogoPNG.LoadAsset<Texture>(),
                scaleMode = ScaleMode.ScaleToFit
            };
            
            var upgradeImage = new Image
            {
                name = "icon",
                image = AdapterWindowConstants.UpgradePNG.LoadAsset<Texture>(),
                scaleMode = ScaleMode.ScaleToFit
            };

            var upgradeButton = new Button(() => UpgradePlatformToLatest(Platform.Android | Platform.IOS));
            upgradeButton.name = "upgrade-button";
            upgradeButton.tooltip = "Upgrade all adapter selections to their latest version!";
            upgradeButton.Add(upgradeImage);
            
            var refreshImage = new Image
            {
                name = "icon",
                image = AdapterWindowConstants.RefreshPNG.LoadAsset<Texture>(),
                scaleMode = ScaleMode.ScaleToFit
            };

            var refreshButton = new Button(() => Refresh(false));
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
            androidVersionLabel.tooltip = "Android Version of Ad Adapters.";
            headers.Add(androidVersionLabel);
        
            var iosVersionLabel = new Label("iOS");
            iosVersionLabel.name = "header-version";
            iosVersionLabel.tooltip = "iOS Version of Ad Adapters.";
            headers.Add(iosVersionLabel);
        
            scrollView.Add(headers);

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
                var androidStartValue = hasSelection && UserSelectedVersions[adapterId] != null ? UserSelectedVersions[adapterId].android : AdapterWindowConstants.Unselected;
                var iosStartValue = hasSelection && UserSelectedVersions[adapterId] != null ? UserSelectedVersions[adapterId].ios : AdapterWindowConstants.Unselected;

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

                    if (selection.Equals(AdapterWindowConstants.Unselected) && UserSelectedVersions.ContainsKey(adapter.id))
                    {
                        if (UserSelectedVersions[adapter.id].android == AdapterWindowConstants.Unselected && UserSelectedVersions[adapter.id].ios == AdapterWindowConstants.Unselected)
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
                image = AdapterWindowConstants.SavePNG.LoadAsset<Texture>(),
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

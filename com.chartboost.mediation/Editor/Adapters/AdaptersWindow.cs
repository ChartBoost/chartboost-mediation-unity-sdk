using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chartboost.Adapters
{
    public partial class AdaptersWindow : EditorWindow
    {
        [MenuItem("Chartboost Mediation/Adapters")]
        public static void ShowAdapterWindow()
        {
            AdaptersWindow wnd = GetWindow<AdaptersWindow>();
            wnd.titleContent = new GUIContent("Chartboost Mediation Adapters");
            var windowSize =new Vector2(410, 520);
            wnd.minSize = windowSize;
            wnd.maxSize = windowSize;
        }

        public static Dictionary<string, PartnerVersions> PartnerSDKVersions { get; set; } = new Dictionary<string, PartnerVersions>();
        public static Dictionary<string, SelectedVersions> UserSelectedVersions { get; set; } = new Dictionary<string, SelectedVersions>();
        public static Dictionary<string, SelectedVersions> SavedVersions { get; set; } = new Dictionary<string, SelectedVersions>();
       
        private static Button _saveButton;

        private const string Unselected = "Unselected";

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
            _saveButton = CreateSaveIcon();
            LoadSelections();

            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.chartboost.mediation/Editor/Adapters/AdaptersWindow.uss");
        
            root.styleSheets.Add(styleSheet);
            root.name = "body";

            CreateTableHeaders(root);
            CreateAdapterTable(root);
        }

        private static void CreateTableHeaders(VisualElement root)
        {
            var mediationLogo = AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.chartboost.mediation/Editor/Adapters/Logo.png");
            var logo = new Image
            {
                name = "mediation-logo",
                image = mediationLogo,
                scaleMode = ScaleMode.ScaleToFit
            };
            
            var upgradeIcon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.chartboost.mediation/Editor/Adapters/Upgrade.png");
            var upgradeImage = new Image
            {
                name = "icon",
                image = upgradeIcon,
                scaleMode = ScaleMode.ScaleToFit
            };

            var upgradeButton = new Button(UpgradeSelectionsToLatest);
            upgradeButton.name = "upgrade-button";
            upgradeButton.tooltip = "Upgrade all adapter selections to their latest version!";
            upgradeButton.Add(upgradeImage);
            
                  
            var updateIcon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.chartboost.mediation/Editor/Adapters/Update.png");
            var updateImage = new Image
            {
                name = "icon",
                image = updateIcon,
                scaleMode = ScaleMode.ScaleToFit
            };

            var updateButton = new Button(Refresh);
            updateButton.name = "update-button";
            updateButton.tooltip = "Fetch adapter updates!";
            updateButton.Add(updateImage);
            
            root.Add(logo);
            root.Add(upgradeButton);
            root.Add(updateButton);
            
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
        
            root.Add(headers);
        }

        private static void CreateAdapterTable(VisualElement root)
        {
            var adapters = AdapterDataSource.LoadedAdapters.adapters;

            if (adapters == null)
                return;
            
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
                var androidStartValue = hasSelection && UserSelectedVersions[adapterId] != null ? UserSelectedVersions[adapterId].android : Unselected;
                var iosStartValue = hasSelection && UserSelectedVersions[adapterId] != null ? UserSelectedVersions[adapterId].ios : Unselected;

                var androidDropdown = CreateAdapterVersionDropdown(root, adapter, androidVersions, Platform.Android, androidStartValue);
                var iosDropdown = CreateAdapterVersionDropdown(root, adapter, iosVersions, Platform.IOS, iosStartValue);

                if (hasSelection)
                {
                    UserSelectedVersions[adapterId].androidDropdown = androidDropdown;
                    UserSelectedVersions[adapterId].iosDropdown = iosDropdown;
                }

                container.Add(androidDropdown);
                container.Add(iosDropdown);
 
                root.Add(container);
            }
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
                        UserSelectedVersions[adapter.id] = new SelectedVersions(adapter.id);
                    
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

                    if (selection.Equals(Unselected) && UserSelectedVersions.ContainsKey(adapter.id))
                    {
                        if (UserSelectedVersions[adapter.id].android == Unselected && UserSelectedVersions[adapter.id].ios == Unselected)
                            UserSelectedVersions.Remove(adapter.id);
                    }
                    
                    CheckForChanges();
                });
                
                toolbar.menu.AppendSeparator();
            }
            return toolbar;
        }

        private static Button CreateSaveIcon()
        {
            var saveIcon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.chartboost.mediation/Editor/Adapters/Save.png");
            var saveIconImage = new Image
            {
                name = "icon",
                image = saveIcon,
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

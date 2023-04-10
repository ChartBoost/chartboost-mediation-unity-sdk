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
            Instance = this;
            PartnerSDKVersions.Clear();
            UserSelectedVersions.Clear();
            SavedVersions.Clear();
            _saveButton = CreateSaveIcon();
            LoadSelection();

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
                name = "mediation_logo",
                image = mediationLogo,
                scaleMode = ScaleMode.ScaleToFit
            };

            root.Add(logo);
            
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
            
                var androidVersions = PartnerSDKVersions[adapter.id].Android;
                var iosVersions = PartnerSDKVersions[adapter.id].IOS;
                
                var androidStartValue = UserSelectedVersions.ContainsKey(adapter.id) && UserSelectedVersions[adapter.id] != null
                    ? UserSelectedVersions[adapter.id].android : Unselected;
            
                var iosStartValue = UserSelectedVersions.ContainsKey(adapter.id) && UserSelectedVersions[adapter.id] != null
                    ? UserSelectedVersions[adapter.id].ios : Unselected;

                container.Add(CreateAdapterVersionDropdown(root, adapter, androidVersions, Platform.Android, androidStartValue));
                container.Add(CreateAdapterVersionDropdown(root, adapter, iosVersions, Platform.IOS, iosStartValue));
 
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
                            break;
                        case Platform.IOS:
                            UserSelectedVersions[adapter.id].ios = selection;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
                    }

                    if (selection.Equals(Unselected) && UserSelectedVersions.ContainsKey(adapter.id))
                    {
                        if (UserSelectedVersions[adapter.id].android == Unselected && UserSelectedVersions[adapter.id].ios == Unselected)
                            UserSelectedVersions.Remove(adapter.id);
                    }

                    var same = new DictionaryComparer<string, SelectedVersions>(new SelectedVersionsComparer()).Equals(UserSelectedVersions, SavedVersions);
                    switch (same)
                    {
                        case false when !root.Contains(_saveButton):
                            root.Add(_saveButton);
                            break;
                        case true:
                            _saveButton.RemoveFromHierarchy();
                            break;
                    }
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
                name = "save_icon",
                image = saveIcon,
                scaleMode = ScaleMode.ScaleToFit
            };

            var saveButton = new Button(SaveSelection);
            saveButton.name = "save_button";
            saveButton.tooltip = "You have unsaved changes!";
            saveButton.Add(saveIconImage);
            return saveButton;
        }

        private void OnDestroy()
        {
            Debug.Log("Closing Window");
        }
    }
}

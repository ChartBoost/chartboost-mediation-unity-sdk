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
        public static void ShowExample()
        {
            AdaptersWindow wnd = GetWindow<AdaptersWindow>();
            wnd.titleContent = new GUIContent("Chartboost Mediation Adapters");
            wnd.minSize = new Vector2(410, 500);
        }

        private readonly Dictionary<string, PartnerVersions> _partnerSDKVersions = new Dictionary<string, PartnerVersions>();

        private readonly Dictionary<string, ProjectSelectedVersions> _selectedVersions = new Dictionary<string, ProjectSelectedVersions>();

        private static Button _saveButton;

        private const string UNSELECTED = "Unselected";

        public static AdaptersWindow Instance { get; private set; }

        public void CreateGUI()
        {
            Instance = this;
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

            var partnerSDKVersions = Instance._partnerSDKVersions;
            var selectedVersions = Instance._selectedVersions;
        
            
            foreach (var partnerAdapter in adapters)
                partnerSDKVersions[partnerAdapter.id] = new PartnerVersions(partnerAdapter.android.versions, partnerAdapter.ios.versions);

            foreach (var adapter in adapters)
            {
                var container = new TemplateContainer(adapter.id);
                container.name = "flex-grid";

                var adapterLabel = new Label(adapter.name);
                adapterLabel.name = "adapter-col";
                container.Add(adapterLabel);
            
                var androidVersions = partnerSDKVersions[adapter.id].Android;
                var iosVersions = partnerSDKVersions[adapter.id].IOS;
                
                var androidSelection = selectedVersions.ContainsKey(adapter.id) && selectedVersions[adapter.id] != null
                    ? selectedVersions[adapter.id].android : UNSELECTED;
            
                var iosSelection = selectedVersions.ContainsKey(adapter.id) && selectedVersions[adapter.id] != null
                    ? selectedVersions[adapter.id].ios : UNSELECTED;

                container.Add(CreateAdapterVersionDropdown(root, adapter, androidVersions, Platform.Android, androidSelection));
                container.Add(CreateAdapterVersionDropdown(root, adapter, iosVersions, Platform.IOS, iosSelection));
 
                root.Add(container);
            }
        }
        
        private static ToolbarMenu CreateAdapterVersionDropdown(VisualElement root, Adapter adapter, IEnumerable<string> versions, Platform platform, string selectedVersion)
        {
            var toolbar = new ToolbarMenu {
                name = "version-col",
                text = selectedVersion,
                tooltip = $"{adapter.name} {platform} SDK Version."
            };
                
            foreach (var version in versions)
            {
                toolbar.menu.AppendAction(version, (dropdownEvent) =>
                {
                    var selection = dropdownEvent.name;

                    if (selection.Equals(toolbar.text))
                        return;

                    toolbar.text = selection;
                        
                    if (!Instance._selectedVersions.ContainsKey(adapter.id))
                        Instance._selectedVersions[adapter.id] = new ProjectSelectedVersions(adapter.id); 

                    switch (platform)
                    {
                        case Platform.Android:
                            Instance._selectedVersions[adapter.id].android = selection;
                            break;
                        case Platform.IOS:
                            Instance._selectedVersions[adapter.id].ios = selection;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(platform), platform, null);
                    }

                    if (!root.Contains(_saveButton))
                        root.Add(_saveButton);
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

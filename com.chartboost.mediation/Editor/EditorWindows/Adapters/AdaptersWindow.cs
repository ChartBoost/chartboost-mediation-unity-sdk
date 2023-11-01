#if !NO_ADAPTERS_WINDOW
using System;
using System.Collections.Generic;
using Chartboost.Editor.EditorWindows.Adapters.Serialization;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Chartboost.Editor.EditorWindows.Adapters
{
    [CustomEditorWindow(typeof(AdaptersWindow), 0)]
    public sealed partial class AdaptersWindow : CustomEditorWindow<AdaptersWindow>
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
        
        private static VisualElement _root;

        public void CreateGUI()
        {
            PartnerSDKVersions.Clear();
            UserSelectedVersions.Clear();
            SavedVersions.Clear();
            _saveButton = CreateSaveButton();
            LoadSelections();
            
            // Each editor window contains a root VisualElement object

            var styleSheet = Resources.Load<StyleSheet>(StyleSheet);
            rootVisualElement.styleSheets.Add(styleSheet);
            rootVisualElement.AddToClassList(ClassBody);
            
            CreateTableHeaders(rootVisualElement);
            CreateAdapterTable(rootVisualElement);
            CheckMediationVersion(rootVisualElement);
        }
        
        private static void CheckMediationVersion(VisualElement root)
        {
            var logo = new Image
            {
                image = Resources.Load<Texture>(WarningPNG),
                scaleMode = ScaleMode.ScaleToFit
            };
            logo.AddToClassList(ClassIcon);
            
            _warningButton = new Button(() => CheckChartboostMediationVersion());
            _warningButton.AddToClassList(ClassWarningButton);
            _warningButton.Add(logo);
            
            if (string.IsNullOrEmpty(MediationSelection) || !PathToMainDependency.FileExist())
            {
                _warningButton.tooltip = $"Dependencies for Chartboost Mediation {ChartboostMediation.Version} have not been found. Press to add.";
                root.Add(_warningButton);
                return;
            }
            
            var version = new Version(MediationSelection);
            var packageVersion = new Version(ChartboostMediation.Version);

            if (version.Minor != packageVersion.Minor)
            {
                _warningButton.tooltip = $"Your selected dependencies for Chartboost Mediation {version} do not match your current package version {packageVersion}. Press to fix.";
                root.Add(_warningButton);
            }
        }

        private static void CreateTableHeaders(VisualElement root)
        {
            var mediationImage = new Image
            {
                image = Resources.Load<Texture>(LogoPNG),
                scaleMode = ScaleMode.ScaleToFit
            };
            mediationImage.AddToClassList(ClassMediationLogo);
            
            var upgradeImage = new Image
            {
                image = Resources.Load<Texture>(UpgradePNG),
                scaleMode = ScaleMode.ScaleToFit
            };
            upgradeImage.AddToClassList(ClassIcon);

            var upgradeButton = new Button(() => UpgradePlatformToLatest(Platform.Android | Platform.IOS));
            upgradeButton.tooltip = ToolTipUpgradeButton;
            upgradeButton.AddToClassList(ClassUpgradeButton);
            upgradeButton.Add(upgradeImage);
            
            var refreshImage = new Image
            {
                image = Resources.Load<Texture>(RefreshPNG),
                scaleMode = ScaleMode.ScaleToFit
            };
            refreshImage.AddToClassList(ClassIcon);

            var refreshButton = new Button(() => AdaptersWindow.Refresh(false));
            refreshButton.AddToClassList(ClassRefreshButton);
            refreshButton.tooltip = ToolTipRefreshButton;
            refreshButton.Add(refreshImage);
            
            root.Add(mediationImage);
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
            
            var headers = new TemplateContainer();
            headers.AddToClassList(ClassFlexGrid);

            var adapterNameLabel = new Label(LabelNetwork);
            adapterNameLabel.AddToClassList(ClassHeaderNetwork);
            adapterNameLabel.tooltip = ToolTipNetwork;
            headers.Add(adapterNameLabel);
        
            var androidVersionLabel = new Label(LabelAndroid);
            androidVersionLabel.AddToClassList(ClassHeaderVersion);
            androidVersionLabel.tooltip = ToolTipAndroidVersion;
            headers.Add(androidVersionLabel);
        
            var iosVersionLabel = new Label(LabelIOS);
            iosVersionLabel.AddToClassList(ClassHeaderVersion);
            iosVersionLabel.tooltip = ToolTipIOSVersion;
            headers.Add(iosVersionLabel);
        
            scrollView.Add(headers);

            foreach (var adapter in adapters)
            {
                var container = new TemplateContainer(adapter.id);
                container.AddToClassList(ClassFlexGrid);

                var adapterLabel = new Label(adapter.name);
                adapterLabel.AddToClassList(ClassAdapterCol);
                container.Add(adapterLabel);

                var adapterId = adapter.id;
                var androidVersions = PartnerSDKVersions[adapterId].android;
                var iosVersions = PartnerSDKVersions[adapterId].ios;

                var hasSelection = UserSelectedVersions.ContainsKey(adapterId);
                var androidStartValue = hasSelection && UserSelectedVersions[adapterId] != null ? UserSelectedVersions[adapterId].android : Unselected;
                var iosStartValue = hasSelection && UserSelectedVersions[adapterId] != null ? UserSelectedVersions[adapterId].ios : Unselected;

                var androidDropdown = CreateAdapterVersionDropdown(adapter, androidVersions, Platform.Android, androidStartValue);
                var iosDropdown = CreateAdapterVersionDropdown(adapter, iosVersions, Platform.IOS, iosStartValue);

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
        
        private static ToolbarMenu CreateAdapterVersionDropdown(Adapter adapter, IEnumerable<string> versions, Platform platform, string startValue)
        {
            var toolbar = new ToolbarMenu {
                text = startValue,
                tooltip = $"{adapter.name} {platform} SDK Version."
            };
            toolbar.AddToClassList(ClassVersionCol);
                
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

        private static Button CreateSaveButton()
        {
            var saveIconImage = new Image
            {
                image = Resources.Load<Texture>(SavePNG),
                scaleMode = ScaleMode.ScaleToFit
            };
            saveIconImage.AddToClassList(ClassIcon);

            var saveButton = new Button(AdaptersWindow.SaveSelections);
            saveButton.AddToClassList(ClassSaveButton);
            saveButton.tooltip = ToolTipSaveButton;
            saveButton.Add(saveIconImage);
            return saveButton;
        }
    }
}
#endif

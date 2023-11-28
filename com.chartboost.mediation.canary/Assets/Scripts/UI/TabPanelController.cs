using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The controller for the tab panel. The panel consists of a horizontal row
/// of buttons each of which loads a unique controller within the assigned
/// panel.
/// </summary>
public class TabPanelController : SimpleSingleton<TabPanelController>
{
    /// <summary>
    /// An enumeration with values that represent each tab within the panel.
    /// </summary>
    private enum Tab {
        Banner,
        BannerView,
        UnityBanner,
        Fullscreen,
        Interstitial,
        Rewarded,
        Settings
    }

    /// <summary>
    /// Configurations for each tab. There must be as many configurations
    /// defined on the script on the GameObject as there are enumeration
    /// values defined in the `Tab` enum.
    /// </summary>
    public TabConfiguration[] configurations;

    /// <summary>
    /// The `RectTransform` of the root panel that this tab panel will
    /// place configurations within as each tab is selected.
    /// </summary>
    public RectTransform rootPanel;

    /// <summary>
    /// The 'InputField' user's modify to search for specific placements.
    /// </summary>
    public InputField searchField;

    /// <summary>
    /// The currently selected tab.
    /// </summary>
    private Tab _selectedTab = Tab.Banner;

    /// <summary>
    /// Standard game object `Start` handler.
    /// </summary>
    private void Start()
    {
        SelectTab(Environment.Shared.UseNewBannerAPI ? Tab.UnityBanner : Tab.Banner);
        DeselectTab(Environment.Shared.UseNewBannerAPI ? Tab.Banner : Tab.UnityBanner);
        DeselectTab(Tab.BannerView);
        DeselectTab(Tab.Fullscreen);
        DeselectTab(Tab.Interstitial);
        DeselectTab(Tab.Rewarded);
        DeselectTab(Tab.Settings);
    }

    /// <summary>
    /// Push handler for the banner tab button. This is linked to from the
    /// Button object in the scene hierarchy.
    /// </summary>
    public void OnBannerButtonPushed()
    {
        SelectTab(Tab.Banner);
    }
    
    /// <summary>
    /// Push handler for the BannerView tab button. This is linked to from the
    /// Button object in the scene hierarchy.
    /// </summary>
    public void OnBannerViewButtonPushed()
    {
        SelectTab(Tab.BannerView);
    }
    
    /// <summary>
    /// Push handler for the UnityBanner tab button. This is linked to from the
    /// Button object in the scene hierarchy.
    /// </summary>
    public void OnUnityBannerButtonPushed()
    {
        SelectTab(Tab.UnityBanner);
    }

    public void OnFullscreenButtonPushed()
    {
        SelectTab(Tab.Fullscreen);
    }

    /// <summary>
    /// Push handler for the interstitial tab button. This is linked to from the
    /// Button object in the scene hierarchy.
    /// </summary>
    public void OnInterstitialButtonPushed()
    {
        SelectTab(Tab.Interstitial);
    }

    /// <summary>
    /// Push handler for the rewarded tab button. This is linked to from the
    /// Button object in the scene hierarchy.
    /// </summary>
    public void OnRewardedButtonPushed()
    {
        SelectTab(Tab.Rewarded);
    }

    /// <summary>
    /// Push handler for the settings tab button. This is linked to from the
    /// Button object in the scene hierarchy.
    /// </summary>
    public void OnSettingsButtonPushed()
    {
        SelectTab(Tab.Settings);
    }

    public void ToggleSearchBarVisibility(bool status)
    {
        searchField.gameObject.SetActive(status);
    }

    /// <summary>
    /// This method should be called when the appropriate tab button is pushed.
    /// </summary>
    /// <param name="tab">The selected tab.</param>
    private void SelectTab(Tab tab)
    {
        ToggleSearchBarVisibility(tab != Tab.Settings);
        DeselectTab(_selectedTab);
        _selectedTab = tab;
        var configuration = configurations[(int)tab];

        if (configuration.instance != null)
            configuration.instance.SetActive(true);
        else if (configuration.prefab != null)
        {
            configuration.instance = Instantiate(configuration.prefab, rootPanel, false);
            var controller = configuration.instance.GetComponent<PlacementListController>();
            if (tab != Tab.Settings)
            {
                configuration.instance.name = $"{tab}ListController";
                switch (tab)
                {
                    case Tab.Banner:
                        controller.placementType = PlacementType.Banner;
                        controller.chartboostMediationAPI = ChartboostMediationAPI.Banner;
                        break;
                    case Tab.BannerView:
                        controller.placementType = PlacementType.Banner;
                        controller.chartboostMediationAPI = ChartboostMediationAPI.BannerView;
                        break;
                    case Tab.UnityBanner:
                        controller.placementType = PlacementType.Banner;
                        controller.chartboostMediationAPI = ChartboostMediationAPI.UnityBanner;
                        break;
                    case Tab.Fullscreen:
                        controller.placementType = PlacementType.Fullscreen;
                        controller.chartboostMediationAPI = ChartboostMediationAPI.Fullscreen;
                        break;
                    case Tab.Interstitial:
                        controller.placementType = PlacementType.Interstitial;
                        controller.chartboostMediationAPI = ChartboostMediationAPI.Interstitial;
                        break;
                    case Tab.Rewarded:
                        controller.placementType = PlacementType.Rewarded;
                        controller.chartboostMediationAPI = ChartboostMediationAPI.Rewarded;
                        break;
                    case Tab.Settings:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(tab), tab, null);
                }
                controller.UpdateActivePlacements(searchField.text);
                controller.selectionPanel.ConfigureForPlacementType(controller.placementType);
                searchField.onValueChanged.AddListener(controller.UpdateActivePlacements);
            }
        }

        var buttonText = GetButtonForTab(tab).GetComponentInChildren<Text>();
        if (buttonText != null)
            buttonText.color = Color.black;
    }

    /// <summary>
    /// This method should be called when a tab button is pushed but the currently
    /// selected tab needs to be deselected.
    /// </summary>
    /// <param name="tab">The deselected tab.</param>
    private void DeselectTab(Tab tab)
    {
        var configuration = configurations[(int)tab];
        var instance = configuration.instance;
        if (instance != null)
            instance.gameObject.SetActive(false);

        var buttonText = GetButtonForTab(tab).GetComponentInChildren<Text>();
        if (buttonText != null)
            buttonText.color = Color.gray;
    }

    /// <summary>
    /// Helper method to get the button for a particular tab.
    /// </summary>
    /// <param name="tab">The tab of interest.</param>
    /// <returns>The button for the tab.</returns>
    private Button GetButtonForTab(Tab tab)
    {
        return configurations[(int)tab].button;
    }
}

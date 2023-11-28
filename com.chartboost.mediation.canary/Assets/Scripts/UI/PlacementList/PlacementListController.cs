using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// The user interface controller that lists out all currently defined placements (from the backend)
/// for a specific placement type.
/// </summary>
public class PlacementListController : MonoBehaviour
{
    /// <summary>
    /// The `ScrollRect` game object in the transform hierarchy that is the scroll view for this UI.
    /// </summary>
    public ScrollRect scrollView;

    /// <summary>
    /// The `RectTransform` of the game object where scrollable content will be placed under.
    /// </summary>
    public RectTransform scrollViewContent;

    /// <summary>
    /// The `SelectionPanel` object which consists of configurable buttons/toggles
    /// </summary>
    public SelectionPanel selectionPanel;

    /// <summary>
    /// The prefab to instantiate for each placement.
    /// </summary>
    public GameObject placementListPrefab;

    /// <summary>
    /// Configurations for each placement type.
    /// </summary>
    public PlacementTypeConfiguration[] configurations;

    /// <summary>
    /// The placement type for an instance of this controller.
    /// </summary>
    public PlacementType placementType;

    /// <summary>
    /// The Chartboost Mediation API in use for an instance of this controller.  
    /// </summary>
    public ChartboostMediationAPI chartboostMediationAPI;

    private readonly Dictionary<string, PlacementListItem> _itemList = new Dictionary<string, PlacementListItem>();

    /// <summary>
    /// Standard game object `Start` handler.  In this implementation, the placements are
    /// fetched from the backend and then processed (generating the list of items) when the
    /// fetch completes.
    /// </summary>
    private void Start()
    {
        ChartboostMediationPlacementDataSource.Instance.DidUpdateDataSource += GenerateListFromPlacements;
        GenerateListFromPlacements(ChartboostMediationPlacementDataSource.Instance.Placements);
    }
    
    /// <summary>
    /// Generate the list of placements in the UI.
    /// </summary>
    /// <param name="placements">The list of placements provided by the backend.</param>
    private void GenerateListFromPlacements(List<Placement> placements)
    {
        if (placements.Count == 0)
            return;
        
        var filteredPlacements = placements.FindAll(p => p.TypeAsEnum == placementType).ToArray();
        Array.Sort(filteredPlacements, new PlacementComparer());
        
        // TODO: This may not be needed once all rewarded and interstitial placements are updated to "rewarded_interstitial"
        if (placementType == PlacementType.Fullscreen)
        {
            var rewardedPlacements = placements.FindAll(p => p.TypeAsEnum == PlacementType.Rewarded).ToArray();
            var interstitialPlacements = placements.FindAll(p => p.TypeAsEnum == PlacementType.Interstitial).ToArray();
            
            var filteredPlacementsList = filteredPlacements.ToList();
            filteredPlacementsList.AddRange(rewardedPlacements.ToList());
            filteredPlacementsList.AddRange(interstitialPlacements.ToList());
            filteredPlacements = filteredPlacementsList.ToArray();
            
            // sort all
            Array.Sort(filteredPlacements, new PlacementComparer());
        }
        
        foreach (var placement in filteredPlacements)
        {
            if (!_itemList.ContainsKey(placement.PlacementLowerCase))
            {
                var instance = Instantiate(placementListPrefab, scrollViewContent, false);
                var asListItem = instance.GetComponent<PlacementListItem>();
                asListItem.name = $"placement:{placement.placement}";
                asListItem.SetHeadline(placement.placement);
                UpdateSubHeadline(asListItem);
                asListItem.button.onClick.AddListener(() => ShowAdController(placement.placement));

                _itemList[placement.PlacementLowerCase] = asListItem;
            }
            else
            {
                var instance = _itemList[placement.PlacementLowerCase];
                var asListItem = instance.GetComponent<PlacementListItem>();
                    UpdateSubHeadline(asListItem);
            }

            void UpdateSubHeadline(PlacementListItem item)
            {
                if (placementType != PlacementType.Banner)
                    return;
                
                var autoRefreshRate = placement.autoRefreshRate;
                if (autoRefreshRate >= 0)
                    item.SetSubheadline($"auto refresh: {autoRefreshRate}");
            }
        }
        
        UpdateActivePlacements(TabPanelController.Instance.searchField.text);
    }

    /// <summary>
    /// Standard game object `OnEnable` handler. This implementation resets the
    /// view hierarchy.
    /// </summary>
    private void OnEnable()
    {
        TabPanelController.Instance.ToggleSearchBarVisibility(true);
    }

    /// <summary>
    /// Show the ad controller UI for a specific placement name.
    /// </summary>
    /// <param name="placementName">The placement.</param>
    private void ShowAdController(string placementName)
    {
        var configuration = configurations[(int)chartboostMediationAPI];
        if (configuration.ShowAdController(placementName, gameObject, selectionPanel.LoadType))
        {
            scrollView.gameObject.SetActive(false);
            selectionPanel.ToggleView(false);
        }
    }

    /// <summary>
    /// Pop the ad controller.
    /// </summary>
    /// <param name="adControllerGameObject">The game object that is the
    /// parent for the ad controller requesting to be removed.</param>
    public void PopAdController(GameObject adControllerGameObject)
    {
        Destroy(adControllerGameObject);
        TabPanelController.Instance.ToggleSearchBarVisibility(true);
        scrollView.gameObject.SetActive(true);
        if(placementType != PlacementType.Banner)
            selectionPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// Updates placement objects based off a search query
    /// </summary>
    /// <param name="searchQuery">Parameter to look for inside the placement name</param>
    public void UpdateActivePlacements(string searchQuery)
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            foreach (var placement in _itemList.Keys)
                _itemList[placement].gameObject.SetActive(true);
            return;
        }

        var inputAsLower = searchQuery.ToLower();
        foreach (var placement in _itemList.Keys)
            _itemList[placement].gameObject.SetActive(placement.Contains(inputAsLower));
    }
}

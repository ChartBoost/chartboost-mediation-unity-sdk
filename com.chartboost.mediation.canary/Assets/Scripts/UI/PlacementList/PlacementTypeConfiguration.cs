using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The configuration for the instantiation of the UI for a specific
/// placement type.
/// </summary>
[System.Serializable]
public class PlacementTypeConfiguration
{
    /// <summary>
    /// The placement type this configuration is for.
    /// </summary>
    [SerializeField]
    public PlacementType placementType;

    /// <summary>
    /// The Chartboost Mediation API this configuration will use
    /// </summary>
    public ChartboostMediationAPI chartboostMediationAPI;

    /// <summary>
    /// The prefab to create an instance of when a placement is selected
    /// in the user interface.
    /// </summary>
    [SerializeField]
    public GameObject prefab;

    /// <summary>
    /// The instantiated instance, if any, of the configured prefab.
    /// </summary>
    [System.NonSerialized]
    public GameObject instance;

    /// <summary>
    /// Show the ad controller UI for a specific placement name.
    /// </summary>
    /// <param name="placementName">The placement.</param>
    /// <returns>true if successful</returns>
    public bool ShowAdController(string placementName, GameObject parentGameObject, AdLoadType loadType)
    {
        if (prefab == null)
            return false;

        var configuration = new AdControllerConfiguration
        {
            placementName = placementName,
            chartboostMediationAPI =  chartboostMediationAPI,
            parentListController = parentGameObject.GetComponent<PlacementListController>(),
            loadType = loadType
        };

        if (instance == null)
            instance = Object.Instantiate(prefab, parentGameObject.transform, false);
        else
            instance.SetActive(true);

        var adController = instance.GetComponent<IAdController>();
        adController.Configure(configuration);
        return true;
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Configuration data for a tab that is manged by the TabController.
/// </summary>
[Serializable]
public class TabConfiguration
{

    /// <summary>
    /// configuration name
    /// </summary>
    public string name;
    
    /// <summary>
    /// The tab button.
    /// </summary>
    [SerializeField]
    public Button button;
    
    /// <summary>
    /// The prefab to instantiate in the scene for the tab.
    /// </summary>
    [SerializeField]
    public GameObject prefab;

    /// <summary>
    /// The current instantiated instance of the prefab.
    /// </summary>
    [Tooltip("Instance of Configuration Prefab, Null unless Playing")]
    [GrayOut]
    public GameObject instance;
}

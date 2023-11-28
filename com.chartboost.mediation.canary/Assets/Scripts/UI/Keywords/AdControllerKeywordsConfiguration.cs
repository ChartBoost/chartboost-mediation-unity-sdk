using UnityEngine;

/// <summary>
/// An Ad controller's configuration for keywords.
/// </summary>
[System.Serializable]
public struct AdControllerKeywordsConfiguration
{
    /// <summary>
    /// The prefab to create an instance of the placement's keyword instance.
    /// </summary>
    [SerializeField]
    public GameObject prefab;

    /// <summary>
    /// The instantiated keywords UI instance, if any, of the configured prefab.
    /// </summary>
    [System.NonSerialized]
    public GameObject instance;

    /// <summary>
    /// Show the keywords controller.
    /// </summary>
    /// <param name="placementName">The placement that the controller manages.</param>
    /// <param name="parentGameObject">The game object that is instantiating the controller.</param>
    /// <param name="listener">The listener of the controller.</param>
    /// <returns>Returns true if successful.</returns>
    public bool ShowKeywordsController(string placementName, GameObject parentGameObject, IKeywordsControllerListener listener, IKeywordsDataSource dataSource)
    {
        if (prefab == null)
            return false;

        if (instance == null)
        {
            instance = GameObject.Instantiate(prefab, parentGameObject.transform, false);
            var controller = instance.GetComponent<KeywordsController>();
            controller.Configure(listener, dataSource);
        }
        else
        {
            instance.SetActive(true);
        }

        return true;
    }

    /// <summary>
    /// Destroy the instance of the controller.
    /// </summary>
    public void DestroyKeywordsController()
    {
        if (instance == null)
            return;
        GameObject.Destroy(instance);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A placement data source that originates from the Helium backend server.
/// </summary>
public class ChartboostMediationPlacementDataSource : SimpleSingleton<ChartboostMediationPlacementDataSource>, IPlacementDataSource
{
    /// <summary>
    /// The Helium backend response for when fetching placements.
    /// </summary>
    #nullable enable
    [System.Serializable]
    public struct PlacementResponse
    {
        /// <summary>
        /// The placements provided in the response.
        /// </summary>
        public List<Placement>? placements;
    }
    #nullable disable
    
    private const string Endpoint = "https://helium-rtb.chartboost.com/v4/config/placements/";
    
    /// <summary>
    /// Unity friendly serializable object that contains cached placements.
    /// </summary>
    [Serializable]
    public class PlacementsCache
    {
        /// <summary>
        /// app specific placements .
        /// </summary>
        public List<Placement> placements;

        public PlacementsCache() { }

        public PlacementsCache(List<Placement> placements)
        {
            this.placements = placements;
        }
    }
    
    [SerializeField]
    private PlacementsCache placementsCache = new PlacementsCache();

    /// <inheritdoc cref="IPlacementDataSource.Placements"/>>
    public List<Placement> Placements => placementsCache.placements;

    /// <inheritdoc cref="IPlacementDataSource.DidUpdateDataSource"/>>
    public event Action<List<Placement>> DidUpdateDataSource; 

    /// <inheritdoc cref="IPlacementDataSource.LoadPlacementCache"/>>
    public async void LoadPlacementCache(string appId)
    {
        var context = SynchronizationContext.Current;
        void SetPlacements(List<Placement> cache)
        {
            placementsCache.placements = cache;
            context.Send(o =>
            {
                DidUpdateDataSource?.Invoke(cache);
            }, null);
        }

        var defaultCache = CheckDefaultCache(appId);

        if (defaultCache != null)
        {
            SetPlacements(defaultCache);
            return;
        }

        var warmCache = CheckWarmCache(appId);

        if (warmCache != null)
        {
            SetPlacements(warmCache);
            return;
        }

        // cache is cold, a new fetch is needed.
        var coldCachePath = Path.Combine(Application.persistentDataPath, $"{appId}.json");
        await FetchPlacements(appId).ContinueWith(placements =>
        {
            SetPlacements(placements.Result);
            StoreCache(coldCachePath, placementsCache);
        });
    }

    /// <inheritdoc cref="IPlacementDataSource.ExpirePlacementCache"/>>
    public void ExpirePlacementCache(string appId)
    {
        if (string.IsNullOrEmpty(appId))
            return;
        
        var warmCacheFile = Path.Combine(Application.persistentDataPath, $"{appId}.json");
        
        if (!File.Exists(warmCacheFile))
            return;
        
        File.Delete(warmCacheFile);
    }

    /// <summary>
    /// Checks warm cache for any data for an app id.
    /// </summary>
    /// <param name="appId">target app id.</param>
    /// <returns>cached placements.</returns>
    private List<Placement> CheckWarmCache(string appId)
    {
        var cachedPlacementsFile = Path.Combine(Application.persistentDataPath, $"{appId}.json");
        if (!File.Exists(cachedPlacementsFile))
            return null;

        var jsonContents = File.ReadAllText(cachedPlacementsFile);
        var cachedPlacements = JsonUtility.FromJson<PlacementsCache>(jsonContents);
        placementsCache = cachedPlacements;
        return placementsCache.placements;
    }

    /// <summary>
    /// Checks default cache for data used by default app ids.
    /// </summary>
    /// <param name="appId">target app id.</param>
    /// <returns>cached placements</returns>
    private List<Placement> CheckDefaultCache(string appId)
    {
        var cachedPlacementsAsset = Resources.Load<TextAsset>(appId);
        if (cachedPlacementsAsset == null) 
            return null;
        
        var cachedPlacements = JsonUtility.FromJson<PlacementsCache>(cachedPlacementsAsset.text);
        placementsCache = cachedPlacements;
        return placementsCache.placements;
    }

    /// <summary>
    /// Creates or updates the default cache for the default app ids.
    /// </summary>
    #if UNITY_EDITOR
    [MenuItem("Chartboost Mediation/Canary/Update Placements Cache")]
    #endif
    public static void PrepareDefaultCache()
    {
        string GetCachePath(string appId) => Path.Combine(Application.dataPath, "Resources", $"{appId}.json");
        CachePlacements(DefaultValue.AndroidAppId, GetCachePath(DefaultValue.AndroidAppId));
        CachePlacements(DefaultValue.IOSAppId, GetCachePath(DefaultValue.IOSAppId));
        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif
    }

    /// <summary>
    /// Fetches and caches placements for an specific app id.
    /// </summary>
    /// <param name="appId">Target app id.</param>
    /// <param name="path">Cache location.</param>
    private static async void CachePlacements(string appId, string path)
    {
        await FetchPlacements(appId).ContinueWith(fetchedPlacements =>
        {
            var placementCache = new PlacementsCache {
                placements = fetchedPlacements.Result
            };
            StoreCache(path, placementCache);
        });
    }

    /// <summary>
    /// Stores a PlacementCache object into an specified location.
    /// </summary>
    /// <param name="path">Location to store the cache.</param>
    /// <param name="cache">PlacementCache object to save.</param>
    private static void StoreCache(string path, PlacementsCache cache)
    {
        var canaryPlacementsJsons = JsonUtility.ToJson(cache, true);
        var bytes = Encoding.UTF8.GetBytes(canaryPlacementsJsons);
        File.WriteAllBytes(path, bytes);
    }

    /// <summary>
    /// Fetches placements for a Helium App ID.
    /// </summary>
    /// <param name="appId">target app id.</param>
    /// <returns></returns>
    private static Task<List<Placement>> FetchPlacements(string appId)
    {
        var url = Endpoint + appId;
        var request = UnityWebRequest.Get(url); // function which prepares request for API fetch
        request.SendWebRequest();

        while (!request.isDone)
            Task.Yield();
        
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error while sending {url}, with error: {request.error}");
            return Task.FromResult<List<Placement>>(null);
        }

        var placementsJson = request.downloadHandler.text;
        var response = JsonConvert.DeserializeObject<PlacementResponse>(placementsJson);
        return Task.FromResult(response.placements);
    }
}

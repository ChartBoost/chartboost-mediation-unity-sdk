using System;
using System.Threading.Tasks;
using Chartboost.Editor.Adapters.Serialization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Chartboost.Editor.Adapters
{
    /// <summary>
    /// Automatically fetches and caches Adapters Data from S3. 
    /// </summary>
    [InitializeOnLoad]
    public class AdapterDataSource
    {
        /// <summary>
        /// Static constructor automatically created by Unity Editor.
        /// </summary>
        static AdapterDataSource()
        {
            // This runs on startup. Only once
            if (InitialCache) 
                return;
            
            InitialCache = false;
            Update();
        }

        /// <summary>
        /// Currently Fetched and Loaded Adapter Data.
        /// </summary>
        public static AdapterData LoadedAdapters { get; private set; }
        
        /// <summary>
        /// Endpoint where Adapter Data is stored.
        /// </summary>
        private const string Endpoint = "https://chartboost.s3.amazonaws.com/chartboost-mediation/mediation-integration/v2/partners.json";

        private static readonly bool InitialCache;

        /// <summary>
        /// Call to Refresh Cache
        /// </summary>
        public static async void Update()
        {
            var result = await FetchCacheAndLoad();
            LoadedAdapters = result;
        }

        /// <summary>
        /// Fetched Adapter Config from JSON, caches if newer or new, returns most update Adapters.
        /// </summary>
        private static Task<AdapterData> FetchCacheAndLoad()
        {
            if (!Constants.PathToLibrary.DirectoryExists())
                return null;

            Constants.PathToLibraryCacheDirectory.DirectoryCreate();
            
            var newConfigJson = FetchAdapters();

            Task.WaitAll(newConfigJson);

            var newAdapters = newConfigJson.Result;
            var newAdapterConfig = JsonConvert.DeserializeObject<AdapterData>(newAdapters);
            if (Constants.PathToAdaptersCachedJson.FileExist())
            {
                var cachedJson = Constants.PathToAdaptersCachedJson.ReadAllText();
                var cacheAdapterConfig = JsonConvert.DeserializeObject<AdapterData>(cachedJson);

                var newVersion = DateTime.Parse(newAdapterConfig.lastUpdated);
                var oldVersion = DateTime.Parse(cacheAdapterConfig.lastUpdated);

                if (newVersion <= oldVersion)
                    return Task.FromResult(cacheAdapterConfig);
                Constants.PathToAdaptersCachedJson.FileCreate(newAdapters);
                return Task.FromResult(newAdapterConfig);
            }
            
            Constants.PathToAdaptersCachedJson.FileCreate(newAdapters);
            return Task.FromResult(newAdapterConfig);
        }

        /// <summary>
        /// Fetches Ad Adapters.
        /// </summary>
        /// <returns></returns>
        private static Task<string> FetchAdapters()
        {
            var request = UnityWebRequest.Get(Endpoint); // function which prepares request for API fetch
            request.SendWebRequest();

            while (!request.isDone)
                Task.Yield();
        
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[Adapter Data Source] Error while sending {Endpoint}, with error: {request.error}");
                return Task.FromResult<string>(null);
            }

            var adaptersJson = request.downloadHandler.text;
            return Task.FromResult(adaptersJson);
        }
    }
}

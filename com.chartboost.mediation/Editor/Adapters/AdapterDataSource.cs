using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Chartboost.Adapters
{
    [InitializeOnLoad]
    public class AdapterDataSource
    {
        static AdapterDataSource()
        {
            // This runs on startup.
            if (InitialCache) 
                return;
            
            InitialCache = false;
            Update();
        }

        public static Partners LoadedAdapters { get; private set; }
        
        private const string Endpoint = "https://chartboost.s3.amazonaws.com/chartboost-mediation/mediation-integration/partners.json";

        private static readonly string LibraryPath = Path.Combine(Directory.GetCurrentDirectory(), "Library");
        
        private static readonly string CacheDirectory = Path.Combine(LibraryPath, "com.chartboost.mediation");

        private static readonly string AdaptersCache = Path.Combine(CacheDirectory, "partners.json");

        private static readonly bool InitialCache;

        /// <summary>
        /// Call to Update Cache and Loaded Adapters
        /// </summary>
        public static async void Update()
        {
            var result = await FetchCacheAndLoad();
            LoadedAdapters = result;
        }

        /// <summary>
        /// Fetched Adapter Config from JSON, caches if newer or new, and Loads into Unity Memory.
        /// </summary>
        private static Task<Partners> FetchCacheAndLoad()
        {
            if (!Directory.Exists(LibraryPath))
                return null;

            if (!Directory.Exists(CacheDirectory))
                Directory.CreateDirectory(CacheDirectory);
            
            var newConfigJson = FetchAdapters();

            Task.WaitAll(newConfigJson);

            var newAdapters = newConfigJson.Result;
            var newAdapterConfig = JsonConvert.DeserializeObject<Partners>(newAdapters);
            if (File.Exists(AdaptersCache))
            {
                var cachedJson = File.ReadAllText(AdaptersCache);
                var cacheAdapterConfig = JsonConvert.DeserializeObject<Partners>(cachedJson);

                var newVersion = DateTime.Parse(newAdapterConfig.lastUpdated);
                var oldVersion = DateTime.Parse(cacheAdapterConfig.lastUpdated);

                if (newVersion <= oldVersion)
                    return Task.FromResult(cacheAdapterConfig);
                
                File.WriteAllText(AdaptersCache, newAdapters);
                return Task.FromResult(newAdapterConfig);
            }
            File.WriteAllText(AdaptersCache, newAdapters);
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
                Debug.LogError($"Error while sending {Endpoint}, with error: {request.error}");
                return Task.FromResult<string>(null);
            }

            var adaptersJson = request.downloadHandler.text;
            return Task.FromResult(adaptersJson);
        }
    }
}

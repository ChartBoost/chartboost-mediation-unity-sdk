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
            FetchCacheAndLoad();
        }

        public static Partners LoadedAdapters;
        
        private const string Endpoint = "https://chauduyphanvu.s3.us-east-2.amazonaws.com/mit/partners.json";

        private static readonly string LibraryPath = Path.Combine(Directory.GetCurrentDirectory(), "Library");
        
        private static readonly string CacheDirectory = Path.Combine(LibraryPath, "com.chartboost.mediation");

        private static readonly string AdaptersCache = Path.Combine(CacheDirectory, "partners.json");

        private static readonly bool InitialCache;

        /// <summary>
        /// Fetched Adapter Config from JSON, caches if newer or new, and Loads into Unity Memory.
        /// </summary>
        public static async void FetchCacheAndLoad()
        {
            if (!Directory.Exists(LibraryPath))
                return;

            if (!Directory.Exists(CacheDirectory))
                Directory.CreateDirectory(CacheDirectory);
            
            var newConfigJson = await FetchAdapters();
            if (newConfigJson == null)
                return;
            
            var newAdapterConfig = JsonConvert.DeserializeObject<Partners>(newConfigJson);

            if (File.Exists(AdaptersCache))
            {
                var cachedJson = File.ReadAllText(AdaptersCache);
                var cacheAdapterConfig = JsonConvert.DeserializeObject<Partners>(cachedJson);

                var newVersion = DateTime.Parse(newAdapterConfig.lastUpdated);
                var oldVersion = DateTime.Parse(cacheAdapterConfig.lastUpdated);

                if (newVersion > oldVersion)
                {
                    LoadedAdapters = newAdapterConfig;
                    File.WriteAllText(AdaptersCache, newConfigJson);
                }
                else
                {
                    LoadedAdapters = cacheAdapterConfig;
                }
            }
            else
                File.WriteAllText(AdaptersCache, newConfigJson);
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

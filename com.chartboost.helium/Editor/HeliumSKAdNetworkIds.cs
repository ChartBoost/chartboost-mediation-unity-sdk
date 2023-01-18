#nullable enable
#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Linq;
using Helium;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using UnityEngine.Networking;
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace Editor
{
    /// <summary>
    /// Adds the SKAdNetwork Ids to the app's Info.plist
    /// </summary>
    public class HeliumSKAdNetworkIds
    {
        [PostProcessBuild]
        public static void PostProcess(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (!HeliumSettings.IsSkAdNetworkResolutionEnabled)
                return;
            
            if (buildTarget != BuildTarget.iOS)
                return;

            var ids = GetSKAdNetworkIds();

            var plistPath = pathToBuiltProject + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            var skanItems = plist.root.CreateArray(SKAdNetworkItemsKey);

            foreach (var skadnetworkID in ids)
            {
                var skanId = skanItems.AddDict();
                skanId.SetString(SKAdNetworkIdKey, skadnetworkID);
            }

            plist.WriteToFile(plistPath);
        }

        private const string AppLovin = "https://dash.applovin.com/docs/v1/skadnetwork_ids.json";
        private const string Chartboost = "https://a3.chartboost.com/skadnetworkids.json";
        private const string Fyber = "https://fyber-engineering.github.io/SKAdNetworks/skadnetwork.json";
        private const string InMobi = "https://www.inmobi.com/skadnetworkids.json";
        private const string TapJoy = "https://skadnetwork.tapjoy.com/skadnetworkids.json";
        private const string Vungle = "https://vungle.com/skadnetworkids.json";
        private const string Unity = "https://skan.mz.unity3d.com/v3/partner/skadnetworks.plist.json";
        private const string AdColony = "https://raw.githubusercontent.com/AdColony/AdColony-iOS-SDK/master/AdNetworks.csv";

        private const string SKAdNetworkItemsKey = "SKAdNetworkItems";
        private const string SKAdNetworkIdKey = "SKAdNetworkIdentifier";
        
        private static IEnumerable<string> GetSKAdNetworkIds()
        {
            var idsToAdd = new List<string>();

            void AppendToFinalList(SKAdNetworkIds ids, ref List<string> finalIds)
            {
                if (ids.skadnetwork_ids != null)
                    finalIds.AddRange(ids.skadnetwork_ids.Select(id => id.skadnetwork_id)!);
            }

            void MergeIDs(params SKAdNetworkIds[] ids)
            {
                foreach (var id in ids)
                {
                    AppendToFinalList(id, ref idsToAdd);
                }
            }

            // json compatible SkAdNetworkFetching
            var appLovinIds = SKAdNetworkRequest(AppLovin);
            var chartboostIds = SKAdNetworkRequest(Chartboost);
            var fyberIds = SKAdNetworkRequest(Fyber);
            var inMobiIds = SKAdNetworkRequest(InMobi);
            var tapJoyIds = SKAdNetworkRequest(TapJoy);
            var vungleIds = SKAdNetworkRequest(Vungle);
            var unityIds = SkAdNetworkRequestUnity(Unity);
            
            // merge all ids
            MergeIDs(appLovinIds, chartboostIds, fyberIds, inMobiIds, tapJoyIds, vungleIds, unityIds);
            
            // cvs based SkAdNetworkFetching
            var adColonyIds = CSVSKAdNetworkRequest(AdColony);
            idsToAdd.AddRange(adColonyIds);

            // SkAdNetwork that does not provide easy json/cvs compatibility
            // 1. Facebook (https://developers.facebook.com/docs/setting-up/platform-setup/ios/SKAdNetwork/)
            idsToAdd.Add("v9wttpbfk9.skadnetwork");
            idsToAdd.Add("n38lu8286q.skadnetwork");
            // 2. IronSource (https://developers.is.com/ironsource-mobile/ios/ios-14-network-support/)
            idsToAdd.Add("su67r6k2v3.skadnetwork");
            // 3. Mintegral (https://www.mintegral.com/en/publisher-platform/ios-sdk-integration_en/)
            idsToAdd.Add("KBD757YWX3.skadnetwork");
            // 4. Pangle (https://www.pangleglobal.com/support/doc/ios14-readiness)
            idsToAdd.Add("238da6jt44.skadnetwork");
            idsToAdd.Add("22mmun2rn5.skadnetwork");
            // 5. Yahoo (https://s.yimg.com/an/skadnetwork/v1/skadnetworkids.xml)
            idsToAdd.Add("e5fvkxwrpn.skadnetwork");
            idsToAdd.Add("4fzdc2evr5.skadnetwork");
            idsToAdd.Add("cg4yq2srnc.skadnetwork");
            idsToAdd.Add("c6k4g5qg8m.skadnetwork");
            idsToAdd.Add("hs6bdukanm.skadnetwork");
            idsToAdd.Add("9rd848q2bz.skadnetwork");
            idsToAdd.Add("klf5c3l5u5.skadnetwork");
            idsToAdd.Add("8s468mfl3y.skadnetwork");
            idsToAdd.Add("uw77j35x4d.skadnetwork");

            // return unique ids, it's possible some of them can be duplicated.
            return idsToAdd.Distinct();
        }
        
        private static SKAdNetworkIds SKAdNetworkRequest(string url)
        {
            var skanIdsRequest = UnityWebRequest.Get(url);
            var request = skanIdsRequest.SendWebRequest();

            while (!request.isDone) { }

            if (skanIdsRequest.error != null)
                Debug.Log( $"SKAdNetworkRequest failed with error: {skanIdsRequest.error}" );

            var skanIds = JsonUtility.FromJson<SKAdNetworkIds>(skanIdsRequest.downloadHandler.text);
            return skanIds;
        }

        private static SKAdNetworkIds SkAdNetworkRequestUnity(string url) 
        {
            var skanIdsRequest = UnityWebRequest.Get(url);
            var request = skanIdsRequest.SendWebRequest();

            while (!request.isDone) { }
            
            if (skanIdsRequest.error != null)
                Debug.Log( $"SkAdNetworkRequestUnity failed with error: {skanIdsRequest.error}" );

            var contents = JsonConvert.DeserializeObject(skanIdsRequest.downloadHandler.text);
            
            var ret = new SKAdNetworkIds
            {
                company_name = "Unity",
                skadnetwork_ids = new List<IdEntry>()
            };
            
            if (!(contents is JArray asArray)) return ret;

            foreach (var element in asArray)
            {
                var id = element["skadnetwork_id"];
                if (id != null)
                    ret.skadnetwork_ids.Add( new IdEntry { skadnetwork_id = id.ToString()});
            }

            return ret;
        }

        private static IEnumerable<string> CSVSKAdNetworkRequest(string url)
        {
            var skanIdsRequest = UnityWebRequest.Get(url);
            var request = skanIdsRequest.SendWebRequest();

            while (!request.isDone) { }

            if (skanIdsRequest.error != null)
                Debug.Log( $"CSVSKAdNetworkRequest failed with error: {skanIdsRequest.error}" );

            var csv = skanIdsRequest.downloadHandler.text;
            var data = csv.Split('\n').ToList();
            data.RemoveAt(0);
            data.RemoveAll(string.IsNullOrEmpty);
            return data;
        }
        
        [Serializable]
        private class SKAdNetworkIds
        {
            public string? company_name;
            public string? company_address;
            public string? company_domain;
            public List<IdEntry>? skadnetwork_ids;
        }
        
        [Serializable]
        private class IdEntry
        {
            public string? skadnetwork_id;
            public string? creation_date;
            public string? entity_name;
        }
    }
}
#endif

#nullable enable
#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Linq;
using Chartboost;
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

namespace Helium.Editor
{
    /// <summary>
    /// Adds the SKAdNetwork Ids to the app's Info.plist
    /// </summary>
    public class HeliumSKAdNetworkIds
    {
        [PostProcessBuild]
        public static void PostProcess(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (!ChartboostMediationSettings.IsSkAdNetworkResolutionEnabled)
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
                skanId.SetString(SKAdNetworkIdKey, skadnetworkID.ToLower());
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
        private const string AdColony = "https://raw.githubusercontent.com/AdColony/AdColony-iOS-SDK/master/skadnetworkids.json";

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
                    AppendToFinalList(id, ref idsToAdd);
            }

            // json compatible SkAdNetworkFetching
            var appLovinIds = SKAdNetworkRequest(AppLovin);
            var adColonyIds = SKAdNetworkRequest(AdColony);
            var chartboostIds = SKAdNetworkRequest(Chartboost);
            var fyberIds = SKAdNetworkRequest(Fyber);
            var inMobiIds = SKAdNetworkRequest(InMobi);
            var tapJoyIds = SKAdNetworkRequest(TapJoy);
            var vungleIds = SKAdNetworkRequest(Vungle);
            var unityIds = SkAdNetworkRequestUnity(Unity);
            
            // merge all ids
            MergeIDs(appLovinIds, adColonyIds, chartboostIds, fyberIds, inMobiIds, tapJoyIds, vungleIds, unityIds);
            
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
            // 6. AdMob (https://developers.google.com/admob/ios/ios14) ........
            idsToAdd.Add("cstr6suwn9.skadnetwork");
            idsToAdd.Add("4fzdc2evr5.skadnetwork");
            idsToAdd.Add("4pfyvq9l8r.skadnetwork");
            idsToAdd.Add("2fnua5tdw4.skadnetwork");
            idsToAdd.Add("ydx93a7ass.skadnetwork");
            idsToAdd.Add("5a6flpkh64.skadnetwork");
            idsToAdd.Add("p78axxw29g.skadnetwork");
            idsToAdd.Add("v72qych5uu.skadnetwork");
            idsToAdd.Add("ludvb6z3bs.skadnetwork");
            idsToAdd.Add("cp8zw746q7.skadnetwork");
            idsToAdd.Add("c6k4g5qg8m.skadnetwork");
            idsToAdd.Add("s39g8k73mm.skadnetwork");
            idsToAdd.Add("3qy4746246.skadnetwork");
            idsToAdd.Add("3sh42y64q3.skadnetwork");
            idsToAdd.Add("f38h382jlk.skadnetwork");
            idsToAdd.Add("hs6bdukanm.skadnetwork");
            idsToAdd.Add("prcb7njmu6.skadnetwork");
            idsToAdd.Add("v4nxqhlyqp.skadnetwork");
            idsToAdd.Add("wzmmz9fp6w.skadnetwork");
            idsToAdd.Add("yclnxrl5pm.skadnetwork");
            idsToAdd.Add("t38b2kh725.skadnetwork");
            idsToAdd.Add("7ug5zh24hu.skadnetwork");
            idsToAdd.Add("9rd848q2bz.skadnetwork");
            idsToAdd.Add("y5ghdn5j9k.skadnetwork");
            idsToAdd.Add("n6fk4nfna4.skadnetwork");
            idsToAdd.Add("v9wttpbfk9.skadnetwork");
            idsToAdd.Add("n38lu8286q.skadnetwork");
            idsToAdd.Add("47vhws6wlr.skadnetwork");
            idsToAdd.Add("kbd757ywx3.skadnetwork");
            idsToAdd.Add("9t245vhmpl.skadnetwork");
            idsToAdd.Add("a2p9lx4jpn.skadnetwork");
            idsToAdd.Add("22mmun2rn5.skadnetwork");
            idsToAdd.Add("4468km3ulz.skadnetwork");
            idsToAdd.Add("2u9pt9hc89.skadnetwork");
            idsToAdd.Add("8s468mfl3y.skadnetwork");
            idsToAdd.Add("av6w8kgt66.skadnetwork");
            idsToAdd.Add("klf5c3l5u5.skadnetwork");
            idsToAdd.Add("ppxm28t8ap.skadnetwork");
            idsToAdd.Add("424m5254lk.skadnetwork");
            idsToAdd.Add("ecpz2srf59.skadnetwork");
            idsToAdd.Add("uw77j35x4d.skadnetwork");
            idsToAdd.Add("mlmmfzh3r3.skadnetwork");
            idsToAdd.Add("578prtvx9j.skadnetwork");
            idsToAdd.Add("4dzt52r2t5.skadnetwork");
            idsToAdd.Add("gta9lk7p23.skadnetwork");
            idsToAdd.Add("e5fvkxwrpn.skadnetwork");
            idsToAdd.Add("8c4e2ghe7u.skadnetwork");
            idsToAdd.Add("zq492l623r.skadnetwork");
            idsToAdd.Add("3rd42ekr43.skadnetwork");
            idsToAdd.Add("3qcr597p9d.skadnetwork");

            // return unique ids, it's possible some of them can be duplicated.
            return idsToAdd.Distinct();
        }
        
        private static SKAdNetworkIds SKAdNetworkRequest(string url)
        {
            var skanIdsRequest = UnityWebRequest.Get(url);
            var request = skanIdsRequest.SendWebRequest();

            while (!request.isDone) { }

            if (skanIdsRequest.error != null)
            {
                Debug.Log($"SKAdNetworkRequest failed with error: {skanIdsRequest.error}");
                return new SKAdNetworkIds();
            }

            try
            {
                var skanIds = JsonUtility.FromJson<SKAdNetworkIds>(skanIdsRequest.downloadHandler.text);
                return skanIds;
            }
            catch (NullReferenceException e)
            {
                Debug.Log($"SKAdNetworkRequest failed to parse json due to exception {e}");
                return new SKAdNetworkIds();
            }
        }

        private static SKAdNetworkIds SkAdNetworkRequestUnity(string url) 
        {
            var skanIdsRequest = UnityWebRequest.Get(url);
            var request = skanIdsRequest.SendWebRequest();

            while (!request.isDone) { }
            
            var ret = new SKAdNetworkIds
            {
                company_name = "Unity",
                skadnetwork_ids = new List<IdEntry>()
            };

            if (skanIdsRequest.error != null)
            {
                Debug.Log($"SkAdNetworkRequestUnity failed with error: {skanIdsRequest.error}");
                return ret;
            }

            var contents = JsonConvert.DeserializeObject(skanIdsRequest.downloadHandler.text);
            
            if (!(contents is JArray asArray)) return ret;


            try
            {
                foreach (var element in asArray)
                {
                    var id = element["skadnetwork_id"];
                    if (id != null)
                        ret.skadnetwork_ids.Add( new IdEntry { skadnetwork_id = id.ToString()});
                }

                return ret;
            }
            catch (NullReferenceException e)
            {
                Debug.Log($"SkAdNetworkRequestUnity failed to parse json due to exception {e}");
                return ret;
            }
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

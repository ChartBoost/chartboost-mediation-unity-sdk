using System;
using System.Collections.Generic;
using System.Linq;
using Chartboost.Editor.SKAdNetwork;
using Chartboost.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Chartboost.Mediation.Editor.iOS.SKAdNetwork
{
    public sealed class SKAdNetworkRequest
    {
        public static IEnumerable<string> GetSKAdNetworkIds()
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
            var appLovinIds = Request(SKAdNetworkConstants.AppLovin);
            var chartboostIds = Request(SKAdNetworkConstants.Chartboost);
            var fyberIds = Request(SKAdNetworkConstants.Fyber);
            var inMobiIds = Request(SKAdNetworkConstants.InMobi);
            var vungleIds = Request(SKAdNetworkConstants.Vungle);
            var unityIds = RequestUnity(SKAdNetworkConstants.Unity);
            
            // merge all ids
            MergeIDs(appLovinIds, chartboostIds, fyberIds, inMobiIds, vungleIds, unityIds);
            
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
        
        private static SKAdNetworkIds Request(string url)
        {
            var skanIdsRequest = UnityWebRequest.Get(url);
            var request = skanIdsRequest.SendWebRequest();

            while (!request.isDone) { }

            if (skanIdsRequest.error != null)
            {
                LogNetworkFailureMessage(skanIdsRequest);
                return new SKAdNetworkIds();
            }

            try
            {
                var skanIds = JsonUtility.FromJson<SKAdNetworkIds>(skanIdsRequest.downloadHandler.text);
                return skanIds;
            }
            catch (Exception e)
            {
                LogSKAdNetworkRequestExceptionMessage(url, e);
                return new SKAdNetworkIds();
            }
        }

        private static SKAdNetworkIds RequestUnity(string url) 
        {
            var skanIdsRequest = UnityWebRequest.Get(url);
            var request = skanIdsRequest.SendWebRequest();

            while (!request.isDone) { }
            
            var ret = new SKAdNetworkIds
            {
                company_name = SKAdNetworkConstants.NetworkUnity,
                skadnetwork_ids = new List<IdEntry>()
            };

            if (skanIdsRequest.error != null)
            {
                LogNetworkFailureMessage(skanIdsRequest);
                return ret;
            }

            var contents = JsonConvert.DeserializeObject(skanIdsRequest.downloadHandler.text);
            
            if (!(contents is JArray asArray)) return ret;
            
            try
            {
                foreach (var element in asArray)
                {
                    var id = element[SKAdNetworkConstants.SKAdNetworkId];
                    if (id != null)
                        ret.skadnetwork_ids.Add( new IdEntry { skadnetwork_id = id.ToString()});
                }
                return ret;
            }
            catch (Exception e)
            {
                LogSKAdNetworkRequestExceptionMessage(url, e);
                return ret;
            }
        }

        private static void LogNetworkFailureMessage(UnityWebRequest request) 
            => LogController.Log($"SKAdNetworkRequest failed for: {request.url} with error: {request.error}", LogLevel.Warning);
        
        private static void LogSKAdNetworkRequestExceptionMessage(string url, Exception e)
            => LogController.Log($"SKAdNetworkRequest failed to parse json for: {url} due to exception {e}", LogLevel.Warning);
    }
}

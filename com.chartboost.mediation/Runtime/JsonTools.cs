using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chartboost {
    public static class JsonTools
    {
        public static object Deserialize(string json)
        {
            try
            {
                json = json.Trim();
                if (json.StartsWith("{"))
                    return JsonConvert.DeserializeObject<IDictionary<object, object>>(json);
                else if (json.StartsWith("["))
                    return JsonConvert.DeserializeObject<IList<object>>(json);
                else
                    return JsonConvert.DeserializeObject(json);
            }
            catch
            {
                return null;
            }
        }

        public static string Serialize(Hashtable data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch
            {
                return null;
            }
        }
    }
}

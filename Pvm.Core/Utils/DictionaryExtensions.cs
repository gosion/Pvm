using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Pvm.Core.Utils
{
    public static class DictionaryExtensions
    {
        public static T GetValue<T>(
            this IDictionary<string, T> dict,
            string key,
            T defaultValue = default(T))
        {
            if (dict.TryGetValue(key, out T obj))
            {
                return (T)obj;
            }
            else
            {
                return defaultValue;
            }
        }

        public static T GetValue<T>(
            this IDictionary<string, object> dict,
            string key,
            T defaultValue = default(T))
        {
            if (dict.TryGetValue(key, out object obj))
            {
                return (T)obj;
            }
            else
            {
                return defaultValue;
            }
        }

        public static IDictionary<string, object> Deserialize(this IDictionary<string, object> raw)
        {
            foreach (var key in raw.Keys.ToArray())
            {
                if (raw[key] is JObject jobj)
                {
                    raw[key] = jobj.ToObject<IDictionary<string, object>>().Deserialize();
                }
                else if (raw[key] is JArray jarr)
                {
                    raw[key] = jarr.Deserialize();
                }
            }

            return raw;
        }

        private static IList<object> Deserialize(this JArray raw)
        {
            var list = new List<object>();

            foreach (var item in raw.ToObject<List<object>>())
            {
                var dict = new Dictionary<string, object>();

                if (item is JObject jobj)
                {
                    list.Add(jobj.ToObject<IDictionary<string, object>>().Deserialize());
                }
                else if (item is JArray jarr)
                {
                    list.Add(jarr.Deserialize());
                }
            }

            return list;
        }
    }
}

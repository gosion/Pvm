using System.Collections.Generic;

namespace Pvm.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static T Get<T>(this IDictionary<string, object> dict, string key, T defaultValue = default(T))
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

        public static T GetOrInit<T>(this IDictionary<string, object> dict, string key, T defaultValue = default(T))
        {
            if (dict.ContainsKey(key) == false)
            {
                dict[key] = defaultValue;
            }

            return dict.Get<T>(key, defaultValue);
        }

        public static IDictionary<string, object> Merge(this IDictionary<string, object> dict, IDictionary<string, object> another)
        {
            if (another == null)
            {
                return dict;
            }

            foreach (var item in another)
            {
                dict[item.Key] = item.Value;
            }

            return dict;
        }
    }
}

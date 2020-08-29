using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace SmartMirror.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool TryGetValueAsStringArray(this IDictionary<string, object> dict, string key, out string[] value)
        {
            if (dict == null)
            {
                value = new string[0];
                return false;
            }

            dict.TryGetValue(key, out object outValue);
            if (outValue is JArray array)
            {
                value = array.ToObject<string[]>();
                return true;
            }

            value = new string[0];
            return false;
        }

        public static bool TryGetValueAsStringListArray(this IDictionary<string, object> dict, string key, out string[][] value)
        {
            if (dict == null)
            {
                value = new string[0][];
                return false;
            }

            dict.TryGetValue(key, out object outValue);
            if (outValue is JArray array)
            {
                value = array.ToObject<string[][]>();
                return true;
            }

            value = new string[0][];
            return false;
        }
    }
}


using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace SmartMirror.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T value)
        {
            byte[] payload = cache.Get(key);
            if (payload != null)
            {
                string decodedPayload = Encoding.UTF8.GetString(payload);
                value = JsonSerializer.Deserialize<T>(decodedPayload);
                return true;
            }

            value = default;
            return false;
        }

        public static void Set<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
        {
            string payload = JsonSerializer.Serialize(value);
            byte[] encodedPayload = Encoding.UTF8.GetBytes(payload);            
            cache.Set(key, encodedPayload, options);
        }
    }
}


using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using UtilityLayer.Interfaces;

namespace UtilityLayer.Services
{
    internal class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string SetCache(int userId, string token, int expireMinutes = 10)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(expireMinutes));

            _cache.Set($"token_{userId}", token, cacheEntryOptions);
            return token;
        }

        public string? GetCache(int userId)
        {
            _cache.TryGetValue($"token_{userId}", out string? token);
            return token;
        }
    }
}

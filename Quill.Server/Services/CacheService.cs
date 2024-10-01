using Microsoft.Extensions.Caching.Memory;

namespace Quill.Server.Services;

public class CacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void Cache<Entity>(string location, Entity entity, string signature = "")
    {
        if (!_cache.TryGetValue(location, out Entity? _))
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(location + signature, entity, cacheEntryOptions);
            // This is a test
        }
    }

    public void Decache(string location, string signature = "")
    {
        if (_cache.TryGetValue(location + signature, out _))
        {
            _cache.Remove(location + signature);
        }
    }

    public Entity? TryGetCache<Entity>(string location, string signature = "")
    {
        if (_cache.TryGetValue(location + signature, out Entity? entity))
        {
            return entity;
        }

        return default;
    }
}

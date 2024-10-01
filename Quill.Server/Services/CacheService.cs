using Microsoft.Extensions.Caching.Memory;

namespace Quill.Server.Services;

public class CacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        // This is a test in pushing to the docker hub
    }

    public void Cache<Entity>(string location, Entity entity, string signature = "")
    {
        _logger.LogTrace("Writing cache for {0}", location + signature);
        if (!_cache.TryGetValue(location, out Entity? _))
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(location + signature, entity, cacheEntryOptions);
            _logger.LogTrace("Cache added with expiration = {0}.", cacheEntryOptions.SlidingExpiration);
        }
        else
        {
            _logger.LogTrace("Cache already exists.");
        }
    }

    public void Decache(string location, string signature = "")
    {
        _logger.LogTrace("Clearing cache for {0}", location + signature);
        if (_cache.TryGetValue(location + signature, out _))
        {
            _cache.Remove(location + signature);
            _logger.LogTrace("Successfully removed cache");
        }
        else
        {
            _logger.LogTrace("No cache found.");
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

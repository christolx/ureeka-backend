using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace ureeka_backend.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache? _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public T? GetData<T>(string key)
    {
        var data = _cache?.GetString(key);

        if (data is null)
            return default(T);

        return JsonSerializer.Deserialize<T>(data);
    }

    public void SetData<T>(string key, T value)
    {
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        };
        _cache?.SetString(key, JsonSerializer.Serialize(value), options);
    }
}
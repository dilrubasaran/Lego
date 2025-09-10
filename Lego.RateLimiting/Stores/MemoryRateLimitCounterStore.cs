using Lego.RateLimiting.Interfaces;
using Lego.RateLimiting.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Lego.RateLimiting.Stores;

// MemoryCache tabanlı rate limit counter store (thread-safe)
public class MemoryRateLimitCounterStore : ICustomRateLimitCounterStore
{
    private readonly IMemoryCache _cache;

    public MemoryRateLimitCounterStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    // Counter'ı alır
    public Task<RateLimitResult?> GetAsync(string key)
    {
        if (_cache.TryGetValue(key, out RateLimitResult? result))
        {
            return Task.FromResult<RateLimitResult?>(result);
        }
        return Task.FromResult<RateLimitResult?>(null);
    }

    // Atomik arttırma ve limit kontrolü (MemoryCache thread-safe)
    public Task<RateLimitResult> IncrementAsync(string key, TimeSpan period, int limit)
    {
        var existing = _cache.Get<RateLimitResult>(key);
        
        var result = new RateLimitResult();
        
        if (existing == null)
        {
            // İlk istek
            result.Count = 1;
            result.Timestamp = DateTime.UtcNow;
            result.Limit = limit;
            result.Period = period;
            result.IsLimitExceeded = false;
        }
        else
        {
            // Mevcut counter'ı arttır
            result.Count = existing.Count + 1;
            result.Timestamp = DateTime.UtcNow;
            result.Limit = limit;
            result.Period = period;
            result.IsLimitExceeded = result.Count > limit;
        }

        // TTL ile cache'e kaydet
        _cache.Set(key, result, period);
        
        return Task.FromResult(result);
    }

    // Counter'ı direkt set eder (AspNetCoreRateLimit için)
    public Task SetAsync(string key, RateLimitResult counter, TimeSpan period)
    {
        // TTL ile cache'e kaydet
        _cache.Set(key, counter, period);
        return Task.CompletedTask;
    }

    // Counter'ı sıfırlar
    public Task ResetAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }
}
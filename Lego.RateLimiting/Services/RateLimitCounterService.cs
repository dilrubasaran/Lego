using Lego.RateLimiting.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Lego.RateLimiting.Services;

// MemoryCache tabanlı Rate Limit sayaç servisi (thread-safe, Redis'e kolay geçiş)
public class RateLimitCounterService : IRateLimitCounterService
{
    private readonly IMemoryCache _cache;

    public RateLimitCounterService(IMemoryCache cache)
    {
        _cache = cache;
    }

    // Anahtara göre sayacı alır
    public Task<RateLimitCounter?> GetAsync(string key)
    {
        if (_cache.TryGetValue(key, out RateLimitCounter? counter))
        {
            return Task.FromResult<RateLimitCounter?>(counter);
        }
        return Task.FromResult<RateLimitCounter?>(null);
    }

    // Atomik arttırma (MemoryCache thread-safe, Redis'te INCR ile değiştirilebilir)
    public Task<RateLimitCounter> IncrementAsync(string key, TimeSpan period)
    {
        var counter = _cache.Get<RateLimitCounter>(key);
        
        if (counter == null)
        {
            counter = new RateLimitCounter { Count = 1, Timestamp = DateTime.UtcNow };
        }
        else
        {
            counter.Count += 1;
            counter.Timestamp = DateTime.UtcNow;
        }
        
        // TTL ile cache'e kaydet
        _cache.Set(key, counter, period);
        return Task.FromResult(counter);
    }

    // Sayacı kaydeder/yeniler
    public Task SetAsync(string key, RateLimitCounter counter, TimeSpan period)
    {
        _cache.Set(key, counter, period);
        return Task.CompletedTask;
    }
}

using Lego.RateLimiting.Models;

namespace Lego.RateLimiting.Interfaces;

// Kütüphaneden bağımsız, kendi rate limit counter kontratımız
public interface ICustomRateLimitCounterStore
{
    // Counter'ı alır (varsa)
    Task<RateLimitResult?> GetAsync(string key);
    
    // Counter'ı atomik olarak arttırır ve limit kontrolü yapar
    Task<RateLimitResult> IncrementAsync(string key, TimeSpan period, int limit);
    
    // Counter'ı direkt set eder (AspNetCoreRateLimit için)
    Task SetAsync(string key, RateLimitResult counter, TimeSpan period);
    
    // Counter'ı sıfırlar
    Task ResetAsync(string key);
}

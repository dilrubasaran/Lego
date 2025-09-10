using Lego.RateLimiting.Interfaces;
using Lego.RateLimiting.Models;

namespace Lego.RateLimiting.Services;

// Rate limiting iş mantığını yöneten servis (Storage-agnostic)
public class RateLimitingCounterService
{
    private readonly ICustomRateLimitCounterStore _store;

    public RateLimitingCounterService(ICustomRateLimitCounterStore store)
    {
        _store = store;
    }

    // Rate limit kontrolü ve arttırma (iş mantığı)
    public async Task<RateLimitResult> CheckAndIncrementAsync(string key, TimeSpan period, int limit)
    {
        // Store'dan direkt RateLimitResult alıyoruz - gereksiz dönüşüm yok
        return await _store.IncrementAsync(key, period, limit);
    }

    // Counter durumunu sorgula
    public async Task<RateLimitResult?> GetStatusAsync(string key)
    {
        // Store'dan direkt RateLimitResult alıyoruz - gereksiz dönüşüm yok
        return await _store.GetAsync(key);
    }

    // Counter'ı sıfırla
    public async Task ResetAsync(string key)
    {
        await _store.ResetAsync(key);
    }
}

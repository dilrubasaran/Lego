using AspNetCoreRateLimit;
using Lego.RateLimiting.Interfaces;
using Lego.RateLimiting.Models;

namespace Lego.RateLimiting.Adapters;
// şuan aspnetcorerate limit kütüphanesi ile kullandığım entpoint ve ip bazlı rate limitngi devre dışı bıraktığı içn kullnılmıyor 
//? adapteri sadece uerid için kullansak nasıl  olur
// AspNetCoreRateLimit kütüphanesi ile bizim custom store'umuz arasındaki köprü
public class RateLimitCounterStoreAdapter : IRateLimitCounterStore
{
    private readonly ICustomRateLimitCounterStore _customStore;

    public RateLimitCounterStoreAdapter(ICustomRateLimitCounterStore customStore)
    {
        _customStore = customStore;
    }

    // AspNetCoreRateLimit'ten gelen çağrıları bizim store'a yönlendir
    public async Task<AspNetCoreRateLimit.RateLimitCounter?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await _customStore.GetAsync(id);
        if (result == null) return null;

        // Default constructor ile oluştur ve reflection ile property'leri set et
        var counter = new AspNetCoreRateLimit.RateLimitCounter();
        
        // Property'lerin adlarını doğru bulmak için try-catch kullan
        try
        {
            var countProperty = counter.GetType().GetProperty("TotalHits") ?? 
                               counter.GetType().GetProperty("Count") ?? 
                               counter.GetType().GetProperty("Hits");
            countProperty?.SetValue(counter, result.Count);
            
            var timeProperty = counter.GetType().GetProperty("Timestamp") ?? 
                              counter.GetType().GetProperty("LastUpdate") ?? 
                              counter.GetType().GetProperty("CreatedDate");
            timeProperty?.SetValue(counter, result.Timestamp);
        }
        catch
        {
            // Property'ler bulunamazsa varsayılan değerlerle döner
        }
        
        return counter;
    }

    // AspNetCoreRateLimit'ten gelen set çağrılarını bizim store'a yönlendir
    public async Task SetAsync(string id, AspNetCoreRateLimit.RateLimitCounter? entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        if (entry == null) return;
        
        // AspNetCoreRateLimit.RateLimitCounter'ı bizim RateLimitResult'a çevir
        var count = 0;
        var timestamp = DateTime.UtcNow;
        
        // Reflection ile property'leri oku
        try
        {
            var countProperty = entry.GetType().GetProperty("TotalHits") ?? 
                               entry.GetType().GetProperty("Count") ?? 
                               entry.GetType().GetProperty("Hits");
            if (countProperty != null)
                count = (int)(countProperty.GetValue(entry) ?? 0);
            
            var timeProperty = entry.GetType().GetProperty("Timestamp") ?? 
                              entry.GetType().GetProperty("LastUpdate") ?? 
                              entry.GetType().GetProperty("CreatedDate");
            if (timeProperty != null)
                timestamp = (DateTime)(timeProperty.GetValue(entry) ?? DateTime.UtcNow);
        }
        catch
        {
            // Property'ler bulunamazsa varsayılan değerleri kullan
        }
        
        var customCounter = new RateLimitResult
        {
            Count = count,
            Timestamp = timestamp,
            IsLimitExceeded = false, // AspNetCoreRateLimit kendi limit kontrolünü yapar
            Limit = 0, // AspNetCoreRateLimit kendi limitini kullanır
            Period = expirationTime ?? TimeSpan.FromHours(1)
        };
        
        // Bizim store'a set et
        await _customStore.SetAsync(id, customCounter, expirationTime ?? TimeSpan.FromHours(1));
    }

    // AspNetCoreRateLimit'ten gelen remove çağrılarını bizim store'a yönlendir
    public async Task RemoveAsync(string id, CancellationToken cancellationToken = default)
    {
        await _customStore.ResetAsync(id);
    }

    // Counter var mı kontrol et
    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await _customStore.GetAsync(id);
        return result != null;
    }
}
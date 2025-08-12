using Lego.Contexts.Models.RateLimiting;

namespace Lego.RateLimiting.Stores;

// Rate limit kurallarını veritabanından yüklemek için kullanılan interface
public interface IRateLimitRuleStore
{
    // Tüm aktif rate limit kurallarını getirir
    Task<IEnumerable<RateLimitRule>> GetActiveRulesAsync();
    
    // Belirli bir endpoint için kuralları getirir
    Task<IEnumerable<RateLimitRule>> GetRulesForEndpointAsync(string endpoint, string httpMethod);
    
    // Belirli bir client type için kuralları getirir
    Task<IEnumerable<RateLimitRule>> GetRulesForClientTypeAsync(string clientType);
}
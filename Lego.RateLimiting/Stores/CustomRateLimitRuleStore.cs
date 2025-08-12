using Lego.Contexts;
using Lego.Contexts.Models.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lego.RateLimiting.Stores;

// Entity Framework kullanarak rate limit kurallarını veritabanından yükleyen store
public class CustomRateLimitRuleStore : IRateLimitRuleStore
{
    private readonly ApiDbContext _context;
    private readonly ILogger<CustomRateLimitRuleStore> _logger;

    public CustomRateLimitRuleStore(ApiDbContext context, ILogger<CustomRateLimitRuleStore> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Tüm aktif rate limit kurallarını getirir
    public async Task<IEnumerable<RateLimitRule>> GetActiveRulesAsync()
    {
        try
        {
            var rules = await _context.RateLimitRules
                .Where(r => r.IsActive)
                .OrderBy(r => r.Endpoint)
                .ThenBy(r => r.HttpMethod)
                .ToListAsync();

            _logger.LogInformation("Toplam {Count} aktif rate limit kuralı yüklendi", rules.Count);
            return rules;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rate limit kuralları yüklenirken hata oluştu");
            return Enumerable.Empty<RateLimitRule>();
        }
    }

    // Belirli bir endpoint için kuralları getirir
    public async Task<IEnumerable<RateLimitRule>> GetRulesForEndpointAsync(string endpoint, string httpMethod)
    {
        try
        {
            var rules = await _context.RateLimitRules
                .Where(r => r.IsActive && 
                           (r.Endpoint == "*" || r.Endpoint == endpoint) &&
                           (r.HttpMethod == "*" || r.HttpMethod == httpMethod))
                .OrderBy(r => r.Endpoint)
                .ThenBy(r => r.HttpMethod)
                .ToListAsync();

            _logger.LogDebug("Endpoint {Endpoint} {Method} için {Count} kural bulundu", endpoint, httpMethod, rules.Count);
            return rules;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Endpoint {Endpoint} {Method} için kurallar yüklenirken hata oluştu", endpoint, httpMethod);
            return Enumerable.Empty<RateLimitRule>();
        }
    }

    // Belirli bir client type için kuralları getirir
    public async Task<IEnumerable<RateLimitRule>> GetRulesForClientTypeAsync(string clientType)
    {
        try
        {
            var rules = await _context.RateLimitRules
                .Where(r => r.IsActive && 
                           (r.ClientType == "*" || r.ClientType == clientType))
                .OrderBy(r => r.ClientType)
                .ThenBy(r => r.Endpoint)
                .ToListAsync();

            _logger.LogDebug("Client type {ClientType} için {Count} kural bulundu", clientType, rules.Count);
            return rules;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Client type {ClientType} için kurallar yüklenirken hata oluştu", clientType);
            return Enumerable.Empty<RateLimitRule>();
        }
    }
}
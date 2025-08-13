using Lego.Contexts;
using Lego.Contexts.Enums;
using Lego.Contexts.Models;
using Lego.Contexts.Models.RateLimiting;
using Lego.RateLimiting.Exceptions;
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

    // Database işlemlerinde hata yakalama ve custom exception fırlatma için merkezi method
    private async Task<T> ExecuteWithDbErrorHandlingAsync<T>(Func<Task<T>> action, string operationName)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Operation} sırasında database hatası oluştu", operationName);
            throw new RateLimitingException(
                $"{operationName} sırasında database hatası oluştu",
                new RateLimitingExceptionData
                {
                    ErrorType = RateLimitingErrorType.Database,
                    Operation = operationName,
                    TableName = "RateLimitRules"
                },
                ex);
        }
    }

    // Tüm aktif rate limit kurallarını getirir
    public Task<IEnumerable<RateLimitRule>> GetActiveRulesAsync()
    {
        return ExecuteWithDbErrorHandlingAsync(async () =>
        {
            var rules = await _context.RateLimitRules
                .Where(r => r.IsActive)
                .OrderBy(r => r.Endpoint)
                .ThenBy(r => r.HttpMethod)
                .ToListAsync();

            _logger.LogInformation("Toplam {Count} aktif rate limit kuralı yüklendi", rules.Count);
            return (IEnumerable<RateLimitRule>)rules;
        }, "Aktif kuralların alınması");
    }

    // Belirli bir endpoint için kuralları getirir
    public Task<IEnumerable<RateLimitRule>> GetRulesForEndpointAsync(string endpoint, string httpMethod)
    {
        // Parameter validation
        if (string.IsNullOrEmpty(endpoint))
            throw new RateLimitingException("Endpoint parametresi boş olamaz", 
                new RateLimitingExceptionData 
                { 
                    ErrorType = RateLimitingErrorType.Validation,
                    FieldName = nameof(endpoint), 
                    InvalidValue = endpoint 
                });
        
        if (string.IsNullOrEmpty(httpMethod))
            throw new RateLimitingException("HttpMethod parametresi boş olamaz", 
                new RateLimitingExceptionData 
                { 
                    ErrorType = RateLimitingErrorType.Validation,
                    FieldName = nameof(httpMethod), 
                    InvalidValue = httpMethod 
                });

        return ExecuteWithDbErrorHandlingAsync(async () =>
        {
            var rules = await _context.RateLimitRules
                .Where(r => r.IsActive && 
                           (r.Endpoint == "*" || r.Endpoint == endpoint) &&
                           (r.HttpMethod == "*" || r.HttpMethod == httpMethod))
                .OrderBy(r => r.Endpoint)
                .ThenBy(r => r.HttpMethod)
                .ToListAsync();

            _logger.LogDebug("Endpoint {Endpoint} {Method} için {Count} kural bulundu", endpoint, httpMethod, rules.Count);
            return (IEnumerable<RateLimitRule>)rules;
        }, $"Endpoint {endpoint} {httpMethod} kurallarının alınması");
    }

    // Belirli bir client type için kuralları getirir
    public Task<IEnumerable<RateLimitRule>> GetRulesForClientTypeAsync(string clientType)
    {
        // Parameter validation
        if (string.IsNullOrEmpty(clientType))
            throw new RateLimitingException("ClientType parametresi boş olamaz", 
                new RateLimitingExceptionData 
                { 
                    ErrorType = RateLimitingErrorType.Validation,
                    FieldName = nameof(clientType), 
                    InvalidValue = clientType 
                });

        return ExecuteWithDbErrorHandlingAsync(async () =>
        {
            var rules = await _context.RateLimitRules
                .Where(r => r.IsActive && 
                           (r.ClientType == "*" || r.ClientType == clientType))
                .OrderBy(r => r.ClientType)
                .ThenBy(r => r.Endpoint)
                .ToListAsync();

            _logger.LogDebug("Client type {ClientType} için {Count} kural bulundu", clientType, rules.Count);
            return (IEnumerable<RateLimitRule>)rules;
        }, $"Client type {clientType} kurallarının alınması");
    }
}
using Lego.RateLimiting.Interfaces;
using Lego.RateLimiting.Stores;
using Lego.RateLimiting.Services;
using Lego.Contexts.Models.RateLimiting;
using Lego.RateLimiting.Exceptions;
using Lego.Contexts.Enums;
using Lego.Contexts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Lego.RateLimiting.Evaluators;

// UserId bazlı rate limiting değerlendirmesi yapan evaluator
public class UserIdRateLimitingEvaluator : IRateLimitingEvaluator
{
    private readonly RateLimitingCounterService _counterService;
    private readonly IUserIdResolver _userIdResolver;
    private readonly IClientIpResolver _clientIpResolver;
    private readonly IConfiguration _configuration;

    public UserIdRateLimitingEvaluator(
        RateLimitingCounterService counterService,
        IUserIdResolver userIdResolver,
        IClientIpResolver clientIpResolver,
        IConfiguration configuration)
    {
        _counterService = counterService;
        _userIdResolver = userIdResolver;
        _clientIpResolver = clientIpResolver;
        _configuration = configuration;
    }

    // UserId bazlı rate limiting kontrolü yapar
    public async Task<bool> EvaluateAsync(HttpContext context)
    {
        // Static files ve system endpoint'lerini atla
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.Contains("/favicon") || path.Contains("/scalar") || 
            path.StartsWith("/_") || path.Contains(".js") || 
            path.Contains(".css") || path.Contains(".map") || path.Contains("/api/openapi"))
        {
            return false; // Rate limiting uygulama
        }

        // UserIdRateLimiting aktif mi kontrol et
        var isEnabled = _configuration.GetValue<bool>("UserIdRateLimiting:EnableUserIdRateLimiting");
        if (!isEnabled)
        {
            return false;
        }

        // UserId'yi al
        var userId = _userIdResolver.GetUserId(context);
        
        if (string.IsNullOrEmpty(userId))
        {
            return false; // UserId yoksa UserId bazlı rate limiting uygulanmaz
        }

        var clientIdentifier = $"user:{userId}";
        
        // appsettings.json'dan UserId kurallarını al
        var userIdRules = _configuration.GetSection("UserIdRateLimiting:Rules").Get<List<RateLimitRule>>();
        
        if (userIdRules == null || !userIdRules.Any())
        {
            return false;
        }

        // Her kural için rate limiting uygula
        foreach (var rule in userIdRules)
        {
            await ApplyUserIdRateLimitRule(rule, clientIdentifier, context);
        }

        return false; // Hiçbir limit aşılmadı
    }

    // UserId bazlı rate limiting kuralını uygular
    private async Task ApplyUserIdRateLimitRule(RateLimitRule rule, string clientIdentifier, HttpContext context)
    {
        var key = $"userid_ratelimit:{clientIdentifier}:{rule.Endpoint}";
        
        // Period doğrudan TimeSpan kullan 
        var period = rule.Period;

        // Rate limit kontrolü ve arttırma
        var result = await _counterService.CheckAndIncrementAsync(key, period, rule.Limit);

        // Limit kontrolü
        if (result.IsLimitExceeded)
        {
            throw new RateLimitingException($"UserId rate limit aşıldı: {result.Count}/{result.Limit}",
                new RateLimitingExceptionData
                {
                    ErrorType = RateLimitingErrorType.LimitExceeded,
                    ClientIdentifier = clientIdentifier
                });
        }

    }

}

using Lego.RateLimiting.Interfaces;
using Lego.RateLimiting.Stores;
using Lego.Contexts.Models.RateLimiting;
using Lego.RateLimiting.Exceptions;
using Lego.Contexts.Enums;
using Lego.Contexts.Models;
using Microsoft.AspNetCore.Http;

namespace Lego.RateLimiting.Evaluators;

// UserId bazlı rate limiting değerlendirmesi yapan evaluator
public class UserIdRateLimitingEvaluator : IRateLimitingEvaluator
{
    private readonly IRateLimitCounterService _counterService;
    private readonly IRateLimitRuleStore _ruleStore;
    private readonly IUserIdResolver _userIdResolver;
    private readonly IClientIpResolver _clientIpResolver;

    public UserIdRateLimitingEvaluator(
        IRateLimitCounterService counterService,
        IRateLimitRuleStore ruleStore,
        IUserIdResolver userIdResolver,
        IClientIpResolver clientIpResolver)
    {
        _counterService = counterService;
        _ruleStore = ruleStore;
        _userIdResolver = userIdResolver;
        _clientIpResolver = clientIpResolver;
    }

    // UserId bazlı rate limiting kontrolü yapar
    public async Task<bool> EvaluateAsync(HttpContext context)
    {
        // Static files ve system endpoint'lerini atla
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.Contains("/favicon") || path.Contains("/scalar") || 
            path.StartsWith("/_") || path.Contains(".js") || 
            path.Contains(".css") || path.Contains(".map"))
        {
            return false; // Rate limiting uygulama
        }

        // Servislerden client bilgilerini al
        var userId = _userIdResolver.GetUserId(context);
        
        // Hangi kural tipini kullanacağımızı belirle
        string clientType;
        string clientIdentifier;
        
        if (!string.IsNullOrEmpty(userId))
        {
            // UserId bazlı rate limiting
            clientType = "UserId";
            clientIdentifier = $"user:{userId}";
        }
        else
        {
            // IP bazlı rate limiting (fallback)
            clientType = "IP";
            var clientIp = _clientIpResolver.GetClientIpAddress(context);
            clientIdentifier = $"ip:{clientIp}";
        }

        // İlgili kuralları al
        var rules = await _ruleStore.GetRulesForClientTypeAsync(clientType);
        
        // DEBUG: Kuralları console'a yazdır
        Console.WriteLine($"🔍 DEBUG - ClientType: {clientType}, ClientIdentifier: {clientIdentifier}");
        Console.WriteLine($"🔍 DEBUG - Found {rules.Count()} rules for {clientType}");
        foreach (var r in rules)
        {
            Console.WriteLine($"  - Rule {r.Id}: {r.ClientType} | {r.Endpoint} | Limit: {r.Limit}");
        }
        
        if (!rules.Any())
        {
            Console.WriteLine($"❌ DEBUG - No rules found for {clientType}, skipping rate limiting");
            return false; // Kural yoksa rate limiting uygulanmaz
        }

        // Her kural için rate limiting uygula
        foreach (var rule in rules)
        {
            Console.WriteLine($"🚀 DEBUG - Applying rule {rule.Id} for {clientIdentifier}");
            await ApplyRateLimitRule(rule, clientIdentifier);
        }

        return false; // Hiçbir limit aşılmadı
    }

    // Belirli bir kural için rate limiting uygular
    private async Task ApplyRateLimitRule(Lego.Contexts.Models.RateLimiting.RateLimitRule rule, string clientIdentifier)
    {
        var key = $"ratelimit:{clientIdentifier}:{rule.Id}";

        // Atomik arttırma - MemoryCache thread-safe, Redis'te INCR ile değiştirilebilir
        var counter = await _counterService.IncrementAsync(key, rule.Period);

        // Limit kontrolü
        if (counter.Count > rule.Limit)
        {
            throw new RateLimitingException($"Rate limit aşıldı: {counter.Count}/{rule.Limit}",
                new RateLimitingExceptionData
                {
                    ErrorType = RateLimitingErrorType.LimitExceeded,
                    ClientIdentifier = clientIdentifier
                });
        }
    }
}

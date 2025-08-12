using Lego.RateLimiting.Middleware;
using Lego.RateLimiting.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lego.RateLimiting.Extensions;

// Rate limiting servislerini ve middleware'lerini kolayca eklemek için extension method'ları
public static class RateLimitingExtensions
{
    // Rate limiting servislerini DI container'a ekler
    public static IServiceCollection AddLegoRateLimiting(this IServiceCollection services)
    {
        // Store'ları ekle
        services.AddScoped<IRateLimitRuleStore, CustomRateLimitRuleStore>();
        
        return services;
    }

    // Rate limit logging middleware'ini pipeline'a ekler
    public static IApplicationBuilder UseRateLimitLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitLoggingMiddleware>();
    }
} 
using Lego.RateLimiting.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Lego.RateLimiting.Middleware;

// Rate limiting kontrollerini yapan ana middleware
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // Middleware pipeline'ını işler ve rate limiting kontrolü yapar
    public async Task InvokeAsync(HttpContext context)
    {
        // Scoped service'leri HttpContext'ten al
        var evaluators = context.RequestServices.GetServices<IRateLimitingEvaluator>();

        // Tüm evaluator'ları kontrol et
        foreach (var evaluator in evaluators)
        {
            await evaluator.EvaluateAsync(context);
        }

        // Rate limit aşılmadıysa pipeline'a devam et
        await _next(context);
    }
}

using Lego.RateLimiting.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lego.RateLimiting.Middleware;

// Global hata yakalama ve işleme middleware'i
// Tüm uygulama hatalarını tek bir yerden yönetir
public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

    public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // Middleware pipeline'ını işler ve hataları yakalar
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🚨 Global Error Handler: {ErrorType} - {Message}", 
                ex.GetType().Name, ex.Message);
            
            // Exception processing'i ExceptionProcessor'a devret
            await ExceptionProcessor.ProcessExceptionAsync(context, ex);
            
            // Client IP ile detaylı logging
            _logger.LogError("❌ Error Response: Client={ClientIP}", GetClientIpAddress(context));
        }
    }

    // Client'ın gerçek IP adresini alır (proxy arkasındaki durumlar için header kontrollü)
    private string GetClientIpAddress(HttpContext context)
    {
        var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            return forwardedHeader.Split(',')[0].Trim();
        }

        var realIpHeader = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIpHeader))
        {
            return realIpHeader;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}

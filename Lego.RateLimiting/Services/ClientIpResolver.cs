using Lego.RateLimiting.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Lego.RateLimiting.Services;

// Client IP adresini çözümleyen servis implementasyonu
public class ClientIpResolver : IClientIpResolver
{
    // HttpContext'ten client'ın gerçek IP adresini döndürür
    // Proxy header'larını da kontrol eder
    public string GetClientIpAddress(HttpContext context)
    {
        // X-Forwarded-For header'ından ilk IP'yi al (proxy arkasındaki durumlar için)
        var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            return forwardedHeader.Split(',')[0].Trim();
        }

        // X-Real-IP header'ından IP'yi al
        var realIpHeader = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIpHeader))
        {
            return realIpHeader;
        }

        // Son çare olarak RemoteIpAddress kullan
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

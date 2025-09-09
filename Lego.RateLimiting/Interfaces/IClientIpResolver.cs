using Microsoft.AspNetCore.Http;

namespace Lego.RateLimiting.Interfaces;

// Client IP adresini çözümleyen servis interface'i
public interface IClientIpResolver
{
    // HttpContext'ten client'ın gerçek IP adresini döndürür
    string GetClientIpAddress(HttpContext context);
}

using Lego.RateLimiting.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Lego.RateLimiting.Services;

// UserId'yi çözümleyen servis implementasyonu
public class UserIdResolver : IUserIdResolver
{
    // HttpContext'ten authenticated kullanıcının UserId'sini döndürür
    // JWT Claims'den NameIdentifier'ı okur
    public string? GetUserId(HttpContext context)
    {
        var user = context.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}

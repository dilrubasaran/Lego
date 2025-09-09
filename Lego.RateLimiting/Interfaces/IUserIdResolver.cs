using Microsoft.AspNetCore.Http;

namespace Lego.RateLimiting.Interfaces;

// UserId'yi çözümleyen servis interface'i
public interface IUserIdResolver
{
    // HttpContext'ten authenticated kullanıcının UserId'sini döndürür
    string? GetUserId(HttpContext context);
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Lego.JWT.Models;

namespace Lego.JWT.Middleware;

// JWT token doğrulama işlemlerini gerçekleştiren özel middleware (opsiyonel)
// Not: Genellikle Microsoft'un built-in JWT authentication middleware'i kullanılır
public class JwtAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtOptions _jwtOptions;

    // JwtAuthenticationMiddleware constructor
    public JwtAuthenticationMiddleware(RequestDelegate next, IOptions<JwtOptions> jwtOptions)
    {
        _next = next;
        _jwtOptions = jwtOptions.Value;
    }

    // HTTP request'i işler ve JWT token doğrulaması yapar
    public async Task InvokeAsync(HttpContext context)
    {
        var token = ExtractTokenFromHeader(context);

        if (!string.IsNullOrEmpty(token))
        {
            await ValidateTokenAsync(context, token);
        }

        await _next(context);
    }

    // Authorization header'ından JWT token'ı çıkarır
    private string? ExtractTokenFromHeader(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(authorizationHeader))
            return null;

        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;

        return authorizationHeader.Substring("Bearer ".Length).Trim();
    }

    // JWT token'ı doğrular ve kullanıcı kimliğini context'e ekler
    private async Task ValidateTokenAsync(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            
            // Kullanıcı kimliğini context'e ekle
            context.User = principal;
        }
        catch (Exception ex)
        {
            // Token doğrulama hatası - loglama yapılabilir
            Console.WriteLine($"JWT Token validation failed: {ex.Message}");
        }
    }
}

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Lego.JWT.Interfaces;
using Lego.JWT.Models;
using Lego.JWT.Services;

namespace Lego.JWT.Extensions;

// JWT servislerini DI container'a kaydetmek için extension metodları
public static class JwtServiceCollectionExtensions
{
    // JWT core servislerini DI container'a kaydeder
    public static IServiceCollection AddJwtCore(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT options'ı yapılandırma dosyasından yükle
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));


        // JWT servislerini kaydet
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();

        return services;
    }

    // JWT authentication middleware'ini yapılandırır ve DI container'a kaydeder
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
         
        if (jwtOptions == null)
        {
            throw new InvalidOperationException("JWT yapılandırması bulunamadı. appsettings.json dosyasında 'Jwt' bölümünü kontrol edin.");
        }

        if (string.IsNullOrEmpty(jwtOptions.SecretKey))
        {
            throw new InvalidOperationException("JWT SecretKey boş olamaz.");
        }

        var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("JWT Token validated successfully");
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
}

using Lego.Contexts;
using Lego.Contexts.Enums;
using Lego.Contexts.Models;
using Lego.Contexts.Models.RateLimiting;
using Lego.RateLimiting.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Lego.RateLimiting.Middleware;
//? try cache azaltılır mı bak 
// Rate limiting işlemlerini loglayan middleware
// RateLimitViolation ve RateLimitLog kayıtlarını otomatik olarak veritabanına yazar
public class RateLimitLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitLoggingMiddleware> _logger;

    public RateLimitLoggingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // Middleware pipeline'ını işler
    public async Task InvokeAsync(HttpContext context)
    {
        var clientIdentifier = GetClientIdentifier(context);
        var clientType = GetClientType(context);
        var ipAddress = GetClientIpAddress(context);
        var endpoint = context.Request.Path.ToString();
        var httpMethod = context.Request.Method;

        _logger.LogDebug("🔍 RateLimitLoggingMiddleware BAŞLADI: {Endpoint} {Method} - {ClientIdentifier} ({ClientType})",
            endpoint, httpMethod, clientIdentifier, clientType);

        int statusCode = 200;
        bool hasError = false;
        string? errorMessage = null;

        try
        {
            // Sonraki middleware'e geç
            await _next(context);

            // HttpContext hala aktifken StatusCode'u al
            statusCode = context.Response.StatusCode;

            _logger.LogDebug("✅ Pipeline TAMAMLANDI: StatusCode={StatusCode} | {Endpoint} {Method}",
                statusCode, endpoint, httpMethod);
        }
        catch (Exception ex)
        {
            hasError = true;
            statusCode = 500;
            errorMessage = ex.Message;

            _logger.LogError(ex, "❌ RateLimitLoggingMiddleware'de hata oluştu: {Endpoint} {Method}",
                endpoint, httpMethod);

            throw; // Hatayı yeniden fırlat
        }
        finally
        {
            // HttpContext dispose olmadan önce logging işlemini başlat
            var isLimited = statusCode == 429;
            var message = hasError ? $"Hata: {errorMessage}" : 
                         isLimited ? "Rate limit aşıldı" : "İstek başarılı";

            _logger.LogInformation("📊 RESPONSE COMPLETED: StatusCode={StatusCode}, IsLimited={IsLimited}, Endpoint={Endpoint}", 
                statusCode, isLimited, endpoint);

            // Fire and forget - database lock'u önlemek için ayrı task'ta çalıştır
            _ = Task.Run(async () =>
            {
                try
                {
                    await ProcessResponseAsync(endpoint, httpMethod, clientIdentifier, clientType, ipAddress, statusCode, isLimited, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Background logging'de hata oluştu: {Endpoint}", endpoint);
                }
            });
        }
    }

    // Response'u işler ve gerekli logları oluşturur
    private async Task ProcessResponseAsync(
        string endpoint,
        string httpMethod,
        string clientIdentifier,
        string clientType,
        string ipAddress,
        int statusCode,
        bool isLimited,
        string message)
    {
        _logger.LogDebug("📊 RESPONSE İŞLENİYOR: StatusCode={StatusCode}, IsLimited={IsLimited}, Endpoint={Endpoint}", 
            statusCode, isLimited, endpoint);

        // RateLimitLog kaydı oluştur
        await LogRateLimitRequestAsync(
            endpoint,
            httpMethod,
            clientIdentifier,
            clientType,
            ipAddress,
            isLimited,
            message
        );

        // Eğer rate limit aşıldıysa RateLimitViolation kaydı oluştur
        if (isLimited)
        {
            _logger.LogWarning("🚨 RATE LIMIT İHLALİ TESPİT EDİLDİ: {ClientIdentifier} ({ClientType}) - {Endpoint} {Method}", 
                clientIdentifier, clientType, endpoint, httpMethod);

            await LogRateLimitViolationAsync(
                clientIdentifier,
                clientType,
                ipAddress,
                endpoint,
                httpMethod,
                message
            );
        }
        else
        {
            _logger.LogDebug("✅ Normal istek kaydedildi: {ClientIdentifier} ({ClientType}) - {Endpoint} {Method}", 
                clientIdentifier, clientType, endpoint, httpMethod);
        }
    }

    // Rate limit isteğini loglar
    private async Task LogRateLimitRequestAsync(
        string endpoint,
        string httpMethod,
        string clientIdentifier,
        string clientType,
        string ipAddress,
        bool isLimited,
        string message)
    {
        try
        {
            // ApiDbContext'i doğrudan oluştur
            var connectionString = "Data Source=LegoApi.db";
            var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>();
            optionsBuilder.UseSqlite(connectionString);
            
            using var dbContext = new ApiDbContext(optionsBuilder.Options);

            var logEntry = new RateLimitLog
            {
                Endpoint = endpoint,
                HttpMethod = httpMethod,
                ClientIdentifier = clientIdentifier,
                ClientType = clientType,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                IsLimited = isLimited,
                Message = message
            };

            dbContext.RateLimitLogs.Add(logEntry);
            await dbContext.SaveChangesAsync();

            _logger.LogDebug("Rate limit log kaydı oluşturuldu: {Endpoint} {Method} - {Client} ({Type}) - Limited: {IsLimited}",
                endpoint, httpMethod, clientIdentifier, clientType, isLimited);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rate limit log kaydı oluşturulurken database hatası oluştu");
            throw new RateLimitingException(
                "Rate limit log kaydı oluşturulamadı", 
                new RateLimitingExceptionData 
                { 
                    ErrorType = RateLimitingErrorType.Database,
                    Operation = "INSERT", 
                    TableName = "RateLimitLogs" 
                }, 
                ex);
        }
    }

    // Rate limit ihlalini loglar
    private async Task LogRateLimitViolationAsync(
        string clientIdentifier,
        string clientType,
        string ipAddress,
        string endpoint,
        string httpMethod,
        string message)
    {
        try
        {
            // ApiDbContext'i doğrudan oluştur
            var connectionString = "Data Source=LegoApi.db";
            var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>();
            optionsBuilder.UseSqlite(connectionString);
            
            using var dbContext = new ApiDbContext(optionsBuilder.Options);

            var violationEntry = new RateLimitViolation
            {
                ClientIdentifier = clientIdentifier,
                ClientType = clientType,
                IpAddress = ipAddress,
                Endpoint = endpoint,
                HttpMethod = httpMethod,
                ViolationTime = DateTime.UtcNow,
                RuleName = $"{clientType} bazlı limit",
                Message = message
            };

            dbContext.RateLimitViolations.Add(violationEntry);
            await dbContext.SaveChangesAsync();

            _logger.LogWarning("Rate limit ihlali kaydedildi: {Client} ({Type}) - {Endpoint} {Method}",
                clientIdentifier, clientType, endpoint, httpMethod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rate limit violation kaydı oluşturulurken database hatası oluştu");
            throw new RateLimitingException(
                "Rate limit violation kaydı oluşturulamadı", 
                new RateLimitingExceptionData 
                { 
                    ErrorType = RateLimitingErrorType.Database,
                    Operation = "INSERT", 
                    TableName = "RateLimitViolations" 
                }, 
                ex);
        }
    }

    // Client identifier'ını alır
    private string GetClientIdentifier(HttpContext context)
    {
        // X-ClientId header'ını kontrol et
        if (context.Request.Headers.TryGetValue("X-ClientId", out var clientId) && !string.IsNullOrEmpty(clientId))
        {
            return clientId.ToString();
        }

        // Authorization header'ını kontrol et
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader) && !string.IsNullOrEmpty(authHeader))
        {
            var authValue = authHeader.ToString();
            if (authValue.StartsWith("Bearer "))
            {
                return authValue.Substring(7);
            }
            return authValue;
        }

        // User ID'yi kontrol et
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst("sub")?.Value ?? 
                        context.User.FindFirst("nameid")?.Value ??
                        context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                return userId;
            }
        }

        // IP adresini al
        return GetClientIpAddress(context);
    }

    // Client tipini belirler
    private string GetClientType(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("X-ClientId"))
        {
            return "ClientId";
        }

        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            return "ApiKey";
        }

        if (context.User?.Identity?.IsAuthenticated == true)
        {
            return "UserId";
        }

        return "IP";
    }

    // Client IP adresini alır
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
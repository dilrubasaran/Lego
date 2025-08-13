using System.Net;
using System.Text.Json;
using Lego.Contexts.Enums;
using Lego.RateLimiting.Exceptions;
using Lego.RateLimiting.Models;
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
            await HandleExceptionAsync(context, ex);
        }
    }

    // Exception'ı analiz eder ve uygun HTTP response oluşturur
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, errorResponse) = exception switch
        {
            // Rate limiting exception'ları için özel işlem
            RateLimitingException rateLimitEx => rateLimitEx.Data.ErrorType switch
            {
                // Validation hataları → 400 Bad Request
                RateLimitingErrorType.Validation => (
                    HttpStatusCode.BadRequest,
                    new ErrorResponse
                    {
                        Error = "VALIDATION_ERROR",
                        Message = rateLimitEx.Message,
                        Details = new
                        {
                                                    FieldName = rateLimitEx.Data.FieldName,
                        InvalidValue = rateLimitEx.Data.InvalidValue,
                            Type = "Validation"
                        }
                    }),
                    
                // Rate limit aşımı → 429 Too Many Requests
                RateLimitingErrorType.LimitExceeded => (
                    HttpStatusCode.TooManyRequests,
                    new ErrorResponse
                    {
                        Error = "RATE_LIMIT_EXCEEDED", 
                        Message = rateLimitEx.Message,
                        Details = new
                        {
                            ClientIdentifier = rateLimitEx.Data.ClientIdentifier,
                            Type = "RateLimitExceeded"
                        }
                    }),
                    
                // Database hataları → 500 Internal Server Error
                RateLimitingErrorType.Database => (
                    HttpStatusCode.InternalServerError,
                    new ErrorResponse
                    {
                        Error = "DATABASE_ERROR",
                        Message = "Veritabanı işleminde hata oluştu",
                        Details = new
                        {
                                                    Operation = rateLimitEx.Data.Operation,
                        TableName = rateLimitEx.Data.TableName,
                            Type = "Database"
                        }
                    }),
                    
                // Diğer rate limiting hataları → 500 Internal Server Error
                _ => (
                    HttpStatusCode.InternalServerError,
                    new ErrorResponse
                    {
                        Error = "RATE_LIMITING_ERROR",
                        Message = rateLimitEx.Message,
                        Details = new
                        {
                            ErrorType = rateLimitEx.Data.ErrorType.ToString(),
                            Type = "RateLimiting"
                        }
                    })
            },

            // .NET built-in exception'ları
            ArgumentNullException argEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Error = "INVALID_ARGUMENT",
                    Message = "Geçersiz parametre",
                    Details = new
                    {
                        Parameter = argEx.ParamName,
                        Type = "ArgumentNull"
                    }
                }),

            // HTTP Request Format hataları
            BadHttpRequestException badHttpEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Error = "BAD_REQUEST_FORMAT",
                    Message = "HTTP istek formatı geçersiz",
                    Details = new
                    {
                        StatusCode = badHttpEx.StatusCode,
                        Type = "BadHttpRequest"
                    }
                }),

            // Tanımlanmamış diğer tüm hatalar → 500 Internal Server Error
            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse
                {
                    Error = "INTERNAL_ERROR",
                    Message = "Beklenmeyen bir hata oluştu",
                    Details = new
                    {
                        Type = exception.GetType().Name,
                        // Production'da inner details gösterme
                        InnerMessage = exception.InnerException?.Message
                    }
                })
        };

        context.Response.StatusCode = (int)statusCode;

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);

        // Detaylı logging
        _logger.LogError("❌ Error Response: StatusCode={StatusCode}, Error={Error}, Client={ClientIP}", 
            (int)statusCode, errorResponse.Error, GetClientIpAddress(context));
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

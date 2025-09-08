using System.Net;
using System.Text.Json;
using Lego.Contexts.Enums;
using Lego.RateLimiting.Models;
using Microsoft.AspNetCore.Http;

namespace Lego.RateLimiting.Exceptions;

// Exception'ları işleyen tek merkezi sınıf
// Tüm exception türlerini tek yerde yönetir
public static class ExceptionProcessor
{
    // Exception'ı işler ve JSON response oluşturur
    public static async Task ProcessExceptionAsync(HttpContext context, Exception exception)
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
    }
}

using Lego.API.DTOs.RateLimiting;
using Lego.Contexts.Enums;
using Lego.Contexts.Models;
using Lego.RateLimiting.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// Rate limiting test controller'ı. Farklı endpoint'ler için rate limiting kurallarını test etmek için kullanılır.

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RateLimitingController : ControllerBase
{
    public RateLimitingController()
    {
    }
    // public endpoint. 1 dakikada 5 istek limiti var.
    [HttpGet("public")]
    [ProducesResponseType(typeof(RateLimitResponse), 200)]
    [ProducesResponseType(typeof(RateLimitErrorResponse), 429)]
    public ActionResult<RateLimitResponse> PublicAccess()
    {
        return Ok(new RateLimitResponse
        {
            Message = "Public endpoint - Herkes erişebilir",
            RateLimit = "1dk/5 istek",
            Timestamp = DateTime.UtcNow,
            Status = "success"
        });
    }
    
    //Private erişim endpoint'i. 1 dakikada 3 istek limiti var.
    [HttpGet("private")]
    [ProducesResponseType(typeof(RateLimitResponse), 200)]
    [ProducesResponseType(typeof(RateLimitErrorResponse), 429)]
    public ActionResult<RateLimitResponse> PrivateAccess()
    {
        return Ok(new RateLimitResponse
        {
            Message = "Private endpoint - Sınırlı erişim",
            RateLimit = "1dk/3 istek",
            Timestamp = DateTime.UtcNow,
            Status = "success"
        });
    }
    
    //POST Veri gönderme endpoint'i. 1 dakikada 2 istek limiti var.

    [HttpPost("submit")]
    [ProducesResponseType(typeof(SubmitDataResponse), 200)]
    [ProducesResponseType(typeof(RateLimitErrorResponse), 400)]
    [ProducesResponseType(typeof(RateLimitErrorResponse), 429)]
    public ActionResult<SubmitDataResponse> SubmitData([FromBody] SubmitDataRequest data)
    {
        if (data == null)
        {
            throw new RateLimitingException("Request body boş olamaz", 
                new RateLimitingExceptionData 
                { 
                    ErrorType = RateLimitingErrorType.Validation,
                    FieldName = nameof(data), 
                    InvalidValue = data 
                });
        }
        
        if (string.IsNullOrEmpty(data.Content))
        {
            throw new RateLimitingException("Content alanı boş olamaz", 
                new RateLimitingExceptionData 
                { 
                    ErrorType = RateLimitingErrorType.Validation,
                    FieldName = nameof(data.Content), 
                    InvalidValue = data.Content 
                });
        }

        return Ok(new SubmitDataResponse
        {
            Message = "Veri başarıyla alındı",
            RateLimit = "1dk/2 istek",
            ReceivedData = data.Content,
            Timestamp = DateTime.UtcNow,
            Status = "success"
        });
    }
    
    // IP bazlı rate limiting test endpoint'i. Genel kurala tabi (1dk/10 istek)..
    [HttpGet("ip-specific")]
    [ProducesResponseType(typeof(RateLimitResponse), 200)]
    [ProducesResponseType(typeof(RateLimitErrorResponse), 429)]
    public ActionResult<RateLimitResponse> IPSpecific()
    {
        var clientIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        
        return Ok(new RateLimitResponse
        {
            Message = "IP bazlı rate limiting test",
            RateLimit = "1dk/10 istek",
            ClientIP = clientIP,
            Timestamp = DateTime.UtcNow,
            Status = "success"
        });
    }

    // 🔥 UserId bazlı rate limiting test endpoint'i - TEST: 1dk/3 istek
    [HttpGet("user-test")]
    [Authorize] // JWT gerekli
    [ProducesResponseType(typeof(RateLimitResponse), 200)]
    [ProducesResponseType(typeof(RateLimitErrorResponse), 429)]
    public ActionResult<RateLimitResponse> UserIdTest()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
        
        return Ok(new RateLimitResponse
        {
            Message = "🔥 UserId bazlı rate limiting test - TEST LİMİTİ",
            RateLimit = "1dk/100 istek (TEST)",
            UserId = userId,
            UserName = userName,
            Timestamp = DateTime.UtcNow,
            Status = "success"
        });
    }


    // Anonymous kullanıcı için IP fallback test endpoint'i
    [HttpGet("anonymous-test")]
    [ProducesResponseType(typeof(RateLimitResponse), 200)]
    [ProducesResponseType(typeof(RateLimitErrorResponse), 429)]
    public ActionResult<RateLimitResponse> AnonymousTest()
    {
        var clientIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        
        return Ok(new RateLimitResponse
        {
            Message = "👤 Anonymous kullanıcı - IP bazlı fallback test",
            RateLimit = "IP bazlı limite tabi",
            ClientIP = clientIP,
            Timestamp = DateTime.UtcNow,
            Status = "success"
        });
    }



}




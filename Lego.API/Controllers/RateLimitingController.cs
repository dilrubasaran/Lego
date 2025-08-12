using Lego.API.DTOs.RateLimiting;
using Microsoft.AspNetCore.Mvc;

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
        if (data == null || string.IsNullOrEmpty(data.Content))
        {
            return BadRequest(new RateLimitErrorResponse
            {
                Error = "Geçersiz veri",
                Message = "Content alanı boş olamaz",
                StatusCode = 400
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



}




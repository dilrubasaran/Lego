using Microsoft.AspNetCore.Mvc;

// Rate limiting test controller'ı. Farklı endpoint'ler için rate limiting kurallarını test etmek için kullanılır.

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RateLimitingController : ControllerBase
{
    // Herkesin erişebileceği public endpoint. 1 dakikada 5 istek limiti var.
    
    /// <summary>
    /// Public erişim endpoint'i. Herkes bu endpoint'e erişebilir.
    /// </summary>
    /// <remarks>
    /// Bu endpoint 1 dakikada maksimum 5 istek kabul eder.
    /// Rate limit aşıldığında HTTP 429 (Too Many Requests) döner.
    /// </remarks>
    /// <response code="200">Başarılı erişim</response>
    /// <response code="429">Rate limit aşıldı</response>
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
    
    // Sınırlı erişim alanı. 1 dakikada 3 istek limiti var.
    
    /// <summary>
    /// Private erişim endpoint'i. Sınırlı erişim alanı.
    /// </summary>
    /// <remarks>
    /// Bu endpoint 1 dakikada maksimum 3 istek kabul eder.
    /// Rate limit aşıldığında HTTP 429 (Too Many Requests) döner.
    /// </remarks>
    /// <response code="200">Başarılı erişim</response>
    /// <response code="429">Rate limit aşıldı</response>
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
    
    // Veri gönderme endpoint'i. 1 dakikada 2 istek limiti var.
    
    /// <summary>
    /// Veri gönderme endpoint'i. POST isteği ile veri alır.
    /// </summary>
    /// <remarks>
    /// Bu endpoint 1 dakikada maksimum 2 istek kabul eder.
    /// Rate limit aşıldığında HTTP 429 (Too Many Requests) döner.
    /// </remarks>
    /// <param name="data">Gönderilecek veri</param>
    /// <response code="200">Veri başarıyla alındı</response>
    /// <response code="400">Geçersiz veri</response>
    /// <response code="429">Rate limit aşıldı</response>
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
    
    // IP bazlı rate limiting test endpoint'i. Genel kurala tabi (1dk/10 istek).
    
    /// <summary>
    /// IP bazlı rate limiting test endpoint'i.
    /// </summary>
    /// <remarks>
    /// Bu endpoint genel rate limiting kuralına tabidir (1dk/10 istek).
    /// Her IP adresi için ayrı sayaç tutulur.
    /// Rate limit aşıldığında HTTP 429 (Too Many Requests) döner.
    /// </remarks>
    /// <response code="200">Başarılı erişim</response>
    /// <response code="429">Rate limit aşıldı</response>
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

// Response model'leri
public class RateLimitResponse
{
    /// <summary>
    /// Response mesajı
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Rate limit bilgisi
    /// </summary>
    public string RateLimit { get; set; } = string.Empty;
    
    /// <summary>
    /// İşlem zamanı
    /// </summary>
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// İşlem durumu
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Client IP adresi (opsiyonel)
    /// </summary>
    public string? ClientIP { get; set; }
}

public class SubmitDataRequest
{
    /// <summary>
    /// Gönderilecek veri içeriği
    /// </summary>
    public string Content { get; set; } = string.Empty;
}

public class SubmitDataResponse : RateLimitResponse
{
    /// <summary>
    /// Alınan veri
    /// </summary>
    public string ReceivedData { get; set; } = string.Empty;
}

public class RateLimitErrorResponse
{
    /// <summary>
    /// Hata türü
    /// </summary>
    public string Error { get; set; } = string.Empty;
    
    /// <summary>
    /// Hata mesajı
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// HTTP status kodu
    /// </summary>
    public int StatusCode { get; set; }
    
    /// <summary>
    /// Hata zamanı
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

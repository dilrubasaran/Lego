namespace Lego.API.DTOs.RateLimiting;

// Rate limiting hata yanıt modeli
public class RateLimitErrorResponse
{
    // Hata türü (RateLimit, Validation vb.)
    public string Error { get; set; } = string.Empty;
    
    // Kullanıcı dostu hata mesajı
    public string Message { get; set; } = string.Empty;
    
    // HTTP durum kodu (400, 429 vb.)
    public int StatusCode { get; set; }
    
    // Hata oluşma zamanı (UTC)
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Rate limit sıfırlanma zamanı (opsiyonel)
    public DateTime? RetryAfter { get; set; }
}

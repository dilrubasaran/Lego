namespace Lego.API.DTOs.RateLimiting;

// Rate limiting başarılı yanıt modeli
public class RateLimitResponse
{
    // Yanıt mesajı
    public string Message { get; set; } = string.Empty;
    
    // Rate limit bilgisi (örn: "1dk/5 istek")
    public string RateLimit { get; set; } = string.Empty;
    
    // İşlem zamanı (UTC)
    public DateTime Timestamp { get; set; }
    
    // İşlem durumu (success, error vb.)
    public string Status { get; set; } = string.Empty;
    
    // Client IP adresi (opsiyonel)
    public string? ClientIP { get; set; }
    
    // UserId (JWT'den alınan - opsiyonel)
    public string? UserId { get; set; }
    
    // UserName (JWT'den alınan - opsiyonel)
    public string? UserName { get; set; }
    
    // Request ID (ardışık testler için - opsiyonel)
    public string? RequestId { get; set; }
}

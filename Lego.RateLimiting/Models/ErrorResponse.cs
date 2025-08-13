namespace Lego.RateLimiting.Models;

// Standardize edilmiş hata yanıt modeli
// Tüm API hatalarında tutarlı format sağlar
public class ErrorResponse
{
    // Hata kodu (RATE_LIMIT_EXCEEDED, DATABASE_ERROR vb.)
    public string Error { get; set; } = string.Empty;
    
    // Kullanıcı dostu hata mesajı
    public string Message { get; set; } = string.Empty;
    
    // Hata oluşma zamanı (UTC)
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Ek hata detayları (opsiyonel)
    public object? Details { get; set; }
}

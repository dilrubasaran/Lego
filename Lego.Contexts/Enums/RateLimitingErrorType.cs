namespace Lego.Contexts.Enums;

// Rate limiting işlemlerinde oluşabilecek hata türleri
public enum RateLimitingErrorType
{
    // Girdi validasyonu veya kural doğrulaması hataları
    Validation,
    
    // Rate limit aşım hataları  
    LimitExceeded,
    
    // Veritabanı işlem hataları
    Database,
    
    // Tanımlanmamış diğer hatalar
    Unknown
}

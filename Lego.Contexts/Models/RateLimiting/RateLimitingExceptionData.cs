using Lego.Contexts.Enums;

namespace Lego.Contexts.Models;

// Rate limiting exception verileri için model
public class RateLimitingExceptionData
{
    // Hata türü kategorisi
    public RateLimitingErrorType ErrorType { get; set; } = RateLimitingErrorType.Unknown;
    
    // Validation hatalarında problem olan alan adı
    public string? FieldName { get; set; }
    
    // Validation hatalarında geçersiz değer
    public object? InvalidValue { get; set; }
    
    // Rate limit aşımında hangi client etkilendi
    public string? ClientIdentifier { get; set; }
    
    // Database hatalarında hangi işlem yapılıyordu
    public string? Operation { get; set; }
    
    // Database hatalarında hangi tablo etkilendi
    public string? TableName { get; set; }
}

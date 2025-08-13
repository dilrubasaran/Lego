using Lego.Contexts.Enums;
using Lego.Contexts.Models;

namespace Lego.RateLimiting.Exceptions;

// Rate limiting işlemlerinde oluşan tüm hataları kapsayan exception sınıfı
public class RateLimitingException : Exception
{
    // Exception verileri
    public RateLimitingExceptionData Data { get; }

    // Basit exception oluşturur (sadece mesaj ve tip)
    public RateLimitingException(string message, RateLimitingErrorType errorType = RateLimitingErrorType.Unknown) 
        : base(message)
    {
        Data = new RateLimitingExceptionData { ErrorType = errorType };
    }

    // Model ile exception oluşturur (tüm detaylarla)
    public RateLimitingException(string message, RateLimitingExceptionData data, Exception? innerException = null) 
        : base(message, innerException)
    {
        Data = data;
    }


}

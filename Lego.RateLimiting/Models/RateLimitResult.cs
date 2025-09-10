namespace Lego.RateLimiting.Models;

// Rate limit işlem sonucu - Hem store hem de servis katmanında kullanılır
public class RateLimitResult
{
    // Sayaç değeri
    public int Count { get; set; }
    
    // Son güncelleme zamanı
    public DateTime Timestamp { get; set; }
    
    // Limit aşıldı mı?
    public bool IsLimitExceeded { get; set; }
    
    // İzin verilen maksimum istek sayısı
    public int Limit { get; set; }
    
    // Periyot süresi
    public TimeSpan Period { get; set; }
}
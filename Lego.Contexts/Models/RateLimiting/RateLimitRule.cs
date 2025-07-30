namespace Lego.Contexts.Models.RateLimiting;

public class RateLimitRule
{
    public int Id { get; set; } // DB'de yönetilebilirlik açısından
    public string Endpoint { get; set; }       
    public string HttpMethod { get; set; }       
    public string ClientType { get; set; }       // IP, UserId, ApiKey, Role gibi (genel kullanım tanımı)
    public int Limit { get; set; }              
    public TimeSpan Period { get; set; }         // 1 dakika, 1 saat vb.

    public bool IsActive { get; set; } = true;   // Geçici devre dışı bırakmalar için
    public string Description { get; set; }      // Açıklama yazılabilir, yönetim panelinde görünür

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

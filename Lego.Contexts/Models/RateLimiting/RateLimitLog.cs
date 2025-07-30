namespace Lego.Contexts.Models.RateLimiting;

public class RateLimitLog
{
    public int Id { get; set; }

    public string Endpoint { get; set; }
    public string HttpMethod { get; set; }

    public string ClientIdentifier { get; set; } // Token, UserId, vb. Rate limiting'in hedeflediği varlığın tam değeri tutar 
    public string ClientType { get; set; }       // "IP", "ApiKey", "UserId", vs  Limitin hangi kritere göre uygulandığını belirtir

    public string IpAddress { get; set; }        // IP adresi (ayrıca tutulur)

    public DateTime Timestamp { get; set; }

    public bool IsLimited { get; set; }          // Limit aşıldı mı?
    public string Message { get; set; }          // Neden engellendi, hangi kural devreye girdi gibi
}

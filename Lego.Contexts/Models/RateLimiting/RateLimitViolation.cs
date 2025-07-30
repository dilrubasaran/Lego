namespace Lego.Contexts.Models.RateLimiting;

public class RateLimitViolation
{
    public int Id { get; set; }

    public string ClientIdentifier { get; set; }
    public string ClientType { get; set; }      
    public string IpAddress { get; set; }

    public string Endpoint { get; set; }
    public string HttpMethod { get; set; }

    public DateTime ViolationTime { get; set; } = DateTime.UtcNow;
    public string RuleName { get; set; }         // İhlal edilen kural (örn. "10req/min per IP")

    public string Message { get; set; }      
}

namespace Lego.Contexts.Models.RateLimiting;

public class ClientWhitelist
{
    public int Id { get; set; }

    public string Identifier { get; set; }       // IP adresi, UserId, Token vs
    public string IdentifierType { get; set; }   // "IP", "UserId", "ApiKey" vb

    public string Description { get; set; }      // "Admin IP", "Geliştirici anahtarı"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

namespace Lego.API.DTOs.DataProtection;

// Link yenileme isteği DTO'su
public sealed class RenewLinkRequest
{
    public string SecureLink { get; set; } = string.Empty; // Yenilenecek güvenli link
    public int? NewExpiryMinutes { get; set; } = null; // Yeni geçerlilik süresi (dakika) - null ise çok uzun
    public string Purpose { get; set; } = "default"; // Amaç
}

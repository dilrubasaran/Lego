namespace Lego.API.DTOs.DataProtection;

// Basitleştirilmiş güvenli link oluşturma isteği DTO'su
public sealed class CreateSecureLinkRequest
{
    public string BaseUrl { get; set; } = string.Empty; // Base URL
    public string Path { get; set; } = string.Empty; // Path
    public string Data { get; set; } = string.Empty; // Şifrelenecek veri
    public int? ExpiryMinutes { get; set; } = null; // Geçerlilik süresi (dakika) - null ise çok uzun (10 yıl)
    public string Purpose { get; set; } = "default"; // Amaç
}

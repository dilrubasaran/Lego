namespace Lego.API.DTOs.DataProtection;

// Şifreleme isteği modeli
public sealed class ProtectRequest
{
    public string Plaintext { get; set; } = string.Empty; // Şifrelenecek veri
    public int? ExpiryMinutes { get; set; } = null; // Geçerlilik süresi (dakika) - null ise çok uzun
    public string Purpose { get; set; } = "default"; // Amaç (izolasyon)
}



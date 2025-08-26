namespace Lego.API.DTOs.DataProtection;

// Şifreleme isteği modeli
public sealed class ProtectRequest
{
    public string Plaintext { get; set; } = string.Empty; // Şifrelenecek veri
    public string Purpose { get; set; } = "default"; // Amaç (izolasyon)
}



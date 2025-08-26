namespace Lego.API.DTOs.DataProtection;

// Çözme isteği modeli
public sealed class UnprotectRequest
{
    public string Token { get; set; } = string.Empty; // Şifreli veri
    public string Purpose { get; set; } = "default"; // Amaç (aynı olmalı)
}



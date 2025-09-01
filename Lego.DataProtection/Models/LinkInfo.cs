namespace Lego.DataProtection.Models;

// Link bilgileri için response model
public sealed class LinkInfo
{
    public string Data { get; set; } = string.Empty; // Çıkarılan veri
    public bool IsValid { get; set; } = true; // Geçerli mi?
    public TimeSpan? RemainingTime { get; set; } = null; // Kalan süre
    public DateTime? ExpiresAt { get; set; } = null; // Geçerlilik bitiş zamanı
}

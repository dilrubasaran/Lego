namespace Lego.API.DTOs.DataProtection;

// Basitleştirilmiş link response DTO'su
public sealed class LinkResponse
{
    public string SecureLink { get; set; } = string.Empty; // Oluşturulan güvenli link
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma zamanı
    public DateTime? ExpiresAt { get; set; } = null; // Geçerlilik bitiş zamanı
    public TimeSpan? RemainingTime { get; set; } = null; // Kalan süre
    public bool IsValid { get; set; } = true; // Geçerlilik durumu
}

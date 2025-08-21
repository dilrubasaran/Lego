namespace Lego.API.DTOs.Auth;

// Şifre değiştirme yanıtı için DTO
public class ChangePasswordResponse
{
    // İşlem başarılı mı?
    public bool Success { get; set; }

    // Yanıt mesajı
    public string Message { get; set; } = string.Empty;

    // Şifre değiştirilme tarihi
    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;

    // Tüm oturumlar kapatıldı mı?
    public bool AllSessionsRevoked { get; set; }
}

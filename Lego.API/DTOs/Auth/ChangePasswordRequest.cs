using System.ComponentModel.DataAnnotations;

namespace Lego.API.DTOs.Auth;

// Şifre değiştirme isteği için DTO
public class ChangePasswordRequest
{
    // Mevcut şifre
    [Required(ErrorMessage = "Mevcut şifre gereklidir")]
    public string CurrentPassword { get; set; } = string.Empty;

    // Yeni şifre
    [Required(ErrorMessage = "Yeni şifre gereklidir")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string NewPassword { get; set; } = string.Empty;

    // Yeni şifre tekrarı (doğrulama için)
    [Required(ErrorMessage = "Şifre tekrarı gereklidir")]
    [Compare(nameof(NewPassword), ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

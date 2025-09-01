using System.ComponentModel.DataAnnotations;

namespace Lego.Web.Models;

// Şifre sıfırlama model'i
public sealed class PasswordResetModel
{
    [Required(ErrorMessage = "Token gerekli")]
    public string Token { get; set; } = string.Empty; // Güvenli token

    [Required(ErrorMessage = "E-posta gerekli")]
    [EmailAddress(ErrorMessage = "Geçerli e-posta adresi giriniz")]
    public string Email { get; set; } = string.Empty; // E-posta adresi

    [Required(ErrorMessage = "Yeni şifre gerekli")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty; // Yeni şifre

    [Required(ErrorMessage = "Şifre tekrarı gerekli")]
    [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty; // Şifre tekrarı

    public TimeSpan? RemainingTime { get; set; } = null; // Kalan süre (UI için)
}

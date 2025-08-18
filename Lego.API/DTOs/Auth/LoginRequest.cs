using System.ComponentModel.DataAnnotations;

namespace Lego.API.DTOs.Auth;

// Kullanıcı giriş isteği modeli
public class LoginRequest
{
    // Kullanıcı adı veya e-posta adresi
    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    [StringLength(100, ErrorMessage = "Kullanıcı adı en fazla 100 karakter olabilir")]
    public string Username { get; set; } = string.Empty;

    // Kullanıcı şifresi
    [Required(ErrorMessage = "Şifre zorunludur")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 100 karakter olmalıdır")]
    public string Password { get; set; } = string.Empty;
}

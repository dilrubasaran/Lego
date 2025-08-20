using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Lego.Contexts.Models.Auth;

namespace Lego.Contexts.Models;

// Kullanıcı bilgilerini tutan model
public class UserModel
{
    // Kullanıcı benzersiz kimliği
    [Key]
    public int Id { get; set; }

    // Kullanıcı adı (benzersiz)
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    // E-posta adresi (benzersiz)
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    // Şifre hash'i
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    // Ad
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    // Soyad
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    // Hesap aktif mi?
    public bool IsActive { get; set; } = true;

    // Hesap oluşturulma tarihi
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Son güncellenme tarihi
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Kullanıcı rolleri (JSON formatında saklanacak)
    [MaxLength(500)]
    public string Roles { get; set; } = "User";

    // Son giriş tarihi
    public DateTime? LastLoginAt { get; set; }

    // Tam ad property'si
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Kullanıcının sahip olduğu refresh token'lar (1 - n ilişki)
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
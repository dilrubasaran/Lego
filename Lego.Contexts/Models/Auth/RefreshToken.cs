using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lego.Contexts.Models;

namespace Lego.Contexts.Models.Auth;

// Kullanıcıya ait refresh token bilgilerini tutan model
public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    // Token değeri (gizli ve yeterince uzun olmalı)
    [Required]
    [MaxLength(200)]
    public string Token { get; set; } = string.Empty;

    // Token'ın ait olduğu kullanıcı kimliği
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    // İlişkili kullanıcı (isteğe bağlı olarak Include ile yüklenir)
    public UserModel? User { get; set; }

    // Oluşturulma tarihi (UTC)
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Son kullanma tarihi (UTC)
    public DateTime ExpiresAtUtc { get; set; }

    // İptal edilme tarihi (rotation veya manuel iptal için)
    public DateTime? RevokedAtUtc { get; set; }

    // Yerine geçen yeni token değeri (rotation takibi)
    [MaxLength(200)]
    public string? ReplacedByToken { get; set; }

    // Geçerli olup olmadığını hesaplayan yardımcı property
    [NotMapped]
    public bool IsActive => RevokedAtUtc == null && DateTime.UtcNow < ExpiresAtUtc;
}



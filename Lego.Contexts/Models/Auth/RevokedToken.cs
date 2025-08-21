using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lego.Contexts.Models.Auth;

// İptal edilen (blacklist'e alınan) refresh token kayıtlarını temsil eden model
public class RevokedToken
{
    [Key]
    public int Id { get; set; }

    // İptal edilen refresh token değeri (hash ya da raw). Basitlik için raw saklanıyor
    [Required]
    [MaxLength(200)]
    public string Token { get; set; } = string.Empty;

    // Token'a sahip kullanıcının kimliği (opsiyonel, sadece raporlama için)
    public int? UserId { get; set; }

    // İptal edilme tarihi (UTC)
    public DateTime RevokedAtUtc { get; set; } = DateTime.UtcNow;
}



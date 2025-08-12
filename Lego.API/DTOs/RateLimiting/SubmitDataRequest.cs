using System.ComponentModel.DataAnnotations;

namespace Lego.API.DTOs.RateLimiting;

// Veri gönderme isteği modeli
public class SubmitDataRequest
{
    // Gönderilecek veri içeriği
    [Required(ErrorMessage = "Content alanı zorunludur")]
    [StringLength(1000, ErrorMessage = "Content 1000 karakterden fazla olamaz")]
    public string Content { get; set; } = string.Empty;
}

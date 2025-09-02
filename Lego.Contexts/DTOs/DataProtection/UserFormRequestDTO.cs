using System;
using System.ComponentModel.DataAnnotations;

namespace Lego.Contexts.DTOs.DataProtection;

// Web'den gelen form verilerini taşır; kritik alanlar protect edilerek API'ye gönderilir
public sealed class UserFormRequestDTO
{
    // Ad
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    // Soyad
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    // IBAN (kritik veri, protect edilmiş gönderilir) - validation yok çünkü protect edilmiş
    public string IBAN { get; set; } = string.Empty;

    // T.C. Kimlik No (kritik veri, protect edilmiş gönderilir) - validation yok çünkü protect edilmiş
    public string TC { get; set; } = string.Empty;

    // E-posta
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    // Doğum Tarihi
    [Required]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    // Telefon Numarası
    [Required]
    [Phone]
    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;
}



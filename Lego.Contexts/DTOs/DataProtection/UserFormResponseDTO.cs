using System;
using System.ComponentModel.DataAnnotations;

namespace Lego.Contexts.DTOs.DataProtection;

// API'den dönen, unprotect edilmiş form verilerini taşır
public sealed class UserFormResponseDTO
{
    // Ad
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    // Soyad
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    // IBAN (unprotect edilmiş)
    [Required]
    [MaxLength(50)]
    [RegularExpression(@"^[A-Z]{2}\d{2}[A-Z0-9]{1,30}$", ErrorMessage = "Geçerli bir IBAN giriniz")]
    public string IBAN { get; set; } = string.Empty;

    // T.C. Kimlik No (unprotect edilmiş)
    [Required]
    [MaxLength(11)]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "TC 11 haneli olmalı")]
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



using System;
using System.ComponentModel.DataAnnotations;

namespace Lego.Contexts.Models.DataProtection;

// Kullanıcı form gönderimlerinin veritabanı modeli
public sealed class UserFormSubmission
{
    // Kayıt benzersiz kimliği
    [Key]
    public int Id { get; set; }

    // Ad
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    // Soyad
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    // IBAN (çözülmüş veri)
    [MaxLength(50)]
    public string IBAN { get; set; } = string.Empty;

    // TC Kimlik (çözülmüş veri)
    [MaxLength(20)]
    public string TC { get; set; } = string.Empty;

    // E-posta
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    // Doğum Tarihi
    public DateTime BirthDate { get; set; }

    // Telefon Numarası
    [MaxLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    // Kayıt oluşturulma tarihi (UTC)
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}



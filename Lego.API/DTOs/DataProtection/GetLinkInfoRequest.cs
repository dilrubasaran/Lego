namespace Lego.API.DTOs.DataProtection;

// Link bilgileri alma isteği DTO'su
public sealed class GetLinkInfoRequest
{
    public string SecureLink { get; set; } = string.Empty; // Bilgileri alınacak güvenli link
    public string Purpose { get; set; } = "default"; // Amaç
}

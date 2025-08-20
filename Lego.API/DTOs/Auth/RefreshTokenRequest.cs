namespace Lego.API.DTOs.Auth;

// Refresh token yenileme isteği DTO'su
public class RefreshTokenRequest
{
    // Refresh token değeri
    public string RefreshToken { get; set; } = string.Empty;
}



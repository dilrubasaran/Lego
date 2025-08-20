namespace Lego.API.DTOs.Auth;

// Refresh token yenileme yanıt DTO'su
public class RefreshTokenResponse
{
    // Yeni access token
    public string AccessToken { get; set; } = string.Empty;

    // Yeni access token süresi (dakika)
    public int ExpiresInMinutes { get; set; }

    // Yeni refresh token (rotation sonucu)
    public string RefreshToken { get; set; } = string.Empty;
}



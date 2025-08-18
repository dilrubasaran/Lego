namespace Lego.API.DTOs.Auth;

// Kullanıcı giriş yanıt modeli
public class LoginResponse
{
    // Üretilen JWT token
    public string Token { get; set; } = string.Empty;

    // Token'ın geçerlilik süresi (dakika)
    public int ExpiresInMinutes { get; set; }

    // Token türü (Bearer)
    public string TokenType { get; set; } = "Bearer";

    // Kullanıcı bilgileri
    public UserInfo User { get; set; } = new();
}

// Kullanıcı bilgi modeli
public class UserInfo
{
    // Kullanıcı kimliği
    public string Id { get; set; } = string.Empty;

    // Kullanıcı adı
    public string Username { get; set; } = string.Empty;

    // E-posta adresi
    public string Email { get; set; } = string.Empty;

    // Kullanıcı rolleri
    public List<string> Roles { get; set; } = new();
}

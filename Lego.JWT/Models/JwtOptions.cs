namespace Lego.JWT.Models;

// JWT yapılandırma seçeneklerini içeren model
public class JwtOptions
{
    // Yapılandırma section adı
    public const string SectionName = "Jwt";

    // Token'ı kim tarafından verildiğini belirten değer
    public string Issuer { get; set; } = string.Empty;

    // Token'ın hangi hedef kitle için oluşturulduğunu belirten değer
    public string Audience { get; set; } = string.Empty;

    // Token imzalama için kullanılan gizli anahtar
    public string SecretKey { get; set; } = string.Empty;

    // Token'ın kaç dakika geçerli olacağını belirten değer
    public int ExpirationMinutes { get; set; } = 15;

    // Refresh token'ın kaç gün geçerli olacağını belirleyen değer
    public int RefreshTokenDays { get; set; } = 7;
}

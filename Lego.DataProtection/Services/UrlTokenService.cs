using System.Text;
using Lego.DataProtection.Interfaces;

namespace Lego.DataProtection.Services;

// IDataProtectionService kullanarak URL güvenli token üretir/çözer
public sealed class UrlTokenService : IUrlTokenService
{
    private const string Purpose = "url-token"; // Amaç string'i
    private readonly IDataProtectionService _dataProtectionService; // DP servisi

    public UrlTokenService(IDataProtectionService dataProtectionService)
    {
        _dataProtectionService = dataProtectionService; // DI ile gelir
    }

    public string ToToken(string plaintext)
    {
        var protectedText = _dataProtectionService.Protect(plaintext, Purpose); // Şifrele
        var bytes = Encoding.UTF8.GetBytes(protectedText); // UTF8 byte'lara dönüştür
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('='); // URL-safe Base64
    }

    public string FromToken(string token)
    {
        var base64 = token.Replace('-', '+').Replace('_', '/'); // URL-safe geri çevir
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        var bytes = Convert.FromBase64String(base64); // Bytes'a çevir
        var protectedText = Encoding.UTF8.GetString(bytes); // Korunan metni al
        return _dataProtectionService.Unprotect(protectedText, Purpose); // Çöz
    }
}

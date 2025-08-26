using Lego.DataProtection.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace Lego.DataProtection.Services;

// IDataProtector tabanlı veri koruma servisi
public sealed class DataProtectionService : IDataProtectionService
{
    private readonly IDataProtectionProvider _dataProtectionProvider; // DP sağlayıcısı

    public DataProtectionService(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtectionProvider = dataProtectionProvider; // DI ile gelir
    }

    public string Protect(string plaintext, string purpose = "default")
    {
        var protector = _dataProtectionProvider.CreateProtector(purpose); // Amaç bazlı protector
        return protector.Protect(plaintext); // Şifreleme
    }

    public string Unprotect(string protectedText, string purpose = "default")
    {
        var protector = _dataProtectionProvider.CreateProtector(purpose); // Amaç bazlı protector
        return protector.Unprotect(protectedText); // Çözme
    }
}



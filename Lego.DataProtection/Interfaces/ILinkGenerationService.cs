using Lego.DataProtection.Models;

namespace Lego.DataProtection.Interfaces;

// Basitleştirilmiş güvenli link oluşturma servisi arayüzü
public interface ILinkGenerationService
{
    // Güvenli link oluşturur - süre null ise çok uzun (10 yıl)
    string CreateSecureLink(string baseUrl, string path, string data, TimeSpan? expiry = null, string purpose = "default");
    
    // Link bilgilerini tek seferde al (veri + geçerlilik + kalan süre)
    LinkInfo GetLinkInfo(string secureLink, string purpose = "default");
    
    // Link'i yeniler (yeni süre ile)
    string RenewLink(string secureLink, TimeSpan? newExpiry = null, string purpose = "default");
}

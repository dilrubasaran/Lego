using System.Text;
using Lego.DataProtection.Interfaces;
using Lego.DataProtection.Models;

namespace Lego.DataProtection.Services;

// Basitleştirilmiş güvenli link oluşturma servisi
public sealed class LinkGenerationService : ILinkGenerationService
{
    private readonly ITimeLimitedDataProtectionService _timeLimitedService; // Süreli koruma servisi

    public LinkGenerationService(ITimeLimitedDataProtectionService timeLimitedService)
    {
        _timeLimitedService = timeLimitedService; // DI ile gelir
    }

    public string CreateSecureLink(string baseUrl, string path, string data, TimeSpan? expiry = null, string purpose = "default")
    {
        var protectedData = _timeLimitedService.ProtectWithExpiry(data, expiry, purpose); // Veriyi süreli koru
        var urlSafeToken = ConvertToUrlSafe(protectedData); // URL-safe hale getir
        //url oluştur URL'lerde çift slash olmaması gerekiyor
        return $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}?token={urlSafeToken}"; // Link oluştur
    }

    public LinkInfo GetLinkInfo(string secureLink, string purpose = "default")
    {
        try
        {
            var token = ExtractTokenFromUrl(secureLink); // URL'den token'ı çıkar
            var protectedData = ConvertFromUrlSafe(token); // URL-safe'den geri çevir
            
            // Kalan süreyi al (süre kontrolü için)
            var remainingTime = _timeLimitedService.GetRemainingTime(protectedData, purpose);
            
            // Süre kontrolü yap
            bool isValid = remainingTime.HasValue && remainingTime.Value > TimeSpan.Zero;
            
            // Link geçersizse exception fırlat
            if (!isValid)
            {
                throw new InvalidOperationException("Link süresi dolmuş veya geçersiz");
            }
            
            // Geçerlilik bitiş zamanını hesapla (remainingTime'dan)
            var expiresAt = remainingTime.HasValue ? DateTime.UtcNow.Add(remainingTime.Value) : DateTime.UtcNow.AddDays(3650);
            
            // Veriyi çöz (link geçerli olduğu için güvenli)
            string data = _timeLimitedService.UnprotectWithExpiry(protectedData, purpose);
            
            return new LinkInfo
            {
                Data = data,
                IsValid = true,
                RemainingTime = remainingTime,
                ExpiresAt = expiresAt
            };
        }
        catch (InvalidOperationException)
        {
            // Link süresi dolmuş - yeniden fırlat
            throw;
        }
        catch
        {
            // Diğer hatalar için genel exception
            throw new InvalidOperationException("Link geçersiz veya bozuk");
        }
    }

    public string RenewLink(string secureLink, TimeSpan? newExpiry = null, string purpose = "default")
    {
        var linkInfo = GetLinkInfo(secureLink, purpose); // Mevcut link bilgilerini al
        
        if (!linkInfo.IsValid)
        {
            throw new InvalidOperationException("Geçersiz link yenilenemez"); // Geçersiz link
        }
        
        var baseUrl = GetBaseUrlFromLink(secureLink); // Base URL'i al
        var path = GetPathFromLink(secureLink); // Path'i al
        
        return CreateSecureLink(baseUrl, path, linkInfo.Data, newExpiry, purpose); // Yeni link oluştur
    }

    // URL'den token'ı çıkarır
    private static string ExtractTokenFromUrl(string url)
    {
        var uri = new Uri(url); // URI parse et
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query); // Query string parse et
        return query["token"] ?? throw new ArgumentException("Token bulunamadı"); // Token'ı al
    }

    // Base URL'i alır
    private static string GetBaseUrlFromLink(string url)
    {
        var uri = new Uri(url); // URI parse et
        return $"{uri.Scheme}://{uri.Authority}"; // Scheme + Authority
    }

    // Path'i alır
    private static string GetPathFromLink(string url)
    {
        var uri = new Uri(url); // URI parse et
        var path = uri.AbsolutePath; // Absolute path
        return path.TrimStart('/'); // Başındaki /'yi kaldır
    }

    // URL-safe Base64'e çevirir
    private static string ConvertToUrlSafe(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input); // UTF8 byte'lara dönüştür
        return Convert.ToBase64String(bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('='); // URL-safe Base64
    }

    // URL-safe Base64'ten geri çevirir
    private static string ConvertFromUrlSafe(string input)
    {
        var base64 = input.Replace('-', '+').Replace('_', '/'); // URL-safe geri çevir
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        var bytes = Convert.FromBase64String(base64); // Bytes'a çevir
        return Encoding.UTF8.GetString(bytes); // UTF8 string'e çevir
    }
}

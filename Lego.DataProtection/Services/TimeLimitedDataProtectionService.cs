using Lego.DataProtection.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace Lego.DataProtection.Services;

// Basitleştirilmiş süreli veri koruma servisi
public sealed class TimeLimitedDataProtectionService : ITimeLimitedDataProtectionService
{
    private readonly IDataProtectionProvider _dataProtectionProvider; // DP sağlayıcısı
    private const int DEFAULT_EXPIRY_DAYS = 3650; // Varsayılan: 10 yıl

    public TimeLimitedDataProtectionService(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtectionProvider = dataProtectionProvider; 
    }

// token koruma 
    public string ProtectWithExpiry(string plaintext, TimeSpan? expiry = null, string purpose = "default")
{
    // 1. Amaç bazlı protector oluştur
    var protector = _dataProtectionProvider.CreateProtector(purpose);
    
    // 2. Süre hesapla
    var expiryDate = expiry.HasValue 
        ? DateTime.UtcNow.Add(expiry.Value)        
        : DateTime.UtcNow.AddDays(DEFAULT_EXPIRY_DAYS); // 10 yıl sonra
    
    // 3. Veri + süre bilgisini birleştir
    var dataWithExpiry = new { 
        Data = plaintext,           // "user123"
        ExpiresAt = expiryDate      // "2024-12-31 15:30:00"
    };
    
    // 4. JSON'a çevir
    var jsonData = JsonSerializer.Serialize(dataWithExpiry);
    
    // 5. Şifrele
    return protector.Protect(jsonData);
   
}

//Token çözme işlemi 
    public string UnprotectWithExpiry(string protectedText, string purpose = "default")
{
    // 1. Şifreyi çöz
    var protector = _dataProtectionProvider.CreateProtector(purpose);
    var jsonData = protector.Unprotect(protectedText);
    
    
    // 2. JSON'dan object'e çevir
    var dataWithExpiry = JsonSerializer.Deserialize<dynamic>(jsonData);
    
    // 3. Süre bilgisini al
    var expiresAt = DateTime.Parse(dataWithExpiry.GetProperty("ExpiresAt").GetString());
    
    // 4. Süre kontrolü
    if (DateTime.UtcNow > expiresAt)
    {
        throw new InvalidOperationException("Token süresi dolmuş");
    }
    
    // 5. Veriyi döner
    return dataWithExpiry.GetProperty("Data").GetString();
}

//Token geçerli mi kontrol eder
   public bool IsTokenValid(string protectedText, string purpose = "default")
{
    try
    {
        // Sadece süre kontrolü yap, veriyi çözme
        var remaining = GetRemainingTime(protectedText, purpose);
        return remaining.HasValue && remaining.Value > TimeSpan.Zero;
    }
    catch
    {
        return false;
    }
}

// Kalan süreyi hesaplar:
    public TimeSpan? GetRemainingTime(string protectedText, string purpose = "default")
    {
        try
        {
            // 1. Şifreyi çöz
            var protector = _dataProtectionProvider.CreateProtector(purpose);
            var jsonData = protector.Unprotect(protectedText);
            
            // 2. JSON'dan süre bilgisini al
            var dataWithExpiry = JsonSerializer.Deserialize<dynamic>(jsonData);
            var expiresAtString = dataWithExpiry.GetProperty("ExpiresAt").GetString();
            var expiresAt = DateTime.Parse(expiresAtString);
            
            // 3. Türkiye saati kullan (UTC+3)
            var now = DateTime.UtcNow.AddHours(3); // Türkiye saati
            
            // 4. Kalan süreyi hesapla
            var remaining = expiresAt - now;
            
            // 5. Süre kontrolü
            if (remaining <= TimeSpan.Zero)
            {
                return TimeSpan.Zero;
            }
            
            return remaining;
        }
        catch
        {
            return null; // Hata durumunda null
        }
    }
}
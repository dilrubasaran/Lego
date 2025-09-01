namespace Lego.DataProtection.Interfaces;

// Basitleştirilmiş süreli veri koruma servisi arayüzü
public interface ITimeLimitedDataProtectionService
{
    // Veriyi süreli korur (şifreler) - süre null ise çok uzun (10 yıl)
    string ProtectWithExpiry(string plaintext, TimeSpan? expiry = null, string purpose = "default");
    
    // Korumayı kaldırır ve süre kontrolü yapar
    string UnprotectWithExpiry(string protectedText, string purpose = "default");
    
    // Token'ın geçerlilik süresini kontrol eder
    bool IsTokenValid(string protectedText, string purpose = "default");
    
    // Token'ın kalan süresini döner
    TimeSpan? GetRemainingTime(string protectedText, string purpose = "default");
}

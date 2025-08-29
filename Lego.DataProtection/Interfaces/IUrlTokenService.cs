namespace Lego.DataProtection.Interfaces;

// URL'de kullanılacak güvenli token üretme/çözme servisi arayüzü
public interface IUrlTokenService
{
    // Metni token'a dönüştürür (ör. ID -> token)
    string ToToken(string plaintext);

    // Token'ı çözer (ör. token -> ID)
    string FromToken(string token);
}

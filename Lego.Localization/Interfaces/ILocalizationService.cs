using System.Security.AccessControl;
using ResourceType = Lego.Localization.Enum.ResourceType;

namespace Lego.Localization.Interfaces;

public interface ILocalizationService
{
    /// Belirtilen key ve resource type'a göre yerelleştirilmiş metni döndürür
    /// <param name="key">Kaynak anahtarı</param>
    /// <param name="type">Kaynak türü (Header, Footer, SharedResource)</param>
    /// <returns>Yerelleştirilmiş metin</returns>
    string Get(string key, ResourceType type);
    // Mevcut culture bilgisini döndürür
    // <returns>Culture kodu (örn: "tr-TR", "en-US")</returns>
    string GetCurrentCulture();
    
    /// <summary>
    /// Mevcut UI culture bilgisini döndürür
    /// </summary>
    /// <returns>UI Culture kodu</returns>
    string GetCurrentUICulture();
}
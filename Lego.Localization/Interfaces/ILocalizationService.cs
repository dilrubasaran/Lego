using System.Security.AccessControl;
using ResourceType = Lego.Localization.Enum.ResourceType;

namespace Lego.Localization.Interfaces;

public interface ILocalizationService
{
    // Belirtilen key ve resource type'a göre yerelleştirilmiş metni döndürür
    string Get(string key, ResourceType type);
    
    // Mevcut culture bilgisini döndürür
    // Returns: Culture kodu (örn: "tr-TR", "en-US")
    string GetCurrentCulture();
    
    // Mevcut UI culture bilgisini döndürürs
    // Returns: UI Culture kodu
    string GetCurrentUICulture();
}
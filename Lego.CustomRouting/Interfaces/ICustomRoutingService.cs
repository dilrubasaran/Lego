using Lego.CustomRouting.Models;

namespace Lego.CustomRouting.Interfaces;

// URL oluşturma ve route yönetimi için merkezi servis
public interface ICustomRoutingService
{
    // Kategori URL'i oluşturur: /category/{categoryId}
    string GetCategoryUrl(int categoryId);
    
    // Ürün URL'i oluşturur: /product/{productId}
    string GetProductUrl(int productId);
    
    // URL'den parametre çıkarma
    bool TryParseCategoryUrl(string url, out int categoryId);
    bool TryParseProductUrl(string url, out int productId);
    
    // URL validasyonu
    bool IsValidCategoryUrl(string url);
    bool IsValidProductUrl(string url);
}

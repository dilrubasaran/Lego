using Lego.CustomRouting.Models;

namespace Lego.CustomRouting.Interfaces;

// Fake data üretimi ve yönetimi için servis
public interface IFakeDataService
{
    // Kategori işlemleri
    List<Category> GetAllCategories();
    Category? GetCategoryById(int id);
    
    // Ürün işlemleri
    List<Product> GetAllProducts();
    List<Product> GetProductsByCategory(int categoryId);
    Product? GetProductById(int productId);
    Product? GetProductByCategoryAndId(int categoryId, int productId);
    
    // Data generation
    void GenerateFakeData();
    void ClearData();
}

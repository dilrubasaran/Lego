using Lego.CustomRouting.Interfaces;
using Lego.CustomRouting.Models;
using System.Text.RegularExpressions;

namespace Lego.CustomRouting.Services;

// URL oluşturma ve route yönetimi merkezi servis
public class CustomRoutingService : ICustomRoutingService
{
    private readonly IFakeDataService _dataService;

    // URL template'leri - merkezi tanım
    private const string CategoryUrlTemplate = "/category/{0}"; // /category/{id}
    private const string ProductUrlTemplate = "/product/{0}"; // /product/{productId}

    // URL parsing için regex pattern'leri
    private readonly Regex _categoryUrlRegex = new(@"^/category/(\d+)/?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private readonly Regex _productUrlRegex = new(@"^/product/(\d+)/?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public CustomRoutingService(IFakeDataService dataService)
    {
        _dataService = dataService;
    }

    // Kategori URL'leri
    public string GetCategoryUrl(int categoryId)
    {
        var category = _dataService.GetCategoryById(categoryId);
        if (category == null) 
            throw new ArgumentException($"Kategori bulunamadı: {categoryId}");
        
        return string.Format(CategoryUrlTemplate, categoryId);
    }

    // Ürün URL'leri
    public string GetProductUrl(int productId)
    {
        var product = _dataService.GetProductById(productId);
        if (product == null)
            throw new ArgumentException($"Ürün bulunamadı: ProductId={productId}");
        
        return string.Format(ProductUrlTemplate, productId);
    }

    // URL parsing
    public bool TryParseCategoryUrl(string url, out int categoryId)
    {
        categoryId = 0;
        
        if (string.IsNullOrEmpty(url)) return false;
        
        var match = _categoryUrlRegex.Match(url);
        if (!match.Success) return false;
        
        return int.TryParse(match.Groups[1].Value, out categoryId);
    }

    public bool TryParseProductUrl(string url, out int productId)
    {
        productId = 0;
        
        if (string.IsNullOrEmpty(url)) return false;
        
        var match = _productUrlRegex.Match(url);
        if (!match.Success) return false;
        
        return int.TryParse(match.Groups[1].Value, out productId);
    }

    // URL validasyonu
    public bool IsValidCategoryUrl(string url)
    {
        if (!TryParseCategoryUrl(url, out int categoryId)) return false;
        
        var category = _dataService.GetCategoryById(categoryId);
        return category != null && category.IsActive;
    }

    public bool IsValidProductUrl(string url)
    {
        if (!TryParseProductUrl(url, out int productId)) return false;
        
        var product = _dataService.GetProductById(productId);
        return product != null && product.IsActive;
    }


}

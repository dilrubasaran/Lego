using Lego.CustomRouting.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lego.Web.Controllers;

// Custom Routing özelliklerini web sayfasında test etmek için MVC controller
public class CustomRoutingController : Controller
{
    private readonly ICustomRoutingService _routingService;
    private readonly IFakeDataService _dataService;

    public CustomRoutingController(ICustomRoutingService routingService, IFakeDataService dataService)
    {
        _routingService = routingService;
        _dataService = dataService;
    }

    // Web için özel URL oluşturma methodları
    private string GetWebCategoryUrl(int categoryId)
    {
        return $"/category/{categoryId}";
    }

    private string GetWebProductUrl(int categoryId, int productId)
    {
        return $"/category/{categoryId}/product/{productId}";
    }

    // Ana sayfa - kategorileri listeler
    public IActionResult Index()
    {
        var categories = _dataService.GetAllCategories();
        
        var viewModel = categories.Select(c => new
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Url = GetWebCategoryUrl(c.Id),
            ProductCount = c.Products.Count,
            IsActive = c.IsActive
        });

        return View(viewModel);
    }

    // Kategori detay sayfası - custom route ile
    [Route("category/{categoryId:int}")]
    public IActionResult CategoryDetail(int categoryId)
    {
        var category = _dataService.GetCategoryById(categoryId);
        if (category == null) return NotFound($"Kategori bulunamadı: {categoryId}");

        var products = _dataService.GetProductsByCategory(categoryId);
        
        var viewModel = new
        {
            Category = new
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Url = GetWebCategoryUrl(category.Id)
            },
            Products = products.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Brand = p.Brand,
                Url = GetWebProductUrl(category.Id, p.Id),
                IsActive = p.IsActive
            }).ToList()
        };

        return View(viewModel);
    }

    // Ürün detay sayfası - custom route ile
    [Route("category/{categoryId:int}/product/{productId:int}")]
    public IActionResult ProductDetail(int categoryId, int productId)
    {
        var product = _dataService.GetProductById(productId);
        if (product == null) return NotFound($"Ürün bulunamadı: ProductId={productId}");

        var category = _dataService.GetCategoryById(categoryId);
        if (category == null) return NotFound($"Kategori bulunamadı: {categoryId}");

        // Ürünün doğru kategoride olup olmadığını kontrol et
        if (product.CategoryId != categoryId)
        {
            return NotFound($"Ürün {productId} kategori {categoryId}'de bulunamadı");
        }

        var viewModel = new
        {
            Product = new
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Brand = product.Brand,
                Url = GetWebProductUrl(category.Id, product.Id),
                IsActive = product.IsActive
            },
            Category = new
            {
                Id = category.Id,
                Name = category.Name,
                Url = GetWebCategoryUrl(category.Id)
            }
        };

        return View(viewModel);
    }

    // URL test sayfası
    public IActionResult UrlTest()
    {
        return View();
    }

    // URL parsing test (AJAX endpoint) - Web formatları için
    [HttpPost]
    public IActionResult ParseUrl(string url)
    {
        var result = new Dictionary<string, object>();

        // Web category URL pattern: /category/{id}
        var categoryPattern = new System.Text.RegularExpressions.Regex(@"^/category/(\d+)/?$");
        var categoryMatch = categoryPattern.Match(url);
        
        // Web product URL pattern: /category/{categoryId}/product/{productId}
        var productPattern = new System.Text.RegularExpressions.Regex(@"^/category/(\d+)/product/(\d+)/?$");
        var productMatch = productPattern.Match(url);

        if (categoryMatch.Success)
        {
            var categoryId = int.Parse(categoryMatch.Groups[1].Value);
            result["Type"] = "Category";
            result["CategoryId"] = categoryId;
            
            var category = _dataService.GetCategoryById(categoryId);
            result["IsValid"] = category != null && category.IsActive;
            
            if (category != null)
            {
                result["Data"] = new { category.Id, category.Name };
                result["Url"] = GetWebCategoryUrl(category.Id);
            }
        }
        else if (productMatch.Success)
        {
            var categoryId = int.Parse(productMatch.Groups[1].Value);
            var productId = int.Parse(productMatch.Groups[2].Value);
            
            result["Type"] = "Product";
            result["CategoryId"] = categoryId;
            result["ProductId"] = productId;
            
            var product = _dataService.GetProductById(productId);
            var category = _dataService.GetCategoryById(categoryId);
            
            result["IsValid"] = product != null && category != null && 
                               product.CategoryId == categoryId && 
                               product.IsActive && category.IsActive;
            
            if (product != null && category != null)
            {
                result["Data"] = new { product.Id, product.Name, product.CategoryId, CategoryName = category.Name };
                result["Url"] = GetWebProductUrl(category.Id, product.Id);
            }
        }
        else
        {
            result["Type"] = "Unknown";
            result["IsValid"] = false;
            result["Message"] = "URL formatı tanınmadı (Web format: /category/{id} veya /category/{id}/product/{id})";
        }

        return Json(result);
    }
}

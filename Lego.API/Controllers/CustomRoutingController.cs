using Lego.API.DTOs.CustomRouting;
using Lego.CustomRouting.Interfaces;
using Lego.CustomRouting.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lego.API.Controllers;

// Custom Routing özelliklerini test etmek için API controller
[ApiController]
[Route("api/custom-routing")]
public class CustomRoutingController : ControllerBase
{
    private readonly ICustomRoutingService _routingService;
    private readonly IFakeDataService _dataService;

    public CustomRoutingController(ICustomRoutingService routingService, IFakeDataService dataService)
    {
        _routingService = routingService;
        _dataService = dataService;
    }

    // Tüm kategorileri ve URL'lerini döner
    [HttpGet("categories")]
    public ActionResult<object> GetCategoriesWithUrls()
    {
        var categories = _dataService.GetAllCategories();
        var result = categories.Select(c => new
        {
            Id = c.Id,
            Name = c.Name,
            Url = _routingService.GetCategoryUrl(c.Id),
            IsActive = c.IsActive,
            ProductCount = c.Products.Count
        });

        return Ok(result);
    }

    // Belirli kategori ve ürünlerini URL'leriyle döner
    [HttpGet("categories/{categoryId:int}")]
    public ActionResult<object> GetCategoryWithProducts(int categoryId)
    {
        var category = _dataService.GetCategoryById(categoryId);
        if (category == null) return NotFound($"Kategori bulunamadı: {categoryId}");

        var products = _dataService.GetProductsByCategory(categoryId);
        var result = new
        {
            Category = new
            {
                Id = category.Id,
                Name = category.Name,
                Url = _routingService.GetCategoryUrl(category.Id),
                IsActive = category.IsActive
            },
            Products = products.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Brand = p.Brand,
                Url = _routingService.GetProductUrl(p.Id),
                IsActive = p.IsActive
            })
        };

        return Ok(result);
    }

    // URL parsing test endpoint'i
    [HttpPost("parse-url")]
    public ActionResult<object> ParseUrl([FromBody] ParseUrlRequest request)
    {
        var result = new Dictionary<string, object>();

        // Kategori URL'i test et
        if (_routingService.TryParseCategoryUrl(request.Url, out int categoryId))
        {
            result["Type"] = "Category";
            result["CategoryId"] = categoryId;
            result["IsValid"] = _routingService.IsValidCategoryUrl(request.Url);
            
            var category = _dataService.GetCategoryById(categoryId);
            if (category != null)
            {
                result["CategoryData"] = new { category.Id, category.Name };
                result["Url"] = _routingService.GetCategoryUrl(category.Id);
            }
        }
        // Ürün URL'i test et
        else if (_routingService.TryParseProductUrl(request.Url, out int productId))
        {
            result["Type"] = "Product";
            result["ProductId"] = productId;
            result["IsValid"] = _routingService.IsValidProductUrl(request.Url);
            
            var product = _dataService.GetProductById(productId);
            if (product != null)
            {
                result["ProductData"] = new { product.Id, product.Name, product.CategoryId };
                result["Url"] = _routingService.GetProductUrl(product.Id);
            }
        }
        else
        {
            result["Type"] = "Unknown";
            result["IsValid"] = false;
            result["Message"] = "URL formatı tanınmadı";
        }

        return Ok(result);
    }



    // Data yenileme endpoint'i (test için)
    [HttpPost("refresh-data")]
    public ActionResult RefreshData()
    {
        _dataService.GenerateFakeData();
        return Ok(new { Message = "Fake data yenilendi", Timestamp = DateTime.Now });
    }
}



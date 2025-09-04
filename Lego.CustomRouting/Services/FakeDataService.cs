using Bogus;
using Lego.CustomRouting.Interfaces;
using Lego.CustomRouting.Models;
using System.Text.RegularExpressions;

namespace Lego.CustomRouting.Services;

// Bogus kullanarak fake data üretir ve saklar
public class FakeDataService : IFakeDataService
{
    private readonly List<Category> _categories = new();
    private readonly List<Product> _products = new();
    private readonly Random _random = new();

    public FakeDataService()
    {
        GenerateFakeData();
    }

    public void GenerateFakeData()
    {
        ClearData();
        
        // Kategori fake data oluşturma
        var categoryFaker = new Faker<Category>()
            .RuleFor(c => c.Id, f => f.IndexFaker + 1)
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
            .RuleFor(c => c.Description, f => f.Commerce.ProductDescription())
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(2))
            .RuleFor(c => c.IsActive, f => f.Random.Bool(0.9f));

        // 10 kategori oluştur
        var categories = categoryFaker.Generate(10);
        _categories.AddRange(categories);

        // Her kategori için ürün oluşturma
        var productFaker = new Faker<Product>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => Math.Round(f.Random.Decimal(10, 1000), 2))
            .RuleFor(p => p.Brand, f => f.Company.CompanyName())
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(1))
            .RuleFor(p => p.IsActive, f => f.Random.Bool(0.85f));

        int productId = 1;
        foreach (var category in _categories)
        {
            // Her kategoride 3-8 ürün oluştur
            var productCount = _random.Next(3, 9);
            var products = productFaker.Generate(productCount);
            
            foreach (var product in products)
            {
                product.Id = productId++;
                product.CategoryId = category.Id;
                product.Category = category;
                _products.Add(product);
            }
            
            category.Products = products;
        }
    }

    public void ClearData()
    {
        _categories.Clear();
        _products.Clear();
    }

    public List<Category> GetAllCategories() => _categories.ToList();

    public Category? GetCategoryById(int id) => _categories.FirstOrDefault(c => c.Id == id);



    public List<Product> GetAllProducts() => _products.ToList();

    public List<Product> GetProductsByCategory(int categoryId) => _products.Where(p => p.CategoryId == categoryId).ToList();

    public Product? GetProductById(int productId) => _products.FirstOrDefault(p => p.Id == productId);



    public Product? GetProductByCategoryAndId(int categoryId, int productId) => 
        _products.FirstOrDefault(p => p.CategoryId == categoryId && p.Id == productId);
}

using Lego.Contexts.Models.RateLimiting;

namespace Lego.Contexts.Seed;

// Rate limiting için seed data
public static class RateLimitSeedData
{
    // Temel rate limit kurallarını döndürür
    public static IEnumerable<RateLimitRule> GetRateLimitRules()
    {
        return new List<RateLimitRule>
        {
            // Genel IP Limiti: 1 dakikada 10 istek
            new RateLimitRule
            {
                Id = 1,
                Endpoint = "*",
                HttpMethod = "*",
                ClientType = "IP",
                Limit = 10,
                Period = TimeSpan.FromMinutes(1),
                IsActive = true,
                Description = "Genel IP bazlı rate limit - 1 dakikada 10 istek",
                CreatedAt = DateTime.UtcNow
            },

            // LocalizationTest endpoint'i için özel limit: 1 dakikada 3 istek
            new RateLimitRule
            {
                Id = 2,
                Endpoint = "/Home/LocalizationTest",
                HttpMethod = "GET",
                ClientType = "IP",
                Limit = 3,
                Period = TimeSpan.FromMinutes(1),
                IsActive = true,
                Description = "LocalizationTest endpoint'i için özel limit - 1 dakikada 3 istek",
                CreatedAt = DateTime.UtcNow
            },

            // ChangeLanguage endpoint'i için özel limit: 1 dakikada 5 istek
            new RateLimitRule
            {
                Id = 3,
                Endpoint = "/Home/ChangeLanguage",
                HttpMethod = "POST",
                ClientType = "IP",
                Limit = 5,
                Period = TimeSpan.FromMinutes(1),
                IsActive = true,
                Description = "ChangeLanguage endpoint'i için özel limit - 1 dakikada 5 istek",
                CreatedAt = DateTime.UtcNow
            },

            // API endpoint'leri için daha sıkı limit: 1 dakikada 2 istek
            new RateLimitRule
            {
                Id = 4,
                Endpoint = "/api/*",
                HttpMethod = "*",
                ClientType = "IP",
                Limit = 2,
                Period = TimeSpan.FromMinutes(1),
                IsActive = true,
                Description = "API endpoint'leri için sıkı limit - 1 dakikada 2 istek",
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    // Temel whitelist kayıtlarını döndürür
    public static IEnumerable<ClientWhitelist> GetClientWhitelists()
    {
        return new List<ClientWhitelist>
        {
            // Localhost IP'si whitelist'te
            new ClientWhitelist
            {
                Id = 1,
                Identifier = "127.0.0.1",
                IdentifierType = "IP",
                Description = "Localhost IP - Geliştirme ortamı",
                CreatedAt = DateTime.UtcNow
            },

            // Test API Key
            new ClientWhitelist
            {
                Id = 2,
                Identifier = "test-api-key-12345",
                IdentifierType = "ApiKey",
                Description = "Test API Key - Geliştirme amaçlı",
                CreatedAt = DateTime.UtcNow
            },

            // Test Client ID
            new ClientWhitelist
            {
                Id = 3,
                Identifier = "test-client-001",
                IdentifierType = "ClientId",
                Description = "Test Client ID - Geliştirme amaçlı",
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    // Tüm seed data'ları veritabanına ekler
    public static void SeedAll(ApiDbContext context)
    {
        // Rate limit kurallarını ekle
        if (!context.RateLimitRules.Any())
        {
            context.RateLimitRules.AddRange(GetRateLimitRules());
            context.SaveChanges();
        }

        // Whitelist kayıtlarını ekle
        if (!context.ClientWhitelists.Any())
        {
            context.ClientWhitelists.AddRange(GetClientWhitelists());
            context.SaveChanges();
        }
    }
} 
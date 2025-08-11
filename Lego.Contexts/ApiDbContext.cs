using Lego.Contexts.Models;
using Microsoft.EntityFrameworkCore;
using Lego.Contexts.Models.RateLimiting;

namespace Lego.Contexts;

// API projesi için Rate Limiting tablolarını içeren DbContext
public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

    // Language tablosu (opsiyonel - API'de localization kullanılırsa)
    public DbSet<Language> Languages { get; set; }

    // Rate Limiting tabloları
    public DbSet<RateLimitRule> RateLimitRules { get; set; }
    public DbSet<RateLimitViolation> RateLimitViolations { get; set; }
    public DbSet<ClientWhitelist> ClientWhitelists { get; set; }
    public DbSet<RateLimitLog> RateLimitLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Language seed data (API'de de olsun)
        modelBuilder.Entity<Language>().HasData(
            new Language
            {
                Id = 1,
                LanguageLocaleCode = "tr",
                Name = "Türkçe",
                IsRtl = false,
                IsActive = true
            },
            new Language
            {
                Id = 2,
                LanguageLocaleCode = "en",
                Name = "English",
                IsRtl = false,
                IsActive = true
            }
        );

    }
}

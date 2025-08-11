using Lego.Contexts.Models;
using Microsoft.EntityFrameworkCore;

namespace Lego.Contexts;

// Web projesi için sadece Language tablosunu içeren DbContext
public class WebDbContext : DbContext
{
    public WebDbContext(DbContextOptions<WebDbContext> options) : base(options) { }

    // Web projesinde sadece Language tablosu kullanılacak
    public DbSet<Language> Languages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Language seed data
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

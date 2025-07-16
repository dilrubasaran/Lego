using Lego.Contexts.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lego.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Language> Languages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Language seed data
        modelBuilder.Entity<Language>().HasData(
            new Language
            {
                Id = 1,
                LanguageLocaleCode  = "tr",
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

    // Eğer başka tablolar olacaksa onları da ekle
}

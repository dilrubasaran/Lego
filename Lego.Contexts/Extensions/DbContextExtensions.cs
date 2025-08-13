using Lego.Contexts.Seed;

namespace Lego.Contexts.Extensions
{
    // DbContext için extension method'lar
    public static class DbContextExtensions
    {
        // API veritabanını seed data ile doldurur
        public static void SeedDatabase(this ApiDbContext context)
        {
            // Rate limiting seed data'larını yükle
            RateLimitSeedData.SeedAll(context);
            
            Console.WriteLine("✅ Rate limiting seed data başarıyla yüklendi.");
        }

        // API veritabanının boş olup olmadığını kontrol eder
        public static bool IsDatabaseEmpty(this ApiDbContext context)
        {
            return !context.Languages.Any() && 
                   !context.RateLimitRules.Any() && 
                   !context.ClientWhitelists.Any() && 
                   !context.RateLimitViolations.Any() && 
                   !context.RateLimitLogs.Any();
        }

        // API veritabanını sıfırlar ve yeniden seed eder
        public static void ResetAndSeedDatabase(this ApiDbContext context)
        {
            // Tüm verileri sil
            context.RateLimitLogs.RemoveRange(context.RateLimitLogs);
            context.RateLimitViolations.RemoveRange(context.RateLimitViolations);
            context.ClientWhitelists.RemoveRange(context.ClientWhitelists);
            context.RateLimitRules.RemoveRange(context.RateLimitRules);
            context.Languages.RemoveRange(context.Languages);
            
            context.SaveChanges();

            // Yeniden seed et
            context.SeedDatabase();
            
            Console.WriteLine("✅ Veritabanı sıfırlandı ve yeniden seed edildi.");
        }
    }
} 
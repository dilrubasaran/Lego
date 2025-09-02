using Lego.DataProtection.Interfaces;
using Lego.DataProtection.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Lego.DataProtection.Extensions;

// Veri koruma servisleri için DI uzantısı
public static class DataProtectionServiceCollectionExtensions
{
    //Interface ve Service kayıtlarını ekler
    public static IServiceCollection AddLegoDataProtection(this IServiceCollection services, IConfiguration configuration = null)
    {
        // Ortak Data Protection anahtarları
        services.AddDataProtection()
            .SetApplicationName("LegoApp"); // Bu minimum gerekli

        services.AddScoped<IDataProtectionService, DataProtectionService>(); // Servis kaydı
        services.AddScoped<IUrlTokenService, UrlTokenService>(); // URL token servisi kaydı
        services.AddScoped<ITimeLimitedDataProtectionService, TimeLimitedDataProtectionService>(); // Süreli koruma servisi kaydı
        services.AddScoped<ILinkGenerationService, LinkGenerationService>(); // Link oluşturma servisi kaydı
        return services; // Zincirleme kullanım için dönüş
    }
}



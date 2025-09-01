using Lego.DataProtection.Interfaces;
using Lego.DataProtection.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Lego.DataProtection.Extensions;

// Veri koruma servisleri için DI uzantısı
public static class DataProtectionServiceCollectionExtensions
{
    //Interface ve Service kayıtlarını ekler
    public static IServiceCollection AddLegoDataProtection(this IServiceCollection services)
    {
        services.AddDataProtection(); // DP altyapısını ekler
        services.AddScoped<IDataProtectionService, DataProtectionService>(); // Servis kaydı
        services.AddScoped<IUrlTokenService, UrlTokenService>(); // URL token servisi kaydı
        services.AddScoped<ITimeLimitedDataProtectionService, TimeLimitedDataProtectionService>(); // Süreli koruma servisi kaydı
        services.AddScoped<ILinkGenerationService, LinkGenerationService>(); // Link oluşturma servisi kaydı
        return services; // Zincirleme kullanım için dönüş
    }
}



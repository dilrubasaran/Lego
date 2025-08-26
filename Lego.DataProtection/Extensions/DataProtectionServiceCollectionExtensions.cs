using Lego.DataProtection.Interfaces;
using Lego.DataProtection.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Lego.DataProtection.Extensions;

// Veri koruma servisleri için DI uzantısı
public static class DataProtectionServiceCollectionExtensions
{
    // IDataProtection + IDataProtectionService kayıtlarını ekler
    public static IServiceCollection AddLegoDataProtection(this IServiceCollection services)
    {
        services.AddDataProtection(); // DP altyapısını ekler
        services.AddScoped<IDataProtectionService, DataProtectionService>(); // Servis kaydı
        return services; // Zincirleme kullanım için dönüş
    }
}



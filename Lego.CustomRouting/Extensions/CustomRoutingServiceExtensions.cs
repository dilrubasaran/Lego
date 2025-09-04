using Lego.CustomRouting.Interfaces;
using Lego.CustomRouting.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lego.CustomRouting.Extensions;

// DI container'a Custom Routing servislerini kaydetmek için extension
public static class CustomRoutingServiceExtensions
{
    // Custom Routing servislerini DI container'a kaydet
    public static IServiceCollection AddCustomRouting(this IServiceCollection services)
    {
        // Fake data service - Singleton olarak kaydet çünkü inmemory data tutuyor
        services.AddSingleton<IFakeDataService, FakeDataService>();
        
        services.AddScoped<ICustomRoutingService, CustomRoutingService>();
        
        return services;
    }
}

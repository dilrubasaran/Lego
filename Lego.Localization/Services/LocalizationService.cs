using System.Globalization;
using System.Reflection;
using Lego.Localization.Resources;
using Microsoft.Extensions.Localization;


namespace Lego.Localization.Services;

public class LocalizationService
{
    private readonly IStringLocalizer _stringLocalizer;

    public LocalizationService(IStringLocalizerFactory factory)
    {
        var type = typeof(SharedResource);
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
        _stringLocalizer = factory.Create("SharedResource", assemblyName.Name);
    }

    public LocalizedString GetLocalizedHTML(string key)
    {
        return _stringLocalizer[key];
    }

    public string GetCurrentCulture()
    {
        var currentCulture = CultureInfo.CurrentCulture.Name;
        return currentCulture;
    }

    public string GetCurrentUICulture()
    {
        var currentUICulture = CultureInfo.CurrentUICulture.Name;
        return currentUICulture;
    }
}
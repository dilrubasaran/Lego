using System.Globalization;
using System.Reflection;
using System.Security.AccessControl;
using Lego.Localization.Interfaces;
using Lego.Localization.Resources;
using Microsoft.Extensions.Localization;
using ResourceType = Lego.Localization.Enum.ResourceType;

namespace Lego.Localization.Services;

public class LocalizationService: ILocalizationService
{
    private readonly IStringLocalizerFactory _stringLocalizerFactory;

    public LocalizationService(IStringLocalizerFactory factory)
    {
        _stringLocalizerFactory = factory;
    }

    public string Get(string key, ResourceType type)
    {
        var resourceName = type.ToString(); // Örn: "Header", "Footer", "SharedResource"
        var assemblyName = new AssemblyName(typeof(LocalizationService).Assembly.FullName);

        // localizer: IStringLocalizer, verdiğin resourceName'e göre uygun .resx dosyasını yükler
        var localizer = _stringLocalizerFactory.Create(resourceName, assemblyName.Name);

        // Asıl çeviri işlemi burada gerçekleşiyor: key'e karşılık gelen değeri bulur
        return localizer[key] ?? "";
    }

    public string GetCurrentCulture()
    {
        return CultureInfo.CurrentCulture.Name;
    }

    public string GetCurrentUICulture()
    {
        return CultureInfo.CurrentUICulture.Name;
    }
}
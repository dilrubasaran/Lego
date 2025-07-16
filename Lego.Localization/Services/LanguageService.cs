using Lego.Contexts;
using Lego.Contexts.Models;
using Lego.Localization.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Lego.Localization.Services;

public class LanguageService : ILanguageService
{
    private readonly AppDbContext _context;
    private readonly LocalizationService _localizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CULTURE_COOKIE = "culture";

    public LanguageService(
        AppDbContext context,
        LocalizationService localizationService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _localizationService = localizationService;
        _httpContextAccessor = httpContextAccessor;
    }

    public Language DedectLanguage()
    {
        var culture = DedectVisitorCulture();
        return _context.Languages.FirstOrDefault(x => x.LanguageLocaleCode == culture);
    }

    public string DedectVisitorCulture()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            var cultureCookie = _httpContextAccessor.HttpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            if (!string.IsNullOrEmpty(cultureCookie))
            {
                var culture = CookieRequestCultureProvider.ParseCookieValue(cultureCookie);
                return culture?.Cultures.FirstOrDefault().Value ?? _localizationService.GetCurrentCulture();
            }
        }

        return _localizationService.GetCurrentCulture();
    }

    public string DedectVisitorUICulture()
    {
        var cultureCookie = _httpContextAccessor.HttpContext?.Request.Cookies[CULTURE_COOKIE];
        if (!string.IsNullOrEmpty(cultureCookie))
        {
            return cultureCookie;
        }

        return _localizationService.GetCurrentUICulture();
    }

    public void SetCulture(string culture)
    {
        var requestCulture = new RequestCulture(culture);
        var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

        var options = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            Path = "/"
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            cookieValue,
            options
        );
    }
}
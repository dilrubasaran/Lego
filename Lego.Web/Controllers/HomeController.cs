using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lego.Web.Models;
using Lego.Localization.Interfaces;

namespace Lego.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ILanguageService _languageService;

    public HomeController(ILanguageService languageService, ILogger<HomeController> logger)
    {
        _languageService = languageService;
        _logger = logger;
    }

    // Ana sayfa
    // Returns: Ana sayfa view'ı
    public IActionResult Index()
    {
        return View();
    }

    // Localization test sayfası
    // Returns: Localization test view'ı
    public IActionResult LocalizationTest()
    {
        return View();
    }

    // Dil değiştirme işlemi
    // culture: Kültür kodu
    // Returns: LocalizationTest sayfasına yönlendirme
    public IActionResult ChangeLanguage(string culture)
    {
        _languageService.SetCulture(culture);
        return RedirectToAction("LocalizationTest");
    }

    // Hata sayfası
    // Returns: Hata view'ı
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

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


    public IActionResult Index()
    {
        return View();
    }

    // Localization test sayfası
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

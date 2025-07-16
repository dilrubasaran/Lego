using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lego.Web.Models;
using Lego.Localization.Interfaces;

namespace Lego.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly ILanguageService _languageService;

    public HomeController(ILanguageService languageService)
    {
        _languageService = languageService;
    }

  
    public IActionResult LocalizationTest()
    {
        return View();
    }

    public IActionResult ChangeLanguage(string culture)
    {
        _languageService.SetCulture(culture);
        return RedirectToAction("LocalizationTest");
    }


}

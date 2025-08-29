using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lego.Web.Models;
using Lego.Localization.Interfaces;
using Lego.DataProtection.Interfaces;

namespace Lego.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ILanguageService _languageService;
    private readonly IUrlTokenService _urlTokenService; // URL token servisi

    public HomeController(ILanguageService languageService, ILogger<HomeController> logger, IUrlTokenService urlTokenService)
    {
        _languageService = languageService;
        _logger = logger;
        _urlTokenService = urlTokenService; // DI al
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

    // Direkt gizli URL'ye yönlendirir
    // id: gerçek sayısal kimlik
    public IActionResult GoToHiddenPage(int id = 5)
    {
        var token = _urlTokenService.ToToken(id.ToString()); // ID'yi token'a çevir
        return RedirectToRoute("ShortEdit", new { token }); // /e/{token} rotasına yönlendir
    }

    // Hakkımızda sayfasına gizli URL ile yönlendirir
    // id: sayfa kimliği (örn: 1 = hakkımızda)
    public IActionResult GoToAboutPage(int id = 1)
    {
        var token = _urlTokenService.ToToken(id.ToString()); // ID'yi token'a çevir
        return RedirectToRoute("ShortAbout", new { token }); // /a/{token} rotasına yönlendir
    }

    // Token üreterek kısa URL üretir
    // id: gerçek sayısal kimlik
    public IActionResult CreateEditLink(int id = 5)
    {
        var token = _urlTokenService.ToToken(id.ToString()); // ID'yi token'a çevir
        var shortUrl = Url.RouteUrl("ShortEdit", new { token }, Request.Scheme, Request.Host.ToString()); // /e/{token}
        var legacyUrl = Url.Action("Edit", "Home", new { token }, Request.Scheme);
        return Json(new { token, shortUrl, legacyUrl }); // Basit demo çıktı
    }

    // Token'lı edit sayfası
    // token: URL güvenli şifreli metin
    [HttpGet("/e/{token}", Name = "ShortEdit")] // Kısa rota ör: /e/{token}
    public IActionResult Edit(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return BadRequest("token gereklidir"); // Basit doğrulama
        var idText = _urlTokenService.FromToken(token); // Token çöz
        if (!int.TryParse(idText, out var id)) return BadRequest("token çözülemedi"); // Geçerli int mi
        ViewData["ResolvedId"] = id; // Görünüme aktar
        return View(); // Views/Home/Edit.cshtml
    }

    // Token'lı hakkımızda sayfası
    // token: URL güvenli şifreli metin
    [HttpGet("/a/{token}", Name = "ShortAbout")] // Kısa rota ör: /a/{token}
    public IActionResult About(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return BadRequest("token gereklidir"); // Basit doğrulama
        var idText = _urlTokenService.FromToken(token); // Token çöz
        if (!int.TryParse(idText, out var id)) return BadRequest("token çözülemedi"); // Geçerli int mi
        ViewData["PageId"] = id; // Görünüme aktar
        return View(); // Views/Home/About.cshtml
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

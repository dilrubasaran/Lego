using Lego.Web.Models;
using Lego.DataProtection.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lego.Web.Controllers;

// Şifre sıfırlama controller'ı
public sealed class PasswordResetController : Controller
{
    private readonly ILinkGenerationService _linkGenerationService; // Link oluşturma servisi
    private readonly ITimeLimitedDataProtectionService _timeLimitedService; // Süreli koruma servisi

    public PasswordResetController(
        ILinkGenerationService linkGenerationService,
        ITimeLimitedDataProtectionService timeLimitedService)
    {
        _linkGenerationService = linkGenerationService; // DI ile gelir
        _timeLimitedService = timeLimitedService; // DI ile gelir
    }

    // GET: /password-reset/reset
    [HttpGet("password-reset/reset")]
    public IActionResult Reset(string token)
    {
        try
        {
            // Token'ı güvenli link formatına çevir
            var secureLink = $"https://{Request.Host}/password-reset/reset?token={token}";
            
            // Link bilgilerini al
            var linkInfo = _linkGenerationService.GetLinkInfo(secureLink, "password-reset");
            
            if (!linkInfo.IsValid)
            {
                return View("Expired"); // Süre dolmuş sayfası
            }

            // Kalan süreyi kontrol et (3 dakikadan az ise uyarı)
            if (linkInfo.RemainingTime.HasValue && linkInfo.RemainingTime.Value.TotalMinutes < 3)
            {
                ViewBag.WarningMessage = "Link çok yakında geçersiz olacak!";
            }

            // Şifre sıfırlama formunu göster
            var model = new PasswordResetModel
            {
                Token = token,
                Email = linkInfo.Data, // Token'dan e-posta bilgisini al
                RemainingTime = linkInfo.RemainingTime
            };

            return View(model);
        }
        catch
        {
            return View("Expired"); // Hata durumunda süre dolmuş sayfası
        }
    }

    // POST: /password-reset/reset
    [HttpPost("password-reset/reset")]
    public IActionResult Reset(PasswordResetModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Validation hatası
        }

        try
        {
            // Token'ı güvenli link formatına çevir
            var secureLink = $"https://{Request.Host}/password-reset/reset?token={model.Token}";
            
            // Link bilgilerini al ve doğrula
            var linkInfo = _linkGenerationService.GetLinkInfo(secureLink, "password-reset");
            
            if (!linkInfo.IsValid)
            {
                return View("Expired"); // Süre dolmuş
            }

            // Şifre değiştirme işlemi (gerçek uygulamada DB'ye kaydedilir)
            // Burada sadece başarılı mesajı gösteriyoruz
            
            ViewBag.SuccessMessage = "Şifreniz başarıyla değiştirildi!";
            return View("Success");
        }
        catch
        {
            ModelState.AddModelError("", "Şifre değiştirilemedi. Lütfen tekrar deneyin.");
            return View(model);
        }
    }

    // GET: /password-reset/expired
    [HttpGet("password-reset/expired")]
    public IActionResult Expired()
    {
        return View();
    }

    // GET: /password-reset/success
    [HttpGet("password-reset/success")]
    public IActionResult Success()
    {
        return View();
    }

    // Şifre sıfırlama talep sayfası
    [HttpGet("password-reset/request")]
    public IActionResult RequestPasswordReset()
    {
        return View("RequestPasswordReset");
    }

    // Şifre sıfırlama talebi işleme
    [HttpPost("password-reset/request")]
    public IActionResult RequestPasswordReset(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError("", "E-posta adresi gerekli");
            return View();
        }

        try
        {
            // Test için 3 dakikalık link oluştur
            var secureLink = $"https://{Request.Host}/password-reset/reset";
            var link = _linkGenerationService.CreateSecureLink(
                $"https://{Request.Host}", 
                "password-reset/reset", 
                email, 
                TimeSpan.FromMinutes(1), 
                "password-reset"
            );

            // Gerçek uygulamada burada e-posta gönderilir
            ViewBag.SuccessMessage = $"Şifre sıfırlama linki oluşturuldu! Test için: {link}";
            ViewBag.TestLink = link;
            
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Link oluşturulamadı: {ex.Message}");
            return View();
        }
    }
}

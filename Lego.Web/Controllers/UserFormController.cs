using System.Net.Http.Json;
using Lego.Contexts.DTOs.DataProtection;
using Lego.DataProtection.Interfaces;
using Lego.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lego.Web.Controllers;

// Kullanıcı formu için controller; kritik alanları protect edip API'ye gönderir
public sealed class UserFormController : Controller
{
    private const string Purpose = "user-form"; // Protect amaç değeri
    private readonly IHttpClientFactory _httpClientFactory; // API çağrıları için HttpClient
    private readonly IDataProtectionService _dataProtectionService; // Protect/Unprotect servis

    public UserFormController(IHttpClientFactory httpClientFactory, IDataProtectionService dataProtectionService)
    {
        _httpClientFactory = httpClientFactory;
        _dataProtectionService = dataProtectionService;
    }

    // GET: /UserForm
    [HttpGet]
    public IActionResult UserForm()
    {
        return View();
    }

    // POST: /UserForm/SubmitForm
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitForm(UserFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("UserForm", model);
        }

        try
        {
            // Kritik alanları protect et
            var protectedIban = _dataProtectionService.Protect(model.IBAN, Purpose);
            var protectedTc = _dataProtectionService.Protect(model.TC, Purpose);

            var requestDto = new UserFormRequestDTO
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                IBAN = protectedIban,
                TC = protectedTc,
                Email = model.Email,
                BirthDate = model.BirthDate,
                PhoneNumber = model.PhoneNumber
            };

            // API çağrısı
            var client = _httpClientFactory.CreateClient("LegoApi");
            var response = await client.PostAsJsonAsync("api/UserFormApi/submit", requestDto);

            if (!response.IsSuccessStatusCode)
            {
                // API'den gelen hata mesajını oku
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Form gönderimi başarısız oldu. API Hatası: {errorContent}");
                return View("UserForm", model);
            }

            // Başarılı ise Success view'a yönlendir
            TempData["SuccessMessage"] = "Form başarıyla gönderildi.";
            return RedirectToAction("Success");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Hata: {ex.Message}");
            return View("UserForm", model);
        }
    }

    // GET: /UserForm/Success
    [HttpGet]
    public IActionResult Success()
    {
        return View();
    }
}

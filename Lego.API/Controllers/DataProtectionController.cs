using Lego.API.DTOs.DataProtection;
using Lego.DataProtection.Interfaces;
using Lego.DataProtection.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lego.API.Controllers;

// Basitleştirilmiş Data Protection controller'ı
[ApiController]
[Route("api/[controller]")]
public sealed class DataProtectionController : ControllerBase
{
    private readonly IDataProtectionService _dataProtectionService; // Veri koruma servisi
    private readonly ITimeLimitedDataProtectionService _timeLimitedService; // Süreli koruma servisi
    private readonly ILinkGenerationService _linkGenerationService; // Link oluşturma servisi

    public DataProtectionController(
        ITimeLimitedDataProtectionService timeLimitedService,
        ILinkGenerationService linkGenerationService,
        IDataProtectionService dataProtectionService)
    {
        _timeLimitedService = timeLimitedService; 
        _linkGenerationService = linkGenerationService; 
        _dataProtectionService = dataProtectionService; 
    }

//Güvenli link oluşturur 
    // POST: api/dataprotection/create-secure-link
    [HttpPost("create-secure-link")]
    public ActionResult<LinkResponse> CreateSecureLink([FromBody] CreateSecureLinkRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BaseUrl)) return BadRequest("baseUrl gerekli");
        if (string.IsNullOrWhiteSpace(request.Data)) return BadRequest("data gerekli");

        try
        {
            // Süre parametresi kontrolü (HasValue = null değil mi?)
            TimeSpan? expiry = request.ExpiryMinutes.HasValue ? TimeSpan.FromMinutes(request.ExpiryMinutes.Value) : null;
            
            //  Link oluştur
var secureLink = _linkGenerationService.CreateSecureLink(
    request.BaseUrl,    // "https://example.com"
    request.Path,       // "reset-password"
    request.Data,       // "user123"
    expiry,             // TimeSpan? (30 dakika veya null)
    request.Purpose     // "password-reset"
);
            // Link bilgilerini al
            var linkInfo = _linkGenerationService.GetLinkInfo(secureLink, request.Purpose);

// Response oluştur
            var response = new LinkResponse
            {
                SecureLink = secureLink,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = linkInfo.ExpiresAt,
                RemainingTime = linkInfo.RemainingTime,
                IsValid = linkInfo.IsValid
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Link oluşturulamadı: {ex.Message}");
        }
    }

//Link bilgilerini al
    // POST: api/dataprotection/get-link-info
    [HttpPost("get-link-info")]
    public ActionResult<LinkResponse> GetLinkInfo([FromBody] GetLinkInfoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SecureLink)) return BadRequest("secureLink gerekli");

        try
        {
            var linkInfo = _linkGenerationService.GetLinkInfo(request.SecureLink, request.Purpose);

            var response = new LinkResponse
            {
                SecureLink = request.SecureLink,
                CreatedAt = DateTime.UtcNow, // Bu bilgi mevcut değil
                ExpiresAt = linkInfo.ExpiresAt,
                RemainingTime = linkInfo.RemainingTime,
                IsValid = linkInfo.IsValid
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Link bilgileri alınamadı: {ex.Message}");
        }
    }
//Link süresini yeniler
    // POST: api/dataprotection/renew-link
    [HttpPost("renew-link")]
    public ActionResult<LinkResponse> RenewLink([FromBody] RenewLinkRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SecureLink)) return BadRequest("secureLink gerekli");

        try
        {
            // Yeni süre parametresini TimeSpan'e çevir (null ise varsayılan: çok uzun)
            TimeSpan? newExpiry = request.NewExpiryMinutes.HasValue ? TimeSpan.FromMinutes(request.NewExpiryMinutes.Value) : null;
            
            var renewedLink = _linkGenerationService.RenewLink(
                request.SecureLink, newExpiry, request.Purpose);

            // Yeni link bilgilerini al
            var linkInfo = _linkGenerationService.GetLinkInfo(renewedLink, request.Purpose);

            var response = new LinkResponse
            {
                SecureLink = renewedLink,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = linkInfo.ExpiresAt,
                RemainingTime = linkInfo.RemainingTime,
                IsValid = linkInfo.IsValid
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Link yenilenemedi: {ex.Message}");
        }
    }

    // POST: api/dataprotection/protect
    [HttpPost("protect")]
    public ActionResult<string> Protect([FromBody] ProtectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Plaintext)) return BadRequest("plaintext gerekli");
        var token = _dataProtectionService.Protect(request.Plaintext, request.Purpose);
        return Ok(token);
    }

    // POST: api/dataprotection/unprotect
    [HttpPost("unprotect")]
    public ActionResult<string> Unprotect([FromBody] UnprotectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token)) return BadRequest("token gerekli");
        try
        {
            var plaintext = _dataProtectionService.Unprotect(request.Token, request.Purpose);
            return Ok(plaintext);
        }
        catch (Exception ex)
        {
            return BadRequest($"Veri çözülemedi: {ex.Message}");
        }
    }
}

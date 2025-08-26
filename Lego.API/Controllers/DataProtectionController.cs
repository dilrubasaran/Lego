using Lego.API.DTOs.DataProtection;
using Lego.DataProtection.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lego.API.Controllers;

// Data Protection örnek controller'ı
[ApiController]
[Route("api/[controller]")]
public sealed class DataProtectionController : ControllerBase
{
    private readonly IDataProtectionService _dataProtectionService; // DP servisi

    public DataProtectionController(IDataProtectionService dataProtectionService)
    {
        _dataProtectionService = dataProtectionService; // DI ile gelir
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
        catch (Exception)
        {
            return BadRequest("Geçersiz token veya purpose");
        }
    }
}

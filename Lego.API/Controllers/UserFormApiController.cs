using Lego.Contexts;
using Lego.Contexts.DTOs.DataProtection;
using Lego.Contexts.Models.DataProtection;
using Lego.DataProtection.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lego.API.Controllers;

// Web'den gelen protect edilmiş form verilerini çözer ve kaydeder
[ApiController]
[Route("api/[controller]")]
public sealed class UserFormApiController : ControllerBase
{
    private const string Purpose = "user-form"; // Amaç: form alanları için koruma
    private readonly IDataProtectionService _dataProtectionService; // Veri koruma servisi
    private readonly ApiDbContext _dbContext; // Veritabanı bağlamı

    public UserFormApiController(IDataProtectionService dataProtectionService, ApiDbContext dbContext)
    {
        _dataProtectionService = dataProtectionService; // DI
        _dbContext = dbContext; // DI
    }

    // POST: api/UserFormApi/submit
    [HttpPost("submit")]
    public async Task<ActionResult<UserFormResponseDTO>> SubmitForm([FromBody] UserFormRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Kritik alanları çöz
            var iban = _dataProtectionService.Unprotect(request.IBAN, Purpose);
            var tc = _dataProtectionService.Unprotect(request.TC, Purpose);

            // Veritabanına kaydet
            var entity = new UserFormSubmission
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                IBAN = iban,
                TC = tc,
                Email = request.Email,
                BirthDate = request.BirthDate,
                PhoneNumber = request.PhoneNumber,
                CreatedAtUtc = DateTime.UtcNow
            };

            _dbContext.UserFormSubmissions.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Response DTO (unprotect edilmiş alanlar)
            var response = new UserFormResponseDTO
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                IBAN = entity.IBAN,
                TC = entity.TC,
                Email = entity.Email,
                BirthDate = entity.BirthDate,
                PhoneNumber = entity.PhoneNumber
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"İşlem başarısız: {ex.Message}");
        }
    }
}



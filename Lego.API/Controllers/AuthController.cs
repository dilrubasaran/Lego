using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Lego.API.DTOs.Auth;
using Lego.JWT.Interfaces;
using Lego.JWT.Models;

namespace Lego.API.Controllers;

// Kullanıcı kimlik doğrulama işlemlerini yöneten controller
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IClaimsService _claimsService;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthController> _logger;

    // AuthController constructor
    public AuthController(
        IJwtService jwtService,
        IClaimsService claimsService,
        IOptions<JwtOptions> jwtOptions,
        ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _claimsService = claimsService;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    // Kullanıcı giriş işlemi
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Kullanıcı giriş denemesi: {Username}", request.Username);

            // Model doğrulaması
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Geçersiz giriş verisi: {Username}", request.Username);
                return BadRequest("Geçersiz giriş bilgileri");
            }

            // Kullanıcı doğrulaması (örnek implementasyon)
            var user = await ValidateUserAsync(request.Username, request.Password);
            if (user == null)
            {
                _logger.LogWarning("Başarısız giriş denemesi: {Username}", request.Username);
                return Unauthorized("Geçersiz kullanıcı adı veya şifre");
            }

            // Claims oluşturma
            var claims = _claimsService.GetClaims(user.Id, user.Username, user.Email, user.Roles);

            // JWT token üretme
            var token = _jwtService.GenerateToken(claims);

            // Başarılı giriş loglama
            _logger.LogInformation("Başarılı kullanıcı girişi: {Username}", request.Username);

            // Yanıt oluşturma
            var response = new LoginResponse
            {
                Token = token,
                ExpiresInMinutes = _jwtOptions.ExpirationMinutes,
                TokenType = "Bearer",
                User = user
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Giriş işlemi sırasında hata oluştu: {Username}", request.Username);
            return StatusCode(500, "Sunucu hatası oluştu");
        }
    }

    // Token geçerliliğini kontrol eden endpoint
    [HttpGet("validate")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ValidateToken()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        return Ok(new
        {
            IsValid = true,
            UserId = userId,
            Username = username,
            Message = "Token geçerli"
        });
    }

    // Kullanıcı doğrulama işlemi (örnek implementasyon)
    // Gerçek uygulamada bu metod veritabanından kullanıcı bilgilerini çeker
    private async Task<UserInfo?> ValidateUserAsync(string username, string password)
    {
        // Bu örnek implementasyondur - gerçek uygulamada:
        // 1. Veritabanından kullanıcı bulunur
        // 2. Şifre hash'i doğrulanır  
        // 3. Kullanıcı rolleri çekilir
        // 4. Hesap durumu kontrol edilir (aktif/pasif, kilitli vs.)

        await Task.Delay(100); // Veritabanı çağrısını simüle et

        // Demo kullanıcı
        if (username == "admin" && password == "123456")
        {
            return new UserInfo
            {
                Id = "1",
                Username = "admin",
                Email = "admin@example.com",
                Roles = new List<string> { "Admin", "User" }
            };
        }

        if (username == "user" && password == "123456")
        {
            return new UserInfo
            {
                Id = "2",
                Username = "user",
                Email = "user@example.com",
                Roles = new List<string> { "User" }
            };
        }

        return null;
    }
}

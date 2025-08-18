using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Lego.API.DTOs.Auth;
using Lego.JWT.Interfaces;
using Lego.JWT.Models;
using Lego.Contexts.Interfaces;

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
    private readonly IUserService _userService;

    // AuthController constructor
    public AuthController(
        IJwtService jwtService,
        IClaimsService claimsService,
        IOptions<JwtOptions> jwtOptions,
        ILogger<AuthController> logger,
        IUserService userService)
    {
        _jwtService = jwtService;
        _claimsService = claimsService;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
        _userService = userService;
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

            // Veritabanından kullanıcı doğrulaması
            var userModel = await _userService.ValidateUserAsync(request.Username, request.Password);
            if (userModel == null)
            {
                _logger.LogWarning("Başarısız giriş denemesi: {Username}", request.Username);
                return Unauthorized("Geçersiz kullanıcı adı veya şifre");
            }

            // Son giriş tarihini güncelle
            await _userService.UpdateLastLoginAsync(userModel.Id);

            // Kullanıcı rollerini al
            var userRoles = _userService.GetUserRoles(userModel);

            // Claims oluşturma
            var claims = _claimsService.GetClaims(userModel.Id.ToString(), userModel.Username, userModel.Email, userRoles);

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
                User = new UserInfo
                {
                    Id = userModel.Id.ToString(),
                    Username = userModel.Username,
                    Email = userModel.Email,
                    Roles = userRoles
                }
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
        
        // Debug için tüm claim'leri göster
        var allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(new
        {
            IsValid = true,
            UserId = userId,
            Username = username,
            AllClaims = allClaims, // Debug için eklendi
            Message = "Token geçerli"
        });
    }

}

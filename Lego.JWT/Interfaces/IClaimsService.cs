using System.Security.Claims;

namespace Lego.JWT.Interfaces;

// Kullanıcıya özel claim üretim işlemlerini tanımlayan interface
public interface IClaimsService
{
    // Kullanıcı için gerekli claim'leri üretir
    IEnumerable<Claim> GetClaims(string userId, string username, string email, IEnumerable<string> roles);
}

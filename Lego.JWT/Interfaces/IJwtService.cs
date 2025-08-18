using System.Security.Claims;

namespace Lego.JWT.Interfaces;

// JWT token üretim işlemlerini tanımlayan interface
public interface IJwtService
{
    // Verilen claim'ler ile JWT token üretir
    string GenerateToken(IEnumerable<Claim> claims);
}

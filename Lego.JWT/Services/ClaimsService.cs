using System.Security.Claims;
using Lego.JWT.Interfaces;

namespace Lego.JWT.Services;

// Kullanıcıya özel claim üretim işlemlerini gerçekleştiren servis
public class ClaimsService : IClaimsService
{
    // Kullanıcı için gerekli claim'leri üretir
    public IEnumerable<Claim> GetClaims(string userId, string username, string email, IEnumerable<string> roles)
    {
        // Temel kullanıcı bilgileri
        // yield return olmasnın sebebi 
        // 1. IEnumerable<Claim> dönüş tipi ile birlikte kullanıldığında,
        //    yield return ile her bir claim'i tek tek döndürür ve böylece
        //    bellekte birden fazla claim nesnesi oluşturulmaz.
        // 2. Bu sayede performans artışı sağlanır ve bellek kullanımı azaltılır.
        // 3. Ayrıca, yield return kullanıldığında, dönüş tipi otomatik olarak
        //    IEnumerable<Claim> olarak algılanır ve bu sayede daha rahat kullanılabilir.
        yield return new Claim(ClaimTypes.NameIdentifier, userId);
        yield return new Claim(ClaimTypes.Name, username);
        yield return new Claim(ClaimTypes.Email, email);

        // Kullanıcı rolleri
        foreach (var role in roles)
        {
            yield return new Claim(ClaimTypes.Role, role);
        }

        // JWT standart claim'leri
        yield return new Claim("jti", Guid.NewGuid().ToString()); // JWT ID
        yield return new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64);

        // Özel claim'ler - gerektiğinde genişletilebilir
        yield return new Claim("department_id", "default");
        yield return new Claim("permission_level", "standard");
    }
}

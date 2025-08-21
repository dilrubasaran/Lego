using System.Threading.Tasks;

namespace Lego.JWT.Interfaces;

// Refresh token blacklist (iptal listesi) işlemlerini tanımlayan servis sözleşmesi
public interface ITokenBlacklistService
{
    // Verilen refresh token değerinin blacklist'te olup olmadığını kontrol eder
    Task<bool> IsTokenRevokedAsync(string token);

    // Verilen refresh token'ı blacklist'e ekler (iptal eder)
    Task RevokeTokenAsync(string token, int? userId = null);

    // Belirli bir kullanıcıya ait tüm refresh token'ları iptal eder
    Task RevokeAllForUserAsync(int userId);
}



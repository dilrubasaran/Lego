using Lego.Contexts.Models.Auth;

namespace Lego.JWT.Interfaces;

// Refresh token işlemlerini tanımlayan servis sözleşmesi
public interface IRefreshTokenService
{
    // Belirtilen kullanıcı için yeni bir refresh token üretir ve kaydeder
    Task<RefreshToken> GenerateRefreshTokenAsync(int userId, TimeSpan lifetime);

    // Verilen refresh token değerini doğrular ve geçerliyse ilişkili kullanıcı kimliğini döner
    Task<RefreshToken?> ValidateRefreshTokenAsync(string token);

    // Refresh token'ı iptal eder (rotation veya manuel iptal)
    Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByToken = null);
}



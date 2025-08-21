using System.Security.Cryptography;
using Lego.Contexts;
using Lego.Contexts.Models.Auth;
using Lego.JWT.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lego.JWT.Services;

// Refresh token üretim, doğrulama ve iptal işlemlerini yöneten servis
public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApiDbContext _dbContext;
    private readonly ITokenBlacklistService _blacklistService;

    public RefreshTokenService(ApiDbContext dbContext, ITokenBlacklistService blacklistService)
    {
        _dbContext = dbContext;
        _blacklistService = blacklistService;
    }

    // Kriptografik olarak güçlü bir token değeri üretir
    private static string GenerateSecureToken(int byteLength = 64)
    {
        var bytes = RandomNumberGenerator.GetBytes(byteLength);
        return Convert.ToBase64String(bytes);
    }

    // Yeni refresh token üretir ve veritabanına kaydeder
    public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId, TimeSpan lifetime)
    {
        var tokenValue = GenerateSecureToken();

        var token = new RefreshToken
        {
            UserId = userId,
            Token = tokenValue,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.Add(lifetime)
        };

        _dbContext.Set<RefreshToken>().Add(token);
        await _dbContext.SaveChangesAsync();
        return token;
    }

    // Verilen refresh token'ı doğrular; aktif ve süresi dolmamışsa döndürür
    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string token)
    {
        // Blacklist kontrolü: token daha önce iptal edilmiş mi?
        if (await _blacklistService.IsTokenRevokedAsync(token))
        {
            return null;
        }

        var entity = await _dbContext.Set<RefreshToken>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Token == token);

        if (entity == null)
        {
            return null;
        }

        if (entity.RevokedAtUtc != null)
        {
            return null;
        }

        if (entity.ExpiresAtUtc <= DateTime.UtcNow)
        {
            return null;
        }

        return entity;
    }

    // Refresh token'ı iptal eder; opsiyonel olarak yerine geçen token değerini kaydeder
    public async Task RevokeRefreshTokenAsync(RefreshToken token, string? replacedByToken = null)
    {
        var entity = await _dbContext.Set<RefreshToken>().FirstOrDefaultAsync(t => t.Id == token.Id);
        if (entity == null)
        {
            return;
        }

        entity.RevokedAtUtc = DateTime.UtcNow;
        entity.ReplacedByToken = replacedByToken;
        await _dbContext.SaveChangesAsync();

        // Blacklist'e ekle
        await _blacklistService.RevokeTokenAsync(entity.Token, entity.UserId);
    }
}



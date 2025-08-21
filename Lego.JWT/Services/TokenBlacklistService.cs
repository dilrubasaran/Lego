using Lego.Contexts;
using Lego.Contexts.Models.Auth;
using Lego.JWT.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lego.JWT.Services;

// Refresh token'ların blacklist'e alınmasını yöneten servis
public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly ApiDbContext _dbContext;

    public TokenBlacklistService(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Token'ın blacklist'te olup olmadığını kontrol eder
    public async Task<bool> IsTokenRevokedAsync(string token)
    {
        var exists = await _dbContext.Set<RevokedToken>()
            .AsNoTracking()
            .AnyAsync(t => t.Token == token);
        return exists;
    }

    // Token'ı blacklist'e ekler
    public async Task RevokeTokenAsync(string token, int? userId = null)
    {
        // Zaten eklenmişse tekrarlama
        var exists = await _dbContext.Set<RevokedToken>()
            .AsNoTracking()
            .AnyAsync(t => t.Token == token);
        if (exists)
        {
            return;
        }

        _dbContext.Set<RevokedToken>().Add(new RevokedToken
        {
            Token = token,
            UserId = userId,
            RevokedAtUtc = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();
    }

    // Kullanıcıya ait tüm refresh token'ları iptal eder şifre değiştirildiğinde kullanılır 
    public async Task RevokeAllForUserAsync(int userId)
    {
        // RefreshToken tablosundaki aktif token'ları iptal et
        var activeTokens = await _dbContext.Set<RefreshToken>()
            .Where(t => t.UserId == userId && t.RevokedAtUtc == null && t.ExpiresAtUtc > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.RevokedAtUtc = DateTime.UtcNow;
            token.ReplacedByToken = null;

            // Blacklist'e ekle
            _dbContext.Set<RevokedToken>().Add(new RevokedToken
            {
                Token = token.Token,
                UserId = userId,
                RevokedAtUtc = DateTime.UtcNow
            });
        }

        await _dbContext.SaveChangesAsync();
    }
}



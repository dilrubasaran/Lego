using Lego.Contexts.Interfaces;
using Lego.Contexts.Models;
using Lego.Contexts.Seed;
using Microsoft.EntityFrameworkCore;

namespace Lego.Contexts.Services;

// Kullanıcı işlemlerini gerçekleştiren servis
public class UserService : IUserService
{
    private readonly ApiDbContext _context;

    public UserService(ApiDbContext context)
    {
        _context = context;
    }

    // Kullanıcı adı ve şifre ile kullanıcı doğrulama
    public async Task<UserModel?> ValidateUserAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null)
            return null;

        // Şifre doğrulaması
        if (!UserSeedData.VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    // ID ile kullanıcı bulma
    public async Task<UserModel?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    // Kullanıcı adı ile kullanıcı bulma
    public async Task<UserModel?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
    }

    // Email ile kullanıcı bulma
    public async Task<UserModel?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    // Yeni kullanıcı oluşturma
    public async Task<UserModel> CreateUserAsync(UserModel user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    // Kullanıcı güncelleme
    public async Task<UserModel> UpdateUserAsync(UserModel user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    // Son giriş tarihini güncelleme
    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    // Kullanıcının rollerini string listesi olarak döndürme
    public List<string> GetUserRoles(UserModel user)
    {
        if (string.IsNullOrEmpty(user.Roles))
            return new List<string> { "User" };

        return user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(r => r.Trim())
                         .ToList();
    }
}
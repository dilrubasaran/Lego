using Lego.Contexts.Models;
using System.Security.Cryptography;
using System.Text;

namespace Lego.Contexts.Seed;

// Kullanıcı seed verilerini içeren static class
public static class UserSeedData
{
    // Varsayılan kullanıcıları döndürür
    public static List<UserModel> GetUsers()
    {
        return new List<UserModel>
        {
            new UserModel
            {
                Id = 1,
                Username = "admin",
                Email = "admin@lego.com",
                PasswordHash = HashPassword("123456"),
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                Roles = "Admin,User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new UserModel
            {
                Id = 2,
                Username = "user",
                Email = "user@lego.com",
                PasswordHash = HashPassword("123456"),
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                Roles = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new UserModel
            {
                Id = 3,
                Username = "manager",
                Email = "manager@lego.com",
                PasswordHash = HashPassword("123456"),
                FirstName = "Test",
                LastName = "Manager",
                IsActive = true,
                Roles = "Manager,User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }

    // Şifre hash'leme metodu (basit SHA256 - gerçek uygulamada BCrypt kullanılmalı)
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    // Şifre doğrulama metodu
    public static bool VerifyPassword(string password, string hash)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == hash;
    }
}

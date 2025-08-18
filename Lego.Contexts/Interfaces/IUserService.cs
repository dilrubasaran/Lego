using Lego.Contexts.Models;

namespace Lego.Contexts.Interfaces;

// Kullanıcı işlemlerini tanımlayan interface
public interface IUserService
{
    // Kullanıcı adı ve şifre ile kullanıcı doğrulama
    Task<UserModel?> ValidateUserAsync(string username, string password);
    
    // ID ile kullanıcı bulma
    Task<UserModel?> GetUserByIdAsync(int id);
    
    // Kullanıcı adı ile kullanıcı bulma
    Task<UserModel?> GetUserByUsernameAsync(string username);
    
    // Email ile kullanıcı bulma
    Task<UserModel?> GetUserByEmailAsync(string email);
    
    // Yeni kullanıcı oluşturma
    Task<UserModel> CreateUserAsync(UserModel user);
    
    // Kullanıcı güncelleme
    Task<UserModel> UpdateUserAsync(UserModel user);
    
    // Son giriş tarihini güncelleme
    Task UpdateLastLoginAsync(int userId);
    
    // Kullanıcının rollerini string listesi olarak döndürme
    List<string> GetUserRoles(UserModel user);
}
namespace Lego.DataProtection.Interfaces;

// Veriyi korumak ve çözmek için servis arayüzü
public interface IDataProtectionService
{
    // Veriyi korur (şifreler)
    string Protect(string plaintext, string purpose = "default");

    // Korumayı kaldırır (çözer)
    string Unprotect(string protectedText, string purpose = "default");
}



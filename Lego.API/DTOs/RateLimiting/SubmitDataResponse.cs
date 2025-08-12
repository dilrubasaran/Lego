namespace Lego.API.DTOs.RateLimiting;

// Veri gönderme başarılı yanıt modeli
public class SubmitDataResponse : RateLimitResponse
{
    // Alınan veri içeriği
    public string ReceivedData { get; set; } = string.Empty;
}

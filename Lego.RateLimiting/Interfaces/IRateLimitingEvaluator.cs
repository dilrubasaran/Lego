using Microsoft.AspNetCore.Http;

namespace Lego.RateLimiting.Interfaces;

// Rate limiting kurallarını değerlendiren evaluator'ları tanımlayan interface
public interface IRateLimitingEvaluator
{
    // Rate limiting değerlendirmesi yapar ve aşılıp aşılmadığını kontrol eder
    Task<bool> EvaluateAsync(HttpContext context);
}

namespace Lego.RateLimiting.Interfaces;

// Rate limit sayaç işlemleri için soyut servis
public interface IRateLimitCounterService
{
    Task<RateLimitCounter?> GetAsync(string key);
    Task<RateLimitCounter> IncrementAsync(string key, TimeSpan period);
    Task SetAsync(string key, RateLimitCounter counter, TimeSpan period);
}

// Bizim kendi sayaç model'imiz
public class RateLimitCounter
{
    public int Count { get; set; }
    public DateTime Timestamp { get; set; }
}

using Lego.RateLimiting.Middleware;
using Lego.RateLimiting.Stores;
using Lego.RateLimiting.Interfaces;
using Lego.RateLimiting.Evaluators;
using Lego.RateLimiting.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lego.RateLimiting.Extensions;

// Rate limiting servislerini ve middleware'lerini kolayca eklemek için extension method'ları
public static class RateLimitingExtensions
{
	// Rate limiting servislerini DI container'a ekler
	public static IServiceCollection AddLegoRateLimiting(this IServiceCollection services)
	{
		// Store'ları ekle
		services.AddScoped<IRateLimitRuleStore, CustomRateLimitRuleStore>();
		
		// Resolver servislerini ekle
		services.AddScoped<IUserIdResolver, UserIdResolver>();
		services.AddScoped<IClientIpResolver, ClientIpResolver>();
		
		// MemoryCache tabanlı counter servisimizi ekle (Redis'e kolay geçiş için abstraction altında)
		services.AddMemoryCache();
		services.AddScoped<IRateLimitCounterService, RateLimitCounterService>();
		
		// Evaluator'ları ekle
		services.AddScoped<IRateLimitingEvaluator, UserIdRateLimitingEvaluator>();
		
		return services;
	}

	// Global error handling middleware'ini pipeline'a ekler
	public static IApplicationBuilder UseGlobalErrorHandling(this IApplicationBuilder app)
	{
		return app.UseMiddleware<GlobalErrorHandlingMiddleware>();
	}

	// Rate limit logging middleware'ini pipeline'a ekler
	public static IApplicationBuilder UseRateLimitLogging(this IApplicationBuilder app)
	{
		return app.UseMiddleware<RateLimitLoggingMiddleware>();
	}

	// UserId bazlı rate limiting middleware'ini pipeline'a ekler
	public static IApplicationBuilder UseUserIdRateLimiting(this IApplicationBuilder app)
	{
		return app.UseMiddleware<RateLimitingMiddleware>();
	}
} 
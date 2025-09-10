using AspNetCoreRateLimit;
using Scalar.AspNetCore;
using Lego.RateLimiting.Extensions;
using Lego.Contexts;
using Microsoft.EntityFrameworkCore;
using Lego.JWT.Extensions;
using Lego.Contexts.Interfaces;
using Lego.Contexts.Services;
using Lego.DataProtection.Extensions;
using Lego.CustomRouting.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Model validation otomatik response'unu devre dışı bırak
        // Custom validation exception'larımızı kullanmak için
        options.SuppressModelStateInvalidFilter = true;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS ayarlarını ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ?? DB Context - API projesi için ApiDbContext kullan
builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// AspNetCoreRateLimit servisleri (IP ve Endpoint bazlı)
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// IP algılama için custom configuration
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.ClientIdHeader = "X-ClientId";
});

// Lego Rate Limiting servisleri (UserId bazlı + Adapter köprüsü)
builder.Services.AddLegoRateLimiting();

// JWT servisleri
builder.Services.AddJwtCore(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

// Data Protection servisleri
builder.Services.AddLegoDataProtection();

//  Custom Routing servisleri
builder.Services.AddCustomRouting();

// User servisleri
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

// CORS middleware'ini ekle
app.UseCors("AllowAll");
// 🔥 Global error handling middleware'ini ekle (İLK SIRADA - tüm hataları yakalar)
app.UseGlobalErrorHandling();

// Rate limit logging middleware'ini ekle (ÖNCE - response'u yakalamak için)
app.UseRateLimitLogging();

// IP/Endpoint bazlı rate limiting (AspNetCoreRateLimit kütüphanesi)
app.UseIpRateLimiting();

// JWT Authentication ve Authorization middleware'lerini ekle
app.UseAuthentication();
app.UseAuthorization();

// UserId bazlı rate limiting middleware'ini ekle (Authentication'dan sonra - sadece JWT token varsa)
app.UseUserIdRateLimiting();

app.MapControllers();

// ?? Seed data'yı çalıştır
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    
    // User seed data
    if (!context.Users.Any())
    {
       context.Users.AddRange(Lego.Contexts.Seed.UserSeedData.GetUsers());
       context.SaveChanges();
    }

    // Rate limiting seed data
    if (!context.RateLimitRules.Any())
    {
        context.RateLimitRules.AddRange(Lego.Contexts.Seed.RateLimitSeedData.GetRateLimitRules());
        context.SaveChanges();
    }
    
    // Client whitelist seed data
    if (!context.ClientWhitelists.Any())
    {
        context.ClientWhitelists.AddRange(Lego.Contexts.Seed.RateLimitSeedData.GetClientWhitelists());
        context.SaveChanges();
    }
}

app.Run();


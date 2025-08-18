using AspNetCoreRateLimit;
using Scalar.AspNetCore;
using Lego.RateLimiting.Stores;
using Lego.RateLimiting.Extensions;
using Lego.Contexts;
using Microsoft.EntityFrameworkCore;
using Lego.JWT.Extensions;
using Lego.Contexts.Interfaces;
using Lego.Contexts.Services;


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

// Rate limiting servislerini ekle
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Lego Rate Limiting servisleri
builder.Services.AddLegoRateLimiting();

// JWT servisleri
builder.Services.AddJwtCore(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

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

// Rate limiting middleware'ini ekle (SONRA - gerçek rate limiting)
app.UseIpRateLimiting();

// JWT Authentication ve Authorization middleware'lerini ekle
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ?? Seed data'yı çalıştır
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    
    //// User seed data
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
    
    if (!context.ClientWhitelists.Any())
    {
        context.ClientWhitelists.AddRange(Lego.Contexts.Seed.RateLimitSeedData.GetClientWhitelists());
        context.SaveChanges();
    }
}

app.Run();

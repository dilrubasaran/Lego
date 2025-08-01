using AspNetCoreRateLimit;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Rate limiting servislerini ekle
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

// Rate limiting middleware'ini ekle
app.UseIpRateLimiting();

app.UseAuthorization();

app.MapControllers();

app.Run();

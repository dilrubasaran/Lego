using Lego.Contexts;
using Microsoft.EntityFrameworkCore;
using Lego.Localization.Interfaces;
using Lego.DataProtection.Extensions;
using Lego.Localization.Services;
using Microsoft.Extensions.Options;
using Lego.Localization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// ?? Localization deste�i
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// ?? DB Context - Web projesi için WebDbContext kullan
builder.Services.AddDbContext<WebDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddProjectLocalization();



// ?? DI Servisleri
builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddHttpContextAccessor();

// ? DataProtection servisleri (URL token için gerekli)
builder.Services.AddLegoDataProtection();

// API HttpClient kaydı
builder.Services.AddHttpClient("LegoApi", client =>
{
    // API base adresi
    client.BaseAddress = new Uri("https://localhost:7087/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ? Localization middleware
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localizationOptions.Value);



app.UseAuthorization();

app.MapStaticAssets();

// Kısa edit rotası: /e/{token} → Home/Edit
app.MapControllerRoute(
        name: "ShortEdit",
        pattern: "e/{token}",
        defaults: new { controller = "Home", action = "Edit" });

// Kısa hakkımızda rotası: /a/{token} → Home/About
app.MapControllerRoute(
        name: "ShortAbout",
        pattern: "a/{token}",
        defaults: new { controller = "Home", action = "About" });

// Varsayılan rota
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
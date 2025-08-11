using Lego.Contexts;
using Microsoft.EntityFrameworkCore;
using Lego.Localization.Interfaces;
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

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
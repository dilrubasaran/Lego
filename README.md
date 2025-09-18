# 🧱 Lego - Modüler .NET Core Kütüphane Koleksiyonu

## 📋 Proje Hakkında

**Lego**, modern web uygulamaları için geliştirilen modüler ve yeniden kullanılabilir .NET Core kütüphane koleksiyonudur. Her bir modül, "Lego tuğlası" konseptiyle bağımsız olarak çalışabilir ve farklı projelerde kolayca entegre edilebilir.

Bu proje, geliştiricilerin sıkça ihtiyaç duyduğu temel işlevleri (localization, rate limiting, jwt, data protection ve custom routing) modüler bir şekilde sunar.

## 🎯 Temel Özellikler

- ✅ **Modüler Yapı**: Her kütüphane bağımsız çalışır
- ✅ **Plug & Play**: Minimum konfigürasyon ile hızlı entegrasyon
- ✅ **SOLID Prensipleri**: Temiz, test edilebilir ve sürdürülebilir kod
- ✅ **Kapsamlı Testler**: HTTP testleri ve entegrasyon testleri
- ✅ **Modern UI**: Bootstrap 5 ve modern tasarım
- ✅ **Best Practices**: Endüstri standartlarına uygun geliştirme

## 🏗️ Proje Mimarisi

```
Lego/
├── 🌐 Lego.Web              # MVC Web uygulaması (Demo & UI)
├── 🔌 Lego.API              # REST API servisleri
├── 💾 Lego.Contexts         # Entity Framework Core & Data Layer
├── 🔐 Lego.JWT              # JWT Authentication & Authorization
├── 🛡️ Lego.DataProtection   # Veri şifreleme ve koruma
├── 🚦 Lego.RateLimiting     # Hız sınırlama ve quota yönetimi
├── 🛣️ Lego.CustomRouting    # Özel URL yönlendirmeleri
└── 🌍 Lego.Localization     # Çoklu dil desteği
```

## 📦 Modüller

### 🔐 [Lego.JWT](./Lego.JWT/JWT.md)
**JWT tabanlı kimlik doğrulama ve yetkilendirme sistemi**

- ✅ Access & Refresh Token yönetimi
- ✅ Token rotation ve sliding expiration
- ✅ Blacklist mekanizması (revocation)
- ✅ Claims ve Policy bazlı yetkilendirme
- 🔄 Role bazlı erişim kontrolü (geliştirilme aşamasında)

```csharp
// Kullanım örneği
builder.Services.AddJwtCore(configuration);
builder.Services.AddJwtAuthentication(configuration);
```

### 🛡️ [Lego.DataProtection](./Lego.DataProtection/dataprotection.md)
**Veri şifreleme ve güvenlik çözümleri**

- ✅ IDataProtector ile veri şifreleme/çözme
- ✅ Time-limited token'lar (şifre sıfırlama, e-posta onayı)
- ✅ URL token gizleme (ID → encrypted token)
- ✅ Form field protection
- 🔄 Anti-tampering ve CSRF koruması (geliştirilme aşamasında)

```csharp
// Kullanım örneği
builder.Services.AddLegoDataProtection();
```

### 🚦 [Lego.RateLimiting](./Lego.RateLimiting/RateLimiting.md)
**Gelişmiş hız sınırlama ve quota yönetimi**

- ✅ IP bazlı hız sınırlama
- ✅ Endpoint bazlı limitler
- ✅ UserId bazlı özel limitler
- ✅ Veritabanı tabanlı kural yönetimi
- ✅ Whitelist/Blacklist desteği

```csharp
// Kullanım örneği
builder.Services.AddLegoRateLimiting();
app.UseRateLimitLogging();
app.UseUserIdRateLimiting();
```

### 🛣️ [Lego.CustomRouting](./Lego.CustomRouting/CustomRouting.md)
**Esnek ve özelleştirilebilir URL yönlendirmeleri**

- ✅ Hiyerarşik URL yapıları (`/category/1/product/5`)
- ✅ Fake data üretimi (Bogus entegrasyonu)
- ✅ Interaktif test sayfaları
- ✅ URL parsing ve validation

```csharp
// Kullanım örneği
builder.Services.AddCustomRouting();
```

### 🌍 Lego.Localization
**Çoklu dil desteği ve yerelleştirme**

- ✅ Resource-based çeviri sistemi (.resx dosyaları)
- ✅ Culture bazlı dil değiştirme
- ✅ Header, Footer, SharedResource kategorileri
- ✅ IStringLocalizer entegrasyonu
- ✅ Dinamik dil seçimi

```csharp
// Kullanım örneği
builder.Services.AddProjectLocalization();
app.UseRequestLocalization();
```

### 💾 Lego.Contexts
**Entity Framework Core ve veri erişim katmanı**

- ✅ ApiDbContext (API projesi için)
- ✅ WebDbContext (Web projesi için)
- ✅ Seed data yönetimi
- ✅ Migration yapısı
- ✅ Repository pattern implementasyonu

## 🚀 Hızlı Başlangıç

### 1. Projeyi Klonlayın
```bash
git clone https://github.com/yourusername/Lego.git
cd Lego
```

### 2. Bağımlılıkları Yükleyin
```bash
dotnet restore
```

### 3. Veritabanını Oluşturun
```bash
# API veritabanı
dotnet ef database update -p Lego.Contexts -s Lego.API -c ApiDbContext

# Web veritabanı  
dotnet ef database update -p Lego.Contexts -s Lego.Web -c WebDbContext
```

### 4. Uygulamaları Çalıştırın
```bash
# API (Port: 7087)
cd Lego.API
dotnet run

# Web (Port: 7156)
cd Lego.Web
dotnet run
```

## 🧪 Test Etme

### API Testleri
- **Scalar UI**: `https://localhost:7087/scalar/v1`
- **HTTP Dosyaları**: `*.http` dosyalarını kullanın
  - `jwt-test.http` - JWT testleri
  - `rate-limiting-test.http` - Rate limiting testleri
  - `secure-link-test.http` - Data protection testleri
  - `custom-routing-test.http` - Custom routing testleri

### Web Testleri
- **Ana Sayfa**: `https://localhost:7156`
- **Lokalizasyon**: Dil değiştirme testleri
- **Custom Routing**: Kategori ve ürün sayfaları
- **Data Protection**: Güvenli link testleri

## 📁 Proje Yapısı

```
├── 🌐 Web Katmanı
│   ├── Lego.Web (MVC Controllers, Views, wwwroot)
│   └── Lego.API (REST API Controllers, Scalar)
│
├── 🔧 Business Katmanı  
│   ├── Lego.JWT (Authentication Services)
│   ├── Lego.DataProtection (Encryption Services)
│   ├── Lego.RateLimiting (Rate Limiting Services)
│   ├── Lego.CustomRouting (Routing Services)
│   └── Lego.Localization (Localization Services)
│
└── 💾 Data Katmanı
    └── Lego.Contexts (DbContext, Models, Migrations)
```

## ⚙️ Konfigürasyon

### appsettings.json Örneği
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=LegoApp.db"
  },
  "Jwt": {
    "Issuer": "LegoAPI",
    "Audience": "LegoUsers", 
    "SecretKey": "your-secret-key",
    "ExpirationMinutes": 15
  },
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "GeneralRules": [
      { "Endpoint": "*", "Period": "1m", "Limit": 10 }
    ]
  }
}
```

## 🔧 Teknolojiler

- **.NET 9.0**
- **Entity Framework Core 9.0**
- **ASP.NET Core MVC**
- **ASP.NET Core Web API**
- **SQLite** (veritabanı)
- **Bootstrap 5** (UI framework)
- **JWT Bearer Authentication**
- **Data Protection API**
- **Localization**
- **AspNetCoreRateLimit**
- **Bogus** (fake data generation)
- **Scalar** (API documentation)

## 📈 Geliştirme Roadmap

### Mevcut Durum (v1.0)
- ✅ Tüm core modüller tamamlandı
- ✅ Basic JWT authentication
- ✅ IP/Endpoint rate limiting
- ✅ userId bazlı rate limiting 
- ✅ Basic data protection
- ✅ Custom routing MVP
- ✅ Localization support

### Sonraki Adımlar (v2.0)
- 🔄 Advanced JWT (Policy/Role-based auth)
- 🔄 Database-driven rate limiting rules
- 🔄 Advanced data protection (field-level encryption)
- 🔄 Real-time monitoring dashboard

### Gelecek Planları (v3.0+)
- 🔮 Redis-based distributed rate limiting
- 🔮 Azure Key Vault integration
- 🔮 Advanced security features
- 🔮 Performance optimizations
- 🔮 Multi-tenant support

## 🤝 Katkıda Bulunma

1. Projeyi fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.


## 🙋‍♂️ İletişim

Proje hakkında sorularınız için:
- 📧 E-posta: [dilrubabasarann@gmail.com]
- 📖 Dokümantasyon: Her modülün kendi MD dosyası


---


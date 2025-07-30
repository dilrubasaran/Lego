# 🚀 Lego.RateLimiting - Modüler Rate Limiting Çözümü

## 🎯 Hedef
✅ **V1**: AspNetCoreRateLimit kullanarak temel senaryoları hızlıca entegre etmek  
🚀 **V2**: Kendi middleware'lerimizi geliştirerek tam kontrolü sağlamak ve genişletmek (Redis, Token, Role bazlı vb.)  
🧱 **Yapı**: Her şey modüler olacak. Yani "lego" gibi her parça başka projelere kolayca entegre edilebilecek.

---

## 📋 Aşama Aşama Yol Haritası

### 📦 **1. Aşama: Lego.RateLimiting Class Library'sini Oluştur**
**Proje adı**: Lego.RateLimiting  
**Amacı**: Her projede tekrar tekrar yazılmayacak, taşınabilir bir yapı oluşturmak.

#### 📁 Katmanlar:
```
Lego.RateLimiting/
├── Configuration/        // AspNetCoreRateLimit ayarları
├── Extensions/           // IServiceCollection & IApplicationBuilder extensions
├── Middleware/           // V2'de kendi middleware'ler buraya gelecek
├── Stores/               // InMemoryStore, RedisStore
├── Models/               // RateLimitRule, QuotaInfo, vs.
```

**V1'de sadece**: Configuration, Extensions, Models kullanılacak.  
**V2'de**: Middleware ve Stores (Redis) dosyaları aktif olacak.

---

### 🔧 **2. Aşama: V1 – AspNetCoreRateLimit ile Hazır Kütüphane Desteği**

#### 📋 Uygulanacak Senaryolar:
| Senaryo       | Durum |

| ✅ IP Bazlı | Yapılacak |
| ✅ Endpoint Bazlı | Yapılacak |
| ✅ Veritabanı Konfigürasyonu | Yapılacak |
| ✅ İhlal Loglama | Yapılacak |

**Not**: Bunlar config ile tanımlanabildiği için hızlıca test edilebilir.

#### ✍️ Detaylı Adımlar:

**1. NuGet Paketleri:**
```bash
# Lego.RateLimiting projesine
dotnet add package AspNetCoreRateLimit

# Lego.Contexts projesine (gerekirse)
dotnet add package Microsoft.EntityFrameworkCore
```

**2. Lego.Contexts.Models.RateLimiting Modellerini Kullan:**
- ✅ `RateLimitRule.cs` - Ana kural modeli (Id, Endpoint, HttpMethod, ClientType, Limit, Period, IsActive, Description, CreatedAt)
- ✅ `RateLimitViolation.cs` - İhlal kayıtları (IP, Endpoint, Timestamp, vb.)
- ✅ `ClientWhitelist.cs` - Beyaz liste (IP'ler, ClientId'ler)
- ✅ `RateLimitLog.cs` - Log kayıtları (Tüm isteklerin loglanması)

**3. AppDbContext Güncellemeleri:**

public DbSet<RateLimitRule> RateLimitRules { get; set; }
public DbSet<RateLimitViolation> RateLimitViolations { get; set; }
public DbSet<ClientWhitelist> ClientWhitelists { get; set; }
public DbSet<RateLimitLog> RateLimitLogs { get; set; }


**4. Seed Data (3 Temel Kural):**

// Genel IP Limiti: 1 dakikada 10 istek
// LocalizationTest: 1 dakikada 3 istek  
// ChangeLanguage: 1 dakikada 5 istek


**5. Lego.RateLimiting Servisleri:**
- `Configuration/RateLimitConfig.cs` - Örnek konfigürasyon şablonları
- `Extensions/RateLimitingExtensions.cs` - Extension method'lar
- `Services/RateLimitConfigurationService.cs` - Veritabanı konfigürasyon servisi
- `Services/IRateLimitConfigurationService.cs` - Interface

**6. Lego.API Entegrasyonu:**
- `Program.cs` dosyasında aşağıdaki satırları ekle:
  ```csharp
  builder.Services.AddIpRateLimiting(builder.Configuration);
  app.UseIpRateLimiting();
  ```
- Test için bir API Controller oluştur:
  ```csharp

  // Rate limit test endpoint'i. Rate limiting kurallarının çalışıp çalışmadığını test etmek için kullanılır.

  [ApiController]
  [Route("api/[controller]")]
  public class RateLimitTestController : ControllerBase
  {
      /// <summary>
      /// Basit bir GET endpoint'i. Rate limit aşıldığında 429 döner.
      /// </summary>
      [HttpGet("test")]
      public IActionResult Test()
      {
          return Ok("Rate limit test başarılı!");
      }
  }
  ```
- Rate limit kurallarını `appsettings.json` dosyasında tanımla (örnek aşağıda mevcut):
  ```json
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 10
      },
      {
        "Endpoint": "GET:/api/RateLimitTest/test",
        "Period": "1m",
        "Limit": 3
      }
    ]
  }
  ```
- v1 de sadewce appsettings.json ile kullanılack v2 de servis ve middleware eklenecek kurealları ve verleri dinamik yönetmek için 

**7. Test Senaryoları:**
- IP bazlı rate limiting (1 dakikada 10 istek)
- Endpoint bazlı rate limiting (login: 1 dakikada 3 istek)
- Farklı IP'lerden test
- Rate limit aşıldığında 429 response
- Rate limit header'larının kontrolü
- Veritabanı konfigürasyon testleri
.
---

### 📌 **3. Aşama: V2 – Gelişmiş ve Özel Middleware Geliştir (Redis dahil)**

#### 🌟 Bu Aşamada:
| Senaryo | Gereken |
|---------|---------|
| UserId bazlı limit | ✅ Middleware |
| Rol bazlı limit | ✅ Middleware |
| Token bazlı (API Key) | ✅ Header parsing + Middleware |
| Quota bazlı (aylık) | ✅ DB veya Redis + Tracking logic |
| Zaman bazlı kampanya | ✅ Tarih kontrolü + Custom Store |
| Concurrency limit | ✅ Semaphore + Pipeline kontrol |
| Redis store | ✅ Performans ve cluster için |

#### 🛠️ Yapılacaklar:
**Middleware/ klasöründe:**
- `UserRateLimitingMiddleware`
- `RoleRateLimitingMiddleware`
- `TokenRateLimitingMiddleware`
- `QuotaRateLimitingMiddleware`
- `ConcurrencyRateLimitingMiddleware`

**Stores/ klasöründe:**
- `RedisRateLimitStore.cs` → Redis ile çalışan metrik ve sayaçlar
- `InMemoryRateLimitStore.cs` → Hızlı test için
- `IRateLimitStore.cs` → Interface

**Redis için**: StackExchange.Redis veya EasyCaching adapter kullanılabilir.

---

## 📅 Versiyonlama Stratejisi

| Versiyon | İçerik |
|----------|--------|
| **v1** | AspNetCoreRateLimit + IP & Endpoint Limitleme |
| **v2** | Custom Middleware + Redis + User/Role/Token Bazlı |
| **v3** | Dashboard + Dinamik Ayarlar (örn: veritabanından) |

---

## ✅ Şimdi Ne Yapıyoruz?

### 🔥 **V1 İmplementasyon Planı:**

#### **Sprint 1: Temel Altyapı**
1. ✅ Lego.RateLimiting class library'sini oluştur
2. ✅ AspNetCoreRateLimit NuGet paketini ekle
3. ✅ Lego.Contexts.Models.RateLimiting modellerini kullan
4. ✅ AppDbContext'e DbSet'leri ekle
5. ✅ Seed data oluştur

#### **Sprint 2: Servis Katmanı**
1. ✅ Configuration/RateLimitConfig.cs oluştur
2. ✅ Extensions/RateLimitingExtensions.cs oluştur
3. ✅ Services/RateLimitConfigurationService.cs oluştur
4. ✅ Services/IRateLimitConfigurationService.cs oluştur
5. ✅ AddDatabaseRateLimiting extension method yaz

#### **Sprint 3: Web Entegrasyonu**
1. ✅ Lego.Web.csproj'e referans ekle
2. ✅ Program.cs'te servisleri entegre et
3. ✅ HomeController'a test endpoint'leri ekle
4. ✅ RateLimitTest.cshtml test sayfası oluştur
5. ✅ appsettings.json konfigürasyonu ekle

#### **Sprint 4: Test ve Doğrulama**
1. ✅ IP bazlı rate limiting testleri
2. ✅ Endpoint bazlı rate limiting testleri
3. ✅ Veritabanı konfigürasyon testleri
4. ✅ İhlal loglama testleri
5. ✅ Farklı IP'lerden test senaryoları

**V1 tamamlandığında:** AspNetCoreRateLimit + Veritabanı konfigürasyonu + 4 model ile çalışan sistem ✅

---

## 🎯 Test Senaryoları

### V1 Test Senaryoları:
- [ ] **IP bazlı rate limiting** (1 dakikada 10 istek)
- [ ] **Endpoint bazlı rate limiting** (LocalizationTest: 1 dakikada 3 istek)
- [ ] **Farklı IP'lerden test** (Her IP'nin kendi limiti)
- [ ] **Rate limit aşıldığında 429 response** (HTTP 429 Too Many Requests)
- [ ] **Rate limit header'larının kontrolü** (X-RateLimit-* header'ları)
- [ ] **Veritabanı konfigürasyon testleri** (Kuralların DB'den yüklenmesi)
- [ ] **İhlal loglama testleri** (RateLimitViolation kayıtları)
- [ ] **Beyaz liste testleri** (ClientWhitelist çalışması)
- [ ] **Log kayıtları testleri** (RateLimitLog kayıtları)
- [ ] **Dinamik kural güncelleme** (DB'den kural ekleme/çıkarma)

### V2 Test Senaryoları:
- [ ] User ID bazlı limiting
- [ ] Role bazlı limiting (Admin: 100/dk, User: 10/dk)
- [ ] API Key bazlı limiting
- [ ] Redis store ile cluster test
- [ ] Quota bazlı aylık limitler

---

## 📚 Kaynaklar ve Referanslar

- [AspNetCoreRateLimit GitHub](https://github.com/stefanprodan/AspNetCoreRateLimit)
- [Microsoft Rate Limiting](https://docs.microsoft.com/en-us/aspnet/core/performance/rate-limit)
- [Redis Rate Limiting Patterns](https://redis.io/docs/manual/patterns/distributed-locks/)

---

## 🚨 Önemli Notlar

1. **Performans**: Redis kullanırken connection pooling önemli
2. **Güvenlik**: Rate limiting bypass edilmemeli
3. **Monitoring**: Rate limit metrikleri loglanmalı
4. **Fallback**: Redis down olduğunda InMemory'e geçiş
5. **Configuration**: Hot reload desteklenmeli

---

## 📝 TODO Listesi

### V1 TODO:
- [x] Lego.RateLimiting projesini oluştur
- [x] AspNetCoreRateLimit NuGet paketini ekle
- [ ] Configuration klasörü oluştur
  - [ ] RateLimitConfig.cs - Örnek konfigürasyon şablonları
- [ ] Extensions klasörü oluştur
  - [ ] RateLimitingExtensions.cs - AddIpRateLimiting, AddClientRateLimiting extension method'ları
- [ ] Services klasörü oluştur
  - [ ] RateLimitConfigurationService.cs - Veritabanı konfigürasyon servisi
  - [ ] IRateLimitConfigurationService.cs - Interface tanımı
- [ ] Lego.Contexts.Models.RateLimiting modellerini kullan
  - [x] RateLimitRule.cs - Ana kural modeli
  - [x] RateLimitViolation.cs - İhlal kayıtları
  - [x] ClientWhitelist.cs - Beyaz liste
  - [x] RateLimitLog.cs - Log kayıtları
- [ ] AppDbContext'e RateLimitRules DbSet ekle
- [ ] Seed data oluştur (3 temel kural)
- [ ] AddDatabaseRateLimiting extension method yaz
- [ ] appsettings.json template oluştur
- [ ] Lego.Web entegrasyonu
  - [ ] Program.cs'te servisleri ekle
  - [ ] HomeController'a test endpoint'leri ekle
  - [ ] RateLimitTest.cshtml test sayfası oluştur
- [ ] IP bazlı test senaryoları
- [ ] Endpoint bazlı test senaryoları
- [ ] Veritabanı konfigürasyon testleri

### V2 TODO:
- [ ] Middleware klasörü oluştur
- [ ] Stores klasörü oluştur
- [ ] IRateLimitStore interface tanımla
- [ ] UserRateLimitingMiddleware yaz
- [ ] RoleRateLimitingMiddleware yaz
- [ ] TokenRateLimitingMiddleware yaz
- [ ] RedisRateLimitStore implement et
- [ ] Redis connection configuration
- [ ] Advanced test senaryoları

---

## 🎉 Başarı Kriterleri

### V1 Başarı Kriterleri:
- [ ] **IP bazlı rate limiting çalışıyor** (1 dakikada 10 istek limiti)
- [ ] **Endpoint bazlı rate limiting çalışıyor** (Farklı endpoint'ler için farklı limitler)
- [ ] **429 response döndürüyor** (Rate limit aşıldığında HTTP 429)
- [ ] **Header'lar doğru set ediliyor** (X-RateLimit-* header'ları)
- [ ] **Farklı IP'ler farklı limitlere sahip** (Her IP'nin kendi sayacı)
- [ ] **Veritabanı konfigürasyonu çalışıyor** (Kurallar DB'den yükleniyor)
- [ ] **İhlal loglama çalışıyor** (RateLimitViolation kayıtları oluşuyor)
- [ ] **Beyaz liste çalışıyor** (ClientWhitelist'teki IP'ler limitlenmiyor)
- [ ] **Log kayıtları çalışıyor** (RateLimitLog kayıtları oluşuyor)
- [ ] **Dinamik kural güncelleme çalışıyor** (DB'den kural ekleme/çıkarma)

### V2 Başarı Kriterleri:
- [ ] User/Role/Token bazlı limiting çalışıyor
- [ ] Redis store ile cluster çalışıyor
- [ ] Quota bazlı limiting çalışıyor
- [ ] Concurrency limiting çalışıyor
- [ ] Performance testleri geçiyor 
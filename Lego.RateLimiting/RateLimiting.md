# 🚀 Lego.RateLimiting - Modüler Rate Limiting Çözümü

---

## ✅ MVP
**Sektörde yaygın, hızlı uygulanabilir, öğrenme aşaması için ideal, çoklu kullanım alanı**

- **IP bazlı limit**: AspNetCoreRateLimit ile `appsettings.json` üzerinden temel IP limitleri
- **Endpoint bazlı limit**: Belirli endpoint'ler için periyot/limit tanımı
- **Program.cs entegrasyonu**: `AddIpRateLimiting()` ve `UseIpRateLimiting()`
- **Basit test endpoint'i**: 429 cevap ve X-RateLimit-* header doğrulaması
- **Konfigürasyon**: Sadece konfig dosyası; veritabanı/dinamik yapı yok

## 🔄 Intermediate 
**MVP'den sonra ihtiyaç duyulabilecek, biraz daha karmaşık, entegrasyon gerektiren**

- **Veritabanı tabanlı kurallar (temel)**: Basit CRUD ile dinamik kural yönetimi
- **İhlal loglama (basit)**: DB'ye yazma, temel raporlama
- **Beyaz liste / kara liste**: DB destekli yönetim
- **User/Role bazlı limit (basit)**: JWT token'dan basit kullanıcı/rol limitleri

## 🚀 Advanced
**İleri seviye, karmaşık, performans, güvenlik veya özel senaryolar için**

- **Custom middleware suite**: Quota (günlük/aylık), Concurrency limit, Token bazlı
- **Dağıtık mimari**: Redis store ile cluster senaryoları
- **Dashboard & dinamik ayarlar**: Real-time konfigürasyon, monitoring
- **Multi-tenant limitler**: Tenant bazlı kural setleri
- **Advanced monitoring**: Metrikler, alarm entegrasyonu, performans izleme
- **Complex rate limiting**: Sliding window, token bucket, adaptive limits

---

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

### 🔧 **2. Aşama: V1 – AspNetCoreRateLimit ile Hazır Kütüphane Desteği [MVP]**

#### 📋 Uygulanacak Senaryolar (MVP):
- ✅ IP Bazlı limit (config)
- ✅ Endpoint Bazlı limit (config)

> Not: V1'de veritabanı/dinamik kural yönetimi, loglama ve beyaz listeleme YOKTUR; bunlar V2/Advanced kapsamındadır.

#### ✍️ Detaylı Adımlar (MVP):

**1. NuGet Paketleri:**
```bash
# Lego.RateLimiting projesine
dotnet add package AspNetCoreRateLimit
```

**2. Program.cs Entegrasyonu:**
```csharp
builder.Services.AddIpRateLimiting(builder.Configuration);
app.UseIpRateLimiting();
```

**3. appsettings.json Örneği:**
```json
"IpRateLimiting": {
  "EnableEndpointRateLimiting": true,
  "StackBlockedRequests": false,
  "RealIpHeader": "X-Real-IP",
  "ClientIdHeader": "X-ClientId",
  "HttpStatusCode": 429,
  "GeneralRules": [
    { "Endpoint": "*", "Period": "1m", "Limit": 10 },
    { "Endpoint": "GET:/api/RateLimitTest/test", "Period": "1m", "Limit": 3 }
  ]
}
```
- v1 de sadewce appsettings.json ile kullanılack v2 de servis ve middleware eklenecek 
kurealları ve verleri dinamik yönetmek için 

**4. Basit Test Endpoint'i (MVP):**
```csharp
[ApiController]
[Route("api/[controller]")]
public class RateLimitTestController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult Test() => Ok("Rate limit test başarılı!");
}
```

---

### 📌 **3. Aşama: V2 – Gelişmiş ve Özel Middleware Geliştir (Redis dahil) [Advanced]**

#### 🌟 Bu Aşamada (Advanced):
| Senaryo | Gereken |
|---------|---------|
| UserId bazlı limit | ✅ Middleware |
| Rol bazlı limit | ✅ Middleware |
| Token bazlı (API Key) | ✅ Header parsing + Middleware |
| Quota bazlı (aylık) | ✅ DB veya Redis + Tracking logic |
| Zaman bazlı kampanya | ✅ Tarih kontrolü + Custom Store |
| Concurrency limit | ✅ Semaphore + Pipeline kontrol |
| Redis store | ✅ Performans ve cluster için |
| Veritabanı tabanlı kurallar | ✅ CRUD + Dynamic config |
| İhlal loglama ve audit | ✅ DB log + raporlama |
| Beyaz liste / kara liste | ✅ DB destekli |

#### 🛠️ Yapılacaklar (Advanced):
- `UserRateLimitingMiddleware`, `RoleRateLimitingMiddleware`, `TokenRateLimitingMiddleware`, `QuotaRateLimitingMiddleware`, `ConcurrencyRateLimitingMiddleware`
- Stores: `RedisRateLimitStore.cs`, `InMemoryRateLimitStore.cs`, `IRateLimitStore.cs`
- Dinamik kural yönetimi API'si + dashboard (opsiyonel)

---

## 📅 Versiyonlama Stratejisi

| Versiyon | İçerik |
|----------|--------|
| **v1 [MVP]** | AspNetCoreRateLimit + IP & Endpoint Limitleme (config-only) |
| **v2 [Advanced]** | Custom Middleware + Redis + User/Role/Token Bazlı + DB/Log |
| **v3 [Advanced]** | Dashboard + Dinamik Ayarlar |

---

## 🎯 Test ve Başarı Kriterleri

### V1 Test Senaryoları (MVP):
- [ ] IP bazlı rate limiting (1 dakikada 10 istek)
- [ ] Endpoint bazlı rate limiting (örnek: 1 dakikada 3 istek)
- [ ] Rate limit aşıldığında 429 response
- [ ] X-RateLimit-* header'larının doğrulaması

### V2 Test Senaryoları (Advanced):
- [ ] User/Role/Token bazlı limiting
- [ ] Redis store ile cluster test
- [ ] Quota bazlı aylık limitler
- [ ] Dinamik kural CRUD + etkisinin anında yansıması
- [ ] İhlal loglama ve beyaz liste senaryoları

---

## 🚨 Önemli Notlar

1. **Performans**: Redis kullanırken connection pooling önemli
2. **Güvenlik**: Rate limiting bypass edilmemeli
3. **Monitoring**: Rate limit metrikleri loglanmalı
4. **Fallback**: Redis down olduğunda InMemory'e geçiş
5. **Configuration**: Hot reload desteklenmeli

---

## 📝 TODO Listesi

### V1 TODO (MVP):
- [x] Lego.RateLimiting projesini oluştur
- [x] AspNetCoreRateLimit NuGet paketini ekle
- [ ] Configuration klasörü oluştur
  - [ ] RateLimitConfig.cs - Örnek konfigürasyon şablonları
- [ ] Extensions klasörü oluştur
  - [ ] RateLimitingExtensions.cs - AddIpRateLimiting, AddClientRateLimiting extension method'ları
- [ ] appsettings.json template oluştur
- [ ] Lego.API entegrasyonu
  - [ ] Program.cs'te servisleri ekle
  - [ ] RateLimitTestController oluştur
- [ ] IP bazlı test senaryoları
- [ ] Endpoint bazlı test senaryoları

### V2 TODO (Intermediate):
- [ ] Lego.Contexts.Models.RateLimiting modellerini kullan
  - [x] RateLimitRule.cs - Ana kural modeli
  - [x] RateLimitViolation.cs - İhlal kayıtları
  - [x] ClientWhitelist.cs - Beyaz liste
  - [x] RateLimitLog.cs - Log kayıtları
- [ ] AppDbContext'e RateLimitRules DbSet ekle
- [ ] Seed data oluştur (3 temel kural)
- [ ] Services klasörü oluştur
  - [ ] RateLimitConfigurationService.cs - Veritabanı konfigürasyon servisi
  - [ ] IRateLimitConfigurationService.cs - Interface tanımı
- [ ] AddDatabaseRateLimiting extension method yaz
- [ ] Veritabanı konfigürasyon testleri

### V3 TODO (Advanced):
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

### V1 Başarı Kriterleri (MVP):
- [ ] **IP bazlı rate limiting çalışıyor** (1 dakikada 10 istek limiti)
- [ ] **Endpoint bazlı rate limiting çalışıyor** (Farklı endpoint'ler için farklı limitler)
- [ ] **429 response döndürüyor** (Rate limit aşıldığında HTTP 429)
- [ ] **Header'lar doğru set ediliyor** (X-RateLimit-* header'ları)
- [ ] **Farklı IP'ler farklı limitlere sahip** (Her IP'nin kendi sayacı)

### V2 Başarı Kriterleri (Intermediate):
- [ ] **Veritabanı konfigürasyonu çalışıyor** (Kurallar DB'den yükleniyor)
- [ ] **İhlal loglama çalışıyor** (RateLimitViolation kayıtları oluşuyor)
- [ ] **Beyaz liste çalışıyor** (ClientWhitelist'teki IP'ler limitlenmiyor)
- [ ] **Log kayıtları çalışıyor** (RateLimitLog kayıtları oluşuyor)
- [ ] **Dinamik kural güncelleme çalışıyor** (DB'den kural ekleme/çıkarma)

### V3 Başarı Kriterleri (Advanced):
- [ ] User/Role/Token bazlı limiting çalışıyor
- [ ] Redis store ile cluster çalışıyor
- [ ] Quota bazlı limiting çalışıyor
- [ ] Concurrency limiting çalışıyor
- [ ] Performance testleri geçiyor 
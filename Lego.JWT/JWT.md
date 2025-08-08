# 🔐 JWT Authentication & Authorization Roadmap

Bu doküman, ASP.NET Core projesinde JWT (JSON Web Token) mimarisini adım adım oluşturmak 
için izlenecek versiyonlara bölünmüş yol haritasını sunar.---

## ✅ MVP
**Sektörde yaygın, hızlı uygulanabilir, öğrenme aşaması için ideal, çoklu kullanım alanı**

- **JWT üretimi ve doğrulaması**: HS256 ile symmetric key imzalama
- **Yapılandırma**: `appsettings.json` üzerinden Issuer, Audience, Secret, Expiry
- **Login endpoint**: Başarılı girişte access token döndürme
- **Korumalı endpoint**: `[Authorize]` ile erişim kontrolü
- **Testler**: Geçerli/geçersiz/süresi dolmuş token ve tokensız istek senaryoları

## 🔄 Intermediate 
**MVP'den sonra ihtiyaç duyulabilecek, biraz daha karmaşık, entegrasyon gerektiren**

- **Claim/Policy yetkilendirme**: Policy bazlı erişim kontrolü
- **Refresh token (temel)**: Yenileme mekanizması
- **Role-based authorization**: `[Authorize(Roles = "Admin")]` kullanımı
- **JWT bazlı rate limiting (basit)**: Kullanıcı/rol bazlı temel limitler

## 🚀 Advanced
**İleri seviye, karmaşık, performans, güvenlik veya özel senaryolar için**

- **Karma policy/role yapıları**: Çok seviyeli, çapraz erişim senaryoları
- **Refresh token revocation**: İptal ve blacklist mekanizmaları
- **JWT bazlı rate limiting (gelişmiş)**: Token içeriğine göre dinamik limitler
- **Güvenlik sertleştirme**: Clock skew, audience/issuer sıkı doğrulama, secret rotation
- **Token monitoring & audit**: Giriş/erişim logları, anomali tespiti, suspicious activity tracking

---

## ✅ v1 – Temel JWT + Token Üretimi [MVP]

### 🎯 Amaç
JWT tabanlı authentication mekanizmasının kurulması ve temel token üretimi.

### 🔧 Yapılacaklar
- [ ] `Microsoft.AspNetCore.Authentication.JwtBearer` kütüphanesini projeye dahil et
- [ ] `JwtService` sınıfını oluştur (Token üretimi için)
- [ ] Symmetric key ile imzalama işlemi (HS256 algoritması)
- [ ] Expire süresi ayarlama (örnek: 60 dakika)
- [ ] Kullanıcı girişinde token üretimi (Login endpoint)
- [ ] JWT ayarlarını `appsettings.json` üzerinden yönetilebilir yap
- [ ] Middleware'de JWT doğrulama yapısının eklenmesi
- [ ] Test endpoint'i oluştur ve token ile erişimi test et

### 📁 appsettings.json Yapılandırması
```json
{
  "JwtSettings": {
    "Issuer": "LegoApp",
    "Audience": "LegoAppUsers",
    "SecretKey": "super-secret-key-change-this-in-production",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

### 🛡️ Middleware Yapılandırması
```csharp
// Program.cs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidAudience = configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
        };
    });
```

### 🔍 Test Edilecek Senaryolar
- [ ] Geçerli token ile korumalı endpoint'e erişim ✅
- [ ] Geçersiz token → 401 ❌
- [ ] Süresi dolmuş token → 401 ❌
- [ ] Token'sız istek → 401 ❌

---

## 🚧 v2 – Claim/Policy Yetkilendirme [Intermediate]

### 🎯 Amaç
Kullanıcının claim'lerine göre endpoint erişim kontrolü yapılması.

### 🔧 Yapılacaklar
- [ ] `Claims` tabanlı yapı kur
- [ ] `Policy` tanımlamaları (`services.AddAuthorization`)
- [ ] `[Authorize(Policy = "...")]` kullanımı
- [ ] Custom claim'lerle erişim kontrolü (örnek: Department, Permission)
- [ ] Test senaryosu: sadece belirli claim'e sahip kullanıcılar erişebilsin

### 📋 Policy Tanımları
```csharp
services.AddAuthorization(options =>
{
    // Permission bazlı policy
    options.AddPolicy("CanEditCourse", policy =>
        policy.RequireClaim("Permission", "CanEditCourse"));
    
    // Department bazlı policy
    options.AddPolicy("HROnly", policy =>
        policy.RequireClaim("Department", "HR"));
    
    // Multi-claim policy
    options.AddPolicy("SeniorDeveloper", policy =>
        policy.RequireClaim("Role", "Developer")
              .RequireClaim("Level", "Senior"));
});
```

### 🔍 Test Edilecek Senaryolar
- [ ] Claim içeren token → Policy'ye erişim ✅
- [ ] Claim içermeyen token → 403 ❌
- [ ] Multi-policy testleri (birden fazla claim isteği)
- [ ] HR olmayan biri GET /users erişemez ❌

---

## 🔁 v3 – Refresh Token Mekanizması [Intermediate]

### 🎯 Amaç
Access token süresi dolduğunda kullanıcıdan tekrar login istenmeden token yenilenmesi.

### 🔧 Yapılacaklar
- [ ] Refresh token üretimi ve veritabanında saklanması
- [ ] Token yenileme endpoint'i (RefreshToken)
- [ ] Token süresi kontrolü ve yeni access/refresh token üretimi
- [ ] Refresh token süresi ve güvenliği (örnek: tek kullanımlık token)
- [ ] Refresh token iptali ve revocation senaryoları

### 📊 Veritabanı Modeli
```csharp
public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 🔍 Test Edilecek Senaryolar
- [ ] Access token expire olduğunda refresh token ile yeni token alma ✅
- [ ] Geçersiz refresh token → 401 ❌
- [ ] Süresi dolmuş refresh token → 401 ❌
- [ ] Revoke edilmiş refresh token → 401 ❌

---

## 🧠 v4 – Karma Policy/Role Yapıları [Advanced]

### 🎯 Amaç
Policy + Role tabanlı esnek bir erişim kontrol sistemi kurmak.

### 🔧 Yapılacaklar
- [ ] Role-based erişim kontrolü (örnek: [Authorize(Roles = "Admin,Manager")])
- [ ] Policy + Role kombinasyonlu yapılar
- [ ] Çok seviyeli erişim senaryoları (örnek: Claim + Role birlikte kontrol)
- [ ] Attribute veya Middleware ile karma kontrol yapısı

### 📋 Karma Policy Örnekleri
```csharp
// Role + Claim kombinasyonu
// bu kısım başka bir şkilde  olabiir 
options.AddPolicy("AdminOrHRManager", policy =>
    policy.RequireRole("Admin")
          .Or()
          .RequireRole("Manager")
          .RequireClaim("Department", "HR"));

// Custom requirement
options.AddPolicy("SeniorInDepartment", policy =>
    policy.RequireRole("Manager")
          .RequireClaim("Level", "Senior")
          .RequireAssertion(context =>
              context.User.HasClaim("Department", "Engineering")));
```

### 🔍 Test Edilecek Senaryolar
- [ ] Admin rolü + HR department claim → erişim ✅
- [ ] Manager rolü + Engineering department → erişim ✅
- [ ] Junior rolü + HR department → erişim ❌

---

## 🚦 v5 – JWT Bazlı Rate Limiting Entegrasyonu [Advanced]

### 🎯 Amaç
JWT token içeriğine göre kullanıcıya özel rate limit uygulamak.

### 🔧 Yapılacaklar
- [ ] Rate limit middleware yaz
- [ ] `HttpContext.User` üzerinden `UserId` veya `Role` bilgisi al
- [ ] Token'a göre istek sayısını sınırla (örnek: UserId başına 50 istek/saat)
- [ ] Token içinde rol bazlı limit tanımı (örnek: Admin sınırsız, Normal 50/saat)
- [ ] Test senaryosu: Token'a göre farklı limitler uygula

### 📊 Rate Limiting Senaryoları

| Senaryo        | Kütüphane Desteği | JWT ile İlgili | Açıklama |
|---------       |-------------------|-----------------|----------|
| IP bazlı       | ✅ Var           | ❌ | AspNetCoreRateLimit destekli |
| Endpoint bazlı | ✅ Var           | ❌ | endpoint seviyesinde yapılandırılabilir |
| UserId bazlı   | ❌ Yok           | ✅ | Token'dan UserId alınarak custom kontrol |
| Rol bazlı      | ❌ Yok           | ✅ | Claims üzerinden rol bilgisi ile sınırlandırma |
| Token bazlı    | ❌ Yok           | ✅ | Token değerine özel sınırlama (API Key gibi) |
| Quota bazlı    | ⚠️ Kısıtlı       | ✅ | Kullanıcıya günlük/aylık istek sınırı |
| Zaman bazlı    | ❌ Yok           | ✅/❌ | Token süresi ve kampanya zamanlarına göre |
| Concurrency    | ❌ Yok           | ✅/❌ | Aktif token sahibi kullanıcıların eş zamanlı isteği |

### 🔍 Test Edilecek Senaryolar
- [ ] Auth olan kullanıcı → 10 istekte limit dolmalı → 429 dönmeli
- [ ] Token'sız kullanıcı → farklı limit uygulanmalı
- [ ] Admin rolü → sınırsız erişim ✅
- [ ] Normal kullanıcı → 100 istek/saat limiti
- [ ] Premium kullanıcı → 1000 istek/saat limiti

---

## 🧪 Genel Test Stratejisi

### Kullanılacak Araçlar
- [ ] **OpenAPI + Swashbuckle**: Test endpoint'leri
- [ ] **Scalar**: API mocking ve gözlem
- [ ] **Unit + Integration Test**: Policy, middleware ve token üretim testleri

### Test Senaryoları
```csharp
// Unit Test Örneği
[Test]
public void JwtService_GenerateToken_ShouldReturnValidToken()
{
    // Arrange
    var jwtService = new JwtService(configuration);
    var user = new User { Id = "1", Email = "test@example.com", Role = "Admin" };
    
    // Act
    var token = jwtService.GenerateToken(user);
    
    // Assert
    Assert.IsNotNull(token);
    Assert.IsTrue(token.Length > 0);
}
```

### Integration Test Örneği
```csharp
[Test]
public async Task ProtectedEndpoint_WithValidToken_ShouldReturn200()
{
    // Arrange
    var client = _factory.CreateClient();
    var token = await GetValidToken(client);
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token);
    
    // Act
    var response = await client.GetAsync("/api/protected");
    
    // Assert
    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
}
```

---

## 📋 Versiyon Geçiş Kontrol Listesi

### v1 → v2 Geçişi
- [ ] Temel JWT çalışıyor mu?
- [ ] Token üretimi ve doğrulama test edildi mi?
- [ ] Middleware yapılandırması tamamlandı mı?

### v2 → v3 Geçişi
- [ ] Policy'ler tanımlandı mı?
- [ ] Claim bazlı yetkilendirme test edildi mi?
- [ ] Custom policy'ler çalışıyor mu?

### v3 → v4 Geçişi
- [ ] Refresh token mekanizması çalışıyor mu?
- [ ] Token yenileme endpoint'i test edildi mi?
- [ ] Revocation mekanizması kuruldu mu?

### v4 → v5 Geçişi
- [ ] Role + Policy kombinasyonları çalışıyor mu?
- [ ] Karma yetkilendirme test edildi mi?
- [ ] Dinamik policy registration kuruldu mu?

### v5 Tamamlandığında
- [ ] JWT bazlı rate limiting çalışıyor mu?
- [ ] Token'a göre farklı limitler uygulanıyor mu?
- [ ] Tüm test senaryoları geçiyor mu?

---

## 🔒 Güvenlik Kontrol Listesi

### Token Güvenliği
- [ ] Secret key yeterince güçlü mü? (en az 256 bit)
- [ ] Token süreleri uygun mu?
- [ ] Refresh token güvenli saklanıyor mu?
- [ ] Token revocation mekanizması var mı?

### Rate Limiting Güvenliği
- [ ] Token'sız istekler için fallback limit var mı?
- [ ] Rate limit bypass edilebiliyor mu?
- [ ] Token spoofing'e karşı koruma var mı?

### Genel Güvenlik
- [ ] HTTPS zorunlu mu?
- [ ] CORS yapılandırması güvenli mi?
- [ ] Logging ve monitoring var mı?
- [ ] Error mesajları güvenli mi? (hassas bilgi sızıntısı yok mu?)

---

## 📚 Kaynaklar ve Referanslar

- [Microsoft JWT Bearer Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [ASP.NET Core Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction)
- [JWT.io](https://jwt.io/) - JWT token decoder
- [Rate Limiting in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/performance/rate-limit) 
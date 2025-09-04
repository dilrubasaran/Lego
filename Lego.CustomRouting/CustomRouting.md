# 🛣️ Custom Routing Yapısı – Versiyonlara Göre Katman ve Planlama

Bu doküman, Lego projesinde Custom Routing mimarisini adım adım oluşturmak için izlenecek versiyonlara bölünmüş yol haritasını sunar.

---

## ✅ MVP
**Sektörde yaygın, hızlı uygulanabilir, öğrenme aşaması için ideal, çoklu kullanım alanı**

- **Basit Route Templates**: Standart dışı, sabit ve değişken parametreli özel URL kalıpları
- **Attribute Routing**: Controller/Action seviyesinde route özelleştirme
- **Route Constraints**: Regex, tip veya özel kurallarla parametre sınırlamaları
- **Testler**: Unit test + Endpoint testleri + Edge case testleri

## 🔄 Intermediate
**MVP'den sonra ihtiyaç duyulabilecek, biraz daha karmaşık, entegrasyon gerektiren**

- **Route Handlers & Middleware**: Özel route işleyiciler ile karmaşık yönlendirme
- **Çoklu Dil/Region Routing**: URL'de dil/bölge bazlı routing desteği
- **Legacy URL Redirects**: Eski URL'lerin yeni yapıya yönlendirilmesi
- **Performance testleri**: Integration + Load testing

## 🚀 Advanced
**İleri seviye, karmaşık, performans, güvenlik veya özel senaryolar için**

- **Dynamic Route Generation**: Runtime'da veritabanı/config'den dinamik route'lar
- **Route Caching & Optimization**: Performans artırma teknikleri
- **Security Based Routing**: Rol/yetki bazlı routing ve erişim kontrolü
- **Advanced testing**: Security testleri + Performans benchmark

---

## V1 – Temel Custom Routing (MVP)

[x] ### 🛤️ Basit Route Templates ve Pattern Tanımlama [MVP] ✅ TAMAMLANDI
- **Açıklama:** Standart dışı, basit sabit ve değişken parametreler içeren özel URL kalıplarının tanımlanması
- **Katman:** Lego.Web (API / MVC)
- **UI:** Web UI oluşturuldu (clickable cards)
- **Endpoint:** API ve Web endpoint'leri
- **Test:** HTTP test dosyası + URL parser test sayfası

### 🏷️ Attribute Routing Kullanımı [MVP] daha önce çok kez yaptım denenmeyecek 
- **Açıklama:** Controller/Action seviyesinde route'ların attribute ile özelleştirilmesi
- **Katman:** Lego.Web
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Unit test + Integration test

### 🔒 Route Constraints (Sınırlamalar) [MVP]
- **Açıklama:** Route parametrelerine regex, tip veya özel kurallar koyma
- **Katman:** Lego.Web
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Unit test + Edge case testleri

---

## V2 – Orta Seviye Custom Routing (Intermediate)

### ⚙️ Route Handlers ve Middleware Entegrasyonu [Intermediate]
- **Açıklama:** Özel route işleyicilerle (route handler) ve middleware ile karmaşık yönlendirme, ön işlem/son işlem yapılması
- **Katman:** Lego.Web
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Integration test + Performans testleri

### 🌍 Çoklu Dil/Region Routing Desteği [Intermediate]
- **Açıklama:** URL'de dil veya bölge bazlı routing yapılması, dil parametresine göre route seçimi
- **Katman:** Lego.Web, Lego.API
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Çoklu locale testleri

### 🔄 Legacy URL Redirects ve Rewrites [Intermediate]
- **Açıklama:** Eskiden kullanılan URL'lerin yeni yapıya yönlendirilmesi veya yeniden yazılması
- **Katman:** Lego.Web
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Redirect testleri

---

## V3 – İleri Seviye Custom Routing (Advanced)

### 🔧 Dynamic Route Generation [Advanced]
- **Açıklama:** Runtime'da veritabanı veya konfigürasyondan dinamik route'lar oluşturulması
- **Katman:** Lego.Web, Lego.API
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Load test + Integration test

### ⚡ Route Caching ve Performans Optimizasyonu [Advanced]
- **Açıklama:** Routing performansını artırmak için cache ve optimizasyon teknikleri
- **Katman:** Lego.Web
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Performans benchmark

### 🛡️ Security Based Routing [Advanced]
- **Açıklama:** Kullanıcı rolü, yetki ve diğer güvenlik parametrelerine göre routing ve erişim kontrolü
- **Katman:** Lego.Web, Lego.API, Auth katmanı
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Security testleri

---

## 📝 TODO Listesi

### V1 TODO (MVP):
- [x] **Route Templates**
  - [] Basit pattern tanımlama implementasyonu
  - [] Değişken parametre desteği
  - [] Sabit route yapıları
- [ ] **Attribute Routing** daha önce çok kez yaptım 
  - [ ] Controller seviyesi attribute'lar
  - [ ] Action seviyesi attribute'lar
  - [ ] Route combination testleri
- [x] **Route Constraints**
  - [ ] Regex constraint implementasyonu
  - [ ] Type constraint (int, guid, etc.)
  - [ ] Custom constraint sınıfları
- [x] **Test Altyapısı**
  - [x] HTTP test dosyası (custom-routing-test.http)
  - [x] Web URL parser test sayfası
  - [x] Endpoint test senaryoları
  - [] Edge case test koleksiyonu

### V2 TODO (Intermediate):
- [ ] **Route Handlers**
  - [ ] Custom route handler sınıfları
  - [ ] Middleware entegrasyonu
  - [ ] Ön/son işlem logic'i
- [ ] **Çoklu Dil Desteği**
  - [ ] Dil bazlı route seçimi
  - [ ] Region/locale parametreleri
  - [ ] Fallback dil mekanizması
- [ ] **Legacy Support**
  - [ ] URL redirect engine
  - [ ] Rewrite rule engine
  - [ ] 301/302 redirect yönetimi
- [ ] **Performance Testing**
  - [ ] Load test senaryoları
  - [ ] Integration test suite

### V3 TODO (Advanced):
- [ ] **Dynamic Routes**
  - [ ] Database-driven route generation
  - [ ] Configuration-based routes
  - [ ] Runtime route registration
- [ ] **Caching & Optimization**
  - [ ] Route cache implementasyonu
  - [ ] Performance monitoring
  - [ ] Memory optimization
- [ ] **Security Integration**
  - [ ] Role-based routing
  - [ ] Permission-based access
  - [ ] Security policy integration
- [ ] **Advanced Testing**
  - [ ] Security penetration tests
  - [ ] Performance benchmark suite
  - [ ] Stress test scenarios

---

## 🎯 Test Senaryoları

### V1 Test Senaryoları (MVP):
- [x] **Route Template Tests**
  - [x] Basit static route (/CustomRouting)
  - [x] Single parameter route (/category/{id})
  - [x] Multiple parameter route (/category/{categoryId}/product/{productId})
- [x] **Attribute Routing Tests**
  - [x] Controller level [Route] attribute
  - [x] Action level [HttpGet] routing
  - [x] Combined routing scenarios
- [x] **Constraint Tests**
  - [x] Integer constraint (/category/{id:int})
  - [x] Multiple integer constraints (/category/{categoryId:int}/product/{productId:int})
  - [x] URL validation ve parsing
  - [x] Custom route patterns

### V2 Test Senaryoları (Intermediate):
- [ ] **Middleware Integration**
  - [ ] Route handler pipeline tests
  - [ ] Pre/post processing verification
  - [ ] Error handling in route handlers
- [ ] **Multi-language Support**
  - [ ] Language prefix routing (/en/home, /tr/home)
  - [ ] Locale-based content routing
  - [ ] Fallback language scenarios
- [ ] **Legacy URL Management**
  - [ ] 301 redirect tests
  - [ ] URL rewrite verification
  - [ ] Chain redirect prevention

### V3 Test Senaryoları (Advanced):
- [ ] **Dynamic Route Generation**
  - [ ] Database-driven route creation
  - [ ] Runtime route modification
  - [ ] Route conflict resolution
- [ ] **Performance & Caching**
  - [ ] Route lookup performance tests
  - [ ] Cache hit/miss scenarios
  - [ ] Memory usage optimization
- [ ] **Security Testing**
  - [ ] Unauthorized route access attempts
  - [ ] Role-based route filtering
  - [ ] Security policy enforcement

---

## 🎉 Başarı Kriterleri

### V1 Başarı Kriterleri (MVP):
- [x] **Route Templates çalışıyor** (static + parameterized routes)
- [x] **Attribute Routing aktif** (controller + action level)
- [x] **Constraints uygulanıyor** (type + regex + custom)
- [] **404 handling doğru** (invalid routes → 404)
- [x] **Parameter binding çalışıyor** (route params → action parameters)

### V2 Başarı Kriterleri (Intermediate):
- [ ] **Route Handlers entegre** (custom processing pipeline)
- [ ] **Multi-language routing** (locale-based URL handling)
- [ ] **Legacy redirects çalışıyor** (old URLs → new structure)
- [ ] **Performance acceptable** (route resolution < 10ms)
- [ ] **Middleware pipeline stable** (no breaking changes)

### V3 Başarı Kriterleri (Advanced):
- [ ] **Dynamic routes operational** (runtime route management)
- [ ] **Caching optimized** (significant performance improvement)
- [ ] **Security integrated** (role/permission-based access)
- [ ] **Load test passed** (handles expected traffic)
- [ ] **Zero downtime updates** (route changes without restart)

---

## 🚨 Önemli Notlar

1. **Modülerlik**: Her versiyon öncekinin üzerine inşa edilmeli, bağımsız deploy edilebilmeli
2. **Geriye Uyumluluk**: Yeni versiyon eski route'ları bozmamalı
3. **Performans**: Route resolution hızı kritik, cache stratejileri önemli
4. **Test Coverage**: Her seviyede comprehensive test coverage gerekli
5. **Documentation**: Route yapıları ve kullanım örnekleri detaylandırılmalı

---

## 📚 Kaynaklar ve Referanslar

- [ASP.NET Core Routing](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing)
- [Route Constraints](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-constraints)
- [Custom Route Constraints](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#custom-route-constraints)
- [Attribute Routing](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing#attribute-routing)

---

Her bir maddede amacın "modüler olarak bu yapıyı farklı projelerde tekrar kullanabilir hale getirmek" olduğu için, class library üzerinden çağrılabilir, test edilebilir ve entegre edilebilir parçalar olarak planlandı.

Her bir senaryo kendi içinde test edilebilir çıkışlar, view entegreleri (`.cshtml`), OpenAPI test planları veya routing kontrol yapıları barındırmalı.

---

## ✅ TAMAMLANAN ÖZELLİKLER (V1 MVP)

### 🏗️ **Geliştirilen Yapı:**
- **Lego.CustomRouting** class library projesi
- **Bogus** ile fake data üretimi
- **Web ve API** katmanlarında farklı URL formatları
- **Clickable card UI** ile modern kullanıcı deneyimi
- **URL parsing ve validation** sistemi

### 🌐 **URL Formatları:**
- **Web:** `/category/1` ve `/category/1/product/5` (Hiyerarşik)
- **API:** `/product/5` (Basit ve direkt)

### 🎯 **Temel Özellikler:**
- ✅ Modüler Custom Routing servisi
- ✅ Fake data generation (10 kategori, 3-8 ürün/kategori)
- ✅ Web MVC interface (kartlar, detay sayfaları)
- ✅ API JSON endpoints
- ✅ URL parsing ve validation
- ✅ HTTP test dosyası
- ✅ Interactive test sayfaları

### 🧪 **Test Kapsamı:**
- ✅ HTTP test scenarios (custom-routing-test.http)
- ✅ Web URL parser test sayfası
- ✅ Edge cases (invalid URLs, missing data)
- ✅ Category ve Product route validation

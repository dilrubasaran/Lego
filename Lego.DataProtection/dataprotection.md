# Veri Koruma Yapısı – Versiyonlara Göre Katman ve Test Planı

---

## ✅ MVP 
**Sektörde yaygın, hızlı uygulanabilir, öğrenme aşaması için ideal, çoklu kullanım alanı**

- **IDataProtector temel kullanım**: Protect/Unprotect ile verilerin güvenli saklanması
- **Time-limited token**: Şifre sıfırlama/e-posta onayı gibi senaryolar için süreli token
- **DI entegrasyonu**: `IDataProtectionProvider` ile scope bazlı basit kullanım
- **URL token gizleme (temel)**: ID → token dönüşümü ve web tarafında decode
- **Hash (SHA256 + salt)**: Tek yönlü doğrulama için temel hash yapısı
- **Testler**: Koruma/çözme doğruluğu ve süre dolumu için unit testler

## 🔄 Intermediate
**MVP'den sonra ihtiyaç duyulabilecek, biraz daha karmaşık, entegrasyon gerektiren**

- **Single-use token**: Tüketim sonrası otomatik geçersiz kılma
- **Süreli link oluşturma**: Timestamp ile link üretimi (anti-tampering olmadan)
- **Form field koruma (temel)**: IBAN/TC gibi alanların form şifrelenmesi
- **Query string encryption**: Pagination/export gibi senaryolar
- **Uygulamalar arası paylaşım**: Ortak key ring ile basit veri paylaşımı

## 🚀 Advanced 
**İleri seviye, karmaşık, performans, güvenlik veya özel senaryolar için**

- **Replay attack protection**: Nonce ve token reuse engelleme
- **Anti-tampering + Link kapsamı**: MAC/HMAC ile bütünlük + URL binding
- **Anti-CSRF + Field Encryption**: CSRF token ile form alanı şifreleme birleşimi
- **Field-level encryption (DB)**: Kolon bazlı şifreleme + migration ve çözüm
- **Request/Response encryption**: Body düzeyinde şifreleme
- **User/Tenant scoped encryption**: Kullanıcı/tenant'a özel anahtarlar
- **Key storage strategy**: Azure Key Vault/AWS KMS
- **Custom key store**: Redis/DB gibi özel saklama çözümleri
- **Key rotation**: Periyodik rotasyon, eski anahtarla çözüm fallback
- **İzlenebilirlik & audit**: Decrypt logları, key değişim logları, access monitoring

---

## İşaretleme Açıklaması
- ✅: Data Protection kütüphanesi ile yapılabilir/desteklenir anlamına gelir 
- ⚠️: Ek geliştirme/entegrasyon gerektirir
- ❌: Şu an kapsam dışı

## V1 – Temel Veri Koruma

### [x] 🔒 IDataProtector Kullanımı [MVP] ✅
- **Açıklama:** IDataProtector arayüzü kullanılarak verilerin şifrelenmesi ve çözülmesi işlemi gerçekleştirilir.
- **Katman:** Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Hayır
- **Test:** Unit test (koruma/çözme doğruluğu)

### [x] 🔁 Token Protect-Unprotect anti tamping  [MVP] ✅
- **Açıklama:** Token gibi kısa ömürlü verilerin şifrelenmesi ve doğrulama işlemleri.
- **Katman:** Lego.API
- **UI:** Gerekli değil
- **Endpoint:** Hayır
- **Test:** API token testleri

### [x] 🌐 URL Token Gizleme [MVP] ✅
- **Açıklama:** ID yerine şifreli token'ların kullanılması. /edit/5 → /edit/AHGsWsa... gibi URL hashleme.
- **Katman:** Lego.Web, Lego.DataProtection
- **UI:** .cshtml sayfasında URL parse edilir, decode işlemi web tarafında yapılır
- **Endpoint:** Hayır
- **Test:** UI testleri

### [x] 🏗️ DI Entegrasyonu [MVP]  ✅
- **Açıklama:** `IDataProtectionProvider` DI üzerinden scope bazlı protector üretimi ve kullanımı.
- **Katman:** Lego.DataProtection, Lego.API, Lego.Web
- **UI:** Gerekli değil
- **Endpoint:** Hayır
- **Test:** Unit test + bağımsız sınıflarda koruma/çözme işlemleri

### [ ] 🔄 Replay Attack Protection [Advanced]  ⚠️ (DP + DB/cache gerekir)
- **Açıklama:** Nonce veya token reuse kontrolü ile aynı isteğin tekrar kullanılmasını engelleme.
- **Katman:** Lego.API, Lego.JWT
- **UI:** Gerekli değil
- **Endpoint:** Hayır
- **Test:** Unit test + Token takibi

---

## V2 – Kritik URL ve Link Koruma

### [x] ⏳ Süreli Link Oluşturma [Intermediate] ✅(ITimeLimitedDataProtector)
- **Açıklama:** Token süresi geçen linkler geçersiz olur. Expire timestamp ile link üretimi.
- **Katman:** Lego.API, Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Evet ✅
- **Test:** Endpoint test (OpenAPI + Scalar) geçerli/geçersiz token test edilir ✅

#### Alt Senaryolar:
- [x] **🔐 Single-Use Token**: Tüketim sonrası otomatik geçersiz kılma (parola sıfırlama gibi)
- [ ] **✉️ Davet Sistemi**: Şifreli link ile kullanıcıyı kayıt ekranına yönlendirme
- [x] **📧 E-posta Onay/Parola Sıfırlama**: Süreli token ile güvenli işlem doğrulama


### [ ] 🔗 Link Kapsamı Kısıtlama [Advanced] ⚠️ (DP var ama URL binding mantığını sen yazacaksın)
- **Açıklama:** Belirli URL ile geçerli olan tokenlar (çapraz kötüye kullanım engeli).
- **Katman:** Lego.API, Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Endpoint bazlı test + expire

---

## V3 – Form ve Alan Bazlı Veri Koruma

### [x] 📝 Form Field Koruma [Intermediate] ⚠️ (DP var ama model binder / hidden field için custom code)
- **Açıklama:** IBAN, TC gibi verilerin form gönderiminde şifrelenmesi.
- **Katman:** Lego.Web, Lego.DataProtection, Lego.API
- **UI:** .cshtml
- **Endpoint:** Evet
- **Test:** UI + Endpoint test

#### Alt Senaryolar:
- **🔑 Query Bazlı Şifreleme**: Query string için özel şifreleme (pagination, export linkleri)

### [ ] 🛡️ Anti-CSRF + Field Encryption [Advanced] ⚠️ (CSRF ASP.NET tarafında, encryption DP ile ama ikisini birleştirmek custom)
- **Açıklama:** CSRF token ile form alanı şifrelemenin birlikte çalıştığı yapı.
- **Katman:** Lego.Web, Lego.DataProtection, Lego.API
- **UI:** .cshtml
- **Endpoint:** Evet
- **Test:** Form post + csrf token eşleşmesi unit test

### [ ] 🔒 Field-Level Encryption [Advanced] ⚠️ (DP var ama EF integration senin işin)
- **Açıklama:** DB'de kritik alanlar (TC, IBAN) şifreli saklanır. Kolon bazlı şifreleme.
- **Katman:** Lego.Context, Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Migration + Test endpoint ile veri okunup çözümlenir (OpenAPI)

### [ ] 🔐 Request/Response Encryption [Advanced]  ❌ (DP değil, middleware + AES/Hybrid crypto gerekir)
- **Açıklama:** Client <-> Server iletişiminde encrypt edilmiş body yapısı (opsiyonel ileri seviye).
- **Katman:** Lego.API, Lego.Web
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** HTTPS üstü veri çözüm testleri

---

## V4 – Kullanıcıya/Tenant'a Özel Şifreleme

### [ ] 🧩 User/Tenant scoped encryption [Advanced] ✅ 
- **Açıklama:** Scope bazlı protector üretimi ile kullanıcı/tenant'a özel anahtar yönetimi.
- **Katman:** Lego.DataProtection, Lego.Context
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Test endpoint ile login olmuş kullanıcının verisi çözülebilir mi kontrol edilir

#### Alt Senaryolar:
- **👤 User-Scope Data Encryption**: `CreateProtector("UserId:123")` örneği gibi kullanıcıya özel anahtar
- **🏢 Tenant Bazlı Şifreleme**: Multi-tenant sistemlerde tenant'a özel anahtar yönetimi

### [ ] #️⃣ Hash Şifreleme Yapısı [MVP] ❌ (DP değil, SHA256+salt ayrı yazılır)
- **Açıklama:** SHA256 + salt bazlı şifreli hash yapısı.
- **Katman:** Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Hayır
- **Test:** Unit test + veri doğrulama

### [ ] 🔑 Key Storage Strategy [Advanced] ✅ (file, registry, Azure Key Vault, Redis vb. destekler)
- **Açıklama:** Azure Key Vault, AWS KMS gibi sistemlerde anahtar saklama.
- **Katman:** Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Hayır
- **Test:** Environment bazlı integration test

---

## V5 – Gelişmiş Entegrasyon & Uygulama Arası Kullanım

### [ ] 🔗 Uygulamalar Arası Paylaşım [Intermediate] ✅ (ortak key ring ile)
- **Açıklama:** Ortak key ring ile data sharing ve şifrelenmiş verinin farklı uygulamalarda çözülmesi.
- **Katman:** Lego.API, Lego.Web, Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Ortak token ile her iki uygulamada veri çözülüp çözülemediği test edilir


### [ ] 🛑 Anahtar Erişim Kısıtlama [Advanced] ⚠️ (DP key ring ortam bazlı olur ama sen yönetirsin)
- **Açıklama:** Ortama özel (Production, Staging) key geçerliliği.
- **Katman:** Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Hayır
- **Test:** Environment bazlı unit/integration test

### [ ] 🔐 Custom Key Store Kullanımı [Advanced] ✅ (IKeyRepository implement)
- **Açıklama:** Redis, DB gibi özel key saklama çözümleri.
- **Katman:** Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Hayır
- **Test:** Bağlantı testi + failover senaryoları

---

## V6 – İleri Seviye İzlenebilirlik ve Audit

### [ ] 🔍 Şifre Çözme Logları [Advanced] ❌ (DP log tutmaz, sen middleware/log eklemelisin)
- **Açıklama:** Verinin kim tarafından ve ne zaman çözüldüğünün loglanması.
- **Katman:** Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Hayır
- **Test:** Unit test + log output incelenmesi

### [ ] 📜 Anahtar Değişiklik Logu [Advanced]  ⚠️ (DP rotation yapar ama loglamayı sen eklersin)
- **Açıklama:** Key rotation geçmişi, versiyon bilgileri ve zaman damgası loglanması.
- **Katman:** Lego.DataProtection
- **UI:** Gerekli değil
- **Endpoint:** Hayır
- **Test:** Rotation sonrası log analizi

### [ ] 🛡️ Access Monitoring [Advanced] ❌ (DP değil, senin monitoring/log sistemin yapar)
- **Açıklama:** Hangi endpoint hangi protector ile ne sıklıkla erişildi, audit trail.
- **Katman:** Lego.API, Lego.Web
- **UI:** Gerekli değil
- **Endpoint:** Evet
- **Test:** Servis logları + monitoring sistem entegrasyonu

---

Her bir maddede amacın "modüler olarak bu yapıyı farklı projelerde tekrar kullanabilir hale getirmek" olduğu için, class library üzerinden çağrılabilir, test edilebilir ve entegre edilebilir parçalar olarak planlandı.

Her bir senaryo kendi içinde test edilebilir çıkışlar, view entegreleri (`.cshtml`), OpenAPI test planları veya DB/migration kontrol yapıları barındırmalı.

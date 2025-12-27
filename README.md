Diyetisyen Randevu ve Takvim Yönetim Sistemi

Bu proje; diyetisyen, klinik, kuaför, veteriner ve psikolog gibi küçük işletmelerin randevu süreçlerini dijital ortama taşıyan web tabanlı bir **Randevu ve Takvim Yönetim Sistemi**dir.  
Kullanıcıların randevu almasını, işletmelerin randevuları yönetmesini ve çakışmaların engellenmesini amaçlayan kapsamlı bir platformdur.

---

Projenin Amacı

Küçük işletmelerin manuel randevu alma süreçlerini dijitalleştirerek:

- Takvim üzerinden hızlı randevu oluşturmayı,
- Çift rezervasyon sorunlarını önlemeyi,
- Abonelik bazlı (haftalık / aylık) otomatik randevu sistemini,
- Randevu yönetimini merkezi ve güvenilir hale getirmeyi amaçlar.

---

Temel Özellikler

- 🗓️ Takvim üzerinden saat seçimi ile randevu oluşturma  
- 👨‍⚕️ Hekim / işletme paneli: randevu onaylama – reddetme  
- 🔒 Çakışma kontrolü (aynı saate ikinci randevu alınamaz)  
- 🔁 Abonelik sistemi (haftalık – aylık otomatik randevu)  
- 📊 Yönetici raporları:
  - En yoğun saatler
  - Toplam randevu sayıları
  - Kullanıcı faaliyet istatistikleri  
- 🧑‍💼 Rol bazlı giriş: Kullanıcı – Hekim – Admin

---

Kullanılan Teknolojiler

- ASP.NET Core MVC  
- C#  
- SQL Server / PostgreSQL  
- HTML, CSS, JavaScript  

---

Sistem Akışı

1. Kullanıcı giriş yapar  
2. Hekim seçer  
3. Hekimin çalışma günlerine göre takvim görüntülenir  
4. Boş gün ve saat seçilir  
5. Onay ekranı gelir  
6. Randevu admin/hekim paneline düşer  
7. Onaylanan randevu kullanıcı panelinde görünür  
8. Reddedilen / iptal edilen randevular tekrar boş duruma düşer

---

Roller

### Kullanıcı
- Randevu oluşturma  
- Abonelik tanımlama  
- Profil düzenleme / şifre değiştirme  

### Hekim
- Kendisine atanmış randevuları görüntüleme  
- Randevuları onaylama / reddetme  
- Onaylanan randevuları takvim görünümünde izleme  

### Admin
- Kullanıcı yönetimi  
- Randevu yönetimi  
- İstatistik ve rapor ekranları  

---

Admin Panel Özellikleri

### Kullanıcı Yönetimi
- Kullanıcı listeleme, silme, güncelleme  
- Yeni kullanıcı ekleme  

### Randevu Yönetimi
- Tüm randevuları filtreleme  
- Randevu durumu değiştirme (Onaylandı / Bekliyor / İptal)  

### Raporlama
- Günlük / haftalık / aylık istatistikler  
- Popüler saatler raporu  
- Kullanıcı faaliyet analizi  


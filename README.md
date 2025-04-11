# Blog Projesi

Modern bir blog platformu olarak tasarlanmış, ASP.NET Core MVC tabanlı web uygulaması.

## 📋 Proje Hakkında

Bu proje, kullanıcıların blog yazıları oluşturabileceği, düzenleyebileceği ve paylaşabileceği kapsamlı bir blog platformudur. Kullanıcı yönetimi, kategori sistemi, yorum özellikleri ve admin paneli içerir.

## 🚀 Özellikler

### Kullanıcı Yönetimi
- Kayıt ve giriş sistemi
- Rol tabanlı yetkilendirme (Admin/User)
- Profil düzenleme ve resim yükleme
- Şifre değiştirme

### Blog Yazıları
- CRUD işlemleri (oluşturma, okuma, güncelleme, silme)
- Resim yükleme
- SEO dostu URL yapısı
- Kategorilere göre sınıflandırma
- Yazı detay sayfası

### Yorum Sistemi
- AJAX ile gerçek zamanlı yorum ekleme
- Yorum silme (yazar veya admin)
- Kullanıcı profil resimli yorumlar

### Admin Paneli
- Kullanıcı yönetimi
- Kategori yönetimi
- Blog yazılarını düzenleme/silme
- İstatistikler

## 🛠️ Kullanılan Teknolojiler

### Backend
- **ASP.NET Core MVC 6.0**
- **Entity Framework Core** - ORM ve veritabanı işlemleri
- **Identity Framework** - Kimlik doğrulama ve yetkilendirme
- **Dependency Injection** - Servis yaşam döngüsü yönetimi
- **Repository Pattern** - Veri erişim katmanı

### Frontend
- **Bootstrap 5** - Responsive tasarım
- **jQuery** - DOM manipülasyonu ve AJAX istekleri
- **TinyMCE** - Zengin metin editörü
- **Font Awesome** - İkonlar

### Veritabanı
- **MS SQL Server** - İlişkisel veritabanı

## 🏗️ Mimari

Proje, aşağıdaki katmanlı mimariye sahiptir:

- **Entities**: Veri modelleri
- **Data**: Veritabanı erişim katmanı
  - **Abstract**: Repository arayüzleri
  - **Concrete**: Repository implementasyonları
- **Controllers**: İstek işleme ve yönlendirme
- **Views**: Kullanıcı arayüzü
- **Areas**: Admin paneli için ayrılmış alan

## 📊 Veritabanı Şeması

Projede kullanılan temel ilişkiler:
- **User** ↔ **Post**: One-to-Many (Bir kullanıcı birden çok yazı yazabilir)
- **Category** ↔ **Post**: One-to-Many (Bir kategori birden çok yazı içerebilir)
- **User** ↔ **Comment**: One-to-Many (Bir kullanıcı birden çok yorum yapabilir)
- **Post** ↔ **Comment**: One-to-Many (Bir yazıya birden çok yorum yapılabilir)

## 🚀 Kurulum

1. Repoyu klonlayın:
   ```
   git clone https://github.com/omersenpai/BlogumProject.git
   ```

2. Projeyi açın ve gerekli paketleri yükleyin:
   ```
   cd BlogumProject
   dotnet restore
   ```

3. Appsettings.json dosyasında veritabanı bağlantı bilgilerini düzenleyin.

4. Veritabanını oluşturun:
   ```
   dotnet ef database update
   ```

5. Uygulamayı çalıştırın:
   ```
   dotnet run
   ```

## 📸 Ekran Görüntüleri

- Ana Sayfa
- Blog Detay Sayfası
- Admin Paneli
- Kullanıcı Profili

## 📝 Lisans

Bu proje MIT lisansı altında dağıtılmaktadır.

## 👨‍💻 İletişim

Proje hakkında sorularınız için lütfen iletişime geçin.

---

© 2025 | Tüm hakları saklıdır. 
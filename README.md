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
![Screenshot_1](https://github.com/user-attachments/assets/2ea0e9f6-065e-42a8-a3fd-11ff4781de62)
![Screenshot_2](https://github.com/user-attachments/assets/b688d559-b5ae-49c8-8aba-26a4242d13f1)
![Screenshot_3](https://github.com/user-attachments/assets/9dc72f31-7873-4d16-a0e1-0ffce6116786)
![Screenshot_3](https://github.com/user-attachments/assets/08580a6e-db36-4dc8-8bae-faf466add514)
![Screenshot_12](https://github.com/user-attachments/assets/37dc7374-c26b-4aff-aa2e-1f519010c1bf)
![Screenshot_11](https://github.com/user-attachments/assets/5d5051e2-1015-4ac8-b2d5-ed399e2535ff)
![Screenshot_10](https://github.com/user-attachments/assets/6cbd82db-d233-46a6-a788-0a244b3838bc)
![Screenshot_9](https://github.com/user-attachments/assets/560ee037-71fd-48d1-a5d3-bf1b6be129a7)
![Screenshot_8](https://github.com/user-attachments/assets/4ded81e3-66ad-4245-8cd2-72d87f357fc0)
![Screenshot_7](https://github.com/user-attachments/assets/c172c8cb-8fd1-41cb-bc52-a63d55510ff4)
![Screenshot_6](https://github.com/user-attachments/assets/0cee1cdb-f840-481b-aef2-1690fa6778ed)
![Screenshot_5](https://github.com/user-attachments/assets/bb0e2cd1-ddad-45c7-8d5b-f24014e6c9ba)
![Screenshot_4](https://github.com/user-attachments/assets/4a8a9326-ebc6-42aa-8acb-b296a47e8c52)
![Screenshot_16](https://github.com/user-attachments/assets/c39fbd86-f47a-465e-8381-5d75f05ee9b0)
![Screenshot_15](https://github.com/user-attachments/assets/2281c40e-3875-4727-8b66-d3e2e1408076)
![Screenshot_14](https://github.com/user-attachments/assets/9cb7762a-305c-414c-8377-8528c0ce7865)
![Screenshot_13](https://github.com/user-attachments/assets/2cc38cb9-a2ec-4739-9553-2986cee60b8b)


## 👨‍💻 İletişim

Proje hakkında sorularınız için lütfen iletişime geçin.

---

© 2025 | Tüm hakları saklıdır. 
